using Microsoft.AspNetCore.Mvc;
using StoryTranslatorReactDotnet.Models;
using StoryTranslatorReactDotnet.Helpers;
using StoryTranslatorReactDotnet.Database;
using Microsoft.AspNetCore.Identity;
using StoryTranslatorReactDotnet.Services;
using Microsoft.EntityFrameworkCore;

namespace StoryTranslatorReactDotnet.Controllers;

public struct LoginData 
{
    public string Username {get; set;}
    public string Password {get; set;}
}

public struct ChangePasswordData
{
    public string OldPassword {get; set;}
    public string NewPassword {get; set;}
}

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly UserService _userService;

    public UserController(ApplicationDbContext db, UserService userService)
    {
        _db = db;
        _userService = userService;
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> Signup([FromBody] LoginData loginData)
    {
        if (!ModelState.IsValid) return BadRequest();

        if (_db.Users.Any(user => user.Username == loginData.Username)) 
            return BadRequest($"Username {loginData.Username} already taken");

        (string apiToken, string cookieToken) = Tokens.GenerateTokens();

        User user = new User(loginData.Username, loginData.Password, apiToken, cookieToken);

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

        User? user = _db.Users.Where(user => user.Username == loginData.Username).SingleOrDefault();

        if (user == null) return Forbid();

        var hasher = new PasswordHasher<User>();
        if (hasher.VerifyHashedPassword(user, user.Password, loginData.Password) == PasswordVerificationResult.Failed)
            return Forbid();

        (string apiToken, string cookieToken) = Tokens.GenerateTokens();

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

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordData changePasswordData)
    {
        string? cookieToken = Request.Cookies["cookieToken"];
        string? apiToken = Request.Headers.Authorization;

        if (cookieToken == null || apiToken == null) return Forbid();

        User user;
        try {
            user = await _db.Users
                            .Where(user => 
                                    user.CookieToken == cookieToken && 
                                    user.ApiToken == apiToken)
                            .SingleAsync();
        } catch (Exception) {
            try {
                user = await _db.Users
                            .Where(user => 
                                    user.OldCookieToken.Contains(cookieToken) ||
                                    user.OldApiTokens.Contains(apiToken))
                            .SingleAsync();
            } catch (Exception) {
                return Forbid();
            }
            await _userService.Logout(user);
            return Forbid();
        }
        
        var isCookieTokenValid = await Tokens.ValidateToken(cookieToken);
        var isApiTokenValid = await Tokens.ValidateToken(apiToken);

        if (isCookieTokenValid == false || isApiTokenValid == false) {
            await _userService.Logout(user);
            return Forbid();
        }

        var hasher = new PasswordHasher<User>();
        if (hasher.VerifyHashedPassword(user, user.Password, changePasswordData.OldPassword) == PasswordVerificationResult.Failed)
            return Forbid();

        user.Password = hasher.HashPassword(user, changePasswordData.NewPassword);
        _db.Users.Update(user);
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        string? cookieToken = Request.Cookies["cookieToken"];
        string? apiToken = Request.Headers.Authorization;

        if (cookieToken == null || apiToken == null) return Ok();

        User user;
        try {
            user = await _db.Users
                            .Where(user => 
                                    user.CookieToken == cookieToken && 
                                    user.ApiToken == apiToken)
                            .SingleAsync();
        } catch (Exception) {
            return Ok();
        }
        
        await _userService.Logout(user);

        Response.Cookies.Delete("cookieToken");

        return Ok();
    }
}
