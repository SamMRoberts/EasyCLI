using System.Globalization;
using System.Security;
using EasyCLI.Console;

namespace EasyCLI
{
    /// <summary>
    /// Collects and aggregates errors for batch processing scenarios, providing grouped summaries.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ErrorCollector"/> class.
    /// </remarks>
    /// <param name="writer">The console writer for output.</param>
    /// <param name="theme">The console theme for styling.</param>
    public class ErrorCollector(IConsoleWriter writer, ConsoleTheme? theme = null)
    {
        private readonly List<CollectedError> _errors = [];
        private readonly IConsoleWriter _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        private readonly ConsoleTheme _theme = theme ?? ConsoleThemes.Dark;

        /// <summary>
        /// Gets the total count of collected errors.
        /// </summary>
        public int TotalCount => _errors.Count;

        /// <summary>
        /// Gets a value indicating whether any errors have been collected.
        /// </summary>
        public bool HasErrors => _errors.Count > 0;

        /// <summary>
        /// Gets all collected errors.
        /// </summary>
        public IReadOnlyList<CollectedError> Errors => _errors.AsReadOnly();

        /// <summary>
        /// Adds an error to the collection.
        /// </summary>
        /// <param name="category">The error category.</param>
        /// <param name="message">The error message.</param>
        /// <param name="source">The source or context where the error occurred.</param>
        /// <param name="details">Additional error details.</param>
        public void AddError(BatchErrorCategory category, string message, string? source = null, string? details = null)
        {
            CollectedError error = new(category, message, source, details);
            _errors.Add(error);
        }

        /// <summary>
        /// Adds a general error to the collection.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="source">The source or context where the error occurred.</param>
        /// <param name="details">Additional error details.</param>
        public void AddError(string message, string? source = null, string? details = null)
        {
            AddError(BatchErrorCategory.General, message, source, details);
        }

        /// <summary>
        /// Adds an error from an exception.
        /// </summary>
        /// <param name="exception">The exception to add.</param>
        /// <param name="source">The source or context where the error occurred.</param>
        /// <param name="category">The error category. If null, attempts to infer from exception type.</param>
        public void AddError(Exception exception, string? source = null, BatchErrorCategory? category = null)
        {
            ArgumentNullException.ThrowIfNull(exception);

            BatchErrorCategory errorCategory = category ?? InferCategoryFromException(exception);
            AddError(errorCategory, exception.Message, source, exception.GetType().Name);
        }

        /// <summary>
        /// Gets error summaries grouped by category.
        /// </summary>
        /// <returns>An enumerable of error summaries grouped by category.</returns>
        public IEnumerable<ErrorSummary> GetSummaries()
        {
            return _errors
                .GroupBy(e => e.Category)
                .OrderBy(g => g.Key)
                .Select(g => new ErrorSummary(g.Key, [.. g]));
        }

        /// <summary>
        /// Gets error summaries for the specified categories only.
        /// </summary>
        /// <param name="categories">The categories to include in the summary.</param>
        /// <returns>An enumerable of error summaries for the specified categories.</returns>
        public IEnumerable<ErrorSummary> GetSummaries(params BatchErrorCategory[] categories)
        {
            HashSet<BatchErrorCategory> categorySet = new(categories);
            return GetSummaries().Where(s => categorySet.Contains(s.Category));
        }

        /// <summary>
        /// Prints a summary of all collected errors grouped by category.
        /// </summary>
        /// <param name="showDetails">Whether to show detailed error information.</param>
        public void PrintSummary(bool showDetails = false)
        {
            if (!HasErrors)
            {
                _writer.WriteSuccessLine("✓ No errors to report", _theme);
                return;
            }

            _writer.WriteHeadingLine($"Error Summary ({TotalCount} total)", _theme);
            _writer.WriteLine();

            List<ErrorSummary> summaries = GetSummaries().ToList();

            // Print category counts table
            PrintCategorySummaryTable(summaries);

            if (showDetails)
            {
                _writer.WriteLine();
                PrintDetailedErrors(summaries);
            }
            else
            {
                _writer.WriteLine();
                _writer.WriteHintLine("💡 Use --verbose or --details to see individual error messages", _theme);
            }
        }

        /// <summary>
        /// Prints detailed error information for a specific category.
        /// </summary>
        /// <param name="category">The category to print details for.</param>
        public void PrintCategoryDetails(BatchErrorCategory category)
        {
            List<CollectedError> categoryErrors = _errors.Where(e => e.Category == category).ToList();
            if (categoryErrors.Count == 0)
            {
                _writer.WriteInfoLine($"No errors in category: {category}", _theme);
                return;
            }

            ErrorSummary summary = new(category, categoryErrors);
            _writer.WriteHeadingLine($"{summary.CategoryDisplayName} Errors ({summary.Count})", _theme);
            _writer.WriteLine();

            PrintErrorList(categoryErrors);
        }

        /// <summary>
        /// Clears all collected errors.
        /// </summary>
        public void Clear()
        {
            _errors.Clear();
        }

        private static BatchErrorCategory InferCategoryFromException(Exception exception)
        {
            return exception switch
            {
                FileNotFoundException or DirectoryNotFoundException or UnauthorizedAccessException => BatchErrorCategory.FileSystem,
                HttpRequestException => BatchErrorCategory.Network,
                ArgumentException or ArgumentNullException or ArgumentOutOfRangeException => BatchErrorCategory.Validation,
                SecurityException => BatchErrorCategory.Security,
                _ => BatchErrorCategory.General,
            };
        }

        private void PrintCategorySummaryTable(IList<ErrorSummary> summaries)
        {
            string[] headers = ["Category", "Count", "%"];
            List<string[]> rows = summaries.Select(s => new[]
            {
                s.CategoryDisplayName,
                s.Count.ToString(CultureInfo.InvariantCulture),
                $"{s.Count * 100.0 / TotalCount:F1}%",
            }).ToList();

            IEnumerable<string> tableLines = ConsoleFormatting.BuildSimpleTable(headers, rows, padding: 1);
            foreach (string line in tableLines)
            {
                _writer.WriteLine(line, _theme.Info);
            }
        }

        private void PrintDetailedErrors(IList<ErrorSummary> summaries)
        {
            foreach (ErrorSummary summary in summaries)
            {
                _writer.WriteRule($"{summary.CategoryDisplayName} ({summary.Count})", '─', _theme.Heading);
                PrintErrorList(summary.Errors);
                _writer.WriteLine();
            }
        }

        private void PrintErrorList(IReadOnlyList<CollectedError> errors)
        {
            for (int i = 0; i < errors.Count; i++)
            {
                CollectedError error = errors[i];
                string prefix = $"{i + 1:D2}.";

                _writer.Write(prefix.PadRight(4), _theme.Hint);
                _writer.WriteErrorLine(error.Message, _theme);

                if (!string.IsNullOrEmpty(error.Source))
                {
                    _writer.Write("    Source: ", _theme.Hint);
                    _writer.WriteLine(error.Source, _theme.Info);
                }

                if (!string.IsNullOrEmpty(error.Details))
                {
                    _writer.Write("    Details: ", _theme.Hint);
                    _writer.WriteLine(error.Details, _theme.Info);
                }

                if (i < errors.Count - 1)
                {
                    _writer.WriteLine();
                }
            }
        }
    }
}
