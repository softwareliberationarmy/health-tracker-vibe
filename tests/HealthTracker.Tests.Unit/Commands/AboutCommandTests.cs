using FluentAssertions;
using HealthTracker.Cli.Commands;
using Spectre.Console.Testing;

namespace HealthTracker.Tests.Unit.Commands;

public class AboutCommandTests
{
    [Fact]
    public async Task Execute_WhenCalled_ShouldDisplayVersionInformation()
    {        // Given
        var console = new TestConsole();
        var aboutCommand = new AboutCommand();

        // When
        await aboutCommand.ExecuteAsync(console);// Then
        var output = console.Output;
        output.Should().Contain("Health Tracker CLI");
        output.Should().Contain("1.0.0");
        output.Should().Contain("Weigh-ins logged");
        output.Should().Contain("Runs logged");
    }

    [Fact]
    public void Constructor_WhenCreated_ShouldNotThrow()
    {
        // Given/When
        var action = () => new AboutCommand();

        // Then
        action.Should().NotThrow();
    }
}
