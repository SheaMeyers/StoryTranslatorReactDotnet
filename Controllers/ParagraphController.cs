using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoryTranslatorReactDotnet.Database;
using StoryTranslatorReactDotnet.Models;
using StoryTranslatorReactDotnet.Services;

namespace ReactDotnetCsvUploader.Controllers;


public struct ParagraphData
{
    public int CurrentId {get; set;}
    public int Change {get; set;}
    public string TranslateFrom {get; set;}
    public string TranslateTo {get; set;}
    public string UserTranslation {get; set;}
}

[Route("[controller]")]
public class ParagraphController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly TokenService _tokenService;
    private readonly UserService _userService;

    public ParagraphController(ApplicationDbContext db, TokenService tokenService, UserService userService)
    {
        _db = db;
        _tokenService = tokenService;
        _userService = userService;
    }

    [HttpGet("GetFirstParagraph/{bookTitle}/{translateFrom}/{translateTo}")]
    public async Task<IActionResult> GetFirstParagraph(string bookTitle, string translateFrom, string translateTo)
    {
        var baseQuery = _db.Paragraphs
                            .Where(paragraph => paragraph.Book.Title == bookTitle)
                            .OrderBy(paragraph => paragraph.Id); 

        Paragraph paragraph = await baseQuery.FirstAsync();
        int firstId = await baseQuery.Select(paragraph => paragraph.Id).FirstAsync();
        int lastId = await baseQuery.Select(paragraph => paragraph.Id).LastAsync();

        return Ok(new { 
            paragraph.Id,
            TranslateFrom = paragraph.GetType().GetProperty(translateFrom)?.GetValue(paragraph),
            TranslateTo = paragraph.GetType().GetProperty(translateTo)?.GetValue(paragraph),
            FirstId = firstId,
            LastId = lastId
        });
    }

    private async Task HandleSavingUserTranslation(User user, ParagraphData paragraphData)
    {
        Paragraph paragraph = await _db.Paragraphs
                                        .Where(paragraph => paragraph.Id == paragraphData.CurrentId)
                                        .SingleAsync();

        UserTranslatedParagraph? userTranslatedParagraph = await _db.UserTranslatedParagraphs
                                                                .Where(utp => utp.User == user && utp.Paragraph == paragraph)
                                                                .SingleOrDefaultAsync();

        if (userTranslatedParagraph == null)
        {
            userTranslatedParagraph = new UserTranslatedParagraph(paragraph, user);
            userTranslatedParagraph.GetType().GetProperty(paragraphData.TranslateTo)?.SetValue(userTranslatedParagraph, paragraphData.UserTranslation);
            _db.Add(userTranslatedParagraph);
        }
        else 
        {
            userTranslatedParagraph.GetType().GetProperty(paragraphData.TranslateTo)?.SetValue(userTranslatedParagraph, paragraphData.UserTranslation);
            _db.Update(userTranslatedParagraph);
        }

        await _db.SaveChangesAsync();
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ParagraphData paragraphData)
    {
        if (!ModelState.IsValid) return BadRequest();

        Token? token = await _tokenService.GetToken(Request);

        if (token != null && !String.IsNullOrWhiteSpace(paragraphData.UserTranslation)) await HandleSavingUserTranslation(token.User, paragraphData);

        Paragraph nextParagraph = await _db.Paragraphs
                                        .Where(paragraph => paragraph.Id == (paragraphData.CurrentId + paragraphData.Change))
                                        .SingleAsync();
        
        UserTranslatedParagraph? userTranslatedParagraph = null;
        string apiToken = "";

        if (token != null)
        {
            userTranslatedParagraph = await _db.UserTranslatedParagraphs
                                                    .Where(utp => utp.User == token.User && utp.ParagraphId == (paragraphData.CurrentId + paragraphData.Change))
                                                    .SingleOrDefaultAsync();

            (apiToken, var cookieToken) = await _tokenService.Regenerate(token.ApiToken, token.CookieToken);

            Response.Cookies.Append("cookieToken", cookieToken, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            });
        }

        return Ok(new {
            nextParagraph.Id,
            TranslateFrom = nextParagraph.GetType().GetProperty(paragraphData.TranslateFrom)?.GetValue(nextParagraph),
            TranslateTo = nextParagraph.GetType().GetProperty(paragraphData.TranslateTo)?.GetValue(nextParagraph),
            UserTranslation = userTranslatedParagraph?.GetType()?.GetProperty(paragraphData.TranslateTo)?.GetValue(userTranslatedParagraph) ?? "",
            ApiToken = apiToken
        });
    }
}
