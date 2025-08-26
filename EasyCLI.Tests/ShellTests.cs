using EasyCLI.Console;
using EasyCLI.Shell;
using System.IO;
using System.Threading.Tasks;
using Xunit;

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
    }
}
