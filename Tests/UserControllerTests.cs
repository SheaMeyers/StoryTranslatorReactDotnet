using Xunit;

namespace StoryTranslatorReactDotnet.Tests;

public class CreatePageTests : IClassFixture<TestDatabaseFixture>
{
    public CreatePageTests(TestDatabaseFixture fixture) => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }

    [Fact]
    public void FakeTest()
    {
        Assert.Equal(1, 1);
    }
}