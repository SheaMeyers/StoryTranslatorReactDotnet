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
    public async Task<IActionResult> Signup([FromBody] LoginData loginData)
    {
        var user = new User(loginData.Username, loginData.Password);

        (string ApiToken, string CookieToken) = Tokens.GenerateToken();

        var result = await Tokens.ValidateToken(ApiToken);

        return Ok(new { ApiToken, CookieToken, result });
    }
}
