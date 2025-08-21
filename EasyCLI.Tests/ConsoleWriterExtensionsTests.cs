using System.IO;
using Xunit;

namespace EasyCLI.Tests
{
    public class ConsoleWriterExtensionsTests
    {
        [Fact]
        public void SemanticWriteLine_UsesExpectedStyles_WhenEnabled()
        {
            var sw = new StringWriter();
            var w = new EasyCLI.ConsoleWriter(enableColors: true, output: sw);

            w.WriteSuccessLine("ok");
            w.WriteWarningLine("warn");
            w.WriteErrorLine("err");
            w.WriteHeadingLine("head");
            w.WriteInfoLine("info");
            w.WriteHintLine("hint");

            var s = sw.ToString();
            Assert.Contains("\u001b[32m", s); // Success
            Assert.Contains("\u001b[33m", s); // Warning
            Assert.Contains("\u001b[91m", s); // Error
            Assert.Contains("\u001b[1;36m", s); // Heading (bold + cyan)
            Assert.Contains("\u001b[36m", s); // Info
            Assert.Contains("\u001b[90m", s); // Hint
            Assert.DoesNotContain("\u001b[0m\u001b[0m", s); // Should not double-reset
        }

        [Fact]
        public void SemanticWriteLine_FallsBackToPlain_WhenDisabled()
        {
            var sw = new StringWriter();
            var w = new EasyCLI.ConsoleWriter(enableColors: false, output: sw);

            w.WriteSuccessLine("ok");
            var s = sw.ToString();
            Assert.Equal("ok\n", s.Replace("\r\n", "\n"));
        }
    }
}
