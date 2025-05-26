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
        // Arrange & Act
        var (exitCode, output, _) = RunCliCommand("log");

        // Assert
        exitCode.Should().Be(0, "log command should be recognized and not error");
        output.Should().Contain("Log command invoked",
            "Output should contain information about the log command or placeholder message");
    }

    [Fact]
    public void WhenViewCommandProvided_ShouldNotThrowAndShowViewHelp()
    {
        // Arrange & Act
        var (exitCode, output, _) = RunCliCommand("view");

        // Assert
        exitCode.Should().Be(0, "view command should be recognized and not error");
        output.Should().Contain("View command invoked",
            "Output should contain information about the view command or placeholder message");
    }

    [Fact]
    public void WhenLogWeightCommandProvided_ShouldReachPlaceholderHandler()
    {
        // Arrange & Act
        var (exitCode, output, _) = RunCliCommand("log weight");

        // Assert
        exitCode.Should().Be(0, "log weight command should be recognized and not error");
        output.Should().Contain("Log weight invoked",
            "Output should indicate the log weight command was reached");
    }

    [Fact]
    public void WhenViewWeightCommandProvided_ShouldReachPlaceholderHandler()
    {
        // Arrange & Act
        var (exitCode, output, _) = RunCliCommand("view weight");

        // Assert
        exitCode.Should().Be(0, "view weight command should be recognized and not error");
        output.Should().Contain("View weight invoked",
            "Output should indicate the view weight command was reached");
    }

    [Fact]
    public void WhenLogRunCommandProvided_ShouldReachPlaceholderHandler()
    {
        // Arrange & Act
        var (exitCode, output, _) = RunCliCommand("log run");

        // Assert
        exitCode.Should().Be(0, "log run command should be recognized and not error");
        output.Should().Contain("Log run invoked",
            "Output should indicate the log run command was reached");
    }

    [Fact]
    public void WhenViewRunCommandProvided_ShouldReachPlaceholderHandler()
    {
        // Arrange & Act
        var (exitCode, output, _) = RunCliCommand("view run");

        // Assert
        exitCode.Should().Be(0, "view run command should be recognized and not error");
        output.Should().Contain("View run invoked",
            "Output should indicate the view run command was reached");
    }

    [Fact]
    public void WhenVerboseOptionProvidedBeforeSubcommand_ShouldIndicateVerboseModeEnabled()
    {
        // Arrange & Act
        var (exitCode, output, _) = RunCliCommand("--verbose log weight");

        // Assert
        exitCode.Should().Be(0, "verbose option should be recognized and not error");
        output.Should().Contain("Log weight invoked",
            "Output should indicate the log weight command was reached");
        output.Should().Contain("Verbose mode ON",
            "Output should indicate verbose mode is enabled when --verbose flag is provided");
    }

    [Fact]
    public void WhenVerboseOptionProvidedAfterSubcommand_ShouldIndicateVerboseModeEnabled()
    {
        // Arrange & Act
        var (exitCode, output, _) = RunCliCommand("log weight --verbose");

        // Assert
        exitCode.Should().Be(0, "verbose option should be recognized and not error");
        output.Should().Contain("Log weight invoked",
            "Output should indicate the log weight command was reached");
        output.Should().Contain("Verbose mode ON",
            "Output should indicate verbose mode is enabled when --verbose flag is provided");
    }
    [Fact]
    public void WhenHelpOptionProvided_ShouldShowRootCommandHelpIncludingLogAndView()
    {
        // Arrange & Act
        var (exitCode, output, _) = RunCliCommand("--help");

        // Assert
        exitCode.Should().Be(0, "help command should exit successfully");
        output.Should().Contain("Health Tracker CLI",
            "Help output should contain the application description");
        output.Should().Contain("log", "Help output should list the log command");
        output.Should().Contain("view", "Help output should list the view command");
        output.Should().Contain("--verbose", "Help output should list the global verbose option");
        output.Should().Contain("--help", "Help output should show help option");
    }

    [Fact]
    public void WhenLogHelpRequested_ShouldShowLogCommandHelpIncludingWeightAndRun()
    {
        // Arrange & Act
        var (exitCode, output, _) = RunCliCommand("log --help");

        // Assert
        exitCode.Should().Be(0, "log help command should exit successfully");
        output.Should().Contain("Log health data",
            "Help output should contain the log command description");
        output.Should().Contain("weight", "Help output should list the weight subcommand");
        output.Should().Contain("run", "Help output should list the run subcommand");
        output.Should().Contain("--verbose", "Help output should show the global verbose option");
    }

    [Fact]
    public void WhenViewHelpRequested_ShouldShowViewCommandHelpIncludingWeightAndRun()
    {
        // Arrange & Act
        var (exitCode, output, _) = RunCliCommand("view --help");

        // Assert
        exitCode.Should().Be(0, "view help command should exit successfully");
        output.Should().Contain("View health data",
            "Help output should contain the view command description");
        output.Should().Contain("weight", "Help output should list the weight subcommand");
        output.Should().Contain("run", "Help output should list the run subcommand");
        output.Should().Contain("--verbose", "Help output should show the global verbose option");
    }

    [Fact]
    public void WhenInvalidCommandProvided_ShouldResultInErrorMessageAndNonZeroExitCode()
    {
        // Arrange & Act
        var (exitCode, output, error) = RunCliCommand("an-invalid-command");

        // Assert
        exitCode.Should().NotBe(0, "invalid command should result in non-zero exit code");

        // System.CommandLine typically outputs error messages to stderr
        var combinedOutput = output + error;
        combinedOutput.Should().Contain("an-invalid-command",
            "Error output should reference the invalid command");
    }

    private (int ExitCode, string Output, string Error) RunCliCommand(string arguments)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project src/HealthTracker.Cli -- {arguments}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = _workingDirectory
        };

        using var process = Process.Start(processStartInfo);
        process.Should().NotBeNull();

        var output = process!.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return (process.ExitCode, output, error);
    }
}
