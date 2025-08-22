namespace EasyCLI.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Pure formatting helpers (no I/O). These build strings/enumerables; rendering is handled elsewhere.
    /// </summary>
    public static class ConsoleFormatting
    {
        // Reused split characters to avoid per-call allocations (CA1861)
        private static readonly char[] WrapSplitChars = new[] { ' ', '\n' };

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
                int w = Console.WindowWidth;
                return w > 0 ? w : fallback;
            }
            catch
            {
                return fallback;
            }
        }

        public static string Rule(char ch = '─', int width = 80)
        {
            if (width <= 0)
            {
                width = GetConsoleWidthOr(80);
            }

            return new string(ch, width);
        }

        public static string TitleRule(string title, char filler = '─', int width = 0, int gap = 1)
        {
            title ??= string.Empty;
            if (width <= 0)
            {
                width = GetConsoleWidthOr(80);
            }

            string g = new string(' ', Math.Max(0, gap));
            string prefix = string.IsNullOrEmpty(title) ? string.Empty : title + g;
            int remaining = Math.Max(0, width - prefix.Length);
            return prefix + new string(filler, remaining);
        }

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

        public static string HeadingUnderline(string text, char underlineChar = '─')
        {
            text ??= string.Empty;
            return new string(underlineChar, text.Length);
        }

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

            string pad = new string(' ', level * size);
            string[] lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = pad + lines[i];
            }

            return string.Join(Environment.NewLine, lines);
        }

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
            StringBuilder sb = new StringBuilder();
            foreach (string word in words)
            {
                if (word == "\n")
                {
                    if (sb.Length > 0)
                    {
                        yield return sb.ToString();
                        sb.Clear();
                    }

                    continue;
                }

                if (sb.Length == 0)
                {
                    sb.Append(word);
                }
                else if (sb.Length + 1 + word.Length <= width)
                {
                    sb.Append(' ').Append(word);
                }
                else
                {
                    yield return sb.ToString();
                    sb.Clear();
                    sb.Append(word);
                }
            }

            if (sb.Length > 0)
            {
                yield return sb.ToString();
            }
        }

        public static IEnumerable<string> BuildKeyValues(IEnumerable<(string key, string value)> items, int indent = 0, int gap = 2, string sep = ":")
        {
            if (items == null)
            {
                yield break;
            }

            int keyWidth = 0;
            List<(string key, string value)> snapshot = new List<(string key, string value)>();
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
            string gapSpaces = new string(' ', gap < 0 ? 0 : gap);
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

        public static IEnumerable<string> BuildSimpleTable(IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows, int padding = 1, int maxWidth = 0, IReadOnlyList<CellAlign>? alignments = null)
        {
            headers ??= Array.Empty<string>();
            int cols = headers.Count;
            List<IReadOnlyList<string>> rowList = new List<IReadOnlyList<string>>();
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
                int borderWidth = (cols + 1);
                for (int c = 0; c < cols; c++)
                {
                    borderWidth += widths[c];
                }
                if (borderWidth > avail)
                {
                    int reduce = borderWidth - avail;
                    int minCol = Math.Max(3, padding * 2 + 1);
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
                if (alignments != null && c < alignments.Count)
                {
                    return alignments[c];
                }

                return CellAlign.Left;
            }

            string PadCell(string? s, int widthPer, CellAlign align)
            {
                s ??= string.Empty;
                int innerWidth = Math.Max(0, widthPer - (padding * 2));
                string content;
                if (s.Length > innerWidth)
                {
                    if (innerWidth <= 0)
                    {
                        content = string.Empty;
                    }
                    else if (innerWidth == 1)
                    {
                        content = "…";
                    }
                    else
                    {
                        content = TruncateWithEllipsis(s, innerWidth - 1);
                    }
                }
                else
                {
                    content = s;
                }

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
            List<string> lines = new();
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
            List<string> lines = new();
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
