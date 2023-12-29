namespace StoryTranslatorReactDotnet.Models;

public class Book: BaseModel
{
    public string Title {get; set;}

    public Book(string title)
    {
        this.Title = title;
    }
}
