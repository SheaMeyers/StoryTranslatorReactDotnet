using Microsoft.AspNetCore.Mvc;
using StoryTranslatorReactDotnet.Controllers;
using StoryTranslatorReactDotnet.Models;
using Xunit;

namespace StoryTranslatorReactDotnet.Tests;

public class SignUpTests : IClassFixture<TestDatabaseFixture>
{
    public SignUpTests(TestDatabaseFixture fixture) => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }

    [Fact]
    public async Task TestSignUpCreatesUser()
    {
        Environment.SetEnvironmentVariable(
            "SecretKey", 
            "A really really really long string to act as a secret key for when the token is generated"
        );

        var context = Fixture.CreateContext();
        var controller = new UserController(context);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var response = await controller.Signup(new LoginData { Username = "username", Password = "password" });

        var responseResult = Assert.IsType<OkObjectResult>(response);

        var responseValue = responseResult.Value as dynamic;
        var apiToken = responseValue?.apiToken;

        Assert.NotNull(apiToken);

        var user = context.Users.Where(user => user.Username == "username").FirstOrDefault();

        Assert.NotNull(user);

        Assert.NotEqual("password", user.Password);
        Assert.Equal(apiToken, user.ApiToken);
        Assert.NotEqual("", user.CookieToken);
    }

    [Fact]
    public async Task TestSignUpFailsWhenUsernameTaken()
    {
        var context = Fixture.CreateContext();
        var user = new User("username2", "password");

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new UserController(context);
        
        var response = await controller.Signup(new LoginData { Username = "username2", Password = "password" });

        var responseResult = Assert.IsType<BadRequestObjectResult>(response);

        Assert.Equal("Username username2 already taken", responseResult.Value);
    }
}

public class LoginTests : IClassFixture<TestDatabaseFixture>
{
    public LoginTests(TestDatabaseFixture fixture) => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }

    [Fact]
    public async Task TestLoginReturnsApiTokenWhenSuccessful()
    {
        Environment.SetEnvironmentVariable(
            "SecretKey", 
            "A really really really long string to act as a secret key for when the token is generated"
        );

        var context = Fixture.CreateContext();

        context.Users.Add(new User("username3", "password"));
        await context.SaveChangesAsync();

        var controller = new UserController(context);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var response = await controller.Login(new LoginData { Username = "username3", Password = "password" });

        var responseResult = Assert.IsType<OkObjectResult>(response);

        var responseValue = responseResult.Value as dynamic;
        var apiToken = responseValue?.apiToken;

        var user = context.Users.Where(user => user.Username == "username").FirstOrDefault();

        Assert.NotNull(user);
        Assert.Equal(apiToken, user.ApiToken);
        Assert.NotEqual("", user.CookieToken);
    }

    [Fact]
    public async Task TestLoginReturnsForbidWhenUserNotFound()
    {
        var context = Fixture.CreateContext();

        context.Users.Add(new User("notusername4", "password"));
        await context.SaveChangesAsync();

        var controller = new UserController(context);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var response = await controller.Login(new LoginData { Username = "username4", Password = "password" });

        Assert.IsType<ForbidResult>(response);
    }

    [Fact]
    public async Task TestLoginReturnsForbidWhenPasswordIsWrong()
    {
        var context = Fixture.CreateContext();

        context.Users.Add(new User("username5", "notpassword"));
        await context.SaveChangesAsync();

        var controller = new UserController(context);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        var response = await controller.Login(new LoginData { Username = "username5", Password = "password" });

        Assert.IsType<ForbidResult>(response);
    }
}