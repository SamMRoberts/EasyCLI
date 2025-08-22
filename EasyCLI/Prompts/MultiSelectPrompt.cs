using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Multi-choice selection prompt returning a list of values.
    /// Accepts comma separated indices (e.g. 1,3,4) or ranges (1-3).
    /// </summary>
    public sealed class MultiSelectPrompt<T> : BasePrompt<IReadOnlyList<T>>
    {
        private readonly IReadOnlyList<Choice<T>> _choices;
        private bool _renderedChoices = false;
        private int _page = 0;
        public MultiSelectPrompt(string prompt, IEnumerable<Choice<T>> choices, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null)
            : base(prompt, writer, reader, options, @default: null)
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

        protected override bool TryConvert(string raw, out IReadOnlyList<T> value)
        {
            // paging navigation (does not select values directly)
            if (_options.EnablePaging && _choices.Count > _options.PageSize)
            {
                if (raw.Equals("n", StringComparison.OrdinalIgnoreCase) || raw.Equals("next", StringComparison.OrdinalIgnoreCase))
                {
                    var totalPages = (_choices.Count + _options.PageSize - 1) / _options.PageSize;
                    _page = (_page + 1) % totalPages;
                    RenderPage();
                    value = default!; return false;
                }
                if (raw.Equals("p", StringComparison.OrdinalIgnoreCase) || raw.Equals("prev", StringComparison.OrdinalIgnoreCase))
                {
                    var totalPages = (_choices.Count + _options.PageSize - 1) / _options.PageSize;
                    _page = (_page - 1 + totalPages) % totalPages;
                    RenderPage();
                    value = default!; return false;
                }
            }

            var results = new List<T>();
            var tokens = raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var token in tokens)
            {
                if (token.Contains('-'))
                {
                    var parts = token.Split('-', 2);
                    if (int.TryParse(parts[0], out var start) && int.TryParse(parts[1], out var end))
                    {
                        if (start > end) (start, end) = (end, start);
                        for (int i = start; i <= end; i++)
                        {
                            if (i >= 1 && i <= _choices.Count)
                                results.Add(_choices[i - 1].Value);
                            else { value = default!; return false; }
                        }
                        continue;
                    }
                    value = default!; return false;
                }
                if (int.TryParse(token, out var idx))
                {
                    if (idx >= 1 && idx <= _choices.Count)
                        results.Add(_choices[idx - 1].Value);
                    else { value = default!; return false; }
                }
                else { value = default!; return false; }
            }
            value = results;
            return results.Count > 0;
        }
    }
}
