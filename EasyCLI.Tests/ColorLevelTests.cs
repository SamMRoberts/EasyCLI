using System;
using Xunit;

namespace EasyCLI.Tests
{
    public class ColorLevelTests
    {
        [Theory]
        [InlineData("truecolor", ColorSupportLevel.TrueColor)]
        [InlineData("TRUECOLOR", ColorSupportLevel.TrueColor)]
        public void Detects_TrueColor_From_COLORTERM(string value, ColorSupportLevel expected)
        {
            using var scope = new EnvVarScope(("COLORTERM", value), ("TERM", "xterm-256color")); // even if 256, truecolor wins
            var w = new ConsoleWriter(enableColors: true, output: new System.IO.StringWriter());
            Assert.Equal(expected, w.ColorLevel);
        }

        [Theory]
        [InlineData("xterm-256color")]
        [InlineData("screen-256color")]
        public void Detects_256_From_TERM(string term)
        {
            using var scope = new EnvVarScope(("COLORTERM", null), ("TERM", term));
            var w = new ConsoleWriter(enableColors: true, output: new System.IO.StringWriter());
            Assert.Equal(ColorSupportLevel.Indexed256, w.ColorLevel);
        }

        [Fact]
        public void Defaults_To_Basic16()
        {
            using var scope = new EnvVarScope(("COLORTERM", null), ("TERM", "xterm"));
            var w = new ConsoleWriter(enableColors: true, output: new System.IO.StringWriter());
            Assert.Equal(ColorSupportLevel.Basic16, w.ColorLevel);
        }

        [Fact]
        public void None_When_Colors_Disabled()
        {
            using var scope = new EnvVarScope(("COLORTERM", "truecolor"));
            var w = new ConsoleWriter(enableColors: false, output: new System.IO.StringWriter());
            Assert.Equal(ColorSupportLevel.None, w.ColorLevel);
        }
    }
}
