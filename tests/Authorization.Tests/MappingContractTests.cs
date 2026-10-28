using System.Text.Json;
using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Models;
using Xunit;

namespace Authorization.Tests;

public class MappingContractTests
{
    [Fact]
    public void RegistrationResponseContainsPublicIdentityOnly()
    {
        var user = new ApplicationUser { Id = "customer", Name = "Learner", UserName = "learner",
            PasswordHash = "fixture-hash", SecurityStamp = "fixture-stamp", PhoneNumber = "fixture-phone" };
        var result = JsonSerializer.SerializeToElement(new MappingConfig().ToUserDTO(user));

        Assert.Equal("customer", result.GetProperty("ID").GetString());
        Assert.Equal("Learner", result.GetProperty("Name").GetString());
        Assert.Equal(3, result.EnumerateObject().Count());
        Assert.False(result.TryGetProperty("PasswordHash", out _));
        Assert.False(result.TryGetProperty("SecurityStamp", out _));
    }
}
