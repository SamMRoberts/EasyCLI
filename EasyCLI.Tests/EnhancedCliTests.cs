using EasyCLI.Console;
using EasyCLI.Shell;

namespace EasyCLI.Tests
{
    public class EnhancedCliTests
    {
        [Fact]
        public void CommandLineArgs_ParsesFlags_Correctly()
        {
            var args = new CommandLineArgs(new[] { "--help", "--verbose", "-q" });

            Assert.True(args.IsHelpRequested);
            Assert.True(args.IsVerbose);
            Assert.True(args.IsQuiet);
            Assert.False(args.IsDryRun);
        }

        [Fact]
        public void CommandLineArgs_ParsesOptions_Correctly()
        {
            var args = new CommandLineArgs(new[] { "--output", "file.txt", "--repeat=3", "-r", "5" });

            Assert.Equal("file.txt", args.GetOption("output"));
            Assert.Equal("3", args.GetOption("repeat"));
            Assert.Equal("5", args.GetOption("r"));
        }

        [Fact]
        public void CommandLineArgs_ParsesPositionalArguments_Correctly()
        {
            var args = new CommandLineArgs(new[] { "command", "arg1", "arg2", "--flag" });

            Assert.Equal(3, args.Arguments.Count);
            Assert.Equal("command", args.GetArgument(0));
            Assert.Equal("arg1", args.GetArgument(1));
            Assert.Equal("arg2", args.GetArgument(2));
            Assert.True(args.HasFlag("flag"));
        }

        [Fact]
        public void ExitCodes_HasStandardValues()
        {
            Assert.Equal(0, ExitCodes.Success);
            Assert.Equal(1, ExitCodes.GeneralError);
            Assert.Equal(2, ExitCodes.FileNotFound);
            Assert.Equal(4, ExitCodes.InvalidArguments);
            Assert.Equal(127, ExitCodes.CommandNotFound);
        }

        [Fact]
        public async Task BaseCliCommand_ShowHelp_IncludesStandardFooter()
        {
            // Create a test command
            var testCommand = new TestCommand();
            var output = new StringWriter();
            var context = new ShellExecutionContext(
                new ConsoleReader(new StringReader("")),
                new ConsoleWriter(enableColors: false, output)
            );

            // Execute with help flag
            int result = await testCommand.ExecuteAsync(context, new[] { "--help" }, CancellationToken.None);

            Assert.Equal(0, result);
            string helpOutput = output.ToString();

            // Verify standard footer is present
            Assert.Contains("SUPPORT:", helpOutput);
            Assert.Contains("Version:", helpOutput);
            Assert.Contains("Issues:  https://github.com/SamMRoberts/EasyCLI/issues", helpOutput);
            Assert.Contains("Docs:    https://github.com/SamMRoberts/EasyCLI", helpOutput);
        }

        // Simple test command for help footer testing
        private class TestCommand : BaseCliCommand
        {
            public override string Name => "test";
            public override string Description => "A test command for footer verification";

            protected override Task<int> ExecuteCommand(CommandLineArgs args, ShellExecutionContext context, CancellationToken cancellationToken)
            {
                return Task.FromResult(ExitCodes.Success);
            }
        }
    }
}
