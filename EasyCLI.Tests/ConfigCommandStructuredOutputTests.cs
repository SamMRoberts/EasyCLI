using System.IO;
using System.Threading.Tasks;
using EasyCLI.Console;
using EasyCLI.Shell;
using EasyCLI.Shell.Commands;
using Xunit;

namespace EasyCLI.Tests
{
    public class ConfigCommandStructuredOutputTests
    {
        [Fact]
        public async Task ConfigCommand_Integration_WithShell()
        {
            var input = new StringReader("config show --json\nconfig show --plain\nconfig show\nexit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output), new ShellOptions { Prompt = "test>" });

            int code = await shell.RunAsync();
            Assert.Equal(0, code);

            var result = output.ToString();

            // Should contain JSON output
            Assert.Contains("{", result);
            Assert.Contains("}", result);

            // Should contain plain text output with colons
            Assert.Contains(":", result);

            // Should contain table headers
            Assert.Contains("Setting", result);
            Assert.Contains("Value", result);
        }

        [Fact]
        public async Task ConfigCommandEnv_Integration_WithShell()
        {
            var input = new StringReader("config env --json\nconfig env --plain\nconfig env\nexit\n");
            var output = new StringWriter();
            var shell = new CliShell(new ConsoleReader(input), new ConsoleWriter(enableColors: false, output), new ShellOptions { Prompt = "test>" });

            int code = await shell.RunAsync();
            Assert.Equal(0, code);

            var result = output.ToString();

            // Should contain JSON output
            Assert.Contains("{", result);
            Assert.Contains("}", result);

            // Should contain plain text output with colons
            Assert.Contains(":", result);

            // Should contain environment information
            Assert.Contains("Platform", result);
        }
    }
}
