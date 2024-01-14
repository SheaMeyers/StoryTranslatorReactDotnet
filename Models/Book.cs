using System.ComponentModel.DataAnnotations;

namespace StoryTranslatorReactDotnet.Models;

public class Book
{
    [Key]
    public int Id {get; set;}
    public DateTime Created {get; set;}
    public DateTime Modified {get; set;}
    public string Title {get; set;}

    public Book(string title)
    {
        this.Created = DateTime.UtcNow;
        this.Modified = DateTime.UtcNow;
        this.Title = title;
    }
}
