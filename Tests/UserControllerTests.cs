using Microsoft.AspNetCore.Mvc;
using StoryTranslatorReactDotnet.Controllers;
using Xunit;

namespace StoryTranslatorReactDotnet.Tests;

public class UserControllerTests : IClassFixture<TestDatabaseFixture>
{
    public UserControllerTests(TestDatabaseFixture fixture) => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }

    [Fact]
    public async Task FakeTest()
    {
        var context = Fixture.CreateContext();
        var controller = new UserController(context);

        var response = controller.Test();

        var responseResult = Assert.IsType<OkObjectResult>(response);

        var responseValue = responseResult.Value as dynamic;

        Console.WriteLine("-----------");
        Console.WriteLine("test", responseValue?.test);
        Console.WriteLine("-----------");
    }
}