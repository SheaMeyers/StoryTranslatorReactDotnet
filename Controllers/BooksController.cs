using Microsoft.AspNetCore.Mvc;
using StoryTranslatorReactDotnet.Database;

namespace ReactDotnetCsvUploader.Controllers;

[Route("[controller]")]
public class BooksController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public BooksController(ApplicationDbContext db)
    {
        _db = db;
    }
    
    [HttpGet]
    public IActionResult Get() => Ok(new { Titles = _db.Books.Select(book => book.Title) });
}
