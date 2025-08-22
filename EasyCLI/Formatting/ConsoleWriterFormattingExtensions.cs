namespace EasyCLI.Formatting;

using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// IConsoleWriter extensions for formatted output using formatting helpers and ANSI styles.
/// </summary>
public static class ConsoleWriterFormattingExtensions
{
    public static void WriteRule(this IConsoleWriter writer, int width = 80, char ch = '─', ConsoleStyle? style = null)
    {
        ArgumentNullException.ThrowIfNull(writer);
        string line = ConsoleFormatting.Rule(ch, width);
        if (style.HasValue)
        {
            writer.WriteLine(line, style.Value);
        }
        else
        {
            writer.WriteLine(line);
        }
    }

    public static void WriteTitleRule(this IConsoleWriter writer, string title, int width = 0, char filler = '─', int gap = 1, ConsoleStyle? titleStyle = null, ConsoleStyle? fillerStyle = null)
    {
        ArgumentNullException.ThrowIfNull(writer);
        string line = ConsoleFormatting.TitleRule(title, filler, width, gap);
        if (string.IsNullOrEmpty(title))
        {
            if (fillerStyle.HasValue)
            {
                writer.WriteLine(line, fillerStyle.Value);
            }
            else
            {
                writer.WriteLine(line);
            }

            return;
        }

        string gapSpaces = new(' ', Math.Max(0, gap));
        string prefix = title + gapSpaces;
        string suffix = line[prefix.Length..];
        if (titleStyle.HasValue)
        {
            writer.Write(prefix, titleStyle.Value);
        }
        else
        {
            writer.Write(prefix);
        }

        if (fillerStyle.HasValue)
        {
            writer.WriteLine(suffix, fillerStyle.Value);
        }
        else
        {
            writer.WriteLine(suffix);
        }
    }

    public static void WriteCenterTitleRule(this IConsoleWriter writer, string title, int width = 0, char filler = '─', int gap = 1, ConsoleStyle? titleStyle = null, ConsoleStyle? fillerStyle = null)
    {
        ArgumentNullException.ThrowIfNull(writer);
        string s = ConsoleFormatting.CenterTitleRule(title, filler, width, gap);
        if (string.IsNullOrEmpty(title) || (!titleStyle.HasValue && !fillerStyle.HasValue))
        {
            if (fillerStyle.HasValue)
            {
                writer.WriteLine(s, fillerStyle.Value);
            }
            else
            {
                writer.WriteLine(s);
            }

            return;
        }

        string titleBlock = (gap > 0 ? new string(' ', gap) : string.Empty) + (title ?? string.Empty) + (gap > 0 ? new string(' ', gap) : string.Empty);
        int idx = s.IndexOf(titleBlock, StringComparison.Ordinal);
        if (idx < 0)
        {
            if (titleStyle.HasValue)
            {
                writer.WriteLine(s, titleStyle.Value);
            }
            else
            {
                writer.WriteLine(s);
            }

            return;
        }

        string left = s[..idx];
        string mid = titleBlock;
        string right = s[(idx + titleBlock.Length)..];
        if (fillerStyle.HasValue)
        {
            writer.Write(left, fillerStyle.Value);
        }
        else
        {
            writer.Write(left);
        }

        if (titleStyle.HasValue)
        {
            writer.Write(mid, titleStyle.Value);
        }
        else
        {
            writer.Write(mid);
        }

        if (fillerStyle.HasValue)
        {
            writer.WriteLine(right, fillerStyle.Value);
        }
        else
        {
            writer.WriteLine(right);
        }
    }

    public static void WriteTitledBox(this IConsoleWriter writer, IEnumerable<string> contentLines, string title, ConsoleStyle? borderStyle = null, ConsoleStyle? titleStyle = null, ConsoleStyle? textStyle = null)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(contentLines);
        foreach (string line in ConsoleFormatting.BuildTitledBox(contentLines, title))
        {
            bool isBorder = line.StartsWith('┌') || line.StartsWith('└') || line.StartsWith('│') || line.StartsWith('┐') || line.StartsWith('┘');
            if (isBorder && borderStyle.HasValue)
            {
                if (titleStyle.HasValue && line.StartsWith('┌'))
                {
                    int start = line.IndexOf(' ');
                    int end = line.LastIndexOf(' ');
                    if (start >= 0 && end > start)
                    {
                        string leftPart = line[..start];
                        string mid = line[start..end];
                        string right = line[end..];
                        writer.Write(leftPart, borderStyle.Value);
                        writer.Write(mid, titleStyle.Value);
                        writer.WriteLine(right, borderStyle.Value);
                        continue;
                    }
                }

                writer.WriteLine(line, borderStyle.Value);
            }
            else if (!isBorder && textStyle.HasValue)
            {
                writer.WriteLine(line, textStyle.Value);
            }
            else
            {
                writer.WriteLine(line);
            }
        }
    }

    public static void WriteHeadingBlock(this IConsoleWriter writer, string text, ConsoleStyle? titleStyle = null, ConsoleStyle? underlineStyle = null, char underlineChar = '─')
    {
        ArgumentNullException.ThrowIfNull(writer);
        if (titleStyle.HasValue)
        {
            writer.WriteLine(text, titleStyle.Value);
        }
        else
        {
            writer.WriteLine(text);
        }

        string underline = ConsoleFormatting.HeadingUnderline(text, underlineChar);
        if (underlineStyle.HasValue)
        {
            writer.WriteLine(underline, underlineStyle.Value);
        }
        else
        {
            writer.WriteLine(underline);
        }
    }

    public static void WriteBullets(this IConsoleWriter writer, IEnumerable<string> items, string bullet = "•", int indent = 0, ConsoleStyle? bulletStyle = null, ConsoleStyle? textStyle = null)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(items);
        string pad = indent > 0 ? new string(' ', indent) : string.Empty;
        foreach (string item in items)
        {
            if (bulletStyle.HasValue)
            {
                writer.Write(pad + bullet + " ", bulletStyle.Value);
            }
            else
            {
                writer.Write(pad + bullet + " ");
            }

            if (textStyle.HasValue)
            {
                writer.WriteLine(item, textStyle.Value);
            }
            else
            {
                writer.WriteLine(item);
            }
        }
    }

    public static void WriteWrapped(this IConsoleWriter writer, string text, int width = 80, int indent = 0, ConsoleStyle? style = null)
    {
        ArgumentNullException.ThrowIfNull(writer);
        string pad = indent > 0 ? new string(' ', indent) : string.Empty;
        foreach (string line in ConsoleFormatting.Wrap(text, width - pad.Length))
        {
            string output = pad + line;
            if (style.HasValue)
            {
                writer.WriteLine(output, style.Value);
            }
            else
            {
                writer.WriteLine(output);
            }
        }
    }

    public static void WriteLineWithPrefix(this IConsoleWriter writer, string prefix, string message, ConsoleStyle? prefixStyle = null, ConsoleStyle? messageStyle = null)
    {
        ArgumentNullException.ThrowIfNull(writer);
        if (prefixStyle.HasValue)
        {
            writer.Write(prefix, prefixStyle.Value);
        }
        else
        {
            writer.Write(prefix);
        }

        writer.Write(" ");
        if (messageStyle.HasValue)
        {
            writer.WriteLine(message, messageStyle.Value);
        }
        else
        {
            writer.WriteLine(message);
        }
    }

    public static string TimestampPrefix(string? format = "HH:mm:ss")
    {
        return DateTime.Now.ToString(format ?? "HH:mm:ss", CultureInfo.InvariantCulture);
    }

    public static void WriteKeyValues(this IConsoleWriter writer, IEnumerable<(string Key, string Value)> items, int indent = 0, int gap = 2, string sep = ":", ConsoleStyle? keyStyle = null, ConsoleStyle? valueStyle = null)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(sep);
        foreach (string line in ConsoleFormatting.BuildKeyValues(items, indent, gap, sep))
        {
            if (keyStyle.HasValue || valueStyle.HasValue)
            {
                int idx = line.IndexOf(sep, StringComparison.Ordinal);
                if (idx > 0)
                {
                    string before = line[..idx];
                    string after = line[idx..];
                    if (keyStyle.HasValue)
                    {
                        writer.Write(before, keyStyle.Value);
                    }
                    else
                    {
                        writer.Write(before);
                    }

                    writer.Write(after[..sep.Length]);
                    string rest = after[sep.Length..];
                    if (valueStyle.HasValue)
                    {
                        writer.WriteLine(rest, valueStyle.Value);
                    }
                    else
                    {
                        writer.WriteLine(rest);
                    }

                    continue;
                }
            }

            writer.WriteLine(line);
        }
    }

    public static void WriteTableSimple(this IConsoleWriter writer, IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows, ConsoleStyle? borderStyle = null, ConsoleStyle? headerStyle = null, ConsoleStyle? cellStyle = null)
    {
        WriteTableSimple(writer, headers, rows, padding: 1, maxWidth: 0, alignments: null, borderStyle, headerStyle, cellStyle);
    }

    public static void WriteTableSimple(this IConsoleWriter writer, IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows, int padding, int maxWidth, IReadOnlyList<ConsoleFormatting.CellAlign>? alignments, ConsoleStyle? borderStyle = null, ConsoleStyle? headerStyle = null, ConsoleStyle? cellStyle = null)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(headers);
        ArgumentNullException.ThrowIfNull(rows);
        bool headerPrinted = false;
        foreach (string line in ConsoleFormatting.BuildSimpleTable(headers, rows, padding, maxWidth, alignments))
        {
            if (line.StartsWith('+') && borderStyle.HasValue)
            {
                writer.WriteLine(line, borderStyle.Value);
            }
            else if (!headerPrinted && headers.Count > 0)
            {
                writer.WriteLine(line, headerStyle ?? cellStyle ?? default);
                headerPrinted = true;
            }
            else if (line.StartsWith('|') && cellStyle.HasValue)
            {
                writer.WriteLine(line, cellStyle.Value);
            }
            else
            {
                writer.WriteLine(line);
            }
        }
    }

    public static void WriteBox(this IConsoleWriter writer, IEnumerable<string> contentLines, ConsoleStyle? borderStyle = null, ConsoleStyle? textStyle = null)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(contentLines);
        foreach (string line in ConsoleFormatting.BuildBox(contentLines))
        {
            if ((line.StartsWith('┌') || line.StartsWith('└') || line.StartsWith('│') || line.StartsWith('┐') || line.StartsWith('┘')) && borderStyle.HasValue)
            {
                writer.WriteLine(line, borderStyle.Value);
            }
            else if (textStyle.HasValue && line.StartsWith('│'))
            {
                writer.WriteLine(line, textStyle.Value);
            }
            else
            {
                writer.WriteLine(line);
            }
        }
    }
}
