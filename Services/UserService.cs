using Microsoft.EntityFrameworkCore;
using StoryTranslatorReactDotnet.Database;
using StoryTranslatorReactDotnet.Helpers;
using StoryTranslatorReactDotnet.Models;

namespace StoryTranslatorReactDotnet.Services;

public class UserService
{
    private readonly ApplicationDbContext _db;
    
    public UserService(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<Tuple<string, string>> Login(User user)
    {
        (string apiToken, string cookieToken) = Tokens.GenerateTokens();

        user.Tokens.Add(new Token(apiToken, cookieToken, user));
        _db.Users.Update(user);
        await _db.SaveChangesAsync();

        return Tuple.Create(apiToken, cookieToken);
    }

    public async Task Logout(User user)
    {
        _db.Tokens.RemoveRange(user.Tokens);
        await _db.SaveChangesAsync();
    }

    public async Task<User?> GetUser(string apiToken, string cookieToken)
    {
        Token? token = await _db.Tokens
                                    .Where(token => 
                                            token.ApiToken == apiToken && 
                                            token.CookieToken == cookieToken)
                                    .SingleOrDefaultAsync();

        if (token == null) return null;

        User? user = await _db.Users.Where(user => user.Tokens.Contains(token)).SingleOrDefaultAsync();

        return user;
    }
}
