using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Single-choice selection via numeric index or label match.
    /// </summary>
    public sealed class ChoicePrompt<T> : BasePrompt<T>
    {
        private readonly IReadOnlyList<Choice<T>> _choices;
        private bool _renderedChoices = false;
        public ChoicePrompt(string prompt, IEnumerable<Choice<T>> choices, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null, T? @default = default)
            : base(prompt, writer, reader, options, @default)
        {
            _choices = choices.ToList();
            if (_choices.Count == 0) throw new ArgumentException("Choices cannot be empty", nameof(choices));
        }

        protected override void RenderPrompt()
        {
            if (!_renderedChoices)
            {
                for (int i = 0; i < _choices.Count; i++)
                {
                    _writer.WriteLine($"  {i + 1}) {_choices[i].Label}");
                }
                _renderedChoices = true;
            }
            base.RenderPrompt();
        }

        protected override bool TryConvert(string raw, out T value)
        {
            // index
            if (int.TryParse(raw, out var idx))
            {
                if (idx >= 1 && idx <= _choices.Count)
                {
                    value = _choices[idx - 1].Value;
                    return true;
                }
            }
            // label match (case-insensitive, startswith)
            var match = _choices.FirstOrDefault(c => c.Label.Equals(raw, StringComparison.OrdinalIgnoreCase))
                        ?? _choices.FirstOrDefault(c => c.Label.StartsWith(raw, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                value = match.Value;
                return true;
            }
            value = default!;
            return false;
        }
    }
}
