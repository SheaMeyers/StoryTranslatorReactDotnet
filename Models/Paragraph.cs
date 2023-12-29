namespace StoryTranslatorReactDotnet.Models;

public class Paragraph
{
    public int Id {get; set;}
    public DateTime Created {get; set;}
    public DateTime Modified {get; set;}
    public Guid BookId {get; set;}
    public Book Book {get; set;}
    public string English {get; set;}
    public string Spanish {get; set;}
    public string French {get; set;}
    public string German {get; set;}

    // Required for Entity Framework Migrations
    public Paragraph(string english, string spanish, string french, string german)
    {
        this.English = english;
        this.Spanish = spanish;
        this.French = french;
        this.German = german;
        this.Book = null!;
    }

    public Paragraph(string english, string spanish, string french, string german, Book book)
    {
        this.English = english;
        this.Spanish = spanish;
        this.French = french;
        this.German = german;
        this.Book = book;
    }
}
