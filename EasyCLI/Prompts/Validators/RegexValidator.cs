using System.Text.RegularExpressions;

namespace EasyCLI.Prompts.Validators
{
    /// <summary>
    /// Validates string input against a regular expression.
    /// </summary>
    public sealed class RegexValidator : IPromptValidator<string>
    {
        private readonly Regex regex;
        private readonly string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexValidator"/> class.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to validate against.</param>
        /// <param name="errorMessage">The error message to display when validation fails.</param>
        /// <param name="options">Regular expression options to apply.</param>
        public RegexValidator(string pattern, string errorMessage, RegexOptions options = RegexOptions.None)
        {
            this.regex = new Regex(pattern, options);
            this.message = errorMessage;
        }

        /// <summary>
        /// Validates that the input matches the configured regular expression pattern.
        /// </summary>
        /// <param name="raw">The raw input string from the user.</param>
        /// <param name="value">The validated string value.</param>
        /// <returns>A validation result indicating success or failure.</returns>
        public PromptValidationResult Validate(string raw, out string value)
        {
            value = raw;

            if (!this.regex.IsMatch(raw))
            {
                return PromptValidationResult.Fail(this.message);
            }

            return PromptValidationResult.Success();
        }
    }
}
