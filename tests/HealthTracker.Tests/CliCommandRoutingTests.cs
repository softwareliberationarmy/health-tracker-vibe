using System.Diagnostics;
using FluentAssertions;

namespace HealthTracker.Tests;

public class CliCommandRoutingTests
{
    private readonly string _workingDirectory;

    public CliCommandRoutingTests()
    {
        _workingDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
    }

    [Fact]
    public void WhenLogCommandProvided_ShouldNotThrowAndShowLogHelp()
    {
        // Arrange
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run --project src/HealthTracker.Cli -- log",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = _workingDirectory
        };

        // Act
        using var process = Process.Start(processStartInfo);
        process.Should().NotBeNull();

        var output = process!.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();        // Assert
        process.ExitCode.Should().Be(0, "log command should be recognized and not error");
        output.Should().Contain("log",
            "Output should contain information about the log command or placeholder message");
    }

    [Fact]
    public void WhenViewCommandProvided_ShouldNotThrowAndShowViewHelp()
    {
        // Arrange
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run --project src/HealthTracker.Cli -- view",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = _workingDirectory
        };

        // Act
        using var process = Process.Start(processStartInfo);
        process.Should().NotBeNull();

        var output = process!.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();        // Assert
        process.ExitCode.Should().Be(0, "view command should be recognized and not error");
        output.Should().Contain("view",
            "Output should contain information about the view command or placeholder message");
    }

    [Fact]
    public void WhenLogWeightCommandProvided_ShouldReachPlaceholderHandler()
    {
        // Arrange
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run --project src/HealthTracker.Cli -- log weight",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = _workingDirectory
        };

        // Act
        using var process = Process.Start(processStartInfo);
        process.Should().NotBeNull();

        var output = process!.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();        // Assert
        process.ExitCode.Should().Be(0, "log weight command should be recognized and not error");
        output.Should().Contain("weight",
            "Output should indicate the log weight command was reached");
    }

    [Fact]
    public void WhenViewWeightCommandProvided_ShouldReachPlaceholderHandler()
    {
        // Arrange
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run --project src/HealthTracker.Cli -- view weight",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = _workingDirectory
        };

        // Act
        using var process = Process.Start(processStartInfo);
        process.Should().NotBeNull();

        var output = process!.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();        // Assert
        process.ExitCode.Should().Be(0, "view weight command should be recognized and not error");
        output.Should().Contain("weight",
            "Output should indicate the view weight command was reached");
    }
}
