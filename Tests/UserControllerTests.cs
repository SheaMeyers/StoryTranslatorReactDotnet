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

        var result = controller.Test();

        var requestResult = Assert.IsType<OkObjectResult>(result);
        Console.WriteLine("-----------");
        Console.WriteLine(requestResult.Value);
        Console.WriteLine("-----------");
    }
}