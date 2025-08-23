using EasyCLI.Console;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Multi-choice selection prompt returning a list of values.
    /// Accepts comma separated indices (e.g. 1,3,4) or ranges (1-3).
    /// </summary>
    /// <typeparam name="T">The type of the selectable values.</typeparam>
    public sealed class MultiSelectPrompt<T> : BasePrompt<IReadOnlyList<T>>
    {
        private readonly List<Choice<T>> _choices;
        private bool _renderedChoices = false;
        private int _page = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiSelectPrompt{T}"/> class.
        /// </summary>
        /// <param name="prompt">The prompt text to display to the user.</param>
        /// <param name="choices">The available choices for the user to select from.</param>
        /// <param name="writer">The console writer to use for output.</param>
        /// <param name="reader">The console reader to use for input.</param>
        /// <param name="options">Options controlling prompt behavior. If null, default options are used.</param>
        public MultiSelectPrompt(string prompt, IEnumerable<Choice<T>> choices, IConsoleWriter writer, IConsoleReader reader, PromptOptions? options = null)
            : base(prompt, writer, reader, options, @default: null)
        {
            _choices = [.. choices];
            if (_choices.Count == 0)
            {
                throw new ArgumentException("Choices cannot be empty", nameof(choices));
            }
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
                    RenderPage();
                }
                else
                {
                    for (int i = 0; i < _choices.Count; i++)
                    {
                        Writer.WriteLine($"  {i + 1}) {_choices[i].Label}");
                    }
                    _renderedChoices = true;
                }
            }
            base.RenderPrompt();
        }

        private void RenderPage()
        {
            int totalPages = (_choices.Count + Options.PageSize - 1) / Options.PageSize;
            int start = _page * Options.PageSize;
            int endExclusive = Math.Min(start + Options.PageSize, _choices.Count);
            for (int i = start; i < endExclusive; i++)
            {
                Writer.WriteLine($"  {i + 1}) {_choices[i].Label}");
            }
            Writer.WriteLine($"  -- Page {_page + 1}/{totalPages} (n=next, p=prev) --");
        }

        /// <summary>
        /// Attempts to convert the raw user input to a list of selected choice values.
        /// </summary>
        /// <param name="raw">The raw input string from the user.</param>
        /// <param name="value">When successful, contains the list of selected choice values.</param>
        /// <returns><c>true</c> if the conversion was successful; otherwise, <c>false</c>.</returns>
        protected override bool TryConvert(string raw, out IReadOnlyList<T> value)
        {
            // paging navigation (does not select values directly)
            if (Options.EnablePaging && _choices.Count > Options.PageSize)
            {
                if (raw.Equals("n", StringComparison.OrdinalIgnoreCase) || raw.Equals("next", StringComparison.OrdinalIgnoreCase))
                {
                    int totalPages = (_choices.Count + Options.PageSize - 1) / Options.PageSize;
                    _page = (_page + 1) % totalPages;
                    RenderPage();
                    value = default!;
                    return false;
                }

                if (raw.Equals("p", StringComparison.OrdinalIgnoreCase) || raw.Equals("prev", StringComparison.OrdinalIgnoreCase))
                {
                    int totalPages = (_choices.Count + Options.PageSize - 1) / Options.PageSize;
                    _page = (_page - 1 + totalPages) % totalPages;
                    RenderPage();
                    value = default!;
                    return false;
                }
            }

            List<T> results = new List<T>();
            string[] tokens = raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (string token in tokens)
            {
                if (token.Contains('-'))
                {
                    string[] parts = token.Split('-', 2);
                    if (int.TryParse(parts[0], out int startRange) && int.TryParse(parts[1], out int endRange))
                    {
                        if (startRange > endRange)
                        {
                            (startRange, endRange) = (endRange, startRange);
                        }

                        for (int i = startRange; i <= endRange; i++)
                        {
                            if (i >= 1 && i <= _choices.Count)
                            {
                                results.Add(_choices[i - 1].Value);
                            }
                            else
                            {
                                value = default!;
                                return false;
                            }
                        }
                        continue;
                    }
                    value = default!;
                    return false;
                }
                if (int.TryParse(token, out int idx))
                {
                    if (idx >= 1 && idx <= _choices.Count)
                    {
                        results.Add(_choices[idx - 1].Value);
                    }
                    else
                    {
                        value = default!;
                        return false;
                    }
                }
                else
                {
                    value = default!;
                    return false;
                }
            }
            value = results;
            return results.Count > 0;
        }
    }
}
