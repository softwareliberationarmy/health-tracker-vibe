using System.CommandLine;

var rootCommand = new RootCommand("Health Tracker CLI");

// Don't add a manual version option - use the built-in one
rootCommand.SetHandler(() =>
{
    // Default behavior when no options are specified
    Console.WriteLine("Health Tracker CLI");
    Console.WriteLine("Use --help for usage information.");
});

return await rootCommand.InvokeAsync(args);
