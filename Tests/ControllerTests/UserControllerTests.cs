using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoryTranslatorReactDotnet.Controllers;
using StoryTranslatorReactDotnet.Database;
using StoryTranslatorReactDotnet.Helpers;
using StoryTranslatorReactDotnet.Models;
using StoryTranslatorReactDotnet.Services;
using Xunit;

namespace StoryTranslatorReactDotnet.Tests;

public class SignUpTests : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture _fixture { get; }
    public SignUpTests(TestDatabaseFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task TestSignUpCreatesUser()
    {
        Environment.SetEnvironmentVariable(
            "SecretKey", 
            "A really really really long string to act as a secret key for when the token is generated"
        );

        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);
        UserController controller = new UserController(context, userService, tokenService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        IActionResult? response = await controller.Signup(new LoginData { Username = "username", Password = "password" });

        OkObjectResult? responseResult = Assert.IsType<OkObjectResult>(response);

        dynamic? responseValue = responseResult.Value as dynamic;
        dynamic? apiToken = responseValue?.apiToken;

        Assert.NotNull(apiToken);

        User? user = await context.Users.Where(user => user.Username == "username").FirstOrDefaultAsync();

        Assert.NotNull(user);

        Assert.NotEqual("password", user.Password);
        Assert.Equal(apiToken, user.Tokens.First().ApiToken);
        Assert.NotEqual("", user.Tokens.First().CookieToken);
    }

    [Fact]
    public async Task TestSignUpFailsWhenUsernameTaken()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        User user = new User("username2", "password");

        context.Users.Add(user);
        await context.SaveChangesAsync();

        UserController controller = new UserController(context, userService, tokenService);
        
        IActionResult? response = await controller.Signup(new LoginData { Username = "username2", Password = "password" });

        BadRequestObjectResult? responseResult = Assert.IsType<BadRequestObjectResult>(response);

        Assert.Equal("Username username2 already taken", responseResult.Value);
    }
}

public class LoginTests : IClassFixture<TestDatabaseFixture>
{
    public LoginTests(TestDatabaseFixture fixture) => _fixture = fixture;

    public TestDatabaseFixture _fixture { get; }

    [Fact]
    public async Task TestLoginReturnsApiTokenWhenSuccessful()
    {
        Environment.SetEnvironmentVariable(
            "SecretKey", 
            "A really really really long string to act as a secret key for when the token is generated"
        );

        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);
        
        var user = new User("username3", "password");
        user.Password = new PasswordHasher<User>().HashPassword(user, "password");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        UserController controller = new UserController(context, userService, tokenService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        IActionResult? response = await controller.Login(new LoginData { Username = "username3", Password = "password" });

        OkObjectResult? responseResult = Assert.IsType<OkObjectResult>(response);

        dynamic? responseValue = responseResult.Value as dynamic;
        dynamic? apiToken = responseValue?.apiToken;

        User? dbUser = await context.Users.Where(user => user.Username == "username3").FirstOrDefaultAsync();

        Assert.NotNull(dbUser);
        Assert.Equal(apiToken, dbUser.Tokens.First().ApiToken);
        Assert.NotEqual("", dbUser.Tokens.First().CookieToken);
    }

    [Fact]
    public async Task TestLoginReturnsForbidWhenUserNotFound()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        context.Users.Add(new User("notusername4", "password"));
        await context.SaveChangesAsync();

        UserController controller = new UserController(context, userService, tokenService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        IActionResult? response = await controller.Login(new LoginData { Username = "username4", Password = "password" });

        Assert.IsType<UnauthorizedResult>(response);
    }

    [Fact]
    public async Task TestLoginReturnsForbidWhenPasswordIsWrong()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        var user = new User("username5", "notpassword");
        user.Password = new PasswordHasher<User>().HashPassword(user, "notpassword");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        UserController controller = new UserController(context, userService, tokenService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        IActionResult? response = await controller.Login(new LoginData { Username = "username5", Password = "password" });

        Assert.IsType<UnauthorizedResult>(response);
    }
}

public class ChangePasswordTests : IClassFixture<TestDatabaseFixture>
{
    public ChangePasswordTests(TestDatabaseFixture fixture) => _fixture = fixture;

    public TestDatabaseFixture _fixture { get; }

    [Fact]
    public async Task TestChangePasswordReturnsForbidWithNoApiToken()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        UserController controller = new UserController(context, userService, tokenService);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=fakeCookieToken" };
        controller.ControllerContext.HttpContext = defaultContext;

        ChangePasswordData passwordChangeData = new ChangePasswordData { 
            OldPassword = "OldPassword", 
            NewPassword = "NewPassword" 
        };

        IActionResult? response = await controller.ChangePassword(passwordChangeData);

        Assert.IsType<UnauthorizedResult>(response);
    }

    [Fact]
    public async Task TestChangePasswordReturnsForbidWithNoCookieToken()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        UserController controller = new UserController(context, userService, tokenService);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = "fakeApiToken";
        controller.ControllerContext.HttpContext = defaultContext;

        ChangePasswordData passwordChangeData = new ChangePasswordData { 
            OldPassword = "OldPassword", 
            NewPassword = "NewPassword" 
        };

        IActionResult? response = await controller.ChangePassword(passwordChangeData);

        Assert.IsType<UnauthorizedResult>(response);
    }

    [Fact]
    public async Task TestChangePasswordReturnsForbidWhenNoUserFound()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        UserController controller = new UserController(context, userService, tokenService);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = "fakeApiToken";
        defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=fakeCookieToken" };
        controller.ControllerContext.HttpContext = defaultContext;

        ChangePasswordData passwordChangeData = new ChangePasswordData { 
            OldPassword = "OldPassword", 
            NewPassword = "NewPassword" 
        };

        IActionResult? response = await controller.ChangePassword(passwordChangeData);

        Assert.IsType<UnauthorizedResult>(response);
    }

    [Fact]
    public async Task TestChangePasswordReturnsForbidWhenInvalidApiTokenPassed()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);
        (_, string cookieToken) = Tokens.GenerateTokens();

        User user = new User("username6", "password", "invalidApiToken", cookieToken);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        UserController controller = new UserController(context, userService, tokenService);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = "invalidApiToken";
        defaultContext.Request.Headers["Cookie"] = new[] { $"cookieToken={cookieToken}" };
        controller.ControllerContext.HttpContext = defaultContext;

        ChangePasswordData passwordChangeData = new ChangePasswordData { 
            OldPassword = "OldPassword", 
            NewPassword = "NewPassword" 
        };

        IActionResult? response = await controller.ChangePassword(passwordChangeData);

        Assert.IsType<UnauthorizedResult>(response);

        await context.Entry(user).ReloadAsync();

        Assert.Equal(new List<Token>(), user.Tokens);
    }

    [Fact]
    public async Task TestChangePasswordReturnsForbidWhenInvalidCookieTokenPassed()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);
        (string apiToken, _) = Tokens.GenerateTokens();

        User user = new User("username7", "password", apiToken, "invalidCookieToken");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        UserController controller = new UserController(context, userService, tokenService);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = apiToken;
        defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=invalidCookieToken" };
        controller.ControllerContext.HttpContext = defaultContext;

        ChangePasswordData passwordChangeData = new ChangePasswordData { 
            OldPassword = "OldPassword", 
            NewPassword = "NewPassword" 
        };

        IActionResult? response = await controller.ChangePassword(passwordChangeData);

        Assert.IsType<UnauthorizedResult>(response);

        await context.Entry(user).ReloadAsync();

        Assert.Equal(new List<Token>(), user.Tokens);
    }

    [Fact]
    public async Task TestChangePasswordForbidsWhenPasswordIsWrong()
    {
        Environment.SetEnvironmentVariable(
            "SecretKey", 
            "A really really really long string to act as a secret key for when the token is generated"
        );

        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        var tokenService = _fixture.CreateTokenService(context);

        var (apiToken, cookieToken) = Tokens.GenerateTokens();

        User user = new User("username10", "password", apiToken, cookieToken);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        UserController controller = new UserController(context, userService, tokenService);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = apiToken;
        defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=fakeCookieToken" };
        controller.ControllerContext.HttpContext = defaultContext;

        ChangePasswordData passwordChangeData = new ChangePasswordData { 
            OldPassword = "OldPassword", 
            NewPassword = "NewPassword" 
        };

        IActionResult? response = await controller.ChangePassword(passwordChangeData);

        Assert.IsType<UnauthorizedResult>(response);

        await context.Entry(user).ReloadAsync();

        Assert.Equal(apiToken, user.Tokens.First().ApiToken);
        Assert.Equal(cookieToken, user.Tokens.First().CookieToken);
    }

    [Fact]
    public async Task TestChangePasswordUpdatesUserPassword()
    {
        Environment.SetEnvironmentVariable(
            "SecretKey", 
            "A really really really long string to act as a secret key for when the token is generated"
        );

        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        var tokenService = _fixture.CreateTokenService(context);

        var (apiToken, cookieToken) = Tokens.GenerateTokens();

        User user = new User("username11", "password", apiToken, cookieToken);
        user.Password = new PasswordHasher<User>().HashPassword(user, "password");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        UserController controller = new UserController(context, userService, tokenService);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = apiToken;
        defaultContext.Request.Headers["Cookie"] = new[] { $"cookieToken={cookieToken}" };
        controller.ControllerContext.HttpContext = defaultContext;

        ChangePasswordData passwordChangeData = new ChangePasswordData { 
            OldPassword = "password", 
            NewPassword = "NewPassword" 
        };

        IActionResult? response = await controller.ChangePassword(passwordChangeData);

        Assert.IsType<OkObjectResult>(response);

        await context.Entry(user).ReloadAsync();

        var hasher = new PasswordHasher<User>();
        Assert.Equal(PasswordVerificationResult.Failed, hasher.VerifyHashedPassword(user, user.Password, "password"));
        Assert.Equal(PasswordVerificationResult.Success, hasher.VerifyHashedPassword(user, user.Password, "NewPassword"));
    }
}

public class LogoutTests : IClassFixture<TestDatabaseFixture>
{
    public LogoutTests(TestDatabaseFixture fixture) => _fixture = fixture;

    public TestDatabaseFixture _fixture { get; }

    [Fact]
    public async Task TestLogoutReturnsOkWithNoApiToken()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        UserController controller = new UserController(context, userService, tokenService);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=fakeCookieToken" };
        controller.ControllerContext.HttpContext = defaultContext;

        IActionResult? response = await controller.Logout();

        Assert.IsType<OkResult>(response);
    }

    [Fact]
    public async Task TestLogoutReturnsOkWithNoCookieToken()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        UserController controller = new UserController(context, userService, tokenService);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = "fakeAuthToken";
        controller.ControllerContext.HttpContext = defaultContext;

        IActionResult? response = await controller.Logout();

        Assert.IsType<OkResult>(response);
    }

    [Fact]
    public async Task TestLogoutReturnsOkWhenUserNotFound()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        UserController controller = new UserController(context, userService, tokenService);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = "fakeAuthToken";
        defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=fakeCookieToken" };
        controller.ControllerContext.HttpContext = defaultContext;

        IActionResult? response = await controller.Logout();

        Assert.IsType<OkResult>(response);
    }

    [Fact]
    public async Task TestLogoutRemovesOnlyOneToken()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        UserController controller = new UserController(context, userService, tokenService);

        User user = new User("username12", "password", "fakeApiToken12", "fakeCookieToken12");
        user.Tokens.Add(new Token("fakeApiToken13", "fakeCookieToken13", user));
        context.Users.Add(user);
        await context.SaveChangesAsync();


        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = "fakeApiToken12";
        defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=fakeCookieToken12" };
        controller.ControllerContext.HttpContext = defaultContext;

        IActionResult? response = await controller.Logout();

        Assert.IsType<OkResult>(response);

        Assert.Single(user.Tokens.Where(token => token.ApiToken == "fakeApiToken13"));
        Assert.Single(user.Tokens.Where(token => token.CookieToken == "fakeCookieToken13"));
        Assert.Empty(user.Tokens.Where(token => token.ApiToken == "fakeApiToken12"));
        Assert.Empty(user.Tokens.Where(token => token.CookieToken == "fakeCookieToken12"));
    }

    [Fact]
    public async Task TestLogoutRemovesAllTokens()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        UserController controller = new UserController(context, userService, tokenService);

        User user = new User("username13", "password", "fakeApiToken12", "fakeCookieToken12");
        user.Tokens.Add(new Token("fakeApiToken13", "fakeCookieToken13", user));
        context.Users.Add(user);
        await context.SaveChangesAsync();


        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = "fakeApiToken12";
        defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=fakeCookieToken12" };
        controller.ControllerContext.HttpContext = defaultContext;

        IActionResult? response = await controller.Logout(true);

        Assert.IsType<OkResult>(response);

        Assert.Empty(user.Tokens);
    }
}


public class GetUsernameAndTokenTests : IClassFixture<TestDatabaseFixture>
{
    public GetUsernameAndTokenTests(TestDatabaseFixture fixture) => _fixture = fixture;

    public TestDatabaseFixture _fixture { get; }

    [Fact]
    public async Task TestGetUsernameAndTokenReturnsForbidWithNoCookieToken()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        UserController controller = new UserController(context, userService, tokenService);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext = defaultContext;

        IActionResult? response = await controller.GetUsernameAndToken();

        Assert.IsType<UnauthorizedResult>(response);
    }

    [Fact]
    public async Task TestGetUsernameAndTokenReturnsForbidWithInvalidCookieToken()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        UserController controller = new UserController(context, userService, tokenService);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=invalidCookieToken" };
        controller.ControllerContext.HttpContext = defaultContext;

        IActionResult? response = await controller.GetUsernameAndToken();

        Assert.IsType<UnauthorizedResult>(response);
    }

    [Fact]
    public async Task TestGetUsernameAndTokenReturnsForbidWhenNoTokenFound()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        UserController controller = new UserController(context, userService, tokenService);

        (_, string cookieToken) = Tokens.GenerateTokens();

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers["Cookie"] = new[] { $"cookieToken={cookieToken}" };
        controller.ControllerContext.HttpContext = defaultContext;

        IActionResult? response = await controller.GetUsernameAndToken();

        Assert.IsType<UnauthorizedResult>(response);
    }

    [Fact]
    public async Task TestGetUsernameAndTokenReturnsUsernameAndToken()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        UserController controller = new UserController(context, userService, tokenService);

        (string apiToken, string cookieToken) = Tokens.GenerateTokens();
        
        User user = new User("username14", "password", apiToken, cookieToken);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers["Cookie"] = new[] { $"cookieToken={cookieToken}" };
        controller.ControllerContext.HttpContext = defaultContext;

        IActionResult? response = await controller.GetUsernameAndToken();

        OkObjectResult? responseResult = Assert.IsType<OkObjectResult>(response);

        dynamic? responseValue = responseResult.Value as dynamic;
        dynamic? returnedApiToken = responseValue?.apiToken;
        dynamic? returnedUsername = responseValue?.username;

        Assert.NotNull(returnedApiToken);
        Assert.NotNull(returnedUsername);

        await context.Entry(user).ReloadAsync();

        Assert.Equal(returnedUsername, user.Username);
        Assert.Equal(returnedApiToken, user.Tokens.First().ApiToken);
    }
}
