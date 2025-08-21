using EasyCLI;
using static EasyCLI.ConsoleFormatting;
using EasyCLI.Prompts;

var w = new ConsoleWriter();

w.WriteCenterTitleRule("EasyCLI Demo", width: 0, titleStyle: ConsoleStyles.Heading, fillerStyle: ConsoleStyles.Hint);

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
	alignments: new[] { CellAlign.Left, CellAlign.Center, CellAlign.Right },
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

	// Simple interactive prompt demo (only runs if input is interactive)
	if (!Console.IsInputRedirected)
	{
		var reader = new ConsoleReader();
		var namePrompt = new StringPrompt("Enter your name", w, reader, @default: "Anon");
		var agePrompt = new IntPrompt("Enter age", w, reader);
		var confirmPrompt = new YesNoPrompt("Proceed", w, reader, @default: true);
		var choices = new [] { new Choice<string>("Apple","apple"), new Choice<string>("Banana","banana"), new Choice<string>("Cherry","cherry") };
		var fruitPrompt = new ChoicePrompt<string>("Pick a fruit", choices, w, reader);
		var multiNumbers = new [] { new Choice<int>("One",1), new Choice<int>("Two",2), new Choice<int>("Three",3), new Choice<int>("Four",4) };
		var multiPrompt = new MultiSelectPrompt<int>("Select numbers", multiNumbers, w, reader);

		var name = namePrompt.Get();
		var age = agePrompt.Get();
		var proceed = confirmPrompt.Get();
		var fruit = fruitPrompt.Get();
		var nums = multiPrompt.Get();
		w.WriteInfoLine($"Hello {name}, age {age}, proceed={proceed}, fruit={fruit}, nums=[{string.Join(',', nums)}]");
	}
