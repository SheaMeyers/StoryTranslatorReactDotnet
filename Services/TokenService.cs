using Microsoft.EntityFrameworkCore;
using StoryTranslatorReactDotnet.Database;
using StoryTranslatorReactDotnet.Helpers;
using StoryTranslatorReactDotnet.Models;

namespace StoryTranslatorReactDotnet.Services;

public class TokenService
{
    private readonly ApplicationDbContext _db;
    
    public TokenService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Token?> GetToken(HttpRequest request)
    {
        string? cookieToken = request.Cookies["cookieToken"];
        string? apiToken = request.Headers.Authorization;

        if (cookieToken == null || apiToken == null) return null;

        var isCookieTokenValid = await Tokens.ValidateToken(cookieToken);
        var isApiTokenValid = await Tokens.ValidateToken(apiToken);

        if (isCookieTokenValid == false || isApiTokenValid == false) return null;

        return await _db.Tokens
                        .Include(token => token.User)
                        .Where(token => 
                                token.ApiToken == apiToken && 
                                token.CookieToken == cookieToken)
                        .SingleOrDefaultAsync();
    }

    public async Task Logout(string apiToken, string cookieToken)
    {
        Token? token = await _db.Tokens
                                    .Where(token => 
                                            token.ApiToken == apiToken && 
                                            token.CookieToken == cookieToken)
                                    .SingleOrDefaultAsync();
        
        if (token == null) return;

        _db.Tokens.Remove(token);
        await _db.SaveChangesAsync();
    }

    public async Task<Tuple<string, string>> Regenerate(string apiToken, string cookieToken)
    {
        (string newApiToken, string newCookieToken) = Tokens.GenerateTokens();

        Token token = await _db.Tokens
                                .Where(token => 
                                        token.ApiToken == apiToken && 
                                        token.CookieToken == cookieToken)
                                .SingleAsync();

        token.Modified = DateTime.UtcNow;
        token.ApiToken = newApiToken;
        token.CookieToken = newCookieToken;

        _db.Tokens.Update(token);
        await _db.SaveChangesAsync();

        return Tuple.Create(newApiToken, newCookieToken);
    }
}