using EasyCLI.Console;
using EasyCLI.Extensions;
using EasyCLI.Formatting;
using EasyCLI.Shell;

// Demo of the enhanced CLI features
var reader = new ConsoleReader();
var writer = new ConsoleWriter();

writer.WriteTitleRule("EasyCLI Enhanced CLI Features Demo", filler: '=', width: 0, titleStyle: EasyCLI.Styling.ConsoleStyles.Heading, fillerStyle: EasyCLI.Styling.ConsoleStyles.Hint);

// Create a shell with the enhanced features
var shell = new CliShell(reader, writer, new ShellOptions
{
    Prompt = "demo>",
    PromptStyle = EasyCLI.Styling.ConsoleStyles.FgGreen,
    HistoryLimit = 100
});

writer.WriteLine("");
writer.WriteInfoLine("Enhanced CLI Shell with best practices features:");
writer.WriteLine("- Professional help system (try 'echo --help')");
writer.WriteLine("- Standardized argument parsing and flags");
writer.WriteLine("- Rich error handling with suggestions");
writer.WriteLine("- Dry-run mode support");
writer.WriteLine("- Exit codes following CLI conventions");
writer.WriteLine("");

writer.WriteSuccessLine("Try these commands:");
writer.WriteLine("  echo --help                     # Show detailed help");
writer.WriteLine("  echo Hello World                # Basic echo");
writer.WriteLine("  echo --success \"Great job!\"     # Success styling");
writer.WriteLine("  echo --warning --uppercase WARN # Warning with uppercase");
writer.WriteLine("  echo --repeat 3 \"Repeated\"      # Repeat text");
writer.WriteLine("  echo --dry-run \"Test\"           # Dry run mode");
writer.WriteLine("  echo --verbose Hello World      # Verbose output");
writer.WriteLine("  help                            # List all commands");
writer.WriteLine("  history                         # Command history");
writer.WriteLine("  exit                            # Leave shell");
writer.WriteLine("");

// Start the enhanced shell
await shell.RunAsync();