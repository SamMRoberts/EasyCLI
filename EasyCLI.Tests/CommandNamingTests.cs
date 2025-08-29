using EasyCLI.Console;
using EasyCLI.Shell;

namespace EasyCLI.Tests
{
    /// <summary>
    /// Tests for command naming collision detection and reserved name validation.
    /// </summary>
    public class CommandNamingTests
    {
        [Fact]
        public void ReservedCommandNames_ContainsExpectedNames()
        {
            // Verify that all expected reserved names are present
            var reservedNames = CliShell.ReservedCommandNames;
            
            Assert.Contains("help", reservedNames);
            Assert.Contains("history", reservedNames);
            Assert.Contains("pwd", reservedNames);
            Assert.Contains("cd", reservedNames);
            Assert.Contains("clear", reservedNames);
            Assert.Contains("complete", reservedNames);
            Assert.Contains("exit", reservedNames);
            Assert.Contains("quit", reservedNames);
            Assert.Contains("echo", reservedNames);
            Assert.Contains("config", reservedNames);
        }

        [Theory]
        [InlineData("help")]
        [InlineData("HELP")]
        [InlineData("Help")]
        [InlineData("history")]
        [InlineData("pwd")]
        [InlineData("cd")]
        [InlineData("clear")]
        [InlineData("complete")]
        [InlineData("exit")]
        [InlineData("quit")]
        [InlineData("echo")]
        [InlineData("config")]
        public void Register_ReservedCommandName_ThrowsCommandNamingException(string reservedName)
        {
            // Arrange
            var input = new StringReader("exit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));
            var testCommand = new TestCommand(reservedName, "Test command");

            // Act & Assert
            var exception = Assert.Throws<CommandNamingException>(() => shell.Register(testCommand));
            
            Assert.Contains($"Command name '{reservedName}' is reserved", exception.Message);
            Assert.Contains("Reserved names:", exception.Message);
        }

        [Theory]
        [InlineData("help")]
        [InlineData("history")]
        [InlineData("pwd")]
        [InlineData("cd")]
        [InlineData("clear")]
        [InlineData("complete")]
        [InlineData("exit")]
        [InlineData("quit")]
        [InlineData("echo")]
        [InlineData("config")]
        public async Task RegisterAsync_ReservedCommandName_ThrowsCommandNamingException(string reservedName)
        {
            // Arrange
            var input = new StringReader("exit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));
            var testCommand = new TestCommand(reservedName, "Test command");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CommandNamingException>(
                async () => await shell.RegisterAsync(testCommand));
            
            Assert.Contains($"Command name '{reservedName}' is reserved", exception.Message);
            Assert.Contains("Reserved names:", exception.Message);
        }

        [Fact]
        public void Register_DuplicateCommandName_ThrowsCommandNamingException()
        {
            // Arrange
            var input = new StringReader("exit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));
            var testCommand1 = new TestCommand("test", "First test command");
            var testCommand2 = new TestCommand("test", "Second test command");

            // Act - Register first command successfully
            shell.Register(testCommand1);

            // Assert - Second registration should fail
            var exception = Assert.Throws<CommandNamingException>(() => shell.Register(testCommand2));
            
            Assert.Contains("Command 'test' is already registered", exception.Message);
            Assert.Contains("Each command name must be unique", exception.Message);
        }

        [Fact]
        public async Task RegisterAsync_DuplicateCommandName_ThrowsCommandNamingException()
        {
            // Arrange
            var input = new StringReader("exit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));
            var testCommand1 = new TestCommand("test", "First test command");
            var testCommand2 = new TestCommand("test", "Second test command");

            // Act - Register first command successfully
            await shell.RegisterAsync(testCommand1);

            // Assert - Second registration should fail
            var exception = await Assert.ThrowsAsync<CommandNamingException>(
                async () => await shell.RegisterAsync(testCommand2));
            
            Assert.Contains("Command 'test' is already registered", exception.Message);
            Assert.Contains("Each command name must be unique", exception.Message);
        }

        [Fact]
        public void Register_CaseInsensitiveDuplicateCommandName_ThrowsCommandNamingException()
        {
            // Arrange
            var input = new StringReader("exit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));
            var testCommand1 = new TestCommand("test", "First test command");
            var testCommand2 = new TestCommand("TEST", "Second test command");

            // Act - Register first command successfully
            shell.Register(testCommand1);

            // Assert - Second registration should fail due to case-insensitive comparison
            var exception = Assert.Throws<CommandNamingException>(() => shell.Register(testCommand2));
            
            Assert.Contains("Command 'TEST' is already registered", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void Register_NullOrWhitespaceCommandName_ThrowsCommandNamingException(string? invalidName)
        {
            // Arrange
            var input = new StringReader("exit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));
            var testCommand = new TestCommand(invalidName!, "Test command");

            // Act & Assert
            var exception = Assert.Throws<CommandNamingException>(() => shell.Register(testCommand));
            
            Assert.Contains("Command name cannot be null, empty, or whitespace", exception.Message);
        }

        [Fact]
        public void Register_ValidCommandName_Succeeds()
        {
            // Arrange
            var input = new StringReader("exit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));
            var testCommand = new TestCommand("valid-command", "Valid test command");

            // Act & Assert - Should not throw
            shell.Register(testCommand);
            
            // Verify command was registered
            Assert.Contains("valid-command", shell.CommandNames);
        }

        [Fact]
        public async Task RegisterAsync_ValidCommandName_Succeeds()
        {
            // Arrange
            var input = new StringReader("exit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));
            var testCommand = new TestCommand("valid-command-async", "Valid test command");

            // Act & Assert - Should not throw
            await shell.RegisterAsync(testCommand);
            
            // Verify command was registered
            Assert.Contains("valid-command-async", shell.CommandNames);
        }

        [Fact]
        public void Register_NullCommand_ThrowsArgumentNullException()
        {
            // Arrange
            var input = new StringReader("exit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => shell.Register(null!));
        }

        [Fact]
        public async Task RegisterAsync_NullCommand_ThrowsArgumentNullException()
        {
            // Arrange
            var input = new StringReader("exit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await shell.RegisterAsync(null!));
        }

        [Fact]
        public async Task BuiltInCommands_AreRegisteredDespiteBeingReserved()
        {
            // Arrange
            var input = new StringReader("help\nexit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));

            // Act - Run shell to verify built-in commands work
            int exitCode = await shell.RunAsync();

            // Assert - Should complete successfully and built-in commands should be available
            Assert.Equal(0, exitCode);
            Assert.Contains("help", shell.CommandNames);
            Assert.Contains("history", shell.CommandNames);
            Assert.Contains("pwd", shell.CommandNames);
            Assert.Contains("cd", shell.CommandNames);
            Assert.Contains("clear", shell.CommandNames);
            Assert.Contains("complete", shell.CommandNames);
        }

        /// <summary>
        /// Test command implementation for testing purposes.
        /// </summary>
        private sealed class TestCommand : ICliCommand
        {
            public TestCommand(string name, string description)
            {
                Name = name;
                Description = description;
            }

            public string Name { get; }
            public string Description { get; }

            public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken cancellationToken)
            {
                context.Writer.WriteLine($"Test command '{Name}' executed");
                return Task.FromResult(0);
            }
        }
    }
}