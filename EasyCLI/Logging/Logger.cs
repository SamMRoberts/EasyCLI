using EasyCLI.Console;

namespace EasyCLI.Logging
{
    /// <summary>
    /// Provides structured logging for CLI applications.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="Logger"/> class.
    /// </remarks>
    /// <param name="writer">The console writer.</param>
    /// <param name="level">The logging level.</param>
    /// <param name="theme">The console theme to use.</param>
    public class Logger(IConsoleWriter writer, LogLevel level, ConsoleTheme? theme = null)
    {
        private readonly IConsoleWriter _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        private readonly ConsoleTheme _theme = theme ?? ConsoleThemes.Dark;

        /// <summary>
        /// Gets the current logging level.
        /// </summary>
        public LogLevel Level { get; } = level;

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void LogDebug(string message)
        {
            if (Level >= LogLevel.Debug)
            {
                _writer.WriteHintLine($"[DEBUG] {message}", _theme);
            }
        }

        /// <summary>
        /// Logs a debug message with formatting.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void LogDebug(string format, params object[] args)
        {
            if (Level >= LogLevel.Debug)
            {
                LogDebug(string.Format(System.Globalization.CultureInfo.InvariantCulture, format, args));
            }
        }

        /// <summary>
        /// Logs a verbose message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void LogVerbose(string message)
        {
            if (Level >= LogLevel.Verbose)
            {
                _writer.WriteInfoLine($"[VERBOSE] {message}", _theme);
            }
        }

        /// <summary>
        /// Logs a verbose message with formatting.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void LogVerbose(string format, params object[] args)
        {
            if (Level >= LogLevel.Verbose)
            {
                LogVerbose(string.Format(System.Globalization.CultureInfo.InvariantCulture, format, args));
            }
        }

        /// <summary>
        /// Logs a normal information message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void LogInfo(string message)
        {
            if (Level >= LogLevel.Normal)
            {
                _writer.WriteLine(message);
            }
        }

        /// <summary>
        /// Logs a normal information message with formatting.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void LogInfo(string format, params object[] args)
        {
            if (Level >= LogLevel.Normal)
            {
                LogInfo(string.Format(System.Globalization.CultureInfo.InvariantCulture, format, args));
            }
        }

        /// <summary>
        /// Logs a success message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void LogSuccess(string message)
        {
            if (Level >= LogLevel.Normal)
            {
                _writer.WriteSuccessLine(message, _theme);
            }
        }

        /// <summary>
        /// Logs a success message with formatting.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void LogSuccess(string format, params object[] args)
        {
            if (Level >= LogLevel.Normal)
            {
                LogSuccess(string.Format(System.Globalization.CultureInfo.InvariantCulture, format, args));
            }
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void LogWarning(string message)
        {
            if (Level >= LogLevel.Normal)
            {
                _writer.WriteWarningLine($"Warning: {message}", _theme);
            }
        }

        /// <summary>
        /// Logs a warning message with formatting.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void LogWarning(string format, params object[] args)
        {
            if (Level >= LogLevel.Normal)
            {
                LogWarning(string.Format(System.Globalization.CultureInfo.InvariantCulture, format, args));
            }
        }

        /// <summary>
        /// Logs an error message (always shown regardless of level).
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void LogError(string message)
        {
            _writer.WriteErrorLine($"Error: {message}", _theme);
        }

        /// <summary>
        /// Logs an error message with formatting.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        public void LogError(string format, params object[] args)
        {
            LogError(string.Format(System.Globalization.CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// Creates a logger with the appropriate level based on command line arguments.
        /// </summary>
        /// <param name="writer">The console writer.</param>
        /// <param name="args">Command line arguments.</param>
        /// <param name="theme">Optional console theme.</param>
        /// <returns>A configured logger.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static members should appear before non-static members", Justification = "Logical grouping")]
        public static Logger CreateFromArgs(IConsoleWriter writer, string[] args, ConsoleTheme? theme = null)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentNullException.ThrowIfNull(args);

            LogLevel level = DetermineLogLevel(args);
            return new Logger(writer, level, theme);
        }

        /// <summary>
        /// Determines the appropriate log level from command line arguments.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>The appropriate log level.</returns>
        public static LogLevel DetermineLogLevel(string[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            // Check environment first
            bool isCI = System.Environment.GetEnvironmentVariable("CI") != null;
            if (isCI && !args.Any(arg => arg.Equals("--verbose", StringComparison.OrdinalIgnoreCase) ||
                                        arg.Equals("-v", StringComparison.OrdinalIgnoreCase)))
            {
                // Default to quiet in CI unless explicitly verbose
                return LogLevel.Quiet;
            }

            // Check command line flags
            return args.Contains("--quiet") || args.Contains("-q")
                ? LogLevel.Quiet
                : args.Contains("--debug")
                ? LogLevel.Debug
                : args.Contains("--verbose") || args.Contains("-v") ? LogLevel.Verbose : LogLevel.Normal;
        }
    }
}
