using System;
using System.IO;
using Xunit;

namespace EasyCLI.Tests
{
    public class ConsoleWriterTests
    {
        [Fact]
        public void WritesPlain_WhenColorsDisabled()
        {
            var sw = new StringWriter();
            var writer = new EasyCLI.ConsoleWriter(enableColors: false, output: sw);

            writer.Write("Hello", ConsoleStyles.FgRed);
            Assert.Equal("Hello", sw.ToString());
        }

        [Fact]
        public void WritesStyled_WhenColorsEnabled()
        {
            var sw = new StringWriter();
            var writer = new EasyCLI.ConsoleWriter(enableColors: true, output: sw);

            writer.Write("Hello", ConsoleStyles.FgRed);
            var s = sw.ToString();
            Assert.StartsWith("\u001b[31m", s);
            Assert.EndsWith("\u001b[0m", s);
            Assert.Contains("Hello", s);
        }

        [Fact]
        public void Apply_ComposesCodesAndResets()
        {
            var style = new EasyCLI.Styling.ConsoleStyle(1, 36);
            var s = style.Apply("X");
            Assert.Equal("\u001b[1;36mX\u001b[0m", s);
        }
    }
}
