using StoryTranslatorReactDotnet.Database;
using StoryTranslatorReactDotnet.Helpers;
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

public class TestGetTokenTokenServiceTests : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture _fixture { get; }
    public TestGetTokenTokenServiceTests(TestDatabaseFixture fixture) => _fixture = fixture;

    // Everything good returns token object

    [Fact]
    public async Task TestGetTokenReturnsNullWhenNoApiToken()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        TokenService tokenService = _fixture.CreateTokenService(context);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=fakeCookieToken" };
        
        Assert.Null(await tokenService.GetToken(defaultContext.Request));
    }

    [Fact]
    public async Task TestGetTokenReturnsNullWhenNoCookieToken()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        TokenService tokenService = _fixture.CreateTokenService(context);

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = "fakeApiToken";
        
        Assert.Null(await tokenService.GetToken(defaultContext.Request));
    }

    [Fact]
    public async Task TestGetTokenReturnsNullWhenCookieTokenIsInvalid()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        TokenService tokenService = _fixture.CreateTokenService(context);
        (string apiToken, _) = Tokens.GenerateTokens();

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = apiToken;
        defaultContext.Request.Headers["Cookie"] = new[] { "cookieToken=invalidCookieToken" };
        
        Assert.Null(await tokenService.GetToken(defaultContext.Request));
    }

    [Fact]
    public async Task TestGetTokenReturnsNullWhenApiTokenIsInvalid()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        TokenService tokenService = _fixture.CreateTokenService(context);
        (_, string cookieToken) = Tokens.GenerateTokens();

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = "invalidApiToken";
        defaultContext.Request.Headers["Cookie"] = new[] { $"cookieToken={cookieToken}" };
        
        Assert.Null(await tokenService.GetToken(defaultContext.Request));
    }

    [Fact]
    public async Task TestGetTokenReturnsToken()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        TokenService tokenService = _fixture.CreateTokenService(context);
        (string apiToken, string cookieToken) = Tokens.GenerateTokens();

        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Request.Headers.Authorization = apiToken;
        defaultContext.Request.Headers["Cookie"] = new[] { $"cookieToken={cookieToken}" };

        User user = new User("getTokenUsername", "password", apiToken, cookieToken);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        Assert.NotNull(await tokenService.GetToken(defaultContext.Request));
    }
}
