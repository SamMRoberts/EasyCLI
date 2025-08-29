using EasyCLI.Console;

namespace EasyCLI.Progress
{
    /// <summary>
    /// Manages a spinner animation for long-running operations with a disposable pattern.
    /// Automatically shows early feedback and manages spinner lifecycle.
    /// </summary>
    public class ProgressScope : IDisposable
    {
        /// <summary>
        /// Default spinner characters.
        /// </summary>
        public static readonly char[] DefaultSpinnerChars = ['|', '/', '-', '\\'];

        /// <summary>
        /// Braille spinner characters for a smoother animation.
        /// </summary>
        public static readonly char[] BrailleSpinnerChars = ['⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏'];

        /// <summary>
        /// Dots spinner characters.
        /// </summary>
        public static readonly char[] DotsSpinnerChars = ['⠁', '⠂', '⠄', '⡀', '⢀', '⠠', '⠐', '⠈'];

        private readonly IConsoleWriter _writer;
        private readonly char[] _spinnerChars;
        private readonly string _message;
        private readonly ConsoleTheme? _theme;
        private readonly Timer? _timer;
        private readonly CancellationToken _cancellationToken;
        private readonly Lock _lock = new();
        private int _currentFrame;
        private bool _disposed;

        /// <summary>
        /// Gets a value indicating whether the spinner is currently active.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressScope"/> class and immediately shows starting message.
        /// </summary>
        /// <param name="writer">The console writer to use for output.</param>
        /// <param name="message">The message to display alongside the spinner.</param>
        /// <param name="spinnerChars">The characters to use for the spinner animation.</param>
        /// <param name="theme">The theme to use for styling.</param>
        /// <param name="intervalMs">The animation interval in milliseconds.</param>
        /// <param name="cancellationToken">Token to observe for cancellation.</param>
        public ProgressScope(
            IConsoleWriter writer,
            string message,
            char[]? spinnerChars = null,
            ConsoleTheme? theme = null,
            int intervalMs = 100,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentException.ThrowIfNullOrWhiteSpace(message);

            _writer = writer;
            _message = message;
            _spinnerChars = spinnerChars ?? DefaultSpinnerChars;
            _theme = theme;
            _cancellationToken = cancellationToken;

            // Immediately show starting message for early feedback (within 100ms requirement)
            ShowStartingMessage();

            // Start spinner animation if not cancelled
            if (!_cancellationToken.IsCancellationRequested)
            {
                IsActive = true;
                _timer = new Timer(UpdateSpinner, null, TimeSpan.FromMilliseconds(intervalMs), TimeSpan.FromMilliseconds(intervalMs));
            }
        }

        /// <summary>
        /// Shows the initial starting message immediately for early feedback.
        /// </summary>
        private void ShowStartingMessage()
        {
            string startingMessage = $"Starting {_message}...";
            if (_theme != null)
            {
                _writer.WriteLine(startingMessage, _theme.Info);
            }
            else
            {
                _writer.WriteLine(startingMessage);
            }
        }

        /// <summary>
        /// Updates the spinner animation frame.
        /// </summary>
        /// <param name="state">Timer state (unused).</param>
        private void UpdateSpinner(object? state)
        {
            if (_disposed || _cancellationToken.IsCancellationRequested)
            {
                return;
            }

            lock (_lock)
            {
                if (_disposed || !IsActive)
                {
                    return;
                }

                char currentChar = _spinnerChars[_currentFrame % _spinnerChars.Length];
                string spinnerLine = $"\r{currentChar} {_message}...";

                if (_theme != null)
                {
                    _writer.Write(spinnerLine, _theme.Info);
                }
                else
                {
                    _writer.Write(spinnerLine);
                }

                _currentFrame++;
            }
        }

        /// <summary>
        /// Updates the message displayed by the spinner.
        /// </summary>
        /// <param name="newMessage">The new message to display.</param>
        public void UpdateMessage(string newMessage)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(newMessage);

            lock (_lock)
            {
                if (_disposed || !IsActive)
                {
                    return;
                }

                // Clear current line and show new message
                string clearLine = "\r" + new string(' ', Math.Max(50, _message.Length + 10)) + "\r";
                _writer.Write(clearLine);

                string updatedLine = $"⟳ {newMessage}...";
                if (_theme != null)
                {
                    _writer.Write(updatedLine, _theme.Info);
                }
                else
                {
                    _writer.Write(updatedLine);
                }
            }
        }

        /// <summary>
        /// Stops the spinner and shows a success message.
        /// </summary>
        /// <param name="completionMessage">Optional completion message. If null, uses default success message.</param>
        public void Complete(string? completionMessage = null)
        {
            if (_disposed)
            {
                return;
            }

            lock (_lock)
            {
                if (!IsActive)
                {
                    return;
                }

                IsActive = false;
                _timer?.Dispose();

                // Clear the spinner line
                string clearLine = "\r" + new string(' ', Math.Max(50, _message.Length + 10)) + "\r";
                _writer.Write(clearLine);

                // Show completion message
                string finalMessage = completionMessage ?? $"✓ {_message} completed";
                if (_theme != null)
                {
                    _writer.WriteLine(finalMessage, _theme.Success);
                }
                else
                {
                    _writer.WriteLine(finalMessage);
                }
            }
        }

        /// <summary>
        /// Stops the spinner and shows an error message.
        /// </summary>
        /// <param name="errorMessage">Optional error message. If null, uses default error message.</param>
        public void Fail(string? errorMessage = null)
        {
            if (_disposed)
            {
                return;
            }

            lock (_lock)
            {
                if (!IsActive)
                {
                    return;
                }

                IsActive = false;
                _timer?.Dispose();

                // Clear the spinner line
                string clearLine = "\r" + new string(' ', Math.Max(50, _message.Length + 10)) + "\r";
                _writer.Write(clearLine);

                // Show error message
                string finalMessage = errorMessage ?? $"✗ {_message} failed";
                if (_theme != null)
                {
                    _writer.WriteLine(finalMessage, _theme.Error);
                }
                else
                {
                    _writer.WriteLine(finalMessage);
                }
            }
        }

        /// <summary>
        /// Disposes the progress scope, stopping the spinner and cleaning up resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            lock (_lock)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;

                if (IsActive)
                {
                    IsActive = false;
                    _timer?.Dispose();

                    // Clear the spinner line if still active
                    string clearLine = "\r" + new string(' ', Math.Max(50, _message.Length + 10)) + "\r";
                    _writer.Write(clearLine);
                }
            }

            GC.SuppressFinalize(this);
        }
    }
}
