using System.ComponentModel.DataAnnotations;

public class BaseModel 
{
    [Key]
    public Guid Id {get; set;}
    public DateTime Created {get; set;}
    public DateTime Modified {get; set;}
}