namespace EasyCLI.Prompts.Validators
{
    /// <summary>
    /// Ensures the input is not null/empty/whitespace.
    /// </summary>
    public sealed class NonEmptyValidator : IPromptValidator<string>
    {
        private readonly string message;

        public NonEmptyValidator(string? message = null)
        {
            this.message = message ?? "Value cannot be empty";
        }

        public PromptValidationResult Validate(string raw, out string value)
        {
            value = raw;

            if (string.IsNullOrWhiteSpace(raw))
            {
                return PromptValidationResult.Fail(this.message);
            }

            return PromptValidationResult.Success();
        }
    }
}
