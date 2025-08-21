using System;
using System.Collections.Generic;

namespace EasyCLI.Prompts
{
    /// <summary>
    /// Common options controlling prompt rendering & behavior.
    /// </summary>
    public sealed class PromptOptions
    {
        public string? Suffix { get; set; } = ": ";
        public ConsoleStyle? LabelStyle { get; set; } = ConsoleStyles.Heading;
        public ConsoleStyle? DefaultStyle { get; set; } = ConsoleStyles.Hint;
        public ConsoleStyle? ErrorStyle { get; set; } = ConsoleStyles.Error;
        public bool EchoInput { get; set; } = true; // future secure input scenario could set false
    public bool EnableEscapeCancel { get; set; } = true;
    public PromptCancelBehavior CancelBehavior { get; set; } = PromptCancelBehavior.ReturnDefault;
    // Paging (used by choice-style prompts). When EnablePaging is true and item count exceeds PageSize, choices are shown in pages.
    public bool EnablePaging { get; set; } = true;
    public int PageSize { get; set; } = 10;
    // Incremental search/filter (choice prompts). When enabled, typing filters visible choices (case-insensitive contains).
    public bool EnableFiltering { get; set; } = true;
        public bool EnableEscapeCancel { get; set; } = true;
        public PromptCancelBehavior CancelBehavior { get; set; } = PromptCancelBehavior.ReturnDefault;
        // Paging (used by choice-style prompts). When EnablePaging is true and item count exceeds PageSize, choices are shown in pages.
        public bool EnablePaging { get; set; } = true;
        public int PageSize { get; set; } = 10;
        // Incremental search/filter (choice prompts). When enabled, typing filters visible choices (case-insensitive contains).
        public bool EnableFiltering { get; set; } = true;
        public bool FilterMatchStartsWith { get; set; } = false; // if true use startswith else substring
    }
}
