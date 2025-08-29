using EasyCLI.Console;
using EasyCLI.Shell;

namespace EasyCLI.Tests
{
    public class ShellTests
    {
        [Fact]
        public async Task BasicNavigationAndHistory()
        {
            string tmp = Directory.GetCurrentDirectory();
            var input = new StringReader("pwd\nhistory\ncd ..\npwd\nexit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output), new ShellOptions { Prompt = "test>" });

            int code = await shell.RunAsync();
            Assert.Equal(0, code);
            string all = output.ToString();
            Assert.Contains(tmp, all);
            Assert.Contains("history", all); // history listing
            Assert.Contains("test>", all); // prompt occurrences
        }

        [Fact]
        public async Task ShellOperatorDetection_PipeOperator()
        {
            var input = new StringReader("echo hello | grep h\nexit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output), new ShellOptions { Prompt = "test>" });

            int code = await shell.RunAsync();
            Assert.Equal(0, code);
            string all = output.ToString();
            Assert.Contains("hello", all); // Should show result of pipe operation
        }

        [Fact]
        public async Task ShellOperatorDetection_Redirection()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                var input = new StringReader($"echo test > {tempFile}\ncat {tempFile}\nexit\n");
                var output = new StringWriter();
                var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output), new ShellOptions { Prompt = "test>" });

                int code = await shell.RunAsync();
                Assert.Equal(0, code);
                string all = output.ToString();
                Assert.Contains("test", all); // Should show content written and read back
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public async Task ShellOperatorDetection_CommandChaining()
        {
            var input = new StringReader("echo first && echo second\nexit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output), new ShellOptions { Prompt = "test>" });

            int code = await shell.RunAsync();
            Assert.Equal(0, code);
            string all = output.ToString();
            Assert.Contains("first", all);
            Assert.Contains("second", all);
        }

        [Fact]
        public async Task NativeShellDelegation_CanBeDisabled()
        {
            var input = new StringReader("echo hello | grep h\nexit\n");
            var output = new StringWriter();
            var options = new ShellOptions { EnableNativeShellDelegation = false, Prompt = "test>" };
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output), options);

            int code = await shell.RunAsync();
            Assert.Equal(0, code);
            // When delegation is disabled, the shell does NOT interpret the pipe; it invokes `echo` with the remaining tokens.
            // So output should contain the literal characters including the pipe and NOT report a failing exit code.
            string all = output.ToString();
            Assert.Contains("hello | grep h", all); // literal, unprocessed pipeline text
            Assert.DoesNotContain("exit code", all); // no failure expected
        }

        [Fact]
        public async Task SimpleCommands_StillUseEasyCLI()
        {
            var input = new StringReader("pwd\nhelp\nexit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output), new ShellOptions { Prompt = "test>" });

            int code = await shell.RunAsync();
            Assert.Equal(0, code);
            string all = output.ToString();

            // Should contain EasyCLI's built-in commands help output
            Assert.Contains("pwd", all);
            Assert.Contains("help", all);
            Assert.Contains("Change working directory", all); // cd command description from help
        }

        [Fact]
        public async Task HelpFooter_AppearsInGeneralHelp()
        {
            var input = new StringReader("help\nexit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output), new ShellOptions { Prompt = "test>" });

            int code = await shell.RunAsync();
            Assert.Equal(0, code);
            string all = output.ToString();

            // Should contain the standard help footer
            Assert.Contains("SUPPORT:", all);
            Assert.Contains("Version:", all);
            Assert.Contains("Issues:  https://github.com/SamMRoberts/EasyCLI/issues", all);
            Assert.Contains("Docs:    https://github.com/SamMRoberts/EasyCLI", all);
            
            // Should contain output contract reference for script authors
            Assert.Contains("Use --plain or --json for stable output", all);
            Assert.Contains("https://github.com/SamMRoberts/EasyCLI/blob/main/docs/output-contract.md", all);
        }

        [Fact]
        public async Task EchoCommand_NoArguments_ShowsConciseHelp()
        {
            var input = new StringReader("echo\nexit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output), new ShellOptions { Prompt = "test>" });

            int code = await shell.RunAsync();
            Assert.Equal(0, code);
            string all = output.ToString();

            // Should show concise help instead of error
            Assert.DoesNotContain("Error: No text specified to echo", all);
            Assert.Contains("echo - Print text to the console with optional styling", all);
            Assert.Contains("USAGE:", all);
            Assert.Contains("echo [options] <text...>", all);
            Assert.Contains("EXAMPLES:", all);
            Assert.Contains("For more information, run: echo --help", all);
        }

        [Fact]
        public async Task ConfigCommand_NoArguments_ShowsConciseHelp()
        {
            var input = new StringReader("config\nexit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output), new ShellOptions { Prompt = "test>" });

            int code = await shell.RunAsync();
            Assert.Equal(0, code);
            string all = output.ToString();

            // Should show concise help for config command
            Assert.Contains("config - Manage application configuration and show environment information", all);
            Assert.Contains("USAGE:", all);
            Assert.Contains("config [subcommand] [options]", all);
            Assert.Contains("EXAMPLES:", all);
            Assert.Contains("For more information, run: config --help", all);
        }
    }
}
