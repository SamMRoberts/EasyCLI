using System.Text.RegularExpressions;

namespace EasyCLI.Prompts.Validators
{
    /// <summary>
    /// Validates string input against a regular expression.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="RegexValidator"/> class.
    /// </remarks>
    /// <param name="pattern">The regular expression pattern to validate against.</param>
    /// <param name="errorMessage">The error message to display when validation fails.</param>
    /// <param name="options">Regular expression options to apply.</param>
    public sealed class RegexValidator(string pattern, string errorMessage, RegexOptions options = RegexOptions.None) : IPromptValidator<string>
    {
        private readonly Regex regex = new(pattern, options);
        private readonly string message = errorMessage;

        /// <summary>
        /// Validates that the input matches the configured regular expression pattern.
        /// </summary>
        /// <param name="raw">The raw input string from the user.</param>
        /// <param name="value">The validated string value.</param>
        /// <returns>A validation result indicating success or failure.</returns>
        public PromptValidationResult Validate(string raw, out string value)
        {
            value = raw;

            return !regex.IsMatch(raw) ? PromptValidationResult.Fail(message) : PromptValidationResult.Success();
        }
    }
}
