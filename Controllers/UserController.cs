using Microsoft.AspNetCore.Mvc;
using StoryTranslatorReactDotnet.Models;
using StoryTranslatorReactDotnet.Helpers;
using StoryTranslatorReactDotnet.Database;
using Microsoft.AspNetCore.Identity;

namespace StoryTranslatorReactDotnet.Controllers;

public struct LoginData 
{
    public string Username {get; set;}
    public string Password {get; set;}
}

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public UserController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> Signup([FromBody] LoginData loginData)
    {
        if (!ModelState.IsValid) return BadRequest();

        if (_db.Users.Any(user => user.Username == loginData.Username)) 
            return BadRequest($"Username {loginData.Username} already taken");

        (string apiToken, string cookieToken) = Tokens.GenerateToken();

        var user = new User(loginData.Username, loginData.Password, apiToken, cookieToken);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        Response.Cookies.Append("cookieToken", cookieToken, new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(new { apiToken });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginData loginData)
    {
        if (!ModelState.IsValid) return BadRequest();

        var user = _db.Users.Where(user => user.Username == loginData.Username).FirstOrDefault();

        if (user == null) return Forbid();

        var hasher = new PasswordHasher<User>();
        if (hasher.VerifyHashedPassword(user, user.Password, loginData.Password) == PasswordVerificationResult.Failed)
            return Forbid();

        (string apiToken, string cookieToken) = Tokens.GenerateToken();

        user.ApiToken = apiToken;
        user.CookieToken = cookieToken;
        user.OldApiTokens = new List<string>();
        user.OldCookieToken = new List<string>();

        _db.Users.Update(user);
        await _db.SaveChangesAsync();

        Response.Cookies.Append("cookieToken", cookieToken, new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(new { apiToken });
    }
}
