using System;

namespace EasyCLI.Prompts.Validators
{
    /// <summary>
    /// Wraps a predicate for simple custom validation logic.
    /// </summary>
    public sealed class PredicateValidator<T> : IPromptValidator<T>
    {
        private readonly Func<string, (bool ok, T value, string? error)> _func;
        public PredicateValidator(Func<string, (bool ok, T value, string? error)> func) => _func = func;
        public PromptValidationResult Validate(string raw, out T value)
        {
            var (ok, v, err) = _func(raw);
            value = v;
            if (!ok)
                return PromptValidationResult.Fail(err ?? "Invalid value");
            return PromptValidationResult.Success();
        }
    }
}
