using Microsoft.AspNetCore.Mvc;
using StoryTranslatorReactDotnet.Controllers;
using StoryTranslatorReactDotnet.Models;
using Xunit;

namespace StoryTranslatorReactDotnet.Tests;

public class UserControllerTests : IClassFixture<TestDatabaseFixture>
{
    public UserControllerTests(TestDatabaseFixture fixture) => Fixture = fixture;

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

        Assert.NotNull(responseValue?.apiToken);

        var user = context.Users.Where(user => user.Username == "username").FirstOrDefault();

        Assert.NotNull(user);
        Assert.NotEqual("password", user.Password);
        Assert.NotEqual("", user.ApiToken);
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