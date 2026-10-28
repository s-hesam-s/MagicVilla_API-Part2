using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MagicVilla_VillaAPI.Security;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Authorization.Tests;

public class TokenPairTests
{
    private readonly string key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    [Theory]
    [InlineData("valid")]
    [InlineData("expired")]
    public void MatchingSignedPairCanBePresentedForRefresh(string kind) =>
        Assert.True(TokenPairValidator.Matches(token(kind), "owner", "chain", key));

    [Theory]
    [InlineData("unsigned")]
    [InlineData("wrongKey")]
    [InlineData("wrongIssuer")]
    [InlineData("wrongAudience")]
    [InlineData("wrongAlgorithm")]
    [InlineData("missingSubject")]
    [InlineData("missingChain")]
    [InlineData("wrongSubject")]
    [InlineData("wrongChain")]
    [InlineData("malformed")]
    [InlineData("oversized")]
    public void UntrustedOrMismatchedAccessTokenCannotRefreshOrRevoke(string kind) =>
        Assert.False(TokenPairValidator.Matches(token(kind), "owner", "chain", key));

    private string token(string kind)
    {
        if (kind == "malformed") return "invalid";
        if (kind == "oversized") return new string('x', 8193);
        var claims = new List<Claim>();
        if (kind != "missingSubject") claims.Add(new Claim(JwtRegisteredClaimNames.Sub, kind == "wrongSubject" ? "other" : "owner"));
        if (kind != "missingChain") claims.Add(new Claim(JwtRegisteredClaimNames.Jti, kind == "wrongChain" ? "other" : "chain"));
        var signingKey = kind == "wrongKey" ? Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)) : key;
        var credentials = kind == "unsigned" ? null : new SigningCredentials(
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey)),
            kind == "wrongAlgorithm" ? SecurityAlgorithms.HmacSha384 : SecurityAlgorithms.HmacSha256);
        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            kind == "wrongIssuer" ? "https://other.example.invalid" : "https://magicvilla-api.com",
            kind == "wrongAudience" ? "other" : "hesamshoushtarian.com", claims,
            DateTime.UtcNow.AddDays(-2), kind == "expired" ? DateTime.UtcNow.AddDays(-1) : DateTime.UtcNow.AddMinutes(5), credentials));
    }
}
