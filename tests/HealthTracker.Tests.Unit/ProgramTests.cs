using FluentAssertions;
using HealthTracker.Cli;
using HealthTracker.Cli.Commands;
using HealthTracker.Cli.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.Parsing;

namespace HealthTracker.Tests.Unit;

public class ProgramTests
{
    [Fact]
    public void CreateRootCommand_WhenCalled_ShouldHaveAboutOption()
    {
        // Given
        var serviceProvider = CreateTestServiceProvider();

        // When
        var rootCommand = Program.CreateRootCommand(serviceProvider);

        // Then
        rootCommand.Should().NotBeNull();
        var aboutOption = rootCommand.Options.FirstOrDefault(o => o.Name == "about");
        aboutOption.Should().NotBeNull();
        aboutOption!.Description.Should().Be("Show version and statistics information");
    }

    [Fact(Skip = "This test needs to be refactored to work with dependency injection")]
    public async Task RootCommand_WithAboutOption_ShouldCallAboutHandler()
    {
        // Given
        var serviceProvider = CreateTestServiceProvider();
        var rootCommand = Program.CreateRootCommand(serviceProvider);

        // When
        var result = await rootCommand.InvokeAsync("--about");

        // Then
        result.Should().Be(0);
        // The main test is that the command parsing works and doesn't throw.
        // Output testing is done in AboutCommandTests with mocked console.
    }

    private static ServiceProvider CreateTestServiceProvider()
    {
        var services = new ServiceCollection();

        // Add mock services
        var mockApiClient = new Mock<IApiClient>();
        services.AddSingleton(mockApiClient.Object);

        var mockAboutCommand = new Mock<AboutCommand>(mockApiClient.Object);
        services.AddSingleton(mockAboutCommand.Object);

        services.AddSingleton<Spectre.Console.IAnsiConsole>(new Spectre.Console.Testing.TestConsole());

        return services.BuildServiceProvider();
    }
}
