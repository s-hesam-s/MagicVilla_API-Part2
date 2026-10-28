using Microsoft.AspNetCore.Http;
using Xunit;
namespace Authorization.Tests;

public class TokenCookieTests
{
    [Fact]
    public void BrowserTokenCookiesAreRestrictedToHttpsAndNotReadableByScripts()
    {
        var context = new DefaultHttpContext();
        var provider = new MagicVilla_Web.Services.TokenProvider(new HttpContextAccessor { HttpContext = context });
        provider.SetToken(new MagicVilla_Web.Models.Dto.TokenDTO { AccessToken = "fixture-access", RefreshToken = "fixture-refresh" });
        var cookies = context.Response.Headers.SetCookie;
        Assert.NotEmpty(cookies);
        foreach (var cookie in cookies)
        {
            var attributes = cookie.ToLowerInvariant();
            Assert.Contains("httponly", attributes);
            Assert.Contains("secure", attributes);
            Assert.Contains("samesite=lax", attributes);
            Assert.Contains("path=/", attributes);
        }
    }
}
