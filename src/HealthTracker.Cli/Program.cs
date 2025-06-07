using System.CommandLine;
using System.Reflection;
using HealthTracker.Cli.Commands;
using HealthTracker.Cli.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace HealthTracker.Cli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Set up configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        // Set up dependency injection
        var services = new ServiceCollection();
        ConfigureServices(services, configuration);

        // Build the service provider
        var serviceProvider = services.BuildServiceProvider();

        // Create root command with services
        var rootCommand = CreateRootCommand(serviceProvider);

        return await rootCommand.InvokeAsync(args);
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add configuration
        services.AddSingleton(configuration);

        // Configure API settings
        var apiSettings = new ApiSettings();
        configuration.GetSection("ApiSettings").Bind(apiSettings);
        services.AddSingleton(apiSettings);

        // Add HttpClient
        services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiSettings.BaseUrl);
        });

        // Add commands
        services.AddTransient<AboutCommand>();

        // Add console
        services.AddSingleton<IAnsiConsole>(AnsiConsole.Console);
    }

    public static RootCommand CreateRootCommand(ServiceProvider serviceProvider)
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
                var aboutCommand = serviceProvider.GetRequiredService<AboutCommand>();
                await aboutCommand.ExecuteAsync(serviceProvider.GetRequiredService<IAnsiConsole>());
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]No command specified. Use --help for available options.[/]");
            }
        }, aboutOption);

        return rootCommand;
    }
}
