using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoryTranslatorReactDotnet.Database;
using StoryTranslatorReactDotnet.Helpers;
using StoryTranslatorReactDotnet.Models;
using StoryTranslatorReactDotnet.Services;

namespace ReactDotnetCsvUploader.Controllers;

public struct ParagraphData
{
    public string TranslateFrom {get; set;}
    public string TranslateTo {get; set;}
    public string? UserTranslation {get; set;}
}

public struct UserTranslatedParagraphData
{
    public string Language {get; set;}
    public string Value {get; set;}
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

    [HttpPost("{id}")]
    public async Task<IActionResult> Post(int id, [FromBody] ParagraphData paragraphData)
    {
        if (!ModelState.IsValid) return BadRequest();

        Paragraph nextParagraph = await _db.Paragraphs
                                        .Where(paragraph => paragraph.Id == id)
                                        .SingleAsync();

        return Ok(new {
            nextParagraph.Id,
            TranslateFrom = nextParagraph.GetType().GetProperty(paragraphData.TranslateFrom)?.GetValue(nextParagraph),
            TranslateTo = nextParagraph.GetType().GetProperty(paragraphData.TranslateTo)?.GetValue(nextParagraph)
        });
    }

    [HttpPost("SetUserParagraphTranslation/{paragraphId}")]
    public async Task<IActionResult> SetUserParagraphTranslation(int paragraphId, [FromBody] UserTranslatedParagraphData data) {

        string? cookieToken = Request.Cookies["cookieToken"];
        string? apiToken = Request.Headers.Authorization;

        if (cookieToken == null || apiToken == null) return Unauthorized();

        var isCookieTokenValid = await Tokens.ValidateToken(cookieToken);
        var isApiTokenValid = await Tokens.ValidateToken(apiToken);

        if (isCookieTokenValid == false || isApiTokenValid == false) return Unauthorized();

        Token? token = await _db.Tokens
                                    .Include(token => token.User)
                                    .Where(token => token.CookieToken == cookieToken && token.ApiToken == apiToken)
                                    .SingleOrDefaultAsync();

        if (token == null) return Unauthorized();

        Paragraph paragraph = await _db.Paragraphs
                                        .Where(paragraph => paragraph.Id == paragraphId)
                                        .SingleAsync();

        UserTranslatedParagraph? userTranslatedParagraph = await _db.UserTranslatedParagraphs
                                                                .Where(utp => utp.User == token.User && utp.Paragraph == paragraph)
                                                                .SingleOrDefaultAsync();

        if (userTranslatedParagraph == null)
        {
            userTranslatedParagraph = new UserTranslatedParagraph(paragraph, token.User);
            userTranslatedParagraph.GetType().GetProperty(data.Language)?.SetValue(userTranslatedParagraph, data.Value);
            _db.Add(userTranslatedParagraph);
        }
        else 
        {
            userTranslatedParagraph.GetType().GetProperty(data.Language)?.SetValue(userTranslatedParagraph, data.Value);
            _db.Update(userTranslatedParagraph);
        }

        await _db.SaveChangesAsync();

        (apiToken, cookieToken) = await _tokenService.Regenerate(token.ApiToken, token.CookieToken);

        Response.Cookies.Append("cookieToken", cookieToken, new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(new { apiToken });
    }

    [HttpGet("GetUserParagraphTranslation/{paragraphId}/{language}")]
    public async Task<IActionResult> GetUserParagraphTranslation(int paragraphId, string language) {

        string? cookieToken = Request.Cookies["cookieToken"];
        string? apiToken = Request.Headers.Authorization;

        if (cookieToken == null || apiToken == null) return Ok(new { Value = "" });

        var isCookieTokenValid = await Tokens.ValidateToken(cookieToken);
        var isApiTokenValid = await Tokens.ValidateToken(apiToken);

        if (isCookieTokenValid == false || isApiTokenValid == false) return Ok(new { Value = "" });

        User? user = await _userService.GetUser(apiToken, cookieToken);

        if (user == null) return Ok(new { Value = "" });

        UserTranslatedParagraph? userTranslatedParagraph = await _db.UserTranslatedParagraphs
                                                                .Where(utp => utp.User == user && utp.ParagraphId == paragraphId)
                                                                .SingleOrDefaultAsync();

        if (userTranslatedParagraph == null) return Ok(new { Value = "" });
        
        return Ok(new { Value = userTranslatedParagraph.GetType().GetProperty(language)?.GetValue(userTranslatedParagraph) });
    }
}
