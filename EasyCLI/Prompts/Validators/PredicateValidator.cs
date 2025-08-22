namespace EasyCLI.Prompts.Validators
{
    using System;

    /// <summary>
    /// Wraps a predicate for simple custom validation logic.
    /// </summary>
    /// <typeparam name="T">The converted value type.</typeparam>
    public sealed class PredicateValidator<T> : IPromptValidator<T>
    {
        private readonly Func<string, (bool ok, T value, string? error)> func;

        public PredicateValidator(Func<string, (bool ok, T value, string? error)> func)
        {
            this.func = func;
        }

        public PromptValidationResult Validate(string raw, out T value)
        {
            (bool ok, T v, string? err) = this.func(raw);
            value = v;

            if (!ok)
            {
                return PromptValidationResult.Fail(err ?? "Invalid value");
            }

            return PromptValidationResult.Success();
        }
    }
}
