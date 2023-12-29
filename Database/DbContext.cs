using Microsoft.EntityFrameworkCore;
using StoryTranslatorReactDotnet.Models;

namespace StoryTranslatorReactDotnet.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Paragraph> Paragraphs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
    }
}
