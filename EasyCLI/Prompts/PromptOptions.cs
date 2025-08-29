namespace EasyCLI.Prompts
{
    /// <summary>
    /// Common options controlling prompt rendering &amp; behavior.
    /// </summary>
    public sealed class PromptOptions
    {
        /// <summary>
        /// Gets or sets the suffix text displayed after the prompt (e.g., ": ").
        /// </summary>
        public string? Suffix { get; set; } = ": ";

        /// <summary>
        /// Gets or sets the style applied to the prompt label text.
        /// </summary>
        public ConsoleStyle? LabelStyle { get; set; } = ConsoleStyles.Heading;

        /// <summary>
        /// Gets or sets the style applied to default value hints.
        /// </summary>
        public ConsoleStyle? DefaultStyle { get; set; } = ConsoleStyles.Hint;

        /// <summary>
        /// Gets or sets the style applied to error messages.
        /// </summary>
        public ConsoleStyle? ErrorStyle { get; set; } = ConsoleStyles.Error;

        /// <summary>
        /// Gets or sets a value indicating whether user input should be echoed to the console.
        /// </summary>
        public bool EchoInput { get; set; } = true; // future secure input scenario could set false

        /// <summary>
        /// Gets or sets a value indicating whether pressing Escape cancels the prompt.
        /// </summary>
        public bool EnableEscapeCancel { get; set; } = true;

        /// <summary>
        /// Gets or sets the behavior when a prompt is canceled.
        /// </summary>
        public PromptCancelBehavior CancelBehavior { get; set; } = PromptCancelBehavior.ReturnDefault;

        // Paging (used by choice-style prompts). When EnablePaging is true and item count exceeds PageSize, choices are shown in pages.

        /// <summary>
        /// Gets or sets a value indicating whether paging is enabled for choice prompts with many options.
        /// </summary>
        public bool EnablePaging { get; set; } = true;

        /// <summary>
        /// Gets or sets the number of choices to display per page when paging is enabled.
        /// </summary>
        public int PageSize { get; set; } = 10;

        // Incremental search/filter (choice prompts). When enabled, typing filters visible choices (case-insensitive contains).

        /// <summary>
        /// Gets or sets a value indicating whether incremental filtering is enabled for choice prompts.
        /// </summary>
        public bool EnableFiltering { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether filtering should match from the start of choice labels (true) or anywhere within them (false).
        /// </summary>
        public bool FilterMatchStartsWith { get; set; } = false; // if true use startswith else substring

        /// <summary>
        /// Gets or sets a value indicating whether the prompt is running in non-interactive mode.
        /// When true, prompts will immediately return defaults or fail if no default is available.
        /// </summary>
        public bool NonInteractive { get; set; } = false;
    }
}
