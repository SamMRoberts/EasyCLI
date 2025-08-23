namespace EasyCLI.Prompts
{
    using System.Globalization;
    using EasyCLI.Console;

    /// <summary>
    /// A prompt that accepts integer input from the user.
    /// </summary>
    public sealed class IntPrompt : BasePrompt<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntPrompt"/> class.
        /// </summary>
        /// <param name="prompt">The prompt text to display to the user.</param>
        /// <param name="writer">The console writer to use for output.</param>
        /// <param name="reader">The console reader to use for input.</param>
        /// <param name="options">Options controlling prompt behavior. If null, default options are used.</param>
        /// <param name="default">The default value to use if the user provides no input.</param>
        /// <param name="validator">The validator to use for validating user input.</param>
        public IntPrompt(string prompt, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null, int? @default = null, IPromptValidator<int>? validator = null)
            : base(prompt, writer, reader, options, @default ?? default, validator)
        {
        }

        /// <summary>
        /// Attempts to convert the raw user input to an integer.
        /// </summary>
        /// <param name="raw">The raw input string from the user.</param>
        /// <param name="value">When successful, contains the parsed integer value.</param>
        /// <returns><c>true</c> if the conversion was successful; otherwise, <c>false</c>.</returns>
        protected override bool TryConvert(string raw, out int value)
        {
            return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }
    }
}
