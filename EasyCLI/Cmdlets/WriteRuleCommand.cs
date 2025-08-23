namespace EasyCLI.Cmdlets
{
    [Cmdlet(VerbsCommunications.Write, "Rule", DefaultParameterSetName = DefaultSet)]
    public class WriteRuleCommand : PSCmdlet
    {
        internal const string DefaultSet = "Default";

        [Parameter]
        [ValidateRange(0, int.MaxValue)]
        public int Width { get; set; } = 0; // 0 => auto console width logic

        [Parameter]
        public char Char { get; set; } = '─';

        [Parameter]
        public SwitchParameter NoColor { get; set; }

        [Parameter]
        public SwitchParameter Center { get; set; }

        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        [ValidateRange(0, 20)]
        public int Gap { get; set; } = 1;

        [Parameter]
        public SwitchParameter PassThruObject { get; set; }

        private ConsoleWriter? _writer;

        protected override void BeginProcessing()
        {
            _writer = new ConsoleWriter(enableColors: !NoColor.IsPresent);
        }

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
