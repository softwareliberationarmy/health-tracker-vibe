using HealthTracker.Shared.Models;
using Spectre.Console;

namespace HealthTracker.Cli.Commands;

public class AboutCommand
{
    public async Task ExecuteAsync(IAnsiConsole console)
    {
        // For now, return mock data as specified in the requirements
        var mockResponse = new AboutResponse
        {
            ApiVersion = "1.0.0",
            WeighInsCount = 0,
            RunsCount = 0,
            LastWeighInDate = null,
            LastRunDate = null
        };

        await DisplayAboutInformation(console, mockResponse);
    }

    private static async Task DisplayAboutInformation(IAnsiConsole console, AboutResponse aboutInfo)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold blue]Health Tracker CLI[/]");

        table.AddColumn("[bold]Property[/]");
        table.AddColumn("[bold]Value[/]");

        table.AddRow("CLI Version", aboutInfo.ApiVersion);
        table.AddRow("API Version", aboutInfo.ApiVersion);
        table.AddRow("Weigh-ins logged", aboutInfo.WeighInsCount.ToString());
        table.AddRow("Runs logged", aboutInfo.RunsCount.ToString());

        if (aboutInfo.LastWeighInDate.HasValue)
        {
            table.AddRow("Last weigh-in", aboutInfo.LastWeighInDate.Value.ToString("yyyy-MM-dd"));
        }
        else
        {
            table.AddRow("Last weigh-in", "[dim]None[/]");
        }

        if (aboutInfo.LastRunDate.HasValue)
        {
            table.AddRow("Last run", aboutInfo.LastRunDate.Value.ToString("yyyy-MM-dd"));
        }
        else
        {
            table.AddRow("Last run", "[dim]None[/]");
        }

        console.Write(table);
        await Task.CompletedTask;
    }
}
