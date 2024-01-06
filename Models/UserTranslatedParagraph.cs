namespace StoryTranslatorReactDotnet.Models;

public class UserTranslatedParagraph: BaseModel
{
    public int ParagraphId {get; set;}
    public Paragraph Paragraph {get; set;}
    public Guid UserId {get; set;}
    public User User {get; set;}
    public string English {get; set;}
    public string Spanish {get; set;}
    public string French {get; set;}
    public string German {get; set;}

    // Required for Entity Framework Migrations
    public UserTranslatedParagraph()
    {
        this.English = "";
        this.Spanish = "";
        this.French = "";
        this.German = "";
        this.User = null!;
        this.Paragraph = null!;
    }

    public UserTranslatedParagraph(Paragraph paragraph, User user)
    {
        this.English = "";
        this.Spanish = "";
        this.French = "";
        this.German = "";
        this.Paragraph = paragraph;
        this.User = user;
    }
}
