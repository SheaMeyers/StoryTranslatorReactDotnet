using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoryTranslatorReactDotnet.Helpers;

public static class Tokens
{
    private static byte[] keyBytes = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SecretKey") ?? "");
    public static (string, string) GenerateTokens()
    {
        var key = new SymmetricSecurityKey(keyBytes);
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var apiTokenClaims = new Claim[] { new Claim("randomId", Guid.NewGuid().ToString()) };
        var cookieTokenClaims = new Claim[] { new Claim("randomId", Guid.NewGuid().ToString()) };

        var ApiToken = new JwtSecurityToken(claims: apiTokenClaims, expires: DateTime.UtcNow.AddDays(7), signingCredentials: cred);
        var CookieToken = new JwtSecurityToken(claims: cookieTokenClaims, expires: DateTime.UtcNow.AddDays(7), signingCredentials: cred);

        var handler = new JwtSecurityTokenHandler();

        return (handler.WriteToken(ApiToken), handler.WriteToken(CookieToken));
    }

    public static async Task<bool> ValidateToken(string token)
    {
        var validationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
            };

        var result = await new JwtSecurityTokenHandler().ValidateTokenAsync(token, validationParameters);

        return result.IsValid;
    }
}