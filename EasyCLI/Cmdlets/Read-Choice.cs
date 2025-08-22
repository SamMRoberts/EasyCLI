namespace EasyCLI.Cmdlets;

/// <summary>
/// Displays a numbered menu of options and returns the selected value. Supports non-interactive selection via -Select.
/// </summary>
[Alias("Select-EasyChoice")]
[Cmdlet(VerbsCommunications.Read, "Choice", DefaultParameterSetName = DefaultSet)]
[OutputType(typeof(string))]
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
            w.WriteHeadingLine(Prompt + (Default != null ? $" [{Default}]" : string.Empty));
            selection = Console.ReadLine();
            if (string.IsNullOrEmpty(selection) && Default != null)
                selection = Default;
        }

        if (string.IsNullOrEmpty(selection))
        {
            WriteWarning("No selection provided.");
            return;
        }

        var chosen = ResolveSelection(selection!);
        if (chosen == null)
        {
            WriteError(new ErrorRecord(new ArgumentException($"Invalid selection: '{selection}'"), "InvalidSelection", ErrorCategory.InvalidArgument, selection));
            return;
        }
        WriteObject(chosen);
    }

    private string? ResolveSelection(string raw)
    {
        if (int.TryParse(raw, out var idx))
        {
            if (idx >= 1 && idx <= Options.Length) return Options[idx - 1];
        }
        // match exact (case-insensitive) then startswith
        var match = Options.FirstOrDefault(o => string.Equals(o, raw, StringComparison.OrdinalIgnoreCase))
                   ?? Options.FirstOrDefault(o => o.StartsWith(raw, StringComparison.OrdinalIgnoreCase));
        return match;
    }
}