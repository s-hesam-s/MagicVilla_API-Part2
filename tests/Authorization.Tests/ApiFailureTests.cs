using System.Net;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Authorization.Tests;

public class ApiFailureTests
{
    [Fact]
    public async Task UnauthorizedResponseWithoutTokenPairRequiresLoginWithoutNullDereference()
    {
        using var client = new HttpClient(new FixtureHandler(HttpStatusCode.Unauthorized, "{}"));
        var service = createService(client);
        await Assert.ThrowsAsync<AuthException>(() => service.SendAsync<APIResponse>(request()));
    }

    [Theory]
    [InlineData("{")]
    [InlineData("null")]
    public async Task MalformedApiResponseCannotReportSuccess(string body)
    {
        using var client = new HttpClient(new FixtureHandler(HttpStatusCode.OK, body));
        var result = await createService(client).SendAsync<APIResponse>(request(), withBearer: false);
        Assert.NotNull(result); Assert.False(result.IsSuccess);
    }

    private static BaseService createService(HttpClient client)
    {
        var factory = new Mock<IHttpClientFactory>(); factory.Setup(f => f.CreateClient("MagicAPI")).Returns(client);
        return new BaseService(factory.Object, Mock.Of<ITokenProvider>(), new ConfigurationBuilder().Build(),
            new HttpContextAccessor { HttpContext = new DefaultHttpContext() }, new ApiMessageRequestBuilder());
    }

    private static APIRequest request() => new() { Url = "https://example.invalid/resource" };

    private sealed class FixtureHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode status;
        private readonly string body;
        public FixtureHandler(HttpStatusCode status, string body) { this.status = status; this.body = body; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(body) });
    }
}
