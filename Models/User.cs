using Microsoft.AspNetCore.Identity;

namespace StoryTranslatorReactDotnet.Models;

public class User: BaseModel
{
    public DateTime LastLogin {get; set;}
    public string Username {get; set;}
    public string Password {get; set;}
    public ICollection<Token> Tokens {get; set;}

    public User(string username, string password)
    {
        this.Username = username;
        this.Password = password;
        this.Password = password;
        this.Tokens = new List<Token>();
        this.Created = DateTime.UtcNow;
        this.Modified = DateTime.UtcNow;
        this.LastLogin = DateTime.UtcNow;
    }

    public User(string username, string password, string apiToken, string cookieToken) : this(username, password)
    {
        this.Tokens = new List<Token>() { new Token(apiToken, cookieToken, this) };
    }
}
