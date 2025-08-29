using EasyCLI.Console;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// A prompt that accepts yes/no input from the user, returning a boolean value.
    /// </summary>
    public sealed class YesNoPrompt(string prompt, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null, bool? @default = null) : BasePrompt<bool>(prompt + " (y/n)", writer, reader, options, @default ?? default, @default.HasValue)
    {
        /// <summary>
        /// Attempts to convert the raw user input to a boolean value based on yes/no responses.
        /// </summary>
        /// <param name="raw">The raw input string from the user.</param>
        /// <param name="value">When successful, contains the boolean value (true for 'y', false for 'n').</param>
        /// <returns><c>true</c> if the conversion was successful; otherwise, <c>false</c>.</returns>
        protected override bool TryConvert(string raw, out bool value)
        {
            value = false;
            if (string.IsNullOrEmpty(raw))
            {
                return false;
            }

            char c = char.ToLowerInvariant(raw.Trim()[0]);
            if (c == 'y')
            {
                value = true;
                return true;
            }

            if (c == 'n')
            {
                value = false;
                return true;
            }

            return false;
        }
    }
}
