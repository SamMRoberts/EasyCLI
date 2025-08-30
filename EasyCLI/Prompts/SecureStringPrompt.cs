using System.Security;
using EasyCLI.Console;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Secure input prompt that returns a SecureString for password entry or similar sensitive information.
    /// Input is not echoed to the console.
    /// </summary>
    public sealed class SecureStringPrompt(string prompt, IConsoleWriter writer, IConsoleReader reader, IHiddenInputSource? hiddenSource = null, PromptOptions? options = null, SecureString? @default = null, char? mask = '*') : BasePrompt<SecureString>(prompt, writer, reader, options, @default, @default != null)
    {
        private readonly IHiddenInputSource? _hiddenSource = hiddenSource;
        private readonly char? _mask = mask;

        /// <summary>
        /// Attempts to convert the raw user input to a SecureString.
        /// </summary>
        /// <param name="raw">The raw input string from the user.</param>
        /// <param name="value">The SecureString value created from the raw input.</param>
        /// <returns>Always returns <c>true</c> since any string input can be converted to SecureString.</returns>
        protected override bool TryConvert(string raw, out SecureString value)
        {
            value = new SecureString();
            foreach (char c in raw)
            {
                value.AppendChar(c);
            }
            value.MakeReadOnly();
            return true;
        }

        /// <summary>
        /// Prompts the user for secure hidden input and returns the value as a SecureString.
        /// </summary>
        /// <returns>The secure input provided by the user as a SecureString.</returns>
        public new SecureString GetValue()
        {
            // Handle non-interactive mode first
            if (Options.NonInteractive)
            {
                if (HasDefault)
                {
                    return Default!;
                }

                // No default available in non-interactive mode - fail with clear error
                throw new InvalidOperationException(
                    $"Cannot prompt for '{Prompt}' in non-interactive mode. " +
                    "Use --no-input only when all prompts have default values, or provide values via command-line arguments.");
            }

            // If we have a hidden source, override the base loop with single capture + optional default handling & no validation
            if (_hiddenSource != null)
            {
                RenderPrompt();
                string captured = _hiddenSource.ReadHidden(_mask);

                if (Options.EnableEscapeCancel && captured == "\u001b")
                {
                    return HandleCancel();
                }

                if (string.IsNullOrEmpty(captured) && HasDefault)
                {
                    return Default!;
                }

                // Convert captured string to SecureString
                SecureString secureString = new();
                foreach (char c in captured)
                {
                    secureString.AppendChar(c);
                }
                secureString.MakeReadOnly();
                return secureString;
            }

            return base.GetValue();
        }
    }
}
