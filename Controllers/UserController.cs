using Microsoft.AspNetCore.Mvc;
using StoryTranslatorReactDotnet.Models;
using StoryTranslatorReactDotnet.Helpers;
using StoryTranslatorReactDotnet.Database;
using System.ComponentModel.DataAnnotations;

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
}
