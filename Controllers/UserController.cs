using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using StoryTranslatorReactDotnet.Models;
using StoryTranslatorReactDotnet.Helpers;

namespace StoryTranslatorReactDotnet.Controllers;

public class LoginData 
{
    public string Username {get; set;}
    public string Password {get; set;}
}

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    [HttpPost("sign-up")]
    public IActionResult Signup([FromBody] LoginData loginData)
    {
        (string apiToken, string cookieToken) = Tokens.GenerateToken();

        var user = new User(loginData.Username, loginData.Password, apiToken, cookieToken);

        Response.Cookies.Append("cookieToken", cookieToken, new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(new { apiToken });
    }
}
