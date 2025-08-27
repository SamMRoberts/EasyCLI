namespace EasyCLI.Formatting
{
    /// <summary>
    /// Pure formatting helpers (no I/O). These build strings/enumerables; rendering is handled elsewhere.
    /// </summary>
    public static class ConsoleFormatting
    {
        // Reused split characters to avoid per-call allocations (CA1861)
        private static readonly char[] WrapSplitChars = [' ', '\n'];

        /// <summary>
        /// Truncates <paramref name="value"/> to the specified maximum length and appends an ellipsis if truncation occurs.
        /// </summary>
        /// <param name="value">The original value.</param>
        /// <param name="maxWithEllipsis">The maximum number of characters including the ellipsis.</param>
        /// <returns>The original string or a truncated form with ellipsis.</returns>
        private static string TruncateWithEllipsis(string value, int maxWithEllipsis)
        {
            if (maxWithEllipsis <= 0)
            {
                return string.Empty;
            }

            if (value.Length <= maxWithEllipsis)
            {
                return value;
            }

            // Use span-based concat (CA1845)
            return string.Concat(value.AsSpan(0, maxWithEllipsis), "…");
        }

        private static int GetConsoleWidthOr(int fallback)
        {
            try
            {
                int w = System.Console.WindowWidth;
                return w > 0 ? w : fallback;
            }
            catch
            {
                return fallback;
            }
        }

        /// <summary>
        /// Creates a horizontal rule line using the specified character and width.
        /// </summary>
        /// <param name="ch">The character to use for the rule.</param>
        /// <param name="width">The width of the rule. If 0 or negative, uses console width.</param>
        /// <returns>A string containing the rule line.</returns>
        public static string Rule(char ch = '─', int width = 80)
        {
            if (width <= 0)
            {
                width = GetConsoleWidthOr(80);
            }

            return new string(ch, width);
        }

        /// <summary>
        /// Creates a horizontal rule line with a title on the left side.
        /// </summary>
        /// <param name="title">The title text to display.</param>
        /// <param name="filler">The character to use for the rule portion.</param>
        /// <param name="width">The total width of the rule. If 0 or negative, uses console width.</param>
        /// <param name="gap">The number of spaces between the title and the rule.</param>
        /// <returns>A string containing the titled rule line.</returns>
        public static string TitleRule(string title, char filler = '─', int width = 0, int gap = 1)
        {
            title ??= string.Empty;
            if (width <= 0)
            {
                width = GetConsoleWidthOr(80);
            }

            string g = new(' ', Math.Max(0, gap));
            string prefix = string.IsNullOrEmpty(title) ? string.Empty : title + g;
            int remaining = Math.Max(0, width - prefix.Length);
            return prefix + new string(filler, remaining);
        }

        /// <summary>
        /// Creates a horizontal rule line with a title centered within it.
        /// </summary>
        /// <param name="title">The title text to display.</param>
        /// <param name="filler">The character to use for the rule portions.</param>
        /// <param name="width">The total width of the rule. If 0 or negative, uses console width.</param>
        /// <param name="gap">The number of spaces around the title.</param>
        /// <returns>A string containing the centered titled rule line.</returns>
        public static string CenterTitleRule(string title, char filler = '─', int width = 0, int gap = 1)
        {
            title ??= string.Empty;
            if (width <= 0)
            {
                width = GetConsoleWidthOr(80);
            }

            string titleBlock = string.IsNullOrEmpty(title) ? string.Empty : new string(' ', Math.Max(0, gap)) + title + new string(' ', Math.Max(0, gap));
            if (titleBlock.Length >= width)
            {
                // Truncate with ellipsis to fit
                if (width <= 1)
                {
                    return new string(filler, Math.Max(0, width));
                }

                if (width == 2)
                {
                    return new string(filler, 2);
                }

                string content = titleBlock.Trim();
                if (content.Length > width - 1)
                {
                    content = TruncateWithEllipsis(content, width - 1);
                }

                return content.PadLeft((width + content.Length) / 2).PadRight(width);
            }

            int remaining = width - titleBlock.Length;
            int left = remaining / 2;
            int right = remaining - left;
            return new string(filler, left) + titleBlock + new string(filler, right);
        }

        /// <summary>
        /// Creates an underline string that matches the length of the specified text.
        /// </summary>
        /// <param name="text">The text to create an underline for.</param>
        /// <param name="underlineChar">The character to use for the underline.</param>
        /// <returns>A string containing the underline characters.</returns>
        public static string HeadingUnderline(string text, char underlineChar = '─')
        {
            text ??= string.Empty;
            return new string(underlineChar, text.Length);
        }

        /// <summary>
        /// Indents all lines of the specified text by the given level and size.
        /// </summary>
        /// <param name="text">The text to indent.</param>
        /// <param name="level">The indentation level (multiplier).</param>
        /// <param name="size">The number of spaces per indentation level.</param>
        /// <returns>The indented text.</returns>
        public static string Indent(string text, int level, int size = 2)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text ?? string.Empty;
            }

            if (level <= 0 || size <= 0)
            {
                return text;
            }

            string pad = new(' ', level * size);
            string[] lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = pad + lines[i];
            }

            return string.Join(System.Environment.NewLine, lines);
        }

        /// <summary>
        /// Wraps text to fit within the specified width, breaking at word boundaries.
        /// </summary>
        /// <param name="text">The text to wrap.</param>
        /// <param name="width">The maximum width per line. If 0 or negative, uses console width.</param>
        /// <returns>An enumerable of strings representing the wrapped lines.</returns>
        public static IEnumerable<string> Wrap(string text, int width)
        {
            if (string.IsNullOrEmpty(text))
            {
                yield break;
            }

            if (width <= 0)
            {
                width = GetConsoleWidthOr(80);
            }

            if (width <= 10)
            {
                yield return text;
                yield break;
            }

            // Use cached array to avoid reallocation (CA1861)
            string[] words = text.Replace("\r\n", "\n").Replace('\r', '\n').Split(WrapSplitChars, StringSplitOptions.None);
            StringBuilder sb = new();
            foreach (string word in words)
            {
                if (word == "\n")
                {
                    if (sb.Length > 0)
                    {
                        yield return sb.ToString();
                        _ = sb.Clear();
                    }

                    continue;
                }

                if (sb.Length == 0)
                {
                    _ = sb.Append(word);
                }
                else if (sb.Length + 1 + word.Length <= width)
                {
                    _ = sb.Append(' ').Append(word);
                }
                else
                {
                    yield return sb.ToString();
                    _ = sb.Clear();
                    _ = sb.Append(word);
                }
            }

            if (sb.Length > 0)
            {
                yield return sb.ToString();
            }
        }

        /// <summary>
        /// Builds formatted key-value pairs with aligned values.
        /// </summary>
        /// <param name="items">The key-value pairs to format.</param>
        /// <param name="indent">The number of spaces to indent each line.</param>
        /// <param name="gap">The minimum number of spaces between the key and value.</param>
        /// <param name="sep">The separator character to place after the key.</param>
        /// <returns>An enumerable of formatted key-value lines.</returns>
        public static IEnumerable<string> BuildKeyValues(IEnumerable<(string key, string value)> items, int indent = 0, int gap = 2, string sep = ":")
        {
            if (items == null)
            {
                yield break;
            }

            int keyWidth = 0;
            List<(string key, string value)> snapshot = [];
            foreach ((string key, string value) in items)
            {
                string kk = key ?? string.Empty;
                string vv = value ?? string.Empty;
                snapshot.Add((kk, vv));
                if (kk.Length > keyWidth)
                {
                    keyWidth = kk.Length;
                }
            }

            string pad = indent > 0 ? new string(' ', indent) : string.Empty;
            string gapSpaces = new(' ', gap < 0 ? 0 : gap);
            foreach ((string key, string value) in snapshot)
            {
                yield return pad + key.PadRight(keyWidth) + " " + sep + gapSpaces + value;
            }
        }

        /// <summary>
        /// Alignment for table cells.
        /// </summary>
        public enum CellAlign
        {
            /// <summary>Left aligned content.</summary>
            Left,

            /// <summary>Centered content.</summary>
            Center,

            /// <summary>Right aligned content.</summary>
            Right,
        }

        /// <summary>
        /// Builds a simple text table with headers and rows.
        /// </summary>
        /// <param name="headers">The column headers.</param>
        /// <param name="rows">The data rows.</param>
        /// <param name="padding">The padding around cell content.</param>
        /// <param name="maxWidth">The maximum table width. If 0, no width limit is applied.</param>
        /// <param name="alignments">The alignment for each column. If null, all columns are left-aligned.</param>
        /// <returns>An enumerable of strings representing the table lines.</returns>
        public static IEnumerable<string> BuildSimpleTable(IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows, int padding = 1, int maxWidth = 0, IReadOnlyList<CellAlign>? alignments = null)
        {
            headers ??= [];
            int cols = headers.Count;
            List<IReadOnlyList<string>> rowList = [];
            if (rows != null)
            {
                foreach (IReadOnlyList<string> r in rows)
                {
                    rowList.Add(r);
                }
            }

            foreach (IReadOnlyList<string> r in rowList)
            {
                if (r.Count > cols)
                {
                    cols = r.Count;
                }
            }

            if (cols == 0)
            {
                yield break;
            }

            int[] widths = new int[cols];
            for (int c = 0; c < cols; c++)
            {
                int w = 0;
                if (c < headers.Count)
                {
                    w = Math.Max(w, (headers[c] ?? string.Empty).Length);
                }

                foreach (IReadOnlyList<string> r in rowList)
                {
                    if (c < r.Count)
                    {
                        w = Math.Max(w, (r[c] ?? string.Empty).Length);
                    }
                }

                widths[c] = w + (padding * 2);
            }

            int avail = maxWidth > 0 ? maxWidth : GetConsoleWidthOr(0);
            if (avail > 0)
            {
                int borderWidth = cols + 1;
                for (int c = 0; c < cols; c++)
                {
                    borderWidth += widths[c];
                }
                if (borderWidth > avail)
                {
                    int reduce = borderWidth - avail;
                    int minCol = Math.Max(3, (padding * 2) + 1);
                    bool reduced;
                    int guard = 10000;
                    do
                    {
                        reduced = false;
                        for (int c = 0; c < cols && reduce > 0; c++)
                        {
                            if (widths[c] > minCol)
                            {
                                widths[c] -= 1;
                                reduce -= 1;
                                reduced = true;
                            }
                        }
                        guard--;
                        if (guard == 0)
                        {
                            break;
                        }
                    }
                    while (reduce > 0 && reduced);
                }
            }

            CellAlign GetAlign(int c)
            {
                return alignments != null && c < alignments.Count ? alignments[c] : CellAlign.Left;
            }

            string PadCell(string? s, int widthPer, CellAlign align)
            {
                s ??= string.Empty;
                int innerWidth = Math.Max(0, widthPer - (padding * 2));
                string content = s.Length > innerWidth ? innerWidth <= 0 ? string.Empty : innerWidth == 1 ? "…" : TruncateWithEllipsis(s, innerWidth - 1) : s;
                int remaining = Math.Max(0, innerWidth - content.Length);
                int leftPad = 0;
                int rightPad = remaining;
                switch (align)
                {
                    case CellAlign.Right:
                        leftPad = remaining;
                        rightPad = 0;
                        break;
                    case CellAlign.Center:
                        leftPad = remaining / 2;
                        rightPad = remaining - leftPad;
                        break;
                    case CellAlign.Left:
                    default:
                        leftPad = 0;
                        rightPad = remaining;
                        break;
                }

                return new string(' ', padding) + new string(' ', leftPad) + content + new string(' ', rightPad) + new string(' ', padding);
            }

            string MakeSep()
            {
                string[] parts = new string[cols];
                for (int c = 0; c < cols; c++)
                {
                    parts[c] = new string('-', widths[c]);
                }

                return "+" + string.Join("+", parts) + "+";
            }

            string MakeRow(IReadOnlyList<string> r)
            {
                string[] parts = new string[cols];
                for (int c = 0; c < cols; c++)
                {
                    string cell = c < r.Count ? r[c] : string.Empty;
                    parts[c] = PadCell(cell, widths[c], GetAlign(c));
                }
                return "|" + string.Join("|", parts) + "|";
            }

            string top = MakeSep();
            yield return top;
            if (headers.Count > 0)
            {
                yield return MakeRow(headers);
                yield return MakeSep();
            }
            foreach (IReadOnlyList<string> r in rowList)
            {
                yield return MakeRow(r);
            }
            yield return MakeSep();
        }

        /// <summary>
        /// Builds a text box around the specified content lines using Unicode box-drawing characters.
        /// </summary>
        /// <param name="contentLines">The lines of content to enclose in the box.</param>
        /// <param name="padding">The padding inside the box around the content.</param>
        /// <param name="h">The horizontal border character.</param>
        /// <param name="v">The vertical border character.</param>
        /// <param name="tl">The top-left corner character.</param>
        /// <param name="tr">The top-right corner character.</param>
        /// <param name="bl">The bottom-left corner character.</param>
        /// <param name="br">The bottom-right corner character.</param>
        /// <returns>An enumerable of strings representing the box lines.</returns>
        public static IEnumerable<string> BuildBox(
            IEnumerable<string> contentLines,
            int padding = 1,
            char h = '─',
            char v = '│',
            char tl = '┌',
            char tr = '┐',
            char bl = '└',
            char br = '┘')
        {
            List<string> lines = [];
            if (contentLines != null)
            {
                lines.AddRange(contentLines);
            }

            int innerWidth = 0;
            foreach (string l in lines)
            {
                innerWidth = Math.Max(innerWidth, (l ?? string.Empty).Length);
            }
            innerWidth += padding * 2;
            string top = tl + new string(h, innerWidth) + tr;
            string bottom = bl + new string(h, innerWidth) + br;
            yield return top;
            string pad = new(' ', padding);
            foreach (string l in lines)
            {
                string s = l ?? string.Empty;
                int extra = innerWidth - (padding * 2) - s.Length;
                if (extra < 0)
                {
                    extra = 0;
                }
                yield return v + pad + s + new string(' ', extra) + pad + v;
            }
            yield return bottom;
        }

        /// <summary>
        /// Builds a text box with a title around the specified content lines using Unicode box-drawing characters.
        /// </summary>
        /// <param name="contentLines">The lines of content to enclose in the box.</param>
        /// <param name="title">The title to display in the top border of the box.</param>
        /// <param name="padding">The padding inside the box around the content.</param>
        /// <param name="h">The horizontal border character.</param>
        /// <param name="v">The vertical border character.</param>
        /// <param name="tl">The top-left corner character.</param>
        /// <param name="tr">The top-right corner character.</param>
        /// <param name="bl">The bottom-left corner character.</param>
        /// <param name="br">The bottom-right corner character.</param>
        /// <returns>An enumerable of strings representing the titled box lines.</returns>
        public static IEnumerable<string> BuildTitledBox(
            IEnumerable<string> contentLines,
            string title,
            int padding = 1,
            char h = '─',
            char v = '│',
            char tl = '┌',
            char tr = '┐',
            char bl = '└',
            char br = '┘')
        {
            title ??= string.Empty;
            List<string> lines = [];
            if (contentLines != null)
            {
                lines.AddRange(contentLines);
            }

            int innerWidth = Math.Max(title.Length + 2, 0);
            foreach (string l in lines)
            {
                innerWidth = Math.Max(innerWidth, (l ?? string.Empty).Length);
            }
            innerWidth += padding * 2;
            string titleBlock = string.IsNullOrEmpty(title) ? string.Empty : " " + title + " ";
            int total = innerWidth;
            int left = Math.Max(0, (total - titleBlock.Length) / 2);
            int right = Math.Max(0, total - titleBlock.Length - left);
            string top = tl + new string(h, left) + titleBlock + new string(h, right) + tr;
            yield return top;
            string pad = new(' ', padding);
            foreach (string l in lines)
            {
                string s = l ?? string.Empty;
                int extra = innerWidth - (padding * 2) - s.Length;
                if (extra < 0)
                {
                    extra = 0;
                }

                yield return v + pad + s + new string(' ', extra) + pad + v;
            }
            string bottom = bl + new string(h, innerWidth) + br;
            yield return bottom;
        }
    }
}
