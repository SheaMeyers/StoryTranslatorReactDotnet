using StoryTranslatorReactDotnet.Database;
using StoryTranslatorReactDotnet.Models;

namespace StoryTranslatorReactDotnet.Services;

public class UserService
{
    private readonly ApplicationDbContext _db;
    
    public UserService(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task Logout(User user)
    {
        user.ApiToken = "";
        user.OldApiTokens = new List<string>();
        user.CookieToken = "";
        user.OldCookieToken = new List<string>();
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }
}