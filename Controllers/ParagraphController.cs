using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoryTranslatorReactDotnet.Database;
using StoryTranslatorReactDotnet.Models;

namespace ReactDotnetCsvUploader.Controllers;

public struct ParagraphData
{
    public string TranslateFrom {get; set;}
    public string TranslateTo {get; set;}
    public string? UserTranslation {get; set;}
}

[Route("[controller]")]
public class ParagraphController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ParagraphController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet("GetFirstParagraph/{bookId}/{translateFrom}/{translateTo}")]
    public async Task<IActionResult> GetFirstParagraph(Guid bookId, string translateFrom, string translateTo)
    {
        Paragraph paragraph = await _db.Paragraphs
                                            .Where(paragraph => paragraph.BookId == bookId)
                                            .OrderBy(paragraph => paragraph.Id)
                                            .FirstAsync();

        return Ok(new { 
            paragraph.Id,
            TranslateFrom = paragraph.GetType().GetProperty(translateFrom)?.GetValue(paragraph),
            TranslateTo = paragraph.GetType().GetProperty(translateTo)?.GetValue(paragraph)
        });
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Post(int id, [FromBody] ParagraphData paragraphData)
    {
        if (!ModelState.IsValid) return BadRequest();

        Paragraph nextParagraph = await _db.Paragraphs
                                        .Where(paragraph => paragraph.Id == (id+1))
                                        .SingleAsync();

        return Ok(new {
            nextParagraph.Id,
            TranslateFrom = nextParagraph.GetType().GetProperty(paragraphData.TranslateFrom)?.GetValue(nextParagraph),
            TranslateTo = nextParagraph.GetType().GetProperty(paragraphData.TranslateTo)?.GetValue(nextParagraph)
        });
    }
}
