using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace StoryTranslatorReactDotnet.Models;

public class User
{
    public Guid Id {get; set;}
    public DateTime Created {get; set;}
    public DateTime Modified {get; set;}
    public string Username {get; set;}
    public string Password {get; set;}
    public string ApiToken {get; set;}
    public List<string> OldApiTokens {get; set;}
    public string CookieToken {get; set;}
    public List<string> OldCookieToken {get; set;}

    public User(string username, string password)
    {
        var hasher = new PasswordHasher<User>();
        this.Username = username;
        this.Password = hasher.HashPassword(this, password);
        this.ApiToken = "";
        this.OldApiTokens = new List<string>();
        this.CookieToken = "";
        this.OldCookieToken = new List<string>();
    }

    public async Task<(string, string)> GenerateToken()
    {
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("this is my custom Secret key for authentication this is my custom Secret key for authentication"));
         var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var ApiToken = new JwtSecurityToken(expires: DateTime.UtcNow.AddMinutes(10), signingCredentials: cred);
        var CookieToken = new JwtSecurityToken(expires: DateTime.UtcNow.AddMinutes(10), signingCredentials: cred);

        var ApiJwt = new JwtSecurityTokenHandler().WriteToken(ApiToken);
        var CookieJwt = new JwtSecurityTokenHandler().WriteToken(CookieToken);

        

        return (ApiJwt, CookieJwt);
    }

    public async Task<bool> ValidateToken(string token)
    {
        var validationParameters = new TokenValidationParameters()
        {
            // IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("this is my custom Secret key for authentication this is my custom Secret key for authentication")),
            LogTokenId = false,
            LogValidationExceptions = false,
            RequireExpirationTime = false,
            RequireSignedTokens = false,
            RequireAudience = false,
            SaveSigninToken = false,
            TryAllIssuerSigningKeys = false,
            ValidateActor = false,
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = false,
            ValidateLifetime = false,
            ValidateTokenReplay = false,
            SignatureValidator = delegate (string token, TokenValidationParameters parameters)
            {
                var jwt = new JwtSecurityToken(token);
                return jwt;
            },

        };

        var result = await new JwtSecurityTokenHandler().ValidateTokenAsync(token, validationParameters);

        bool isValid = result.IsValid;

        return isValid;
    }
}
