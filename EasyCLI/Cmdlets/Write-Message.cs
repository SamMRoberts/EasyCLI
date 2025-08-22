namespace EasyCLI.Cmdlets
{
    [Cmdlet(VerbsCommunications.Write, "Message", DefaultParameterSetName = DefaultSet)]
    [Alias("Show-Message")] // Backward compatibility alias
    [OutputType(typeof(string))]
    public class WriteMessageCommand : PSCmdlet
    {
        internal const string DefaultSet = "Default";

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = DefaultSet)]
        public string? Message { get; set; }

        [Parameter]
        public SwitchParameter Success { get; set; }

        [Parameter]
        public SwitchParameter Warning { get; set; }

        [Parameter]
        public SwitchParameter Error { get; set; }

        [Parameter]
        public SwitchParameter Info { get; set; }

        [Parameter]
        public SwitchParameter Hint { get; set; }

        [Parameter]
        public SwitchParameter NoColor { get; set; }

        private ConsoleWriter? _writer;

        protected override void BeginProcessing()
        {
            var enableColors = !NoColor.IsPresent;
            _writer = new ConsoleWriter(enableColors: enableColors);
        }

        protected override void ProcessRecord()
        {
            if (Message is null)
            {
                return;
            }

            var w = _writer!;
            if (Success)
            {
                w.WriteSuccessLine(Message);
            }
            else if (Warning)
            {
                w.WriteWarningLine(Message);
            }
            else if (Error)
            {
                w.WriteErrorLine(Message);
            }
            else if (Info)
            {
                w.WriteInfoLine(Message);
            }
            else if (Hint)
            {
                w.WriteHintLine(Message);
            }
            else
            {
                w.WriteLine(Message);
            }

            WriteObject(Message);
        }
    }
}
