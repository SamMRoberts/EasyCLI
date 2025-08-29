using EasyCLI.Console;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Single-choice selection via numeric index or label match.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with each choice.</typeparam>
    public sealed class ChoicePrompt<T> : BasePrompt<T>
    {
        private readonly List<Choice<T>> _choices;
        private readonly IKeyReader? _keyReader;
        private bool _renderedChoices = false;
        private int _page = 0;
        // Retained for future paging diff logic (line clearing); suppress "unused / make readonly" suggestions intentionally.
#pragma warning disable IDE0052, IDE0044
        private int _lastRenderLines = 0;
#pragma warning restore IDE0052, IDE0044
        private bool _savedCursor = false; // whether we've issued an ANSI save cursor position

        /// <summary>
        /// Initializes a new instance of the <see cref="ChoicePrompt{T}"/> class.
        /// </summary>
        /// <param name="prompt">The prompt text to display to the user.</param>
        /// <param name="choices">The available choices for the user to select from.</param>
        /// <param name="writer">The console writer to use for output.</param>
        /// <param name="reader">The console reader to use for input.</param>
        /// <param name="options">Options controlling prompt behavior. If null, default options are used.</param>
        /// <param name="default">The default value to use if the user provides no input.</param>
        /// <param name="keyReader">The key reader for interactive selection. If null, text-based selection is used.</param>
        public ChoicePrompt(string prompt, IEnumerable<Choice<T>> choices, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null, T? @default = default, IKeyReader? keyReader = null)
            : base(prompt, writer, reader, options, @default, !EqualityComparer<T>.Default.Equals(@default, default))
        {
            _choices = [.. choices];
            if (_choices.Count == 0)
            {
                throw new ArgumentException("Choices cannot be empty", nameof(choices));
            }
            _keyReader = keyReader;
        }

        /// <summary>
        /// Renders the prompt and available choices to the console.
        /// </summary>
        protected override void RenderPrompt()
        {
            if (!_renderedChoices)
            {
                if (Options.EnablePaging && _choices.Count > Options.PageSize)
                {
                    RenderPageFiltered(string.Empty);
                }
                else
                {
                    RenderList(_choices);
                    _renderedChoices = true;
                }
            }
            base.RenderPrompt();
        }

        private void RenderList(List<Choice<T>> list, int offset = 0)
        {
            foreach ((Choice<T> item, int idx) in list.Select((c, i) => (c, i)))
            {
                Writer.WriteLine($"  {offset + idx + 1}) {item.Label}");
            }
            _lastRenderLines = list.Count; // update count (no footer)
        }

        private List<Choice<T>> ApplyFilter(string filter)
        {
            if (!Options.EnableFiltering || string.IsNullOrEmpty(filter))
            {
                return _choices;
            }
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            return Options.FilterMatchStartsWith
                ? [.. _choices.Where(c => c.Label.StartsWith(filter, comparison))]
                : [.. _choices.Where(c => c.Label.Contains(filter, comparison))];
        }

        private void RenderPageFiltered(string filter)
        {
            List<Choice<T>> list = ApplyFilter(filter);
            int totalPages = (list.Count + Options.PageSize - 1) / Options.PageSize;
            if (_page >= totalPages && totalPages > 0)
            {
                _page = 0;
            }
            int start = _page * Options.PageSize;
            List<Choice<T>> slice = [.. list.Skip(start).Take(Options.PageSize)];
            RenderList(slice, start);
            if (Options.EnablePaging && totalPages > 1)
            {
                Writer.WriteLine($"  -- Page {_page + 1}/{totalPages} (n=next, p=prev) --");
                _lastRenderLines += 1; // include footer line
            }
            _renderedChoices = true;
        }

        // TODO: Implement clearing of previous render
        // (no-op in new save/restore model)

        /// <summary>
        /// Attempts to convert the raw user input to a choice value by number or label matching.
        /// </summary>
        /// <param name="raw">The raw input string from the user.</param>
        /// <param name="value">When successful, contains the selected choice value.</param>
        /// <returns><c>true</c> if the conversion was successful; otherwise, <c>false</c>.</returns>
        protected override bool TryConvert(string raw, out T value)
        {
            if (int.TryParse(raw, out int idx))
            {
                if (idx >= 1 && idx <= _choices.Count)
                {
                    value = _choices[idx - 1].Value;
                    return true;
                }
            }
            Choice<T>? match = _choices.FirstOrDefault(c => c.Label.Equals(raw, StringComparison.OrdinalIgnoreCase))
                        ?? _choices.FirstOrDefault(c => c.Label.StartsWith(raw, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                value = match.Value;
                return true;
            }
            value = default!;
            return false;
        }

        /// <summary>
        /// Prompts the user to select from the available choices and returns the selected value.
        /// </summary>
        /// <returns>The value of the selected choice.</returns>
        public new T GetValue()
        {
            if (_keyReader != null)
            {
                return InteractiveKeyLoop();
            }
            // fallback line-based
            while (true)
            {
                RenderPrompt();
                string? raw = Reader.ReadLine();
                if (Options.EnableEscapeCancel && raw == "\u001b")
                {
                    return HandleCancel();
                }
                // Simple line-based paging navigation (legacy behavior for tests)
                if (Options.EnablePaging && _choices.Count > Options.PageSize && !string.IsNullOrEmpty(raw))
                {
                    List<Choice<T>> list = _choices; // original list for page calculations
                    int totalPages = (list.Count + Options.PageSize - 1) / Options.PageSize;
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
                if (string.IsNullOrEmpty(raw) && Default is not null)
                {
                    return Default!;
                }
                if (TryConvert(raw, out T? converted))
                {
                    return converted;
                }
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
                    System.Console.Write("\u001b[s"); // save cursor
                    _savedCursor = true;
                }
                else
                {
                    System.Console.Write("\u001b[u\u001b[J"); // restore & clear to end of screen
                }

                // Re-render list (always fresh block)
                _renderedChoices = false;
                if (Options.EnablePaging && _choices.Count > Options.PageSize)
                {
                    RenderPageFiltered(filter);
                }
                else
                {
                    List<Choice<T>> listCurrent = ApplyFilter(filter);
                    RenderList(listCurrent);
                    _renderedChoices = true;
                }
                // Prompt line
                RenderPrompt();
                if (Options.EnableFiltering)
                {
                    System.Console.Write($"Filter: {filter}");
                }

                ConsoleKeyInfo key = _keyReader!.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                {
                    if (Options.EnableEscapeCancel)
                    {
                        return HandleCancel();
                    }
                    filter = string.Empty;
                    _page = 0;
                    continue;
                }
                if (key.Key == ConsoleKey.N && Options.EnablePaging && _choices.Count > Options.PageSize)
                {
                    List<Choice<T>> list = ApplyFilter(filter);
                    int totalPages = (list.Count + Options.PageSize - 1) / Options.PageSize;
                    if (totalPages > 1)
                    {
                        _page = (_page + 1) % totalPages;
                    }
                    continue;
                }
                if (key.Key == ConsoleKey.P && Options.EnablePaging && _choices.Count > Options.PageSize)
                {
                    List<Choice<T>> list = ApplyFilter(filter);
                    int totalPages = (list.Count + Options.PageSize - 1) / Options.PageSize;
                    if (totalPages > 1)
                    {
                        _page = (_page - 1 + totalPages) % totalPages;
                    }
                    continue;
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (filter.Length > 0)
                    {
                        filter = filter[..^1];
                        _page = 0;
                    }
                    continue;
                }
                if (key.Key == ConsoleKey.Enter)
                {
                    List<Choice<T>> list = ApplyFilter(filter);
                    if (string.IsNullOrEmpty(filter) && Default is not null)
                    {
                        return Default!;
                    }
                    if (int.TryParse(filter, out int idxEnter) && idxEnter >= 1 && idxEnter <= _choices.Count)
                    {
                        return _choices[idxEnter - 1].Value;
                    }
                    if (list.Count == 1)
                    {
                        return list[0].Value;
                    }
                    Choice<T>? matchEnter = _choices.FirstOrDefault(c => c.Label.Equals(filter, StringComparison.OrdinalIgnoreCase))
                               ?? _choices.FirstOrDefault(c => c.Label.StartsWith(filter, StringComparison.OrdinalIgnoreCase));
                    if (matchEnter != null)
                    {
                        return matchEnter.Value;
                    }
                    WriteError($"Invalid value: '{filter}'");
                    filter = string.Empty;
                    _page = 0;
                    continue;
                }
                if (!char.IsControl(key.KeyChar))
                {
                    if (Options.EnableFiltering)
                    {
                        filter += key.KeyChar;
                        _page = 0;
                        continue;
                    }
                    else
                    {
                        if (char.IsDigit(key.KeyChar))
                        {
                            int d = key.KeyChar - '0';
                            if (d >= 1 && d <= _choices.Count)
                            {
                                return _choices[d - 1].Value;
                            }
                        }
                    }
                }
            }
        }
    }
}
