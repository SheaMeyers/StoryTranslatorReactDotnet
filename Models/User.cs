using Microsoft.AspNetCore.Identity;

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

    public User(string username, string password, string apiToken, string cookieToken)
    {
        var hasher = new PasswordHasher<User>();
        this.Username = username;
        this.Password = hasher.HashPassword(this, password);
        this.ApiToken = apiToken;
        this.OldApiTokens = new List<string>();
        this.CookieToken = cookieToken;
        this.OldCookieToken = new List<string>();
    }
}
