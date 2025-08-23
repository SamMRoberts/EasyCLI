namespace EasyCLI.Cmdlets
{
    /// <summary>
    /// PowerShell cmdlet that writes horizontal rule lines to the console.
    /// Supports title text, custom characters, and centering.
    /// </summary>
    [Cmdlet(VerbsCommunications.Write, "Rule", DefaultParameterSetName = DefaultSet)]
    public class WriteRuleCommand : PSCmdlet
    {
        internal const string DefaultSet = "Default";

        /// <summary>
        /// Gets or sets the width of the rule. If 0, auto-detects console width.
        /// </summary>
        [Parameter]
        [ValidateRange(0, int.MaxValue)]
        public int Width { get; set; } = 0; // 0 => auto console width logic

        /// <summary>
        /// Gets or sets the character to use for the rule line.
        /// </summary>
        [Parameter]
        public char Char { get; set; } = '─';

        /// <summary>
        /// Gets or sets a value indicating whether to disable color output.
        /// </summary>
        [Parameter]
        public SwitchParameter NoColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to center the title text.
        /// </summary>
        [Parameter]
        public SwitchParameter Center { get; set; }

        /// <summary>
        /// Gets or sets the title text to display in the rule.
        /// </summary>
        [Parameter]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the gap size around the title text.
        /// </summary>
        [Parameter]
        [ValidateRange(0, 20)]
        public int Gap { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether to return a RuleInfo object instead of a string.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThruObject { get; set; }

        private ConsoleWriter? _writer;

        /// <summary>
        /// Initializes the cmdlet for processing.
        /// </summary>
        protected override void BeginProcessing()
        {
            _writer = new ConsoleWriter(enableColors: !NoColor.IsPresent);
        }

        /// <summary>
        /// Processes the input and writes the rule to the console.
        /// </summary>
        protected override void ProcessRecord()
        {
            ConsoleWriter w = _writer!;
            string title = Title ?? string.Empty;
            string line = Center
                ? ConsoleFormatting.CenterTitleRule(title, filler: Char, width: Width, gap: Gap)
                : !string.IsNullOrEmpty(title)
                    ? ConsoleFormatting.TitleRule(title, filler: Char, width: Width, gap: Gap)
                    : ConsoleFormatting.Rule(Char, Width);
            w.WriteLine(line);
            if (PassThruObject.IsPresent)
            {
                WriteObject(new RuleInfo(line, Title, Width, Char, Center.IsPresent, Gap));
            }
            else
            {
                WriteObject(line);
            }
        }
    }
}
