using FluentAssertions;
using HealthTracker.Cli.Commands;
using HealthTracker.Cli.Services;
using HealthTracker.Shared.Models;
using Moq;
using Spectre.Console;
using Spectre.Console.Testing;

namespace HealthTracker.Tests.Unit.Commands;

public class AboutCommandWithApiTests
{
    [Fact]
    public async Task ExecuteAsync_WhenApiReturnsData_ShouldDisplayApiData()
    {
        // Given
        var testConsole = new TestConsole();
        var mockApiClient = new Mock<IApiClient>();
        var mockResponse = new AboutResponse
        {
            ApiVersion = "1.2.3",
            WeighInsCount = 42,
            RunsCount = 10,
            LastWeighInDate = new DateTime(2025, 6, 1),
            LastRunDate = new DateTime(2025, 6, 2)
        };
        mockApiClient.Setup(c => c.GetAboutAsync()).ReturnsAsync(mockResponse);

        var command = new AboutCommand(mockApiClient.Object);

        // When
        await command.ExecuteAsync(testConsole);

        // Then
        var output = testConsole.Output;
        output.Should().Contain("Health Tracker CLI");
        output.Should().Contain("1.2.3"); // API version from mock
        output.Should().Contain("42"); // Weigh-ins count from mock
        output.Should().Contain("10"); // Runs count from mock
        output.Should().Contain("2025-06-01"); // Last weigh-in date
        output.Should().Contain("2025-06-02"); // Last run date
        mockApiClient.Verify(c => c.GetAboutAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenApiIsUnavailable_ShouldDisplayErrorMessage()
    {
        // Given
        var testConsole = new TestConsole();
        var mockApiClient = new Mock<IApiClient>();
        mockApiClient.Setup(c => c.GetAboutAsync()).ReturnsAsync((AboutResponse?)null);

        var command = new AboutCommand(mockApiClient.Object);

        // When
        await command.ExecuteAsync(testConsole);

        // Then
        var output = testConsole.Output;
        output.Should().Contain("Error");
        output.Should().Contain("API is unreachable");
        mockApiClient.Verify(c => c.GetAboutAsync(), Times.Once);
    }
}
