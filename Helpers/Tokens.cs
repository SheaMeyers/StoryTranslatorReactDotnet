using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace StoryTranslatorReactDotnet.Helpers;

public static class Tokens
{
    private static byte[] keyBytes = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SecretKey") ?? "");
    public static (string, string) GenerateToken()
    {
        var key = new SymmetricSecurityKey(keyBytes);
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var ApiToken = new JwtSecurityToken(expires: DateTime.UtcNow.AddMinutes(10), signingCredentials: cred);
        var CookieToken = new JwtSecurityToken(expires: DateTime.UtcNow.AddMinutes(10), signingCredentials: cred);

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