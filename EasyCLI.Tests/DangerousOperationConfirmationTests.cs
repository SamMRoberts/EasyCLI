using EasyCLI.Shell;
using EasyCLI.Tests.Fakes;
using Xunit;

namespace EasyCLI.Tests
{
    /// <summary>
    /// Tests for the dangerous operation confirmation framework.
    /// </summary>
    public class DangerousOperationConfirmationTests
    {
        /// <summary>
        /// Creates a test shell execution context with fake console reader/writer.
        /// </summary>
        /// <param name="readerInputs">Lines to simulate user input.</param>
        /// <returns>A tuple containing the context and writer for output verification.</returns>
        private static (ShellExecutionContext context, FakeConsoleWriter writer) CreateTestContext(params string[] readerInputs)
        {
            var reader = new FakeConsoleReader(readerInputs);
            var writer = new FakeConsoleWriter();
            var shell = new CliShell(reader, writer);
            var context = new ShellExecutionContext(shell, writer, reader);
            return (context, writer);
        }

        [Fact]
        public void ConfirmDangerous_WithYesFlag_ReturnsTrue()
        {
            // Arrange
            var (context, writer) = CreateTestContext();
            var args = new CommandLineArgs(new[] { "--yes" });

            // Act
            bool result = DangerousOperationConfirmation.ConfirmDangerous(
                "delete all files", context, args);

            // Assert
            Assert.True(result);
            Assert.Empty(writer.Output); // No prompt should be shown
        }

        [Fact]
        public void ConfirmDangerous_WithForceFlag_ReturnsTrue()
        {
            // Arrange
            var (context, writer) = CreateTestContext();
            var args = new CommandLineArgs(new[] { "--force" });

            // Act
            bool result = DangerousOperationConfirmation.ConfirmDangerous(
                "overwrite configuration", context, args);

            // Assert
            Assert.True(result);
            Assert.Empty(writer.Output); // No prompt should be shown
        }

        [Fact]
        public void ConfirmDangerous_WithShortFlags_ReturnsTrue()
        {
            // Arrange
            var (context, writer) = CreateTestContext();
            var argsY = new CommandLineArgs(new[] { "-y" });
            var argsF = new CommandLineArgs(new[] { "-f" });

            // Act
            bool resultY = DangerousOperationConfirmation.ConfirmDangerous(
                "delete files", context, argsY);
            bool resultF = DangerousOperationConfirmation.ConfirmDangerous(
                "delete files", context, argsF);

            // Assert
            Assert.True(resultY);
            Assert.True(resultF);
        }

        [Fact]
        public void ConfirmDangerous_InteractiveYes_ReturnsTrue()
        {
            // Arrange  
            var (context, writer) = CreateTestContext("y");
            var args = new CommandLineArgs(Array.Empty<string>());

            // Mock CI environment variable to be null
            System.Environment.SetEnvironmentVariable("CI", null);

            // Act
            bool result = DangerousOperationConfirmation.ConfirmDangerous(
                "delete important data", context, args);

            // Assert
            Assert.True(result);
            Assert.Contains("DANGEROUS OPERATION", writer.Output);
            Assert.Contains("delete important data", writer.Output);
        }

        [Fact]
        public void ConfirmDangerous_InteractiveNo_ReturnsFalse()
        {
            // Arrange
            var (context, writer) = CreateTestContext("n");
            var args = new CommandLineArgs(Array.Empty<string>());

            // Mock CI environment variable to be null
            System.Environment.SetEnvironmentVariable("CI", null);

            // Act
            bool result = DangerousOperationConfirmation.ConfirmDangerous(
                "format hard drive", context, args);

            // Assert
            Assert.False(result);
            Assert.Contains("DANGEROUS OPERATION", writer.Output);
            Assert.Contains("format hard drive", writer.Output);
        }

        [Fact]
        public void ConfirmDangerous_WithAdditionalWarnings_ShowsWarnings()
        {
            // Arrange
            var (context, writer) = CreateTestContext("n");
            var args = new CommandLineArgs(Array.Empty<string>());
            var warnings = new[] { "This action cannot be undone", "All backups will be lost" };

            // Mock CI environment variable to be null
            System.Environment.SetEnvironmentVariable("CI", null);

            // Act
            bool result = DangerousOperationConfirmation.ConfirmDangerous(
                "delete database", context, args, warnings);

            // Assert
            Assert.False(result);
            Assert.Contains("This action cannot be undone", writer.Output);
            Assert.Contains("All backups will be lost", writer.Output);
        }

        [Fact]
        public void ConfirmDangerous_WithCustomPrompt_UsesCustomPrompt()
        {
            // Arrange
            var (context, writer) = CreateTestContext("n");
            var args = new CommandLineArgs(Array.Empty<string>());
            var customPrompt = "Are you absolutely certain you want to proceed?";

            // Mock CI environment variable to be null
            System.Environment.SetEnvironmentVariable("CI", null);

            // Act
            bool result = DangerousOperationConfirmation.ConfirmDangerous(
                "nuclear option", context, args, customPrompt: customPrompt);

            // Assert
            Assert.False(result);
            Assert.Contains(customPrompt, writer.Output);
        }

        [Fact]
        public void ConfirmDangerous_InCIWithoutFlag_ReturnsFalse()
        {
            // Arrange
            var (context, writer) = CreateTestContext();
            var args = new CommandLineArgs(Array.Empty<string>());

            // Mock CI environment
            System.Environment.SetEnvironmentVariable("CI", "true");

            try
            {
                // Act
                bool result = DangerousOperationConfirmation.ConfirmDangerous(
                    "deploy to production", context, args);

                // Assert
                Assert.False(result);
                Assert.Contains("automation context without explicit confirmation", writer.Output);
                Assert.Contains("Use --yes or --force", writer.Output);
            }
            finally
            {
                // Cleanup
                System.Environment.SetEnvironmentVariable("CI", null);
            }
        }

        [Fact]
        public void ConfirmDangerous_InCIWithFlag_ReturnsTrue()
        {
            // Arrange
            var (context, writer) = CreateTestContext();
            var args = new CommandLineArgs(new[] { "--yes" });

            // Mock CI environment
            System.Environment.SetEnvironmentVariable("CI", "true");

            try
            {
                // Act
                bool result = DangerousOperationConfirmation.ConfirmDangerous(
                    "deploy to production", context, args);

                // Assert
                Assert.True(result);
                Assert.Empty(writer.Output); // No warnings should be shown with explicit flag
            }
            finally
            {
                // Cleanup
                System.Environment.SetEnvironmentVariable("CI", null);
            }
        }

        [Fact]
        public void BaseCliCommand_ConfirmDangerous_WorksAsExpected()
        {
            // Arrange
            var (context, writer) = CreateTestContext("y");
            var args = new CommandLineArgs(Array.Empty<string>());

            // Mock CI environment variable to be null
            System.Environment.SetEnvironmentVariable("CI", null);

            // Act
            bool result = BaseCliCommand.ConfirmDangerous(
                "test operation", context, args);

            // Assert
            Assert.True(result);
            Assert.Contains("DANGEROUS OPERATION", writer.Output);
        }
    }
}