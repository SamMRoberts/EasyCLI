using EasyCLI.Console;

namespace EasyCLI.Cmdlets
{
    /// <summary>
    /// Displays a numbered menu of options and returns the selected value. Supports non-interactive selection via -Select.
    /// </summary>
    [Alias("Select-EasyChoice")]
    [Cmdlet(VerbsCommunications.Read, "Choice", DefaultParameterSetName = DefaultSet)]
    [OutputType(typeof(string))]
    [OutputType(typeof(int))]
    [OutputType(typeof(ChoiceSelection))]
    public class ReadChoiceCommand : PSCmdlet
    {
        internal const string DefaultSet = "Default";

        /// <summary>
        /// Gets or sets the array of option strings to present to the user.
        /// </summary>
        [Parameter(Position = 0)]
        public string[] Options { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets accept objects from the pipeline; if they have a Name (string) property that is used as the option label, else ToString().
        /// If both pipeline objects and -Options are supplied, pipeline objects take precedence.
        /// </summary>
        [Parameter(ValueFromPipeline = true)]
        public object? InputObject { get; set; }

        /// <summary>
        /// Gets or sets the prompt text to display to the user.
        /// </summary>
        [Parameter]
        public string Prompt { get; set; } = "Select an option";

        /// <summary>
        /// Gets or sets the default selection value.
        /// </summary>
        [Parameter]
        public string? Default { get; set; }

        /// <summary>
        /// Gets or sets non-interactive selection (number or label). Used mainly for scripting and tests.
        /// </summary>
        [Parameter]
        public string? Select { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable color output.
        /// </summary>
        [Parameter]
        public SwitchParameter NoColor { get; set; }

        /// <summary>
        /// Gets or sets allow the user to press ESC to cancel (no output produced).
        /// </summary>
        [Parameter]
        public SwitchParameter CancelOnEscape { get; set; }

        /// <summary>
        /// Gets or sets test-only: simulated key sequence. Use \u001b for ESC, \n for Enter. Not shown in help.
        /// </summary>
        [Parameter(DontShow = true)]
        public string? SimulateKeys { get; set; }

        /// <summary>
        /// Gets or sets when set, emit the zero-based index (int) of the selection instead of the value.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThruIndex { get; set; }

        /// <summary>
        /// Gets or sets when set, emit an object with Index (int) and Value (string). Overrides PassThruIndex if both provided.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThruObject { get; set; }

        private ConsoleWriter? _writer;
        private ConsoleTheme _theme = new();

        /// <summary>
        /// Initializes the cmdlet for processing.
        /// </summary>
        protected override void BeginProcessing()
        {
            _writer = new ConsoleWriter(enableColors: !NoColor.IsPresent);
        }

        private readonly List<string> _pipelineOptions = new();

        /// <summary>
        /// Processes each input record from the pipeline and adds it to the options.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (InputObject != null)
            {
                string? label = null;
                // Handle PSObject properties dynamically (avoid strong PowerShell dependency for reflection only scenario)
                if (InputObject is System.Management.Automation.PSObject pso)
                {
                    PSPropertyInfo nameProp = pso.Properties["Name"];
                    if (nameProp != null && nameProp.Value is string s)
                    {
                        label = s;
                    }
                }
                if (label == null)
                {
                    Type type = InputObject.GetType();
                    System.Reflection.PropertyInfo? nameProp = type.GetProperty("Name");
                    if (nameProp != null && nameProp.PropertyType == typeof(string))
                    {
                        label = nameProp.GetValue(InputObject) as string;
                    }
                }
                label ??= InputObject.ToString();
                if (!string.IsNullOrEmpty(label))
                {
                    _pipelineOptions.Add(label!);
                }
            }
        }

        /// <summary>
        /// Finalizes processing and displays the choice menu to the user.
        /// </summary>
        protected override void EndProcessing()
        {
            // If pipeline provided options, prefer those over explicit Options (unless none were gathered)
            string[] activeOptions = _pipelineOptions.Count > 0 ? _pipelineOptions.ToArray() : Options;
            if (activeOptions.Length == 0)
            {
                WriteError(new ErrorRecord(new InvalidOperationException("No options supplied."), "NoOptions", ErrorCategory.InvalidArgument, null));
                return;
            }
            ConsoleWriter w = _writer!;
            // Render menu
            for (int i = 0; i < activeOptions.Length; i++)
            {
                string label = activeOptions[i] ?? string.Empty;
                // number prefix styled as hint, value as info
                w.WriteHint($"{i + 1}) ");
                w.WriteInfoLine(label);
            }

            string? selection = Select;
            if (string.IsNullOrEmpty(selection))
            {
                selection = ReadInteractiveSelection(w);
                // cancelled
                if (selection == null)
                {
                    return;
                }
            }

            if (string.IsNullOrEmpty(selection))
            {
                WriteWarning("No selection provided.");
                return;
            }

            (string? chosenValue, int chosenIndex) = ResolveSelection(selection!, activeOptions);
            if (chosenValue == null)
            {
                WriteError(new ErrorRecord(new ArgumentException($"Invalid selection: '{selection}'"), "InvalidSelection", ErrorCategory.InvalidArgument, selection));
                return;
            }
            if (PassThruObject)
            {
                WriteObject(new ChoiceSelection(chosenIndex, chosenValue));
            }
            else if (PassThruIndex)
            {
                WriteObject(chosenIndex);
            }
            else
            {
                WriteObject(chosenValue);
            }
        }

        private static (string? Value, int Index) ResolveSelection(string raw, string[] activeOptions)
        {
            if (int.TryParse(raw, out int idx))
            {
                if (idx >= 1 && idx <= activeOptions.Length)
                {
                    return (activeOptions[idx - 1], idx - 1);
                }
            }
            // match exact (case-insensitive) then startswith
            for (int i = 0; i < activeOptions.Length; i++)
            {
                if (string.Equals(activeOptions[i], raw, StringComparison.OrdinalIgnoreCase))
                {
                    return (activeOptions[i], i);
                }
            }
            for (int i = 0; i < activeOptions.Length; i++)
            {
                if (activeOptions[i].StartsWith(raw, StringComparison.OrdinalIgnoreCase))
                {
                    return (activeOptions[i], i);
                }
            }
            return (null, -1);
        }

        private string? ReadInteractiveSelection(ConsoleWriter w)
        {
            string promptText = Prompt + (Default != null ? $" [{Default}]" : string.Empty) + ": ";
            w.WriteHeading(promptText);
            StringBuilder buffer = new StringBuilder();
            if (!string.IsNullOrEmpty(SimulateKeys))
            {
                for (int i = 0; i < SimulateKeys.Length; i++)
                {
                    char ch = SimulateKeys[i];
                    if (ch == '\u001b')
                    {
                        if (CancelOnEscape)
                        {
                            w.WriteWarningLine("<cancelled>");
                            return null;
                        }
                        continue; // ignore if not canceling
                    }
                    if (ch == '\n' || ch == '\r')
                    {
                        w.WriteLine(string.Empty);
                        if (Default != null)
                        {
                            return Default;
                        }
                        return buffer.ToString();
                    }
                    if (ch == '\b')
                    {
                        if (buffer.Length > 0)
                        {
                            buffer.Length--;
                        }
                        continue;
                    }
                    if (!char.IsControl(ch))
                    {
                        buffer.Append(ch);
                    }
                }
                // If simulate ended without explicit enter, treat as enter
                if (buffer.Length == 0 && Default != null)
                {
                    return Default;
                }
                return buffer.ToString();
            }
            while (true)
            {
                ConsoleKeyInfo key = System.Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Escape)
                {
                    if (CancelOnEscape)
                    {
                        w.WriteWarningLine("<cancelled>");
                        return null;
                    }
                    continue; // ignore ESC if not canceling
                }
                if (key.Key == ConsoleKey.Enter)
                {
                    w.WriteLine(string.Empty);
                    if (buffer.Length == 0 && Default != null)
                    {
                        return Default;
                    }
                    return buffer.ToString();
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Length--;
                        System.Console.Write("\b \b");
                    }
                    continue;
                }
                if (!char.IsControl(key.KeyChar))
                {
                    buffer.Append(key.KeyChar);
                    System.Console.Write(key.KeyChar);
                }
            }
        }
    }
}
