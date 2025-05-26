using System.CommandLine;

var rootCommand = new RootCommand("Health Tracker CLI");

// Create log command
var logCommand = new Command("log", "Log health data");
logCommand.SetHandler(() =>
{
    Console.WriteLine("log command invoked");
});

// Create log weight sub-subcommand
var logWeightCommand = new Command("weight", "Log weight data");
logWeightCommand.SetHandler(() =>
{
    Console.WriteLine("log weight command invoked");
});
logCommand.AddCommand(logWeightCommand);

// Create view command
var viewCommand = new Command("view", "View health data");
viewCommand.SetHandler(() =>
{
    Console.WriteLine("view command invoked");
});

// Create view weight sub-subcommand
var viewWeightCommand = new Command("weight", "View weight data");
viewWeightCommand.SetHandler(() =>
{
    Console.WriteLine("view weight command invoked");
});
viewCommand.AddCommand(viewWeightCommand);

// Add subcommands to root command
rootCommand.AddCommand(logCommand);
rootCommand.AddCommand(viewCommand);

// System.CommandLine automatically provides --version support based on assembly version
rootCommand.SetHandler(() =>
{
    // Default behavior when no command is specified
    Console.WriteLine("Health Tracker CLI");
    Console.WriteLine("Use --help for usage information.");
});

return await rootCommand.InvokeAsync(args);
