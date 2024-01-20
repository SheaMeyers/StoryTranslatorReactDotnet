using StoryTranslatorReactDotnet.Database;
using StoryTranslatorReactDotnet.Models;
using StoryTranslatorReactDotnet.Services;
using Xunit;

public class TestLogoutTokenServiceTests : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture _fixture { get; }
    public TestLogoutTokenServiceTests(TestDatabaseFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task TestLogoutRemovesToken()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        TokenService tokenService = _fixture.CreateTokenService(context);

        User user = new User("userToken1", "password");
        Token token = new Token("fakeApiToken", "fakeCookieToken", user);
        context.Users.Add(user);
        context.Tokens.Add(token);
        await context.SaveChangesAsync();

        await tokenService.Logout("fakeApiToken", "fakeCookieToken");
        
        Assert.Empty(context.Tokens.Where(token => token.ApiToken == "fakeApiToken" && token.CookieToken == "fakeCookieToken"));
    }

    [Fact]
    public async Task TestLogoutDoesNothingWithInvalidToken()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        TokenService tokenService = _fixture.CreateTokenService(context);

        User user = new User("userToken2", "password");
        Token token = new Token("fakeApiToken2", "fakeCookieToken2", user);
        context.Users.Add(user);
        context.Tokens.Add(token);
        await context.SaveChangesAsync();

        await tokenService.Logout("invalidFakeApiToken", "invalidFakeCookieToken");
        
        Assert.Single(context.Tokens.Where(token => token.ApiToken == "fakeApiToken2" && token.CookieToken == "fakeCookieToken2"));
    }
}

public class TestRegenerateTokenServiceTests : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture _fixture { get; }
    public TestRegenerateTokenServiceTests(TestDatabaseFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task TestRegenerateUpdatesTokenValues()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        TokenService tokenService = _fixture.CreateTokenService(context);

        User user = new User("userToken3", "password");
        Token token = new Token("initialApiToken", "initialCookieToken", user);
        context.Users.Add(user);
        context.Tokens.Add(token);
        await context.SaveChangesAsync();

        (string updatedApiToken, string updatedCookieToken) = await tokenService.Regenerate("initialApiToken", "initialCookieToken");
        
        Assert.Single(context.Tokens.Where(token => token.ApiToken == updatedApiToken && token.CookieToken == updatedCookieToken));
        Assert.Empty(context.Tokens.Where(token => token.ApiToken == "initialApiToken" && token.CookieToken == "initialCookieToken"));
    }
}