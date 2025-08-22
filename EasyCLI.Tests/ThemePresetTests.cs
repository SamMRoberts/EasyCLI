using Xunit;

namespace EasyCLI.Tests
{
    public class ThemePresetTests
    {
        [Fact]
        public void DarkTheme_UsesExpectedStyles()
        {
            var sw = new System.IO.StringWriter();
            var w = new EasyCLI.ConsoleWriter(enableColors: true, output: sw);
            var theme = EasyCLI.Styling.ConsoleThemes.Dark;

            w.WriteErrorLine("boom", theme);     // bright red = 91
            w.WriteHeadingLine("head", theme);   // bold + cyan = 1;36

            var s = sw.ToString();
            Assert.Contains("\u001b[91m", s);
            Assert.Contains("\u001b[1;36m", s);
        }

        [Fact]
        public void LightTheme_UsesExpectedStyles()
        {
            var sw = new System.IO.StringWriter();
            var w = new EasyCLI.ConsoleWriter(enableColors: true, output: sw);
            var theme = EasyCLI.Styling.ConsoleThemes.Light;

            w.WriteWarningLine("careful", theme); // blue = 34
            w.WriteErrorLine("err", theme);       // red = 31

            var s = sw.ToString();
            Assert.Contains("\u001b[34m", s);
            Assert.Contains("\u001b[31m", s);
        }

        [Fact]
        public void HighContrastTheme_UsesExpectedStyles()
        {
            var sw = new System.IO.StringWriter();
            var w = new EasyCLI.ConsoleWriter(enableColors: true, output: sw);
            var theme = EasyCLI.Styling.ConsoleThemes.HighContrast;

            w.WriteSuccessLine("ok", theme);      // bright green = 92
            w.WriteHeadingLine("head", theme);    // bold + bright white = 1;97

            var s = sw.ToString();
            Assert.Contains("\u001b[92m", s);
            Assert.Contains("\u001b[1;97m", s);
        }
    }
}
