using FluentAssertions;
using HealthTracker.Cli;
using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.Parsing;

namespace HealthTracker.Tests.Unit;

public class ProgramTests
{
    [Fact]
    public void CreateRootCommand_WhenCalled_ShouldHaveAboutOption()
    {
        // Given/When
        var rootCommand = Program.CreateRootCommand();

        // Then
        rootCommand.Should().NotBeNull();
        var aboutOption = rootCommand.Options.FirstOrDefault(o => o.Name == "about");
        aboutOption.Should().NotBeNull();
        aboutOption!.Description.Should().Be("Show version and statistics information");
    }
    [Fact]
    public async Task RootCommand_WithAboutOption_ShouldCallAboutHandler()
    {
        // Given
        var rootCommand = Program.CreateRootCommand();

        // When
        var result = await rootCommand.InvokeAsync("--about");

        // Then
        result.Should().Be(0);
        // The main test is that the command parsing works and doesn't throw.
        // Output testing is done in AboutCommandTests with mocked console.
    }
}
