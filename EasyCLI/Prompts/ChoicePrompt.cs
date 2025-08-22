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
        private int _lastRenderLines = 0; // retained for compatibility (not used in save/restore mode)
        private bool _savedCursor = false; // whether we've issued an ANSI save cursor position
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
                    RenderPageFiltered("");
                }
                else
                {
                    RenderList(_choices);
                    _renderedChoices = true;
                }
            }
            base.RenderPrompt();
        }

        private void RenderList(IReadOnlyList<Choice<T>> list, int offset = 0)
        {
            foreach (var (item, idx) in list.Select((c, i) => (c, i)))
                _writer.WriteLine($"  {offset + idx + 1}) {item.Label}");
            _lastRenderLines = list.Count; // update count (no footer)
        }

        private IReadOnlyList<Choice<T>> ApplyFilter(string filter)
        {
            if (!_options.EnableFiltering || string.IsNullOrEmpty(filter)) return _choices;
            var comparison = StringComparison.OrdinalIgnoreCase;
            return _options.FilterMatchStartsWith
                ? _choices.Where(c => c.Label.StartsWith(filter, comparison)).ToList()
                : _choices.Where(c => c.Label.IndexOf(filter, comparison) >= 0).ToList();
        }

        private void RenderPageFiltered(string filter)
        {
            var list = ApplyFilter(filter);
            var totalPages = (list.Count + _options.PageSize - 1) / _options.PageSize;
            if (_page >= totalPages && totalPages > 0) _page = 0;
            var start = _page * _options.PageSize;
            var slice = list.Skip(start).Take(_options.PageSize).ToList();
            RenderList(slice, start);
            if (_options.EnablePaging && totalPages > 1)
            {
                _writer.WriteLine($"  -- Page {_page + 1}/{totalPages} (n=next, p=prev) --");
                _lastRenderLines += 1; // include footer line
            }
            _renderedChoices = true;
        }

        // TODO: Implement clearing of previous render
    private static void ClearPreviousRender() { /* no-op in new save/restore model */ }

        protected override bool TryConvert(string raw, out T value)
        {
            if (int.TryParse(raw, out var idx))
            {
                if (idx >= 1 && idx <= _choices.Count)
                {
                    value = _choices[idx - 1].Value; return true;
                }
            }
            var match = _choices.FirstOrDefault(c => c.Label.Equals(raw, StringComparison.OrdinalIgnoreCase))
                        ?? _choices.FirstOrDefault(c => c.Label.StartsWith(raw, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                value = match.Value; return true;
            }
            value = default!; return false;
        }

        public new T Get()
        {
            if (_keyReader != null)
                return InteractiveKeyLoop();
            // fallback line-based
            while (true)
            {
                RenderPrompt();
                var raw = _reader.ReadLine();
                if (_options.EnableEscapeCancel && raw == "\u001b") return HandleCancel();
                // Simple line-based paging navigation (legacy behavior for tests)
                if (_options.EnablePaging && _choices.Count > _options.PageSize && !string.IsNullOrEmpty(raw))
                {
                    var list = _choices; // original list for page calculations
                    var totalPages = (list.Count + _options.PageSize - 1) / _options.PageSize;
                    if (totalPages > 1)
                    {
                        if (string.Equals(raw, "n", StringComparison.OrdinalIgnoreCase))
                        {
                            _page = (_page + 1) % totalPages;
                            _renderedChoices = false; // force re-render of new page
                            continue; // reprompt
                        }
                        if (string.Equals(raw, "p", StringComparison.OrdinalIgnoreCase))
                        {
                            _page = (_page - 1 + totalPages) % totalPages;
                            _renderedChoices = false;
                            continue;
                        }
                    }
                }
                if (string.IsNullOrEmpty(raw) && Default is not null) return Default!;
                if (TryConvert(raw, out var converted)) return converted;
                WriteError($"Invalid value: '{raw}'");
            }
        }

        private T InteractiveKeyLoop()
        {
            string filter = string.Empty;
            while (true)
            {
                // Save cursor position once; thereafter restore + clear region.
                if (!_savedCursor)
                {
                    Console.Write("\u001b[s"); // save cursor
                    _savedCursor = true;
                }
                else
                {
                    Console.Write("\u001b[u\u001b[J"); // restore & clear to end of screen
                }

                // Re-render list (always fresh block)
                _renderedChoices = false;
                if (_options.EnablePaging && _choices.Count > _options.PageSize)
                    RenderPageFiltered(filter);
                else
                {
                    var listCurrent = ApplyFilter(filter);
                    RenderList(listCurrent);
                    _renderedChoices = true;
                }
                // Prompt line
                RenderPrompt();
                if (_options.EnableFiltering)
                {
                    Console.Write($"Filter: {filter}");
                }

                var key = _keyReader!.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                {
                    if (_options.EnableEscapeCancel)
                        return HandleCancel();
                    filter = string.Empty; _page = 0; continue;
                }
                if (key.Key == ConsoleKey.N && _options.EnablePaging && _choices.Count > _options.PageSize)
                {
                    var list = ApplyFilter(filter);
                    var totalPages = (list.Count + _options.PageSize - 1) / _options.PageSize;
                    if (totalPages > 1)
                    { _page = (_page + 1) % totalPages; }
                    continue;
                }
                if (key.Key == ConsoleKey.P && _options.EnablePaging && _choices.Count > _options.PageSize)
                {
                    var list = ApplyFilter(filter);
                    var totalPages = (list.Count + _options.PageSize - 1) / _options.PageSize;
                    if (totalPages > 1)
                    { _page = (_page - 1 + totalPages) % totalPages; }
                    continue;
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (filter.Length > 0) { filter = filter[..^1]; _page = 0; }
                    continue;
                }
                if (key.Key == ConsoleKey.Enter)
                {
                    var list = ApplyFilter(filter);
                    if (string.IsNullOrEmpty(filter) && Default is not null) return Default!;
                    if (int.TryParse(filter, out var idx) && idx >= 1 && idx <= _choices.Count) return _choices[idx - 1].Value;
                    if (list.Count == 1) return list[0].Value;
                    var match = _choices.FirstOrDefault(c => c.Label.Equals(filter, StringComparison.OrdinalIgnoreCase))
                               ?? _choices.FirstOrDefault(c => c.Label.StartsWith(filter, StringComparison.OrdinalIgnoreCase));
                    if (match != null) return match.Value;
                    WriteError($"Invalid value: '{filter}'");
                    filter = string.Empty; _page = 0; continue;
                }
                if (!char.IsControl(key.KeyChar))
                {
                    if (_options.EnableFiltering)
                    { filter += key.KeyChar; _page = 0; continue; }
                    else
                    {
                        if (char.IsDigit(key.KeyChar))
                        {
                            var d = key.KeyChar - '0';
                            if (d >= 1 && d <= _choices.Count) return _choices[d - 1].Value;
                        }
                    }
                }
            }
        }
    }
}
