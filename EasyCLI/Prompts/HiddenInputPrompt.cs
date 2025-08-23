namespace EasyCLI.Prompts
{
    /// <summary>
    /// Secure/hidden input prompt (e.g., passwords). Falls back to plain prompt if hidden source not available.
    /// </summary>
    public sealed class HiddenInputPrompt(string prompt, IConsoleWriter writer, IConsoleReader reader, IHiddenInputSource? hiddenSource = null, PromptOptions? options = null, string? @default = null, char? mask = '*') : BasePrompt<string>(prompt, writer, reader, options, @default)
    {
        private readonly IHiddenInputSource? _hiddenSource = hiddenSource;
        private readonly char? _mask = mask;

        protected override bool TryConvert(string raw, out string value)
        {
            value = raw;
            return true;
        }

        public new string GetValue()
        {
            // If we have a hidden source, override the base loop with single capture + optional default handling & no validation
            if (_hiddenSource != null)
            {
                RenderPrompt();
                string captured = _hiddenSource.ReadHidden(_mask);
                if (Options.EnableEscapeCancel && captured == "\u001b")
                {
                    return HandleCancel();
                }
                if (string.IsNullOrEmpty(captured) && Default is not null)
                {
                    return Default;
                }
                return captured;
            }
            return base.GetValue();
        }
    }
}
