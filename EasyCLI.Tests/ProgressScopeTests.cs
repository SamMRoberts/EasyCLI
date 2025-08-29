using EasyCLI.Console;
using EasyCLI.Extensions;
using EasyCLI.Progress;
using EasyCLI.Styling;
using Xunit;

namespace EasyCLI.Tests
{
    /// <summary>
    /// Tests for progress scope and spinner functionality.
    /// </summary>
    public class ProgressScopeTests
    {
        [Fact]
        public void ProgressScope_Constructor_ImmediatelyShowsStartingMessage()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            using var scope = new ProgressScope(writer, "test operation");

            var result = output.ToString();
            Assert.Contains("Starting test operation...", result);
        }

        [Fact]
        public void ProgressScope_Constructor_WithTheme_AppliesTheme()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(enableColors: true, output: output);
            var theme = ConsoleThemes.Dark;

            using var scope = new ProgressScope(writer, "test operation", theme: theme);

            var result = output.ToString();
            Assert.Contains("Starting test operation...", result);
            // Should contain ANSI escape sequences from theme
            Assert.Contains("\u001b[", result);
        }

        [Fact]
        public void ProgressScope_Constructor_WithNullWriter_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ProgressScope(null!, "test"));
        }

        [Fact]
        public void ProgressScope_Constructor_WithEmptyMessage_ThrowsArgumentException()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            Assert.Throws<ArgumentException>(() => new ProgressScope(writer, ""));
            Assert.Throws<ArgumentException>(() => new ProgressScope(writer, "   "));
        }

        [Fact]
        public void ProgressScope_Complete_ShowsSuccessMessage()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            using var scope = new ProgressScope(writer, "test operation");
            Thread.Sleep(50); // Allow spinner to start
            scope.Complete();

            var result = output.ToString();
            Assert.Contains("✓ test operation completed", result);
        }

        [Fact]
        public void ProgressScope_Complete_WithCustomMessage_ShowsCustomMessage()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            using var scope = new ProgressScope(writer, "test operation");
            Thread.Sleep(50); // Allow spinner to start
            scope.Complete("Custom success message");

            var result = output.ToString();
            Assert.Contains("Custom success message", result);
        }

        [Fact]
        public void ProgressScope_Fail_ShowsErrorMessage()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            using var scope = new ProgressScope(writer, "test operation");
            Thread.Sleep(50); // Allow spinner to start
            scope.Fail();

            var result = output.ToString();
            Assert.Contains("✗ test operation failed", result);
        }

        [Fact]
        public void ProgressScope_Fail_WithCustomMessage_ShowsCustomMessage()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            using var scope = new ProgressScope(writer, "test operation");
            Thread.Sleep(50); // Allow spinner to start
            scope.Fail("Custom error message");

            var result = output.ToString();
            Assert.Contains("Custom error message", result);
        }

        [Fact]
        public void ProgressScope_IsActive_StartsTrue()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            using var scope = new ProgressScope(writer, "test operation");

            Assert.True(scope.IsActive);
        }

        [Fact]
        public void ProgressScope_IsActive_FalseAfterComplete()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            using var scope = new ProgressScope(writer, "test operation");
            scope.Complete();

            Assert.False(scope.IsActive);
        }

        [Fact]
        public void ProgressScope_IsActive_FalseAfterFail()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            using var scope = new ProgressScope(writer, "test operation");
            scope.Fail();

            Assert.False(scope.IsActive);
        }

        [Fact]
        public void ProgressScope_UpdateMessage_ChangesDisplayedMessage()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            using var scope = new ProgressScope(writer, "test operation");
            Thread.Sleep(50); // Allow spinner to start
            scope.UpdateMessage("new message");
            Thread.Sleep(50); // Allow update to occur

            var result = output.ToString();
            Assert.Contains("new message", result);
        }

        [Fact]
        public void ProgressScope_UpdateMessage_WithNullOrEmpty_ThrowsArgumentException()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            using var scope = new ProgressScope(writer, "test operation");

            Assert.Throws<ArgumentException>(() => scope.UpdateMessage(""));
            Assert.Throws<ArgumentException>(() => scope.UpdateMessage("   "));
        }

        [Fact]
        public void ProgressScope_WithCancellationToken_RespectsToken()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);
            using var cts = new CancellationTokenSource();

            // Cancel immediately
            cts.Cancel();

            using var scope = new ProgressScope(writer, "test operation", cancellationToken: cts.Token);

            // Should still show starting message but not start spinner
            var result = output.ToString();
            Assert.Contains("Starting test operation...", result);
        }

        [Fact]
        public void ConsoleWriter_CreateProgressScope_ReturnsValidScope()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            using var scope = writer.CreateProgressScope("test operation");

            Assert.NotNull(scope);
            Assert.True(scope.IsActive);

            var result = output.ToString();
            Assert.Contains("Starting test operation...", result);
        }

        [Fact]
        public void ConsoleWriter_WriteStarting_ShowsStartingMessage()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            writer.WriteStarting("deployment");

            var result = output.ToString();
            Assert.Contains("Starting deployment...", result);
        }

        [Fact]
        public void ConsoleWriter_WriteCompleted_ShowsCompletionMessage()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            writer.WriteCompleted("deployment");

            var result = output.ToString();
            Assert.Contains("✓ deployment completed", result);
        }

        [Fact]
        public void ConsoleWriter_WriteFailed_ShowsFailureMessage()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            writer.WriteFailed("deployment");

            var result = output.ToString();
            Assert.Contains("✗ deployment failed", result);
        }

        [Fact]
        public void ConsoleWriter_WriteFailed_WithErrorMessage_ShowsErrorDetails()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            writer.WriteFailed("deployment", "network timeout");

            var result = output.ToString();
            Assert.Contains("✗ deployment failed: network timeout", result);
        }

        [Fact]
        public void ConsoleWriter_EarlyFeedbackMethods_WithTheme_ApplyTheme()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(enableColors: true, output: output);
            var theme = ConsoleThemes.Dark;

            writer.WriteStarting("test", theme);
            writer.WriteCompleted("test", theme);
            writer.WriteFailed("test", null, theme);

            var result = output.ToString();
            Assert.Contains("Starting test...", result);
            Assert.Contains("✓ test completed", result);
            Assert.Contains("✗ test failed", result);
            // Should contain ANSI escape sequences from theme
            Assert.Contains("\u001b[", result);
        }
    }
}
