using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCLI
{
    /// <summary>
    /// Pure formatting helpers (no I/O). Useful for composing text with rules, headings, indentation, and wrapping.
    /// </summary>
    public static class ConsoleFormatting
    {
        public static string Rule(char ch = '─', int width = 80)
        {
            if (width <= 0) width = 80;
            return new string(ch, width);
        }

        public static string HeadingUnderline(string text, char underlineChar = '─')
        {
            if (text == null) text = string.Empty;
            return new string(underlineChar, text.Length);
        }

        public static string Indent(string text, int level, int size = 2)
        {
            if (string.IsNullOrEmpty(text)) return text ?? string.Empty;
            if (level <= 0 || size <= 0) return text;
            string pad = new string(' ', level * size);
            var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            for (int i = 0; i < lines.Length; i++) lines[i] = pad + lines[i];
            return string.Join(Environment.NewLine, lines);
        }

        public static IEnumerable<string> Wrap(string text, int width)
        {
            if (string.IsNullOrEmpty(text)) yield break;
            if (width <= 10) { yield return text; yield break; }

            var words = text.Replace("\r\n", "\n").Replace('\r', '\n').Split(new[] { ' ', '\n' }, StringSplitOptions.None);
            var sb = new StringBuilder();
            foreach (var word in words)
            {
                if (word == "\n")
                {
                    if (sb.Length > 0) { yield return sb.ToString(); sb.Clear(); }
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
            if (sb.Length > 0) yield return sb.ToString();
        }

        public static IEnumerable<string> BuildKeyValues(IEnumerable<(string key, string value)> items, int indent = 0, int gap = 2, string sep = ":")
        {
            if (items == null) yield break;
            int keyWidth = 0;
            var snapshot = new List<(string key, string value)>();
            foreach (var (k, v) in items)
            {
                var kk = k ?? string.Empty;
                var vv = v ?? string.Empty;
                snapshot.Add((kk, vv));
                if (kk.Length > keyWidth) keyWidth = kk.Length;
            }
            string pad = indent > 0 ? new string(' ', indent) : string.Empty;
            string gapSpaces = new string(' ', gap < 0 ? 0 : gap);
            foreach (var (k, v) in snapshot)
            {
                yield return pad + k.PadRight(keyWidth) + " " + sep + gapSpaces + v;
            }
        }

        public static IEnumerable<string> BuildSimpleTable(IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows, int padding = 1)
        {
            headers ??= Array.Empty<string>();
            var cols = headers.Count;
            var rowList = new List<IReadOnlyList<string>>();
            if (rows != null)
                rowList.AddRange(rows);

            // Determine column count by max across headers/rows
            foreach (var r in rowList)
                if (r.Count > cols) cols = r.Count;

            int[] widths = new int[cols];
            for (int c = 0; c < cols; c++)
            {
                int w = 0;
                if (c < headers.Count) w = Math.Max(w, (headers[c] ?? string.Empty).Length);
                foreach (var r in rowList)
                    if (c < r.Count) w = Math.Max(w, (r[c] ?? string.Empty).Length);
                widths[c] = w + (padding * 2);
            }

            string PadCell(string? s, int width)
            {
                s ??= string.Empty;
                var inner = new string(' ', padding) + s + new string(' ', Math.Max(0, width - padding - s.Length));
                return inner;
            }

            string MakeSep()
            {
                var parts = new string[cols];
                for (int c = 0; c < cols; c++) parts[c] = new string('-', widths[c]);
                return "+" + string.Join("+", parts) + "+";
            }

            string MakeRow(IReadOnlyList<string> r)
            {
                var parts = new string[cols];
                for (int c = 0; c < cols; c++)
                {
                    var cell = c < r.Count ? r[c] : string.Empty;
                    parts[c] = PadCell(cell, widths[c]);
                }
                return "|" + string.Join("|", parts) + "|";
            }

            if (cols == 0)
                yield break;

            var top = MakeSep();
            yield return top;
            if (headers.Count > 0)
            {
                yield return MakeRow(headers);
                yield return MakeSep();
            }
            foreach (var r in rowList)
                yield return MakeRow(r);
            yield return MakeSep();
        }

        public static IEnumerable<string> BuildBox(IEnumerable<string> contentLines, int padding = 1,
            char h = '─', char v = '│', char tl = '┌', char tr = '┐', char bl = '└', char br = '┘')
        {
            var lines = new List<string>();
            if (contentLines != null) lines.AddRange(contentLines);
            int innerWidth = 0;
            foreach (var l in lines) innerWidth = Math.Max(innerWidth, (l ?? string.Empty).Length);
            innerWidth += padding * 2;
            string top = tl + new string(h, innerWidth) + tr;
            string bottom = bl + new string(h, innerWidth) + br;
            yield return top;
            string pad = new string(' ', padding);
            foreach (var l in lines)
            {
                var s = l ?? string.Empty;
                var extra = innerWidth - (padding * 2) - s.Length;
                if (extra < 0) extra = 0;
                yield return v + pad + s + new string(' ', extra) + pad + v;
            }
            yield return bottom;
        }
    }

    /// <summary>
    /// IConsoleWriter extensions for formatted output using the above helpers and ANSI styles.
    /// </summary>
    public static class ConsoleWriterFormattingExtensions
    {
        public static void WriteRule(this IConsoleWriter w, int width = 80, char ch = '─', ConsoleStyle? style = null)
        {
            var line = ConsoleFormatting.Rule(ch, width);
            if (style.HasValue) w.WriteLine(line, style.Value); else w.WriteLine(line);
        }

        public static void WriteHeadingBlock(this IConsoleWriter w, string text, ConsoleStyle? titleStyle = null, ConsoleStyle? underlineStyle = null, char underlineChar = '─')
        {
            if (titleStyle.HasValue) w.WriteLine(text, titleStyle.Value); else w.WriteLine(text);
            var underline = ConsoleFormatting.HeadingUnderline(text, underlineChar);
            if (underlineStyle.HasValue) w.WriteLine(underline, underlineStyle.Value); else w.WriteLine(underline);
        }

        public static void WriteBullets(this IConsoleWriter w, IEnumerable<string> items, string bullet = "•", int indent = 0, ConsoleStyle? bulletStyle = null, ConsoleStyle? textStyle = null)
        {
            string pad = indent > 0 ? new string(' ', indent) : string.Empty;
            foreach (var item in items)
            {
                if (bulletStyle.HasValue) w.Write(pad + bullet + " ", bulletStyle.Value); else w.Write(pad + bullet + " ");
                if (textStyle.HasValue) w.WriteLine(item, textStyle.Value); else w.WriteLine(item);
            }
        }

        public static void WriteWrapped(this IConsoleWriter w, string text, int width = 80, int indent = 0, ConsoleStyle? style = null)
        {
            string pad = indent > 0 ? new string(' ', indent) : string.Empty;
            foreach (var line in ConsoleFormatting.Wrap(text, width - pad.Length))
            {
                var output = pad + line;
                if (style.HasValue) w.WriteLine(output, style.Value); else w.WriteLine(output);
            }
        }

        public static void WriteLineWithPrefix(this IConsoleWriter w, string prefix, string message, ConsoleStyle? prefixStyle = null, ConsoleStyle? messageStyle = null)
        {
            if (prefixStyle.HasValue) w.Write(prefix, prefixStyle.Value); else w.Write(prefix);
            w.Write(" ");
            if (messageStyle.HasValue) w.WriteLine(message, messageStyle.Value); else w.WriteLine(message);
        }

        public static string TimestampPrefix(string? format = "HH:mm:ss")
        {
            return DateTime.Now.ToString(format ?? "HH:mm:ss");
        }

        public static void WriteKeyValues(this IConsoleWriter w, IEnumerable<(string key, string value)> items, int indent = 0, int gap = 2, string sep = ":", ConsoleStyle? keyStyle = null, ConsoleStyle? valueStyle = null)
        {
            foreach (var line in ConsoleFormatting.BuildKeyValues(items, indent, gap, sep))
            {
                if (keyStyle.HasValue || valueStyle.HasValue)
                {
                    // Split back into key/sep/value to apply styles separately
                    var idx = line.IndexOf(sep);
                    if (idx > 0)
                    {
                        var before = line.Substring(0, idx);
                        var after = line.Substring(idx, line.Length - idx);
                        if (keyStyle.HasValue) w.Write(before, keyStyle.Value); else w.Write(before);
                        w.Write(after.Substring(0, sep.Length));
                        var rest = after.Substring(sep.Length);
                        if (valueStyle.HasValue) w.WriteLine(rest, valueStyle.Value); else w.WriteLine(rest);
                        continue;
                    }
                }
                w.WriteLine(line);
            }
        }

        public static void WriteTableSimple(this IConsoleWriter w, IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows, ConsoleStyle? borderStyle = null, ConsoleStyle? headerStyle = null, ConsoleStyle? cellStyle = null)
        {
            bool headerPrinted = false;
            foreach (var line in ConsoleFormatting.BuildSimpleTable(headers, rows))
            {
                if (line.StartsWith("+") && borderStyle.HasValue)
                {
                    w.WriteLine(line, borderStyle.Value);
                }
                else if (!headerPrinted && headers.Count > 0)
                {
                    w.WriteLine(line, headerStyle ?? cellStyle ?? default);
                    headerPrinted = true;
                }
                else if (line.StartsWith("|") && cellStyle.HasValue)
                {
                    w.WriteLine(line, cellStyle.Value);
                }
                else
                {
                    w.WriteLine(line);
                }
            }
        }

        public static void WriteBox(this IConsoleWriter w, IEnumerable<string> contentLines, ConsoleStyle? borderStyle = null, ConsoleStyle? textStyle = null)
        {
            foreach (var line in ConsoleFormatting.BuildBox(contentLines))
            {
                if ((line.StartsWith("┌") || line.StartsWith("└") || line.StartsWith("│") || line.StartsWith("┐") || line.StartsWith("┘")) && borderStyle.HasValue)
                    w.WriteLine(line, borderStyle.Value);
                else if (textStyle.HasValue && line.StartsWith("│"))
                    w.WriteLine(line, textStyle.Value);
                else
                    w.WriteLine(line);
            }
        }
    }
}
