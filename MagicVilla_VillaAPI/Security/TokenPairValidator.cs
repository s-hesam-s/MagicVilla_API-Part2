using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MagicVilla_VillaAPI.Security;

public static class TokenPairValidator
{
    public static bool Matches(string accessToken, string userId, string tokenId, string signingKey)
    {
        if (string.IsNullOrWhiteSpace(accessToken) || accessToken.Length > 8192 ||
            string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(tokenId) || string.IsNullOrWhiteSpace(signingKey))
            return false;
        try
        {
            var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var principal = handler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey)),
                RequireSignedTokens = true,
                ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
                ValidateIssuer = true, ValidIssuer = "https://magicvilla-api.com",
                ValidateAudience = true, ValidAudience = "hesamshoushtarian.com",
                // Refresh is allowed to present an expired access token; the refresh-token expiry is checked separately.
                ValidateLifetime = false
            }, out var validated);
            return validated is JwtSecurityToken jwt && jwt.Payload.Exp.HasValue &&
                principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value == userId &&
                principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value == tokenId;
        }
        catch (SecurityTokenException) { return false; }
        catch (ArgumentException) { return false; }
    }
}
