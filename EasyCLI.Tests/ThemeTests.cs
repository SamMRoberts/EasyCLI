using Xunit;

namespace EasyCLI.Tests
{
    public class ThemeTests
    {
        [Fact]
        public void Theme_Overrides_AreUsed_ByExtensions()
        {
            var sw = new System.IO.StringWriter();
            var w = new EasyCLI.ConsoleWriter(enableColors: true, output: sw);
            var theme = new EasyCLI.ConsoleTheme
            {
                Success = new EasyCLI.ConsoleStyle(92), // bright green
                Heading = new EasyCLI.ConsoleStyle(95)  // bright magenta
            };

            w.WriteSuccessLine("ok", theme);
            w.WriteHeadingLine("head", theme);

            var s = sw.ToString();
            Assert.Contains("\u001b[92m", s);
            Assert.Contains("\u001b[95m", s);
        }
    }
}
