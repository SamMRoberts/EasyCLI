using EasyCLI.Console;
using EasyCLI.Shell;

namespace EasyCLI.Tests
{
    public class CommandCategorizationTests
    {
        [Fact]
        public void ICliCommand_Category_PropertyRequired()
        {
            // Arrange & Act
            var echoCommand = new TestCommand("test", "Test command");

            // Assert
            Assert.NotNull(echoCommand.Category);
            Assert.Equal("General", echoCommand.Category);
        }

        [Fact]
        public void BaseCliCommand_DefaultCategory_IsGeneral()
        {
            // Arrange & Act
            var command = new TestBaseCommand();

            // Assert
            Assert.Equal("General", command.Category);
        }

        [Fact]
        public void BaseCliCommand_Category_CanBeOverridden()
        {
            // Arrange & Act
            var command = new TestCustomCategoryCommand();

            // Assert
            Assert.Equal("Test", command.Category);
        }

        [Fact]
        public async Task CliShell_Help_ShowsCategorizedCommands()
        {
            // Arrange
            var input = new StringReader("help\nexit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));
            
            // Add a test command with custom category
            await shell.RegisterAsync(new TestCustomCategoryCommand());

            // Act
            int code = await shell.RunAsync();

            // Assert
            Assert.Equal(0, code);
            string result = output.ToString();
            Assert.Contains("Available Commands", result);
            Assert.Contains("Core:", result);
            Assert.Contains("Test:", result);
            Assert.Contains("Use 'help all' to see all commands", result);
        }

        [Fact]
        public async Task CliShell_HelpAll_ShowsAllCategorizedCommands()
        {
            // Arrange
            var input = new StringReader("help all\nexit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));
            
            // Add test commands with different categories
            await shell.RegisterAsync(new TestCustomCategoryCommand());
            await shell.RegisterAsync(new TestUtilityCommand());

            // Act
            int code = await shell.RunAsync();

            // Assert
            Assert.Equal(0, code);
            string result = output.ToString();
            Assert.Contains("Command Index - All Categories", result);
            Assert.Contains("Core:", result);
            Assert.Contains("Test:", result);
            Assert.Contains("Utility:", result);
            Assert.Contains("help", result); // Should show help command
            Assert.Contains("test-command", result); // Should show our test command
            Assert.Contains("test-utility", result); // Should show our utility command
        }

        [Fact]
        public async Task CliShell_HelpSpecificCommand_ShowsDetailedHelp()
        {
            // Arrange
            var input = new StringReader("help test-command\nexit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));
            
            await shell.RegisterAsync(new TestCustomCategoryCommand());

            // Act
            int code = await shell.RunAsync();

            // Assert
            Assert.Equal(0, code);
            string result = output.ToString();
            Assert.Contains("test-command - A test command with custom category", result);
            Assert.Contains("USAGE:", result);
            Assert.Contains("OPTIONS:", result);
        }

        [Fact]
        public async Task CliShell_HelpUnknownCommand_ShowsSuggestions()
        {
            // Arrange
            var input = new StringReader("help echoo\nexit\n"); // Typo in "echo"
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));

            // Act
            int code = await shell.RunAsync();

            // Assert
            Assert.Equal(0, code); // Shell should continue running
            string result = output.ToString();
            Assert.Contains("Unknown command 'echoo'", result);
            Assert.Contains("Did you mean:", result);
        }

        [Theory]
        [InlineData("Core")]
        [InlineData("Utility")]
        [InlineData("Configuration")]
        [InlineData("General")]
        public void CommandCategories_ValidCategories_AreSupported(string category)
        {
            // Arrange & Act
            var command = new TestCommandWithCategory("test", "Test", category);

            // Assert
            Assert.Equal(category, command.Category);
        }

        // Test command implementations
        private class TestCommand(string name, string description) : ICliCommand
        {
            public string Name { get; } = name;
            public string Description { get; } = description;
            public string Category => "General";

            public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken cancellationToken)
            {
                context.Writer.WriteLine($"Test command {Name} executed");
                return Task.FromResult(0);
            }
        }

        private class TestBaseCommand : BaseCliCommand
        {
            public override string Name => "test-base";
            public override string Description => "A test base command";

            protected override Task<int> ExecuteCommand(CommandLineArgs args, ShellExecutionContext context, CancellationToken cancellationToken)
            {
                context.Writer.WriteLine("Test base command executed");
                return Task.FromResult(0);
            }
        }

        private class TestCustomCategoryCommand : BaseCliCommand
        {
            public override string Name => "test-command";
            public override string Description => "A test command with custom category";
            public override string Category => "Test";

            protected override Task<int> ExecuteCommand(CommandLineArgs args, ShellExecutionContext context, CancellationToken cancellationToken)
            {
                context.Writer.WriteLine("Test custom category command executed");
                return Task.FromResult(0);
            }
        }

        private class TestUtilityCommand : BaseCliCommand
        {
            public override string Name => "test-utility";
            public override string Description => "A test utility command";
            public override string Category => "Utility";

            protected override Task<int> ExecuteCommand(CommandLineArgs args, ShellExecutionContext context, CancellationToken cancellationToken)
            {
                context.Writer.WriteLine("Test utility command executed");
                return Task.FromResult(0);
            }
        }

        private class TestCommandWithCategory(string name, string description, string category) : ICliCommand
        {
            public string Name { get; } = name;
            public string Description { get; } = description;
            public string Category { get; } = category;

            public Task<int> ExecuteAsync(ShellExecutionContext context, string[] args, CancellationToken cancellationToken)
            {
                context.Writer.WriteLine($"Test command {Name} in category {Category} executed");
                return Task.FromResult(0);
            }
        }
    }
}