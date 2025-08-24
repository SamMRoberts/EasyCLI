namespace EasyCLI.Prompts.Validators
{
    /// <summary>
    /// Validates that an integer is within an inclusive range.
    /// </summary>
    public sealed class IntRangeValidator : IPromptValidator<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntRangeValidator"/> class.
        /// </summary>
        /// <param name="min">The minimum allowed value (inclusive).</param>
        /// <param name="max">The maximum allowed value (inclusive).</param>
        public IntRangeValidator(int min, int max)
        {
            if (max < min)
            {
                throw new ArgumentException("max must be >= min");
            }

            Min = min;
            Max = max;
        }

        /// <summary>
        /// Gets the minimum allowed value.
        /// </summary>
        public int Min { get; }

        /// <summary>
        /// Gets the maximum allowed value.
        /// </summary>
        public int Max { get; }

        /// <summary>
        /// Validates that the input is an integer within the specified range.
        /// </summary>
        /// <param name="raw">The raw input string from the user.</param>
        /// <param name="value">When successful, contains the parsed integer value.</param>
        /// <returns>A validation result indicating success or failure.</returns>
        public PromptValidationResult Validate(string raw, out int value)
        {
            return !int.TryParse(raw, out value)
                ? PromptValidationResult.Fail("Not a number")
                : value < Min || value > Max
                ? PromptValidationResult.Fail($"Value must be between {Min} and {Max}")
                : PromptValidationResult.Success();
        }
    }
}
