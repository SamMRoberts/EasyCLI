using System.Management.Automation;

namespace EasyCLI.Cmdlets
{
    [Cmdlet(VerbsCommunications.Write, "EasyRule", DefaultParameterSetName = DefaultSet)]
    public class WriteEasyRuleCommand : PSCmdlet
    {
        internal const string DefaultSet = "Default";

        [Parameter]
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
        public int Gap { get; set; } = 1;

        private ConsoleWriter? _writer;

        protected override void BeginProcessing()
        {
            _writer = new ConsoleWriter(enableColors: !NoColor.IsPresent);
        }

        protected override void ProcessRecord()
        {
            var w = _writer!;
            var title = Title ?? string.Empty;
            string line;
            if (Center)
            {
                line = ConsoleFormatting.CenterTitleRule(title, filler: Char, width: Width, gap: Gap);
            }
            else if (!string.IsNullOrEmpty(title))
            {
                line = ConsoleFormatting.TitleRule(title, filler: Char, width: Width, gap: Gap);
            }
            else
            {
                line = ConsoleFormatting.Rule(Char, Width);
            }
            w.WriteLine(line);
            WriteObject(line);
        }
    }
}
