using System.CommandLine;
using System.CommandLine.Invocation;

const string VerboseModeMessage = "Verbose mode ON";

var rootCommand = new RootCommand("Health Tracker CLI");

// Define global verbose option
var verboseOption = new Option<bool>("--verbose", "Enable verbose output.");
rootCommand.AddGlobalOption(verboseOption);

// Create log command
var logCommand = new Command("log", "Log health data");
logCommand.SetHandler(() =>
{
    Console.WriteLine("Log command invoked");
});

// Create log weight sub-subcommand
var logWeightCommand = new Command("weight", "Log weight data");
logWeightCommand.SetHandler((InvocationContext context) =>
{
    var verbose = context.ParseResult.GetValueForOption(verboseOption);
    if (verbose)
    {
        Console.WriteLine(VerboseModeMessage);
    }
    Console.WriteLine("Log weight invoked");
});
logCommand.AddCommand(logWeightCommand);

// Create log run sub-subcommand
var logRunCommand = new Command("run", "Log run data");
logRunCommand.SetHandler((InvocationContext context) =>
{
    var verbose = context.ParseResult.GetValueForOption(verboseOption);
    if (verbose)
    {
        Console.WriteLine(VerboseModeMessage);
    }
    Console.WriteLine("Log run invoked");
});
logCommand.AddCommand(logRunCommand);

// Create view command
var viewCommand = new Command("view", "View health data");
viewCommand.SetHandler(() =>
{
    Console.WriteLine("View command invoked");
});

// Create view weight sub-subcommand
var viewWeightCommand = new Command("weight", "View weight data");
viewWeightCommand.SetHandler((InvocationContext context) =>
{
    var verbose = context.ParseResult.GetValueForOption(verboseOption);
    if (verbose)
    {
        Console.WriteLine(VerboseModeMessage);
    }
    Console.WriteLine("View weight invoked");
});
viewCommand.AddCommand(viewWeightCommand);

// Create view run sub-subcommand
var viewRunCommand = new Command("run", "View run data");
viewRunCommand.SetHandler((InvocationContext context) =>
{
    var verbose = context.ParseResult.GetValueForOption(verboseOption);
    if (verbose)
    {
        Console.WriteLine(VerboseModeMessage);
    }
    Console.WriteLine("View run invoked");
});
viewCommand.AddCommand(viewRunCommand);

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
