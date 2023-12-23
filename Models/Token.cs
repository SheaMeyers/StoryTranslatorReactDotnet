namespace StoryTranslatorReactDotnet.Models;

public class Token: BaseModel
{
    public string ApiToken {get; set;}
    public string CookieToken {get; set;}
    public Guid UserId {get; set;}
    public User User {get; set;}

    public Token(string apiToken, string cookieToken)
    {
        this.ApiToken = apiToken;
        this.CookieToken = cookieToken;
        this.User = null!;
    }

    public Token(string apiToken, string cookieToken, User user)
    {
        this.ApiToken = apiToken;
        this.CookieToken = cookieToken;
        this.User = user;
        this.UserId = user.Id;
        this.Created = DateTime.UtcNow;
        this.Modified = DateTime.UtcNow;
    }
}
