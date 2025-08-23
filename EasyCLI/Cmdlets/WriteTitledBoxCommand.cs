using EasyCLI.Console;

namespace EasyCLI.Cmdlets
{
    /// <summary>
    /// PowerShell cmdlet that writes content inside a titled box to the console.
    /// Creates a box with Unicode border characters and an optional title.
    /// </summary>
    [Cmdlet(VerbsCommunications.Write, "TitledBox", DefaultParameterSetName = DefaultSet)]
    public class WriteTitledBoxCommand : PSCmdlet
    {
        internal const string DefaultSet = "Default";

        /// <summary>
        /// Gets or sets the line of content to add to the box.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromRemainingArguments = true)]
        public string? Line { get; set; }

        /// <summary>
        /// Gets or sets the title to display at the top of the box.
        /// </summary>
        [Parameter]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to disable color output.
        /// </summary>
        [Parameter]
        public SwitchParameter NoColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to return a TitledBoxInfo object instead of strings.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThruObject { get; set; }

        private readonly List<string> _lines = new();
        private ConsoleWriter? _writer;

        /// <summary>
        /// Initializes the cmdlet for processing.
        /// </summary>
        protected override void BeginProcessing()
        {
            _writer = new ConsoleWriter(enableColors: !NoColor.IsPresent);
        }

        /// <summary>
        /// Processes each input line and adds it to the box content.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (Line != null)
            {
                _lines.Add(Line);
            }
        }

        /// <summary>
        /// Finalizes processing and writes the completed box to the console.
        /// </summary>
        protected override void EndProcessing()
        {
            ConsoleWriter w = _writer!;
            ConsoleTheme theme = new();
            List<string> boxLines = ConsoleFormatting.BuildTitledBox(_lines, Title).ToList();
            // Write with styles similar to WriteTitledBox extension
            // Define border characters for box drawing
            char[] borderChars = { '┌', '└', '│', '┐', '┘' };
            foreach (string? line in boxLines)
            {
                bool isBorder = line.Length > 0 && borderChars.Contains(line[0]);
                if (isBorder)
                {
                    if (line.Contains(" " + Title + " "))
                    {
                        // Attempt to color title differently
                        int start = line.IndexOf(' ');
                        int end = line.LastIndexOf(' ');
                        if (start >= 0 && end > start)
                        {
                            string left = line[..start];
                            string mid = line[start..end];
                            string right = line[end..];
                            w.Write(left, theme.Heading);
                            w.Write(mid, theme.Success);
                            w.WriteLine(right, theme.Heading);
                            continue;
                        }
                    }
                    w.WriteLine(line, theme.Heading);
                }
                else
                {
                    w.WriteLine(line, theme.Hint);
                }
            }
            if (PassThruObject.IsPresent)
            {
                WriteObject(new TitledBoxInfo(Title, _lines.AsReadOnly(), boxLines));
            }
            else
            {
                foreach (string? l in boxLines)
                {
                    WriteObject(l);
                }
            }
        }
    }
}
