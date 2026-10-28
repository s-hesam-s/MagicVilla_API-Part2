using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

using MagicVilla_VillaAPI.Controllers.V1;
using Xunit;

namespace Authorization.Tests;

public class ManagementPolicyTests
{
    [Theory]
    [InlineData(typeof(MagicVilla_VillaAPI.Controllers.V1.VillaAPIController), "anonymous", false)]
    [InlineData(typeof(MagicVilla_VillaAPI.Controllers.V1.VillaAPIController), "customer", false)]
    [InlineData(typeof(MagicVilla_VillaAPI.Controllers.V1.VillaAPIController), "admin", true)]
    [InlineData(typeof(MagicVilla_VillaAPI.Controllers.V2.VillaAPIController), "anonymous", false)]
    [InlineData(typeof(MagicVilla_VillaAPI.Controllers.V2.VillaAPIController), "customer", false)]
    [InlineData(typeof(MagicVilla_VillaAPI.Controllers.V2.VillaAPIController), "admin", true)]
    [InlineData(typeof(MagicVilla_VillaAPI.Controllers.V1.VillaNumberAPIController), "anonymous", false)]
    [InlineData(typeof(MagicVilla_VillaAPI.Controllers.V1.VillaNumberAPIController), "customer", false)]
    [InlineData(typeof(MagicVilla_VillaAPI.Controllers.V1.VillaNumberAPIController), "admin", true)]
    [InlineData(typeof(MagicVilla_VillaAPI.Controllers.v2.VillaNumberAPIController), "anonymous", false)]
    [InlineData(typeof(MagicVilla_VillaAPI.Controllers.v2.VillaNumberAPIController), "customer", false)]
    [InlineData(typeof(MagicVilla_VillaAPI.Controllers.v2.VillaNumberAPIController), "admin", true)]
    public async Task EveryManagementActionEnforcesAdministratorPolicy(Type controller, string caller, bool allowed)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization();
        using var provider = services.BuildServiceProvider();
        var policyProvider = provider.GetRequiredService<IAuthorizationPolicyProvider>();
        var authorization = provider.GetRequiredService<IAuthorizationService>();
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, caller),
            new Claim(ClaimTypes.Role, caller == "admin" ? "admin" : "customer")
        }, caller == "anonymous" ? null : "Test"));

        var actions = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotEmpty(actions);
        foreach (var action in actions)
        {
            var attributes = controller.GetCustomAttributes(true).Concat(action.GetCustomAttributes(true)).ToArray();
            Assert.DoesNotContain(attributes, a => a is IAllowAnonymous);
            var policy = await AuthorizationPolicy.CombineAsync(policyProvider, attributes.OfType<IAuthorizeData>());
            if (!attributes.Any(a => a is Microsoft.AspNetCore.Mvc.HttpPostAttribute or Microsoft.AspNetCore.Mvc.HttpPutAttribute or Microsoft.AspNetCore.Mvc.HttpDeleteAttribute or Microsoft.AspNetCore.Mvc.HttpPatchAttribute)) continue;
            Assert.NotNull(policy);
            var result = await authorization.AuthorizeAsync(principal, resource: null, policy);
            Assert.Equal(allowed, result.Succeeded);
        }
    }
}
