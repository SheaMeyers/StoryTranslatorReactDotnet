using StoryTranslatorReactDotnet.Database;
using Microsoft.EntityFrameworkCore;
using StoryTranslatorReactDotnet.Services;

public class TestDatabaseFixture
{
    private const string ConnectionString = "User ID=storytranslatorreactdotnet;Password=storytranslatorreactdotnet;Host=localhost;Port=5432;Database=storytranslatorreactdotnettest;Pooling=true;";
    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }
    }

    public ApplicationDbContext CreateContext()
        => new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(ConnectionString)
                .Options);
    
    public UserService CreateUserService(ApplicationDbContext db) => new UserService(db);
    public TokenService CreateTokenService(ApplicationDbContext db) => new TokenService(db);
}
