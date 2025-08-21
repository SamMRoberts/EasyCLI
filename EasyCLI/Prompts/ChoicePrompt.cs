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
    private int _page = 0;
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
                if (_options.EnablePaging && _choices.Count > _options.PageSize)
                {
                    RenderPage();
                }
                else
                {
                    for (int i = 0; i < _choices.Count; i++)
                        _writer.WriteLine($"  {i + 1}) {_choices[i].Label}");
                    _renderedChoices = true;
                }
            }
            base.RenderPrompt();
        }

        private void RenderPage()
        {
            var totalPages = (_choices.Count + _options.PageSize - 1) / _options.PageSize;
            var start = _page * _options.PageSize;
            var endExclusive = Math.Min(start + _options.PageSize, _choices.Count);
            for (int i = start; i < endExclusive; i++)
            {
                _writer.WriteLine($"  {i + 1}) {_choices[i].Label}");
            }
            _writer.WriteLine($"  -- Page {_page + 1}/{totalPages} (n=next, p=prev) --");
        }

        protected override bool TryConvert(string raw, out T value)
        {
            // paging navigation
            if (_options.EnablePaging && _choices.Count > _options.PageSize)
            {
                if (raw.Equals("n", StringComparison.OrdinalIgnoreCase) || raw.Equals("next", StringComparison.OrdinalIgnoreCase))
                {
                    var totalPages = (_choices.Count + _options.PageSize - 1) / _options.PageSize;
                    _page = (_page + 1) % totalPages;
                    RenderPage();
                    value = default!; return false; // continue loop
                }
                if (raw.Equals("p", StringComparison.OrdinalIgnoreCase) || raw.Equals("prev", StringComparison.OrdinalIgnoreCase))
                {
                    var totalPages = (_choices.Count + _options.PageSize - 1) / _options.PageSize;
                    _page = (_page - 1 + totalPages) % totalPages;
                    RenderPage();
                    value = default!; return false;
                }
            }

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
