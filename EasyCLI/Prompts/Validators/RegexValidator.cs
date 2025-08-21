using System.Text.RegularExpressions;

namespace EasyCLI.Prompts.Validators
{
    /// <summary>
    /// Validates string input against a regular expression.
    /// </summary>
    public sealed class RegexValidator : IPromptValidator<string>
    {
        private readonly Regex _regex; private readonly string _message;
        public RegexValidator(string pattern, string errorMessage, RegexOptions options = RegexOptions.None)
        { _regex = new Regex(pattern, options); _message = errorMessage; }
        public PromptValidationResult Validate(string raw, out string value)
        {
            value = raw;
            if (!_regex.IsMatch(raw))
                return PromptValidationResult.Fail(_message);
            return PromptValidationResult.Success();
        }
    }
}
