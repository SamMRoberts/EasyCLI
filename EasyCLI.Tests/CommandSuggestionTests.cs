using EasyCLI.Console;
using EasyCLI.Shell;
using Xunit;

namespace EasyCLI.Tests
{
    public class CommandSuggestionTests
    {
        [Fact]
        public async Task CliShell_SuggestsNearestCommand_WhenTypoExists()
        {
            // Arrange
            var input = new StringReader("hep\nexit\n"); // Typo for "help"
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));

            // Act
            int exitCode = await shell.RunAsync();

            // Assert
            Assert.Equal(0, exitCode); // Shell itself exits cleanly
            string allOutput = output.ToString();
            Assert.Contains("Unknown command 'hep'", allOutput);
            Assert.Contains("Did you mean 'help'?", allOutput);
        }

        [Fact]
        public async Task CliShell_SuggestsNearestCommand_ForMultipleTypos()
        {
            // Arrange
            var input = new StringReader("histroy\ncler\nconig\nexit\n"); // Multiple typos
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));

            // Act
            await shell.RunAsync();

            // Assert
            string allOutput = output.ToString();
            Assert.Contains("Did you mean 'history'?", allOutput);
            Assert.Contains("Did you mean 'clear'?", allOutput);
            Assert.Contains("Did you mean 'config'?", allOutput);
        }

        [Fact]
        public async Task CliShell_DoesNotSuggest_WhenDistanceTooLarge()
        {
            // Arrange
            var input = new StringReader("xyzzyx\nexit\n"); // No reasonable suggestion
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));

            // Act
            await shell.RunAsync();

            // Assert
            string allOutput = output.ToString();
            Assert.DoesNotContain("Did you mean", allOutput);
        }

        [Fact]
        public async Task ConfigCommand_SuggestsSimilarSubcommand_WhenTypoExists()
        {
            // Arrange
            var input = new StringReader("config shw\nexit\n"); // Typo for "show"
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));

            // Act
            await shell.RunAsync();

            // Assert
            string allOutput = output.ToString();
            Assert.Contains("Unknown subcommand: shw", allOutput);
            Assert.Contains("Did you mean 'show'?", allOutput);
        }

        [Fact]
        public async Task ConfigCommand_SuggestsSimilarSubcommand_ForMultipleTypos()
        {
            // Arrange
            var input = new StringReader("config gt\nconfig st\nconfig evn\nexit\n"); // Multiple typos
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));

            // Act
            await shell.RunAsync();

            // Assert
            string allOutput = output.ToString();
            Assert.Contains("Did you mean 'get'?", allOutput);
            Assert.Contains("Did you mean 'set'?", allOutput);
            Assert.Contains("Did you mean 'env'?", allOutput);
        }

        [Fact]
        public async Task ConfigCommand_ShowsAvailableSubcommands_WhenNoCloseMatch()
        {
            // Arrange
            var input = new StringReader("config abcdefgh\nexit\n"); // No reasonable suggestion - distance too high
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));

            // Act
            await shell.RunAsync();

            // Assert
            string allOutput = output.ToString();
            Assert.Contains("Available subcommands:", allOutput);
            Assert.Contains("show", allOutput);
            Assert.Contains("get", allOutput);
            Assert.Contains("set", allOutput);
        }

        [Theory]
        [InlineData("help")]
        [InlineData("history")]
        [InlineData("pwd")]
        [InlineData("cd")]
        [InlineData("clear")]
        [InlineData("complete")]
        [InlineData("config")]
        public async Task CliShell_DoesNotSuggest_ForValidCommands(string validCommand)
        {
            // Arrange
            var input = new StringReader($"{validCommand}\nexit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));

            // Act
            await shell.RunAsync();

            // Assert
            string allOutput = output.ToString();
            Assert.DoesNotContain("Did you mean", allOutput);
            Assert.DoesNotContain("Unknown command", allOutput);
        }

        [Fact]
        public async Task BaseCliCommand_SuggestsSimilarOption_WhenTypoExists()
        {
            // Arrange
            var input = new StringReader("test --verbos\nexit\n"); // Typo for "--verbose"
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));
            await shell.RegisterAsync(new TestCommand());

            // Act
            await shell.RunAsync();

            // Assert
            string allOutput = output.ToString();
            Assert.Contains("Unknown option: --verbos", allOutput);
            Assert.Contains("Did you mean '--verbose'?", allOutput);
        }

        [Fact]
        public async Task BaseCliCommand_SuggestsSimilarOption_ForMultipleTypos()
        {
            // Arrange
            var input = new StringReader("test --hlep\ntest --outpt\ntest --confi\nexit\n"); // Multiple option typos
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output));
            await shell.RegisterAsync(new TestCommand());

            // Act
            await shell.RunAsync();

            // Assert
            string allOutput = output.ToString();
            Assert.Contains("Did you mean '--help'?", allOutput);
            Assert.Contains("Did you mean '--output'?", allOutput);
            Assert.Contains("Did you mean '--config'?", allOutput);
        }

        private sealed class TestCommand : BaseCliCommand
        {
            public override string Name => "test";
            public override string Description => "A test command for option suggestion testing";

            protected override Task<int> ExecuteCommand(CommandLineArgs args, ShellExecutionContext context, CancellationToken cancellationToken)
            {
                // Check for unknown options in the raw arguments
                // Note: In shell context, arguments come as strings, not parsed CommandLineArgs
                for (int i = 0; i < args.Arguments.Count; i++)
                {
                    string arg = args.Arguments[i];
                    if (arg.StartsWith("--", StringComparison.Ordinal) && !IsKnownOption(arg))
                    {
                        context.Writer.WriteErrorLine($"Unknown option: {arg}");
                        string[] knownOptions = ["--help", "--verbose", "--output", "--config"];
                        SuggestSimilarOption(arg, context, knownOptions);
                        return Task.FromResult(ExitCodes.InvalidArguments);
                    }
                }

                context.Writer.WriteLine("Test command executed successfully");
                return Task.FromResult(ExitCodes.Success);
            }

            private static bool IsKnownOption(string option)
            {
                string[] knownOptions = ["--help", "--verbose", "--output", "--config"];
                return knownOptions.Contains(option);
            }
        }
    }
}