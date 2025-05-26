using System.CommandLine;
using System.CommandLine.Invocation;

const string VerboseModeMessage = "Verbose mode ON";

var rootCommand = new RootCommand("Health Tracker CLI");

// Define global verbose option
var verboseOption = new Option<bool>("--verbose", "Enable verbose output.");
rootCommand.AddGlobalOption(verboseOption);

// Create and add subcommands
rootCommand.AddCommand(CreateLogCommand(verboseOption));
rootCommand.AddCommand(CreateViewCommand(verboseOption));

// System.CommandLine automatically provides --version support based on assembly version
rootCommand.SetHandler(() =>
{
    // Default behavior when no command is specified
    Console.WriteLine("Health Tracker CLI");
    Console.WriteLine("Use --help for usage information.");
});

return await rootCommand.InvokeAsync(args);

static Command CreateLogCommand(Option<bool> verboseOption)
{
    var logCommand = new Command("log", "Log health data");
    logCommand.SetHandler(() =>
    {
        Console.WriteLine("Log command invoked");
    });

    var logWeightCommand = new Command("weight", "Log weight data");
    logWeightCommand.SetHandler((InvocationContext context) =>
    {
        HandleVerboseOutput(context, verboseOption);
        Console.WriteLine("Log weight invoked");
    });

    var logRunCommand = new Command("run", "Log run data");
    logRunCommand.SetHandler((InvocationContext context) =>
    {
        HandleVerboseOutput(context, verboseOption);
        Console.WriteLine("Log run invoked");
    });

    logCommand.AddCommand(logWeightCommand);
    logCommand.AddCommand(logRunCommand);

    return logCommand;
}

static Command CreateViewCommand(Option<bool> verboseOption)
{
    var viewCommand = new Command("view", "View health data");
    viewCommand.SetHandler(() =>
    {
        Console.WriteLine("View command invoked");
    });

    var viewWeightCommand = new Command("weight", "View weight data");
    viewWeightCommand.SetHandler((InvocationContext context) =>
    {
        HandleVerboseOutput(context, verboseOption);
        Console.WriteLine("View weight invoked");
    });

    var viewRunCommand = new Command("run", "View run data");
    viewRunCommand.SetHandler((InvocationContext context) =>
    {
        HandleVerboseOutput(context, verboseOption);
        Console.WriteLine("View run invoked");
    });

    viewCommand.AddCommand(viewWeightCommand);
    viewCommand.AddCommand(viewRunCommand);

    return viewCommand;
}

static void HandleVerboseOutput(InvocationContext context, Option<bool> verboseOption)
{
    var verbose = context.ParseResult.GetValueForOption(verboseOption);
    if (verbose)
    {
        Console.WriteLine(VerboseModeMessage);
    }
}
