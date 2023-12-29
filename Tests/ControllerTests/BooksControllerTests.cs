using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using ReactDotnetCsvUploader.Controllers;
using StoryTranslatorReactDotnet.Controllers;
using StoryTranslatorReactDotnet.Database;
using StoryTranslatorReactDotnet.Helpers;
using StoryTranslatorReactDotnet.Models;
using StoryTranslatorReactDotnet.Services;
using Xunit;

namespace StoryTranslatorReactDotnet.Tests;

public class GetBooksTests : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture _fixture { get; }
    public GetBooksTests(TestDatabaseFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task TestGetBooksReturns()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        context.Books.Add(new Book("Title 1"));
        context.Books.Add(new Book("Title 2"));
        await context.SaveChangesAsync();
        BooksController controller = new BooksController(context);
        
        IActionResult? response = controller.Get();

        Assert.IsType<OkObjectResult>(response);
    }
}

