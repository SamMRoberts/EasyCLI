namespace EasyCLI.Cmdlets;

/// <summary>
/// Displays a numbered menu of options and returns the selected value. Supports non-interactive selection via -Select.
/// </summary>
[Alias("Select-EasyChoice")]
[Cmdlet(VerbsCommunications.Read, "Choice", DefaultParameterSetName = DefaultSet)]
[OutputType(typeof(string))]
[OutputType(typeof(int))]
[OutputType(typeof(EasyCLI.Cmdlets.ChoiceSelection))]
public class ReadChoiceCommand : PSCmdlet
{
    internal const string DefaultSet = "Default";

    [Parameter(Mandatory = true, Position = 0)]
    public string[] Options { get; set; } = Array.Empty<string>();

    [Parameter]
    public string Prompt { get; set; } = "Select an option";

    [Parameter]
    public string? Default { get; set; }

    /// <summary>
    /// Non-interactive selection (number or label). Used mainly for scripting and tests.
    /// </summary>
    [Parameter]
    public string? Select { get; set; }

    [Parameter]
    public SwitchParameter NoColor { get; set; }

    /// <summary>
    /// Allow the user to press ESC to cancel (no output produced).
    /// </summary>
    [Parameter]
    public SwitchParameter CancelOnEscape { get; set; }

    /// <summary>
    /// Test-only: simulated key sequence. Use \u001b for ESC, \n for Enter. Not shown in help.
    /// </summary>
    [Parameter(DontShow = true)]
    public string? SimulateKeys { get; set; }

    /// <summary>
    /// When set, emit the zero-based index (int) of the selection instead of the value.
    /// </summary>
    [Parameter]
    public SwitchParameter PassThruIndex { get; set; }

    /// <summary>
    /// When set, emit an object with Index (int) and Value (string). Overrides PassThruIndex if both provided.
    /// </summary>
    [Parameter]
    public SwitchParameter PassThruObject { get; set; }

    private ConsoleWriter? _writer;
    private ConsoleTheme _theme = new();

    protected override void BeginProcessing()
    {
        if (Options is null || Options.Length == 0)
            throw new ParameterBindingException("Options parameter cannot be empty.");
        _writer = new ConsoleWriter(enableColors: !NoColor.IsPresent);
    }

    protected override void ProcessRecord()
    {
        // Single execution only; ignore additional pipeline calls (no pipeline input expected)
    }

    protected override void EndProcessing()
    {
        var w = _writer!;
        // Render menu
        for (int i = 0; i < Options.Length; i++)
        {
            var label = Options[i] ?? string.Empty;
            // number prefix styled as hint, value as info
            w.WriteHint($"{i + 1}) ");
            w.WriteInfoLine(label);
        }

        string? selection = Select;
        if (string.IsNullOrEmpty(selection))
        {
            selection = ReadInteractiveSelection(w);
            if (selection == null) // cancelled
                return;
        }

        if (string.IsNullOrEmpty(selection))
        {
            WriteWarning("No selection provided.");
            return;
        }

        var (chosenValue, chosenIndex) = ResolveSelection(selection!);
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

    private (string? value, int index) ResolveSelection(string raw)
    {
        if (int.TryParse(raw, out var idx))
        {
            if (idx >= 1 && idx <= Options.Length) return (Options[idx - 1], idx - 1);
        }
        // match exact (case-insensitive) then startswith
        for (int i = 0; i < Options.Length; i++)
        {
            if (string.Equals(Options[i], raw, StringComparison.OrdinalIgnoreCase))
                return (Options[i], i);
        }
        for (int i = 0; i < Options.Length; i++)
        {
            if (Options[i].StartsWith(raw, StringComparison.OrdinalIgnoreCase))
                return (Options[i], i);
        }
        return (null, -1);
    }

    private string? ReadInteractiveSelection(ConsoleWriter w)
    {
        var promptText = Prompt + (Default != null ? $" [{Default}]" : string.Empty) + ": ";
        w.WriteHeading(promptText);
        var buffer = new StringBuilder();
        if (!string.IsNullOrEmpty(SimulateKeys))
        {
            for (int i = 0; i < SimulateKeys.Length; i++)
            {
                var ch = SimulateKeys[i];
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
                    w.WriteLine("");
                    if (buffer.Length == 0 && Default != null)
                        return Default;
                    return buffer.ToString();
                }
                if (ch == '\b')
                {
                    if (buffer.Length > 0) buffer.Length--;
                    continue;
                }
                if (!char.IsControl(ch))
                {
                    buffer.Append(ch);
                }
            }
            // If simulate ended without explicit enter, treat as enter
            if (buffer.Length == 0 && Default != null)
                return Default;
            return buffer.ToString();
        }
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
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
                w.WriteLine("");
                if (buffer.Length == 0 && Default != null)
                    return Default;
                return buffer.ToString();
            }
            if (key.Key == ConsoleKey.Backspace)
            {
                if (buffer.Length > 0)
                {
                    buffer.Length--;
                    Console.Write("\b \b");
                }
                continue;
            }
            if (!char.IsControl(key.KeyChar))
            {
                buffer.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }
    }
}