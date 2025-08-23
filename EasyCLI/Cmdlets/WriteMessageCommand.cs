namespace EasyCLI.Cmdlets
{
    /// <summary>
    /// PowerShell cmdlet that writes styled messages to the console.
    /// Supports success, warning, error, info, and hint styles.
    /// </summary>
    [Cmdlet(VerbsCommunications.Write, "Message", DefaultParameterSetName = DefaultSet)]
    [Alias("Show-Message")] // Backward compatibility alias
    [OutputType(typeof(string))]
    public class WriteMessageCommand : PSCmdlet
    {
        internal const string DefaultSet = "Default";

        /// <summary>
        /// Gets or sets the message to write to the console.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = DefaultSet)]
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to apply success styling to the message.
        /// </summary>
        [Parameter]
        public SwitchParameter Success { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to apply warning styling to the message.
        /// </summary>
        [Parameter]
        public SwitchParameter Warning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to apply error styling to the message.
        /// </summary>
        [Parameter]
        public SwitchParameter Error { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to apply info styling to the message.
        /// </summary>
        [Parameter]
        public SwitchParameter Info { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to apply hint styling to the message.
        /// </summary>
        [Parameter]
        public SwitchParameter Hint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable color output.
        /// </summary>
        [Parameter]
        public SwitchParameter NoColor { get; set; }

        private ConsoleWriter? _writer;

        /// <summary>
        /// Initializes the cmdlet for processing.
        /// </summary>
        protected override void BeginProcessing()
        {
            bool enableColors = !NoColor.IsPresent;
            _writer = new ConsoleWriter(enableColors: enableColors);
        }

        /// <summary>
        /// Processes each input record and writes the styled message to the console.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (Message is null)
            {
                return;
            }

            ConsoleWriter w = _writer!;
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
