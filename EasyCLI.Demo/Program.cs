using EasyCLI;
using EasyCLI.Console;
using EasyCLI.Demo;
using EasyCLI.Environment;
using EasyCLI.Extensions;
using EasyCLI.Formatting;
using EasyCLI.Prompts;
using EasyCLI.Prompts.Validators;
using EasyCLI.Shell;
using EasyCLI.Styling;

// Parse command line arguments for --plain flag support
var parsedArgs = new CommandLineArgs(Environment.GetCommandLineArgs().Skip(1).ToArray());

// Create console writer using factory that respects --plain flag and NO_COLOR
var w = ConsoleWriterFactory.Create(parsedArgs);

// Show current mode in demo
if (parsedArgs.IsPlainOutput)
{
    w.WriteLine("=== EasyCLI Demo (PLAIN MODE) ===");
}
else
{
    w.WriteCenterTitleRule("EasyCLI Demo", width: 0, titleStyle: ConsoleStyles.Heading, fillerStyle: ConsoleStyles.Hint);
}

var lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur quis orci vitae leo consequat.";
w.WriteWrapped(lorem, width: 0, indent: 2, style: ConsoleStyles.Dim);

w.WriteHeadingBlock("Key-Values", ConsoleStyles.Heading, ConsoleStyles.Hint);
w.WriteKeyValues(new[] { ("Name", "EasyCLI"), ("Version", "0.1.0"), ("Status", "OK") }, keyStyle: ConsoleStyles.Info);

w.WriteHeadingBlock("Table", ConsoleStyles.Heading, ConsoleStyles.Hint);
w.WriteTableSimple(
	new[] { "Left", "Center", "Right" },
	new[]
	{
		new[] { "a", "b", "c" },
		new[] { "longtext", "middle", "end" }
	},
	padding: 1,
	maxWidth: 0,
	alignments: new[] { ConsoleFormatting.CellAlign.Left, ConsoleFormatting.CellAlign.Center, ConsoleFormatting.CellAlign.Right },
	borderStyle: ConsoleStyles.Hint,
	headerStyle: ConsoleStyles.Bold,
	cellStyle: null);


// Demo all built-in theme presets
var themes = new[] {
	("Dark", ConsoleThemes.Dark),
	("Light", ConsoleThemes.Light),
	("HighContrast", ConsoleThemes.HighContrast)
};
foreach (var (label, theme) in themes)
{
	w.WriteTitleRule($"Theme: {label}", filler: '-', width: 0, titleStyle: theme.Heading, fillerStyle: theme.Hint);
	w.WriteSuccessLine("Success message", theme);
	w.WriteWarningLine("Warning message", theme);
	w.WriteErrorLine("Error message", theme);
	w.WriteInfoLine("Info message", theme);
	w.WriteHintLine("Hint message", theme);
	w.WriteTitledBox(new[] { $"This is a titled box in {label} theme." }, title: $"{label} Box", borderStyle: theme.Info, titleStyle: theme.Heading);
	w.WriteLine("");
}

// Enhanced prompt demo with non-interactive support
w.WriteTitleRule("Prompts", filler: '-', width: 0, titleStyle: ConsoleStyles.Heading, fillerStyle: ConsoleStyles.Hint);
var reader = new ConsoleReader();

// Create prompts with environment-aware options
var promptOptions = EnvironmentDetector.CreatePromptOptions(parsedArgs);

// Show the mode being used
if (promptOptions.NonInteractive)
{
	w.WriteInfoLine("Non-interactive mode detected - using default values where available");
}
else
{
	w.WriteInfoLine("Interactive mode - prompts will ask for user input");
}

// Prompts with defaults - work in both modes
var namePrompt = new StringPrompt("Enter your name", w, reader, promptOptions, @default: "Anon");
var confirmPrompt = new YesNoPrompt("Proceed", w, reader, promptOptions, @default: true);
var hidden = new HiddenInputPrompt("Enter secret", w, reader, hiddenSource: new ConsoleHiddenInputSource(), promptOptions, @default: "***");

// Prompt without default - only works in interactive mode
IntPrompt? agePrompt = null;
if (!promptOptions.NonInteractive)
{
	agePrompt = new IntPrompt("Enter age", w, reader, promptOptions, validator: new IntRangeValidator(1, 120));
}

try
{
	string name = namePrompt.GetValue();
	bool proceed = confirmPrompt.GetValue();
	string secret = hidden.GetValue();
	
	int age = 25; // Default for non-interactive
	if (agePrompt != null)
	{
		age = agePrompt.GetValue();
	}
	else if (promptOptions.NonInteractive)
	{
		w.WriteInfoLine("Age prompt skipped in non-interactive mode - using default: 25");
	}
	
	if (proceed)
	{
		w.WriteSuccessLine($"Hello {name}, age {age}, secret received!");
	}
	else
	{
		w.WriteInfoLine("User declined to proceed");
	}
}
catch (InvalidOperationException ex) when (ex.Message.Contains("non-interactive mode"))
{
	w.WriteErrorLine("Error in non-interactive mode:");
	w.WriteErrorLine(ex.Message);
	w.WriteHintLine("Tip: Use --no-input only when all prompts have default values, or provide input via command-line arguments");
}

// Only run complex prompts in interactive mode
if (!promptOptions.NonInteractive)
{
	// Large choice list to show paging + filtering (provide key reader for interactive filtering)
	var fruits = new List<Choice<string>>();
	var fruitNames = new [] {"Apple","Apricot","Avocado","Banana","Blackberry","Blueberry","Cherry","Coconut","Cranberry","Date","Dragonfruit","Fig","Grape","Grapefruit","Guava","Kiwi","Lemon","Lime","Mango","Melon","Nectarine","Orange","Papaya","Peach","Pear","Pineapple","Plum","Pomegranate","Raspberry","Strawberry","Tangerine","Watermelon"};
	foreach (var f in fruitNames) fruits.Add(new Choice<string>(f,f.ToLowerInvariant()));
	var choiceOpts = new PromptOptions { PageSize = 10, EnablePaging = true, EnableFiltering = true };
	var fruitPrompt = new ChoicePrompt<string>("Pick a fruit", fruits, w, reader, options: choiceOpts, keyReader: new ConsoleKeyReader());

	var multiNumbers = new [] { new Choice<int>("One",1), new Choice<int>("Two",2), new Choice<int>("Three",3), new Choice<int>("Four",4) };
	var multiPrompt = new MultiSelectPrompt<int>("Select numbers", multiNumbers, w, reader, options: new PromptOptions { EnablePaging = false });

	string fruit = fruitPrompt.GetValue();
	var nums = multiPrompt.GetValue();
	w.WriteInfoLine($"Advanced prompts completed: fruit={fruit}, nums=[{string.Join(',', nums)}]");
}

// Plain mode demonstration
if (parsedArgs.IsPlainOutput)
{
	w.WriteLine("");
	w.WriteLine("=== PLAIN MODE ACTIVE ===");
	w.WriteLine("All colors, symbols, and decorations have been stripped.");
	w.WriteLine("This output is optimized for:");
	w.WriteLine("• Script parsing and automation");
	w.WriteLine("• Text processing tools");
	w.WriteLine("• Accessibility and screen readers");
	w.WriteLine("• Environments with limited display capabilities");
}
else
{
	w.WriteLine("");
	w.WriteTitleRule("Plain Mode Info", filler: '-', width: 0, titleStyle: ConsoleStyles.Heading, fillerStyle: ConsoleStyles.Hint);
	w.WriteInfoLine("💡 Try running this demo with --plain or -p flag to see plain output mode.");
	w.WriteHintLine("Example: dotnet run --project EasyCLI.Demo --plain");
	
	// Demo ErrorCollector
	w.WriteLine("");
	w.WriteTitleRule("Error Collector Demo", filler: '-', width: 0, titleStyle: ConsoleStyles.Heading, fillerStyle: ConsoleStyles.Hint);
	ErrorCollectorDemo.Run();
	
	// Demo progress utilities
	w.WriteLine("");
	w.WriteTitleRule("Progress Utilities Demo", filler: '-', width: 0, titleStyle: ConsoleStyles.Heading, fillerStyle: ConsoleStyles.Hint);
	
	// Early feedback demo
	w.WriteStarting("progress demo initialization");
	await Task.Delay(500);
	w.WriteCompleted("progress demo initialization");
	
	// Progress bar demo
	w.WriteInfoLine("Progress bar example:");
	for (int i = 0; i <= 20; i++)
	{
		w.Write("\r");
		w.WriteProgressBar(i, 20, width: 30);
		await Task.Delay(50);
	}
	w.WriteLine("");
	
	// Spinner demo
	w.WriteInfoLine("Spinner example:");
	using (var scope = w.CreateProgressScope("processing demo data"))
	{
		await Task.Delay(1000);
		scope.Complete("Demo data processed successfully");
	}
}