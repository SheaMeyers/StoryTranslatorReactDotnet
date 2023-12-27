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
    private readonly TokenService _tokenService;

    public UserController(ApplicationDbContext db, UserService userService, TokenService tokenService)
    {
        _db = db;
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> Signup([FromBody] LoginData loginData)
    {
        if (!ModelState.IsValid) return BadRequest();

        if (_db.Users.Any(user => user.Username == loginData.Username)) 
            return BadRequest($"Username {loginData.Username} already taken");

        (string apiToken, string cookieToken) = Tokens.GenerateTokens();

        User user = new User(loginData.Username, loginData.Password, apiToken, cookieToken);
        user.Password = new PasswordHasher<User>().HashPassword(user, loginData.Password);
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

        if (user == null) return Unauthorized();

        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, loginData.Password) == PasswordVerificationResult.Failed)
            return Unauthorized();

        (string apiToken, string cookieToken) = await _userService.Login(user);

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

        if (cookieToken == null || apiToken == null) return Unauthorized();

        User? user = await _userService.GetUser(apiToken, cookieToken);

        if (user == null) return Unauthorized();
        
        var isCookieTokenValid = await Tokens.ValidateToken(cookieToken);
        var isApiTokenValid = await Tokens.ValidateToken(apiToken);

        if (isCookieTokenValid == false || isApiTokenValid == false) {
            await _tokenService.Logout(apiToken, cookieToken);
            return Unauthorized();
        }

        var hasher = new PasswordHasher<User>();
        if (hasher.VerifyHashedPassword(user, user.Password, changePasswordData.OldPassword) == PasswordVerificationResult.Failed)
            return Unauthorized();

        user.Password = hasher.HashPassword(user, changePasswordData.NewPassword);
        _db.Users.Update(user);
        await _db.SaveChangesAsync();

        (apiToken, cookieToken) = await _tokenService.Regenerate(apiToken, cookieToken);

        Response.Cookies.Append("cookieToken", cookieToken, new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(new { apiToken });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromQuery] bool logoutAll = false)
    {
        string? cookieToken = Request.Cookies["cookieToken"];
        string? apiToken = Request.Headers.Authorization;

        if (cookieToken == null || apiToken == null) return Ok();

        Token? token = await _db.Tokens
                                    .Where(token => 
                                            token.ApiToken == apiToken && 
                                            token.CookieToken == cookieToken)
                                    .SingleOrDefaultAsync();

        if (token == null) return Ok();
            
        if (logoutAll) {
            User? user = await _userService.GetUser(apiToken, cookieToken);
            if (user == null) {
                _db.Tokens.Remove(token);
                await _db.SaveChangesAsync();
            } else {
                await _userService.Logout(user);
            }
        } else {
            _db.Tokens.Remove(token);
            await _db.SaveChangesAsync();
        }

        Response.Cookies.Delete("cookieToken");

        return Ok();
    }

    [HttpGet("get-username-and-token")]
    public async Task<IActionResult> GetUsernameAndToken()
    {
        string? cookieToken = Request.Cookies["cookieToken"];

        if (cookieToken == null) return Unauthorized();

        var isCookieTokenValid = await Tokens.ValidateToken(cookieToken);

        if (isCookieTokenValid == false) return Unauthorized();

        Token? token = await _db.Tokens
                                    .Include(token => token.User)
                                    .Where(token => token.CookieToken == cookieToken)
                                    .SingleOrDefaultAsync();

        if (token == null) return Unauthorized();

        (string apiToken, string newCookieToken) = await _tokenService.Regenerate(token.ApiToken, token.CookieToken);

        Response.Cookies.Append("cookieToken", newCookieToken, new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(new { apiToken, username = token.User.Username });
    }
}
