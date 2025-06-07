using System.CommandLine;
using HealthTracker.Cli.Commands;
using Spectre.Console;

namespace HealthTracker.Cli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = CreateRootCommand();
        return await rootCommand.InvokeAsync(args);
    }

    public static RootCommand CreateRootCommand()
    {
        var rootCommand = new RootCommand("Health Tracker CLI - Track your health data including weigh-ins and runs");

        var aboutOption = new Option<bool>(
            name: "--about",
            description: "Show version and statistics information");
        rootCommand.AddOption(aboutOption);

        rootCommand.SetHandler(async (bool about) =>
        {
            if (about)
            {
                var aboutCommand = new AboutCommand();
                await aboutCommand.ExecuteAsync(AnsiConsole.Console);
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]No command specified. Use --help for available options.[/]");
            }
        }, aboutOption);

        return rootCommand;
    }
}
