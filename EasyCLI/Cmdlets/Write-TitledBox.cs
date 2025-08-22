using System.Collections.Generic; // specific for List<>
using System.Linq; // used for ToList()

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

        private readonly List<string> _lines = new();
        private ConsoleWriter? _writer;

        protected override void BeginProcessing()
        {
            _writer = new ConsoleWriter(enableColors: !NoColor.IsPresent);
        }

        protected override void ProcessRecord()
        {
            if (Line != null)
                _lines.Add(Line);
        }

        protected override void EndProcessing()
        {
            var w = _writer!;
            var theme = new ConsoleTheme();
            var boxLines = ConsoleFormatting.BuildTitledBox(_lines, Title).ToList();
            // Write with styles similar to WriteTitledBox extension
            foreach (var line in boxLines)
            {
                bool isBorder = line.StartsWith("┌") || line.StartsWith("└") || line.StartsWith("│") || line.StartsWith("┐") || line.StartsWith("┘");
                if (isBorder)
                {
                    if (line.Contains(" " + Title + " "))
                    {
                        // Attempt to color title differently
                        int start = line.IndexOf(' ');
                        int end = line.LastIndexOf(' ');
                        if (start >= 0 && end > start)
                        {
                            var left = line.Substring(0, start);
                            var mid = line.Substring(start, end - start);
                            var right = line.Substring(end);
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
            foreach (var l in boxLines)
                WriteObject(l);
        }
    }
}
