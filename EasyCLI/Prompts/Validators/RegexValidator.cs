namespace EasyCLI.Prompts.Validators
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Validates string input against a regular expression.
    /// </summary>
    public sealed class RegexValidator : IPromptValidator<string>
    {
        private readonly Regex regex;
        private readonly string message;

        public RegexValidator(string pattern, string errorMessage, RegexOptions options = RegexOptions.None)
        {
            this.regex = new Regex(pattern, options);
            this.message = errorMessage;
        }

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
