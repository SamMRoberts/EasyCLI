using System;

namespace EasyCLI.Prompts.Validators
{
    /// <summary>
    /// Validates that an integer is within an inclusive range.
    /// </summary>
    public sealed class IntRangeValidator : IPromptValidator<int>
    {
        public IntRangeValidator(int min, int max)
        {
            if (max < min) throw new ArgumentException("max must be >= min");
            Min = min; Max = max;
        }
        public int Min { get; }
        public int Max { get; }
        public PromptValidationResult Validate(string raw, out int value)
        {
            if (!int.TryParse(raw, out value))
                return PromptValidationResult.Fail("Not a number");
            if (value < Min || value > Max)
                return PromptValidationResult.Fail($"Value must be between {Min} and {Max}");
            return PromptValidationResult.Success();
        }
    }
}
