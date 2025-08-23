namespace EasyCLI.Prompts.Validators
{
    /// <summary>
    /// Ensures the input is not null/empty/whitespace.
    /// </summary>
    public sealed class NonEmptyValidator : IPromptValidator<string>
    {
        private readonly string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonEmptyValidator"/> class.
        /// </summary>
        /// <param name="message">The error message to display when validation fails. If null, a default message is used.</param>
        public NonEmptyValidator(string? message = null)
        {
            this.message = message ?? "Value cannot be empty";
        }

        /// <summary>
        /// Validates that the input is not null, empty, or whitespace.
        /// </summary>
        /// <param name="raw">The raw input string from the user.</param>
        /// <param name="value">The validated string value.</param>
        /// <returns>A validation result indicating success or failure.</returns>
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
