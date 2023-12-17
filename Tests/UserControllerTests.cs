using Microsoft.AspNetCore.Mvc;
using StoryTranslatorReactDotnet.Controllers;
using Xunit;

namespace StoryTranslatorReactDotnet.Tests;

public class UserControllerTests : IClassFixture<TestDatabaseFixture>
{
    public UserControllerTests(TestDatabaseFixture fixture) => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }

    [Fact]
    public async Task TestSignUp()
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
    }
}