using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        var context = _fixture.CreateContext();
        var userService = _fixture.CreateUserService(context);
        var tokenService = _fixture.CreateTokenService(context);
        var controller = new UserController(context, userService, tokenService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var response = await controller.Signup(new LoginData { Username = "username", Password = "password" });

        var responseResult = Assert.IsType<OkObjectResult>(response);

        var responseValue = responseResult.Value as dynamic;
        var apiToken = responseValue?.apiToken;

        Assert.NotNull(apiToken);

        var user = context.Users.Where(user => user.Username == "username").FirstOrDefault();

        Assert.NotNull(user);

        Assert.NotEqual("password", user.Password);
        Assert.Equal(apiToken, user.Tokens.First().ApiToken);
        Assert.NotEqual("", user.Tokens.First().CookieToken);
    }

    [Fact]
    public async Task TestSignUpFailsWhenUsernameTaken()
    {
        var context = _fixture.CreateContext();
        var userService = _fixture.CreateUserService(context);
        var tokenService = _fixture.CreateTokenService(context);

        var user = new User("username2", "password");

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new UserController(context, userService, tokenService);
        
        var response = await controller.Signup(new LoginData { Username = "username2", Password = "password" });

        var responseResult = Assert.IsType<BadRequestObjectResult>(response);

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

        var context = _fixture.CreateContext();
        var userService = _fixture.CreateUserService(context);
        var tokenService = _fixture.CreateTokenService(context);
        
        context.Users.Add(new User("username3", "password"));
        await context.SaveChangesAsync();

        var controller = new UserController(context, userService, tokenService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var response = await controller.Login(new LoginData { Username = "username3", Password = "password" });

        var responseResult = Assert.IsType<OkObjectResult>(response);

        var responseValue = responseResult.Value as dynamic;
        var apiToken = responseValue?.apiToken;

        var user = context.Users.Where(user => user.Username == "username3").FirstOrDefault();

        Assert.NotNull(user);
        Assert.Equal(apiToken, user.Tokens.First().ApiToken);
        Assert.NotEqual("", user.Tokens.First().CookieToken);
    }

    [Fact]
    public async Task TestLoginReturnsForbidWhenUserNotFound()
    {
        var context = _fixture.CreateContext();
        var userService = _fixture.CreateUserService(context);
        var tokenService = _fixture.CreateTokenService(context);

        context.Users.Add(new User("notusername4", "password"));
        await context.SaveChangesAsync();

        var controller = new UserController(context, userService, tokenService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var response = await controller.Login(new LoginData { Username = "username4", Password = "password" });

        Assert.IsType<ForbidResult>(response);
    }

    [Fact]
    public async Task TestLoginReturnsForbidWhenPasswordIsWrong()
    {
        var context = _fixture.CreateContext();
        var userService = _fixture.CreateUserService(context);
        var tokenService = _fixture.CreateTokenService(context);

        context.Users.Add(new User("username5", "notpassword"));
        await context.SaveChangesAsync();

        var controller = new UserController(context, userService, tokenService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var response = await controller.Login(new LoginData { Username = "username5", Password = "password" });

        Assert.IsType<ForbidResult>(response);
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
        var tokenService = _fixture.CreateTokenService(context);

        UserController controller = new UserController(context, userService, tokenService);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=fakeCookieToken" };
        controller.ControllerContext.HttpContext = defaultContext;

        ChangePasswordData passwordChangeData = new ChangePasswordData { 
            OldPassword = "OldPassword", 
            NewPassword = "NewPassword" 
        };

        IActionResult? response = await controller.ChangePassword(passwordChangeData);

        Assert.IsType<ForbidResult>(response);
    }

    [Fact]
    public async Task TestChangePasswordReturnsForbidWithNoCookieToken()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        var tokenService = _fixture.CreateTokenService(context);

        UserController controller = new UserController(context, userService, tokenService);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = "fakeApiToken";
        controller.ControllerContext.HttpContext = defaultContext;

        ChangePasswordData passwordChangeData = new ChangePasswordData { 
            OldPassword = "OldPassword", 
            NewPassword = "NewPassword" 
        };

        IActionResult? response = await controller.ChangePassword(passwordChangeData);

        Assert.IsType<ForbidResult>(response);
    }

    [Fact]
    public async Task TestChangePasswordReturnsForbidWhenNoUserFound()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        var tokenService = _fixture.CreateTokenService(context);

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

        Assert.IsType<ForbidResult>(response);
    }

    // [Fact]
    // public async Task TestChangePasswordReturnsLogsUserOutWhenOldApiTokenPassed()
    // {
    //     ApplicationDbContext context = _fixture.CreateContext();
    //     UserService userService = _fixture.CreateUserService(context);
    //     var tokenService = _fixture.CreateTokenService(context);

    //     User user = new User("username6", "password", "fakeApiToken");
    //     user.OldCookieToken.Add("fakeCookieToken");
    //     context.Users.Add(user);
    //     await context.SaveChangesAsync();

    //     UserController controller = new UserController(context, userService, tokenService);

    //     DefaultHttpContext defaultContext = new DefaultHttpContext();
    //     defaultContext.Request.Headers.Authorization = "fakeApiToken";
    //     defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=fakeCookieToken" };
    //     controller.ControllerContext.HttpContext = defaultContext;

    //     ChangePasswordData passwordChangeData = new ChangePasswordData { 
    //         OldPassword = "OldPassword", 
    //         NewPassword = "NewPassword" 
    //     };

    //     IActionResult? response = await controller.ChangePassword(passwordChangeData);

    //     Assert.IsType<ForbidResult>(response);

    //     await context.Entry(user).ReloadAsync();

    //     Assert.Equal("", user.ApiToken);
    //     Assert.Equal("", user.CookieToken);
    //     Assert.Empty(user.OldApiTokens);
    //     Assert.Empty(user.OldCookieToken);
    // }

    // [Fact]
    // public async Task TestChangePasswordReturnsLogsUserOutWhenOldCookieTokenPassed()
    // {
    //     ApplicationDbContext context = _fixture.CreateContext();
    //     UserService userService = _fixture.CreateUserService(context);

    //     User user = new User("username7", "password", cookieToken: "fakeCookieToken");
    //     user.OldApiTokens.Add("fakeApiToken");
    //     context.Users.Add(user);
    //     await context.SaveChangesAsync();

    //     UserController controller = new UserController(context, userService);

    //     DefaultHttpContext defaultContext = new DefaultHttpContext();
    //     defaultContext.Request.Headers.Authorization = "fakeApiToken";
    //     defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=fakeCookieToken" };
    //     controller.ControllerContext.HttpContext = defaultContext;

    //     ChangePasswordData passwordChangeData = new ChangePasswordData { 
    //         OldPassword = "OldPassword", 
    //         NewPassword = "NewPassword" 
    //     };

    //     IActionResult? response = await controller.ChangePassword(passwordChangeData);

    //     Assert.IsType<ForbidResult>(response);

    //     await context.Entry(user).ReloadAsync();

    //     Assert.Equal("", user.ApiToken);
    //     Assert.Equal("", user.CookieToken);
    //     Assert.Empty(user.OldApiTokens);
    //     Assert.Empty(user.OldCookieToken);
    // }

    // [Fact]
    // public async Task TestChangePasswordReturnsLogsUserOutApiTokenIsInvalid()
    // {
    //     Environment.SetEnvironmentVariable(
    //         "SecretKey", 
    //         "A really really really long string to act as a secret key for when the token is generated"
    //     );

    //     ApplicationDbContext context = _fixture.CreateContext();
    //     UserService userService = _fixture.CreateUserService(context);

    //     var (_, cookieToken) = Tokens.GenerateTokens();

    //     User user = new User("username8", "password", "invalidApiToken", cookieToken);
    //     context.Users.Add(user);
    //     await context.SaveChangesAsync();

    //     UserController controller = new UserController(context, userService);

    //     DefaultHttpContext defaultContext = new DefaultHttpContext();
    //     defaultContext.Request.Headers.Authorization = "invalidApiToken";
    //     defaultContext.Request.Headers["Cookie"] = new[] { $"cookieToken={cookieToken}" };
    //     controller.ControllerContext.HttpContext = defaultContext;

    //     ChangePasswordData passwordChangeData = new ChangePasswordData { 
    //         OldPassword = "OldPassword", 
    //         NewPassword = "NewPassword" 
    //     };

    //     IActionResult? response = await controller.ChangePassword(passwordChangeData);

    //     Assert.IsType<ForbidResult>(response);

    //     await context.Entry(user).ReloadAsync();

    //     Assert.Equal("", user.ApiToken);
    //     Assert.Equal("", user.CookieToken);
    //     Assert.Empty(user.OldApiTokens);
    //     Assert.Empty(user.OldCookieToken);
    // }

    // [Fact]
    // public async Task TestChangePasswordReturnsLogsUserOutCookieTokenIsInvalid()
    // {
    //     Environment.SetEnvironmentVariable(
    //         "SecretKey", 
    //         "A really really really long string to act as a secret key for when the token is generated"
    //     );

    //     ApplicationDbContext context = _fixture.CreateContext();
    //     UserService userService = _fixture.CreateUserService(context);

    //     var (apiToken, _) = Tokens.GenerateTokens();

    //     User user = new User("username9", "password", apiToken, "fakeCookieToken");
    //     context.Users.Add(user);
    //     await context.SaveChangesAsync();

    //     UserController controller = new UserController(context, userService);

    //     DefaultHttpContext defaultContext = new DefaultHttpContext();
    //     defaultContext.Request.Headers.Authorization = apiToken;
    //     defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=fakeCookieToken" };
    //     controller.ControllerContext.HttpContext = defaultContext;

    //     ChangePasswordData passwordChangeData = new ChangePasswordData { 
    //         OldPassword = "OldPassword", 
    //         NewPassword = "NewPassword" 
    //     };

    //     IActionResult? response = await controller.ChangePassword(passwordChangeData);

    //     Assert.IsType<ForbidResult>(response);

    //     await context.Entry(user).ReloadAsync();

    //     Assert.Equal("", user.ApiToken);
    //     Assert.Equal("", user.CookieToken);
    //     Assert.Empty(user.OldApiTokens);
    //     Assert.Empty(user.OldCookieToken);
    // }

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

        Assert.IsType<ForbidResult>(response);

        await context.Entry(user).ReloadAsync();

        // Assert.Equal(apiToken, user.ApiToken);
        // Assert.Equal(cookieToken, user.CookieToken);
        // Assert.Empty(user.OldApiTokens);
        // Assert.Empty(user.OldCookieToken);
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
    public async Task TestChangePasswordReturnsOkWithNoApiToken()
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
    public async Task TestChangePasswordReturnsOkWithNoCookieToken()
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
    public async Task TestChangePasswordReturnsOkWhenUserNotFound()
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
    public async Task TestChangePasswordReturnsLogsUserOut()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);
        TokenService tokenService = _fixture.CreateTokenService(context);

        UserController controller = new UserController(context, userService, tokenService);

        User user = new User("username12", "password", "fakeApiToken12", "fakeCookieToken12");
        context.Users.Add(user);
        await context.SaveChangesAsync();


        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = "fakeApiToken12";
        defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=fakeCookieToken12" };
        controller.ControllerContext.HttpContext = defaultContext;

        IActionResult? response = await controller.Logout();

        Assert.IsType<OkResult>(response);

        // Assert.Equal("", user.ApiToken);
        // Assert.Equal("", user.CookieToken);
        // Assert.Empty(user.OldApiTokens);
        // Assert.Empty(user.OldCookieToken);
    }
}
