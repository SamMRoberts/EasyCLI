namespace EasyCLI.Cmdlets
{
    [Cmdlet(VerbsCommunications.Write, "TitledBox", DefaultParameterSetName = DefaultSet)]
    public class WriteTitledBoxCommand : PSCmdlet
    {
        internal const string DefaultSet = "Default";

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromRemainingArguments = true)]
        public string? Line { get; set; }

        [Parameter]
        public string Title { get; set; } = string.Empty;

        [Parameter]
        public SwitchParameter NoColor { get; set; }

        [Parameter]
        public SwitchParameter PassThruObject { get; set; }

        private readonly List<string> _lines = new();
        private ConsoleWriter? _writer;

        protected override void BeginProcessing()
        {
            _writer = new ConsoleWriter(enableColors: !NoColor.IsPresent);
        }

        protected override void ProcessRecord()
        {
            if (Line != null)
            {
                _lines.Add(Line);
            }
        }

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
