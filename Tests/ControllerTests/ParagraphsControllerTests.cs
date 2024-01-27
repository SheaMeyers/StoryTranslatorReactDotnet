using Microsoft.AspNetCore.Mvc;
using ReactDotnetCsvUploader.Controllers;
using StoryTranslatorReactDotnet.Database;
using StoryTranslatorReactDotnet.Models;
using StoryTranslatorReactDotnet.Services;
using Xunit;

namespace StoryTranslatorReactDotnet.Tests;

public class GetFirstParagraphTests : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture _fixture { get; }
    public GetFirstParagraphTests(TestDatabaseFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task TestGetFirstParagraphReturns()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        Book book = new Book("Title 1");
        Paragraph paragraph = new Paragraph("English", "Spanish", "French", "German", book);
        context.Books.Add(book);
        context.Paragraphs.Add(paragraph);
        context.Paragraphs.Add(new Paragraph("English 2", "Spanish 2", "French 2", "German 2", book));
        await context.SaveChangesAsync();
        TokenService tokenService = _fixture.CreateTokenService(context);
        ParagraphController controller = new ParagraphController(context, tokenService);
        
        IActionResult? response = await controller.GetFirstParagraph("Title 1", "English", "German");

        OkObjectResult? responseResult = Assert.IsType<OkObjectResult>(response);
        dynamic? responseValue = responseResult.Value;

        Assert.Equal(1, responseValue?.Id);
        Assert.Equal("English", responseValue?.TranslateFrom);
        Assert.Equal("German", responseValue?.TranslateTo);
        Assert.Equal(1, responseValue?.FirstId);
        Assert.Equal(2, responseValue?.LastId);
    }
}


public class GetNextParagraphTests : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture _fixture { get; }
    public GetNextParagraphTests(TestDatabaseFixture fixture) => _fixture = fixture;

    public async Task TestGetNextParagraphWhenNotSignedIn()
    {
        ApplicationDbContext context = _fixture.CreateContext();
        Book book = new Book("Title 1");
        Paragraph paragraph = new Paragraph("English", "Spanish", "French", "German", book);
        context.Books.Add(book);
        context.Paragraphs.Add(paragraph);
        context.Paragraphs.Add(new Paragraph("English 2", "Spanish 2", "French 2", "German 2", book));
        await context.SaveChangesAsync();
        TokenService tokenService = _fixture.CreateTokenService(context);
        ParagraphController controller = new ParagraphController(context, tokenService);

        ParagraphData paragraphData = new ParagraphData
        {
            CurrentId=1,
            Change=1,
            TranslateFrom="English",
            TranslateTo="Spanish"
        };


        IActionResult? response = await controller.GetNextParagraph(paragraphData);
        OkObjectResult? responseResult = Assert.IsType<OkObjectResult>(response);
        dynamic? responseValue = responseResult.Value;

        Assert.Equal(2, responseValue?.Id);
        Assert.Equal("English 2", responseValue?.TranslateFrom);
        Assert.Equal("German 2", responseValue?.TranslateTo);
        Assert.Equal("", responseValue?.UserTranslation);
        Assert.Equal("", responseValue?.ApiToken);
    }
}
