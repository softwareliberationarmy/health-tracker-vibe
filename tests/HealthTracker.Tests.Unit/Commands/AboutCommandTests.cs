using FluentAssertions;
using HealthTracker.Cli.Commands;
using HealthTracker.Cli.Services;
using HealthTracker.Shared.Models;
using Moq;
using Spectre.Console.Testing;

namespace HealthTracker.Tests.Unit.Commands;

public class AboutCommandTests
{
    [Fact]
    public async Task Execute_WhenCalled_ShouldDisplayVersionInformation()
    {
        // Given
        var console = new TestConsole();
        var mockApiClient = new Mock<IApiClient>();
        mockApiClient.Setup(c => c.GetAboutAsync()).ReturnsAsync(new AboutResponse
        {
            ApiVersion = "1.0.0",
            WeighInsCount = 0,
            RunsCount = 0,
            LastWeighInDate = null,
            LastRunDate = null
        });

        var aboutCommand = new AboutCommand(mockApiClient.Object);

        // When
        await aboutCommand.ExecuteAsync(console);

        // Then
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
        var mockApiClient = new Mock<IApiClient>();
        var action = () => new AboutCommand(mockApiClient.Object);

        // Then
        action.Should().NotThrow();
    }
}
