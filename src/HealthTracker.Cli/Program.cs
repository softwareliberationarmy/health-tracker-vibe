using System.CommandLine;

var rootCommand = new RootCommand("Health Tracker CLI");

// System.CommandLine automatically provides --version support based on assembly version
rootCommand.SetHandler(() =>
{
    // Default behavior when no command is specified
    Console.WriteLine("Health Tracker CLI");
    Console.WriteLine("Use --help for usage information.");
});

return await rootCommand.InvokeAsync(args);
