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