using System.Diagnostics;
using FluentAssertions;

namespace HealthTracker.Tests;

public class VersionCommandTests
{
    [Fact]
    public void WhenVersionArgumentProvided_ShouldOutputVersionNumber()
    {        // Arrange
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run --project src/HealthTracker.Cli -- --version",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = @"d:\git\sideprojects\health-tracker"
        };

        // Act
        using var process = Process.Start(processStartInfo);
        process.Should().NotBeNull();
        
        var output = process!.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();        // Assert
        output.Should().Be($"0.1.0{Environment.NewLine}", "CLI should output exactly '0.1.0' followed by a newline, but got: '{0}'", output.Replace("\n", "\\n").Replace("\r", "\\r"));
        process.ExitCode.Should().Be(0, "CLI should exit successfully. Error output: {0}", error);
        error.Should().BeEmpty("There should be no error output");
    }
}
