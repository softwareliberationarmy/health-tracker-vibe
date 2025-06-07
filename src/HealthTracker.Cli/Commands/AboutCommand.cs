using HealthTracker.Cli.Services;
using HealthTracker.Shared.Models;
using Spectre.Console;

namespace HealthTracker.Cli.Commands;

public class AboutCommand
{
    private readonly IApiClient _apiClient;

    public AboutCommand(IApiClient apiClient)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    }

    public async Task ExecuteAsync(IAnsiConsole console)
    {
        var aboutResponse = await _apiClient.GetAboutAsync();

        if (aboutResponse == null)
        {
            console.MarkupLine("[bold red]Error:[/] API is unreachable. Please ensure the API is running and try again.");
            return;
        }

        await DisplayAboutInformation(console, aboutResponse);
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
