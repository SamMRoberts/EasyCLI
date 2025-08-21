namespace EasyCLI.Prompts.Validators
{
    /// <summary>
    /// Ensures the input is not null/empty/whitespace.
    /// </summary>
    public sealed class NonEmptyValidator : IPromptValidator<string>
    {
        private readonly string _message;
        public NonEmptyValidator(string? message = null) => _message = message ?? "Value cannot be empty";
        public PromptValidationResult Validate(string raw, out string value)
        {
            value = raw;
            if (string.IsNullOrWhiteSpace(raw))
                return PromptValidationResult.Fail(_message);
            return PromptValidationResult.Success();
        }
    }
}
