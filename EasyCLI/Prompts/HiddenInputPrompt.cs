using EasyCLI.Console;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Secure/hidden input prompt (e.g., passwords). Falls back to plain prompt if hidden source not available.
    /// </summary>
    public sealed class HiddenInputPrompt(string prompt, IConsoleWriter writer, IConsoleReader reader, IHiddenInputSource? hiddenSource = null, PromptOptions? options = null, string? @default = null, char? mask = '*') : BasePrompt<string>(prompt, writer, reader, options, @default)
    {
        private readonly IHiddenInputSource? _hiddenSource = hiddenSource;
        private readonly char? _mask = mask;

        /// <summary>
        /// Attempts to convert the raw user input to a string (always succeeds for hidden input).
        /// </summary>
        /// <param name="raw">The raw input string from the user.</param>
        /// <param name="value">The string value (same as raw input).</param>
        /// <returns>Always returns <c>true</c> since any string input is valid.</returns>
        protected override bool TryConvert(string raw, out string value)
        {
            value = raw;
            return true;
        }

        /// <summary>
        /// Prompts the user for hidden input and returns the value.
        /// </summary>
        /// <returns>The hidden input provided by the user.</returns>
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
