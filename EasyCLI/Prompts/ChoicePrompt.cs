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
        private readonly IKeyReader? _keyReader;
        public ChoicePrompt(string prompt, IEnumerable<Choice<T>> choices, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null, T? @default = default, IKeyReader? keyReader = null)
            : base(prompt, writer, reader, options, @default)
        {
            _choices = choices.ToList();
            if (_choices.Count == 0) throw new ArgumentException("Choices cannot be empty", nameof(choices));
            _keyReader = keyReader;
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
                    RenderList(_choices);
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
            RenderList(_choices.Skip(start).Take(endExclusive - start).ToList(), start);
            _writer.WriteLine($"  -- Page {_page + 1}/{totalPages} (n=next, p=prev) --");
        }

        private void RenderList(IReadOnlyList<Choice<T>> list, int offset = 0)
        {
            for (int i = 0; i < list.Count; i++)
                _writer.WriteLine($"  {offset + i + 1}) {list[i].Label}");
        }

        protected override bool TryConvert(string raw, out T value)
        {
            // interactive key-based filter path: if keyReader provided and filtering enabled, bypass raw line parsing with interactive session.
            if (_keyReader != null && _options.EnableFiltering)
            {
                return InteractiveFilter(out value);
            }

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

        private bool InteractiveFilter(out T value)
        {
            // Basic incremental filtering: user types letters; Enter selects if unique or numeric index; ESC clears filter; Backspace edits.
            var filter = string.Empty;
            var current = _choices;
            while (true)
            {
                Console.Write($"\rFilter: {filter.PadRight(Console.WindowWidth - 8)}\rFilter: {filter}");
                var key = _keyReader!.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    if (int.TryParse(filter, out var idx) && idx >= 1 && idx <= _choices.Count)
                    {
                        value = _choices[idx - 1].Value; return true;
                    }
                    if (current.Count == 1)
                    {
                        value = current[0].Value; return true;
                    }
                    continue; // need disambiguation
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (filter.Length > 0) filter = filter[..^1];
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    filter = string.Empty;
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    filter += key.KeyChar;
                }
                var comparison = StringComparison.OrdinalIgnoreCase;
                current = _options.FilterMatchStartsWith
                    ? _choices.Where(c => c.Label.StartsWith(filter, comparison)).ToList()
                    : _choices.Where(c => c.Label.IndexOf(filter, comparison) >= 0).ToList();
                // redraw list below filter line
                // (simplistic: no clearing of prior lines; acceptable for initial implementation)
                if (filter.Length == 0) current = _choices;
                // show top page of filtered results if paging still enabled
                // truncated to PageSize
                var display = current;
                if (_options.EnablePaging && current.Count > _options.PageSize)
                    display = current.Take(_options.PageSize).ToList();
                RenderList(display);
            }
        }
    }
}
