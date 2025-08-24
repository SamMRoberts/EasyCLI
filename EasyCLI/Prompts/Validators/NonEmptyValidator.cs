namespace EasyCLI.Prompts.Validators
{
    /// <summary>
    /// Ensures the input is not null/empty/whitespace.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="NonEmptyValidator"/> class.
    /// </remarks>
    /// <param name="message">The error message to display when validation fails. If null, a default message is used.</param>
    public sealed class NonEmptyValidator(string? message = null) : IPromptValidator<string>
    {
        private readonly string message = message ?? "Value cannot be empty";

        /// <summary>
        /// Validates that the input is not null, empty, or whitespace.
        /// </summary>
        /// <param name="raw">The raw input string from the user.</param>
        /// <param name="value">The validated string value.</param>
        /// <returns>A validation result indicating success or failure.</returns>
        public PromptValidationResult Validate(string raw, out string value)
        {
            value = raw;

            return string.IsNullOrWhiteSpace(raw) ? PromptValidationResult.Fail(message) : PromptValidationResult.Success();
        }
    }
}
