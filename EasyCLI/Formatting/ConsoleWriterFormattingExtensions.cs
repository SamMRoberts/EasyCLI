namespace EasyCLI.Formatting;

using System;
using System.Collections.Generic;
using System.Globalization;
using EasyCLI.Console;

/// <summary>
/// IConsoleWriter extensions for formatted output using formatting helpers and ANSI styles.
/// </summary>
public static class ConsoleWriterFormattingExtensions
{
    /// <summary>
    /// Writes a horizontal rule line to the console.
    /// </summary>
    /// <param name="writer">The console writer to write to.</param>
    /// <param name="width">The width of the rule. If 0 or negative, uses console width.</param>
    /// <param name="ch">The character to use for the rule.</param>
    /// <param name="style">The style to apply to the rule. If null, no styling is applied.</param>
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

    /// <summary>
    /// Writes a horizontal rule line with a title on the left side to the console.
    /// </summary>
    /// <param name="writer">The console writer to write to.</param>
    /// <param name="title">The title text to display.</param>
    /// <param name="width">The total width of the rule. If 0 or negative, uses console width.</param>
    /// <param name="filler">The character to use for the rule portion.</param>
    /// <param name="gap">The number of spaces between the title and the rule.</param>
    /// <param name="titleStyle">The style to apply to the title. If null, no styling is applied.</param>
    /// <param name="fillerStyle">The style to apply to the filler. If null, no styling is applied.</param>
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

    /// <summary>
    /// Writes a horizontal rule line with a title centered within it to the console.
    /// </summary>
    /// <param name="writer">The console writer to write to.</param>
    /// <param name="title">The title text to display.</param>
    /// <param name="width">The total width of the rule. If 0 or negative, uses console width.</param>
    /// <param name="filler">The character to use for the rule portions.</param>
    /// <param name="gap">The number of spaces around the title.</param>
    /// <param name="titleStyle">The style to apply to the title. If null, no styling is applied.</param>
    /// <param name="fillerStyle">The style to apply to the filler. If null, no styling is applied.</param>
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

    /// <summary>
    /// Writes content inside a titled box to the console using Unicode box-drawing characters.
    /// </summary>
    /// <param name="writer">The console writer to write to.</param>
    /// <param name="contentLines">The lines of content to enclose in the box.</param>
    /// <param name="title">The title to display in the top border of the box.</param>
    /// <param name="borderStyle">The style to apply to the border. If null, no styling is applied.</param>
    /// <param name="titleStyle">The style to apply to the title. If null, no styling is applied.</param>
    /// <param name="textStyle">The style to apply to the text content. If null, no styling is applied.</param>
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

    /// <summary>
    /// Writes a heading with an underline to the console.
    /// </summary>
    /// <param name="writer">The console writer to write to.</param>
    /// <param name="text">The heading text to write.</param>
    /// <param name="titleStyle">The style to apply to the title. If null, no styling is applied.</param>
    /// <param name="underlineStyle">The style to apply to the underline. If null, no styling is applied.</param>
    /// <param name="underlineChar">The character to use for the underline.</param>
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

    /// <summary>
    /// Writes a bulleted list to the console.
    /// </summary>
    /// <param name="writer">The console writer to write to.</param>
    /// <param name="items">The items to write as bullets.</param>
    /// <param name="bullet">The bullet character or string to use.</param>
    /// <param name="indent">The number of spaces to indent the bullets.</param>
    /// <param name="bulletStyle">The style to apply to the bullet. If null, no styling is applied.</param>
    /// <param name="textStyle">The style to apply to the text. If null, no styling is applied.</param>
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

    /// <summary>
    /// Writes text with word wrapping to the console.
    /// </summary>
    /// <param name="writer">The console writer to write to.</param>
    /// <param name="text">The text to wrap and write.</param>
    /// <param name="width">The maximum width per line. If 0 or negative, uses console width.</param>
    /// <param name="indent">The number of spaces to indent each line.</param>
    /// <param name="style">The style to apply to the text. If null, no styling is applied.</param>
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

    /// <summary>
    /// Writes a line with a prefix to the console.
    /// </summary>
    /// <param name="writer">The console writer to write to.</param>
    /// <param name="prefix">The prefix text to write.</param>
    /// <param name="message">The message text to write after the prefix.</param>
    /// <param name="prefixStyle">The style to apply to the prefix. If null, no styling is applied.</param>
    /// <param name="messageStyle">The style to apply to the message. If null, no styling is applied.</param>
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

    /// <summary>
    /// Creates a timestamp prefix string using the specified format.
    /// </summary>
    /// <param name="format">The datetime format string to use. If null, uses "HH:mm:ss".</param>
    /// <returns>A formatted timestamp string with brackets.</returns>
    public static string TimestampPrefix(string? format = "HH:mm:ss")
    {
        return DateTime.Now.ToString(format ?? "HH:mm:ss", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Writes formatted key-value pairs to the console with aligned values.
    /// </summary>
    /// <param name="writer">The console writer to write to.</param>
    /// <param name="items">The key-value pairs to write.</param>
    /// <param name="indent">The number of spaces to indent each line.</param>
    /// <param name="gap">The minimum number of spaces between the key and value.</param>
    /// <param name="sep">The separator character to place after the key.</param>
    /// <param name="keyStyle">The style to apply to the keys. If null, no styling is applied.</param>
    /// <param name="valueStyle">The style to apply to the values. If null, no styling is applied.</param>
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

    /// <summary>
    /// Writes a simple text table with headers and rows to the console.
    /// </summary>
    /// <param name="writer">The console writer to write to.</param>
    /// <param name="headers">The column headers.</param>
    /// <param name="rows">The data rows.</param>
    /// <param name="borderStyle">The style to apply to the table borders. If null, no styling is applied.</param>
    /// <param name="headerStyle">The style to apply to the headers. If null, no styling is applied.</param>
    /// <param name="cellStyle">The style to apply to the cell content. If null, no styling is applied.</param>
    public static void WriteTableSimple(this IConsoleWriter writer, IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows, ConsoleStyle? borderStyle = null, ConsoleStyle? headerStyle = null, ConsoleStyle? cellStyle = null)
    {
        WriteTableSimple(writer, headers, rows, padding: 1, maxWidth: 0, alignments: null, borderStyle, headerStyle, cellStyle);
    }

    /// <summary>
    /// Writes a simple text table with headers and rows to the console with advanced formatting options.
    /// </summary>
    /// <param name="writer">The console writer to write to.</param>
    /// <param name="headers">The column headers.</param>
    /// <param name="rows">The data rows.</param>
    /// <param name="padding">The padding around cell content.</param>
    /// <param name="maxWidth">The maximum table width. If 0, no width limit is applied.</param>
    /// <param name="alignments">The alignment for each column. If null, all columns are left-aligned.</param>
    /// <param name="borderStyle">The style to apply to the table borders. If null, no styling is applied.</param>
    /// <param name="headerStyle">The style to apply to the headers. If null, no styling is applied.</param>
    /// <param name="cellStyle">The style to apply to the cell content. If null, no styling is applied.</param>
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

    /// <summary>
    /// Writes content inside a box to the console using Unicode box-drawing characters.
    /// </summary>
    /// <param name="writer">The console writer to write to.</param>
    /// <param name="contentLines">The lines of content to enclose in the box.</param>
    /// <param name="borderStyle">The style to apply to the border. If null, no styling is applied.</param>
    /// <param name="textStyle">The style to apply to the text content. If null, no styling is applied.</param>
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
