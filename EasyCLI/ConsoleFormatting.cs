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
    }
}
