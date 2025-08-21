using EasyCLI;
using static EasyCLI.ConsoleFormatting;

var w = new ConsoleWriter();

w.WriteTitleRule("EasyCLI Demo", width: 0, titleStyle: ConsoleStyles.Heading, fillerStyle: ConsoleStyles.Hint);

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

var theme = new ConsoleTheme { Success = new ConsoleStyle(92), Heading = new ConsoleStyle(95) };
w.WriteHeadingBlock("Themed", theme.Heading, ConsoleStyles.Hint);
w.WriteSuccessLine("All good", theme);

w.WriteBox(new[] { "Boxed line 1", "Boxed line 2" }, borderStyle: ConsoleStyles.Info);
