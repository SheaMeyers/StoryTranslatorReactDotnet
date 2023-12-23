using StoryTranslatorReactDotnet.Database;
using StoryTranslatorReactDotnet.Models;
using StoryTranslatorReactDotnet.Services;
using Xunit;

public class UserServiceTests : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture _fixture { get; }
    public UserServiceTests(TestDatabaseFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task TestUserLogin()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);

        User user = new User("userLogin1", "password");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        (string apiToken, string cookieToken) = await userService.Login(user);
        
        Assert.Single(context.Tokens.Where(token => token.ApiToken == apiToken && token.CookieToken == cookieToken));
        Assert.Equal(apiToken, user.Tokens.First().ApiToken);
        Assert.Equal(cookieToken, user.Tokens.First().CookieToken);
    }

    [Fact]
    public async Task TestUserLogout()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);

        User user = new User("userLogin2", "password", "fakeApiToken1", "fakeCookieToken1");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        await userService.Logout(user);
        
        Assert.Empty(user.Tokens);
        Assert.Empty(context.Tokens.Where(token => token.ApiToken == "fakeApiToken" || token.CookieToken == "fakeCookieToken"));
    }

    [Fact]
    public async Task TestUserGetUserReturnsNullForInvalidTokens()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);

        User user = new User("userLogin3", "password", "fakeApiToken2", "fakeCookieToken2");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        User? retrieveddUser = await userService.GetUser("otherFakeApiToken", "otherFakeCookieToken");
        
        Assert.Null(retrieveddUser);
    }

    [Fact]
    public async Task TestUserGetUserReturnsUserForValidTokens()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        UserService userService = _fixture.CreateUserService(context);

        User user = new User("userLogin4", "password", "fakeApiToken3", "fakeCookieToken3");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        User? retrieveddUser = await userService.GetUser("fakeApiToken3", "fakeCookieToken3");
        
        Assert.Equal(user, retrieveddUser);
    }
}