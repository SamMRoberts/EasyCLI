using EasyCLI.Console;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// A prompt that accepts string input from the user.
    /// </summary>
    public sealed class StringPrompt(string prompt, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null, string? @default = null, IPromptValidator<string>? validator = null) : BasePrompt<string>(prompt, writer, reader, options, @default, @default != null, validator)
    {
        /// <summary>
        /// Attempts to convert the raw user input to a string (always succeeds).
        /// </summary>
        /// <param name="raw">The raw input string from the user.</param>
        /// <param name="value">The string value (same as raw input).</param>
        /// <returns>Always returns <c>true</c> since any string input is valid.</returns>
        protected override bool TryConvert(string raw, out string value)
        {
            value = raw;
            return true;
        }
    }
}
