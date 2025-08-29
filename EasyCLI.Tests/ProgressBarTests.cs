using EasyCLI.Console;
using EasyCLI.Extensions;
using EasyCLI.Progress;
using EasyCLI.Styling;
using Xunit;

namespace EasyCLI.Tests
{
    /// <summary>
    /// Tests for progress bar functionality and formatting.
    /// </summary>
    public class ProgressBarTests
    {
        [Fact]
        public void ProgressBar_DefaultConstructor_SetsDefaultValues()
        {
            var progressBar = new ProgressBar();

            // Test with default settings
            var result = progressBar.Render(50, 100);

            Assert.Contains("[", result);
            Assert.Contains("]", result);
            Assert.Contains("50%", result);
        }

        [Fact]
        public void ProgressBar_Render_WithValidProgress_ShowsCorrectPercentage()
        {
            var progressBar = new ProgressBar(width: 20, showPercentage: true);

            var result = progressBar.Render(25, 100);

            Assert.Contains("25%", result);
            Assert.Contains("[", result);
            Assert.Contains("]", result);
        }

        [Fact]
        public void ProgressBar_Render_WithZeroTotal_ShowsIndeterminate()
        {
            var progressBar = new ProgressBar();

            var result = progressBar.Render(10, 0);

            Assert.Contains("working...", result);
        }

        [Fact]
        public void ProgressBar_Render_WithNegativeTotal_ShowsIndeterminate()
        {
            var progressBar = new ProgressBar();

            var result = progressBar.Render(10, -5);

            Assert.Contains("working...", result);
        }

        [Fact]
        public void ProgressBar_Render_WithPercentage_ShowsCorrectFormat()
        {
            var progressBar = new ProgressBar(width: 10);

            var result = progressBar.Render(0.75); // 75%

            Assert.Contains("75%", result);
            Assert.Contains("[", result);
            Assert.Contains("]", result);
        }

        [Fact]
        public void ProgressBar_Render_WithPercentageGreaterThanOne_ClampedToHundredPercent()
        {
            var progressBar = new ProgressBar();

            var result = progressBar.Render(1.5); // 150% should be clamped to 100%

            Assert.Contains("100%", result);
        }

        [Fact]
        public void ProgressBar_Render_WithNegativePercentage_ClampedToZero()
        {
            var progressBar = new ProgressBar();

            var result = progressBar.Render(-0.5); // -50% should be clamped to 0%

            Assert.Contains("0%", result);
        }

        [Fact]
        public void ProgressBar_Render_WithFraction_ShowsCurrentAndTotal()
        {
            var progressBar = new ProgressBar(showFraction: true);

            var result = progressBar.Render(42, 100);

            Assert.Contains("42/100", result);
        }

        [Fact]
        public void ProgressBar_Render_WithCustomChars_UsesSpecifiedCharacters()
        {
            var progressBar = new ProgressBar(width: 4, filledChar: '#', emptyChar: '-');

            var result = progressBar.Render(50, 100);

            Assert.Contains("[##--]", result);
        }

        [Fact]
        public void ProgressBar_CalculatePercentage_ReturnsCorrectValue()
        {
            Assert.Equal(50, ProgressBar.CalculatePercentage(50, 100));
            Assert.Equal(75, ProgressBar.CalculatePercentage(3, 4));
            Assert.Equal(0, ProgressBar.CalculatePercentage(0, 100));
            Assert.Equal(100, ProgressBar.CalculatePercentage(100, 100));
        }

        [Fact]
        public void ProgressBar_CalculatePercentage_WithZeroTotal_ReturnsZero()
        {
            Assert.Equal(0, ProgressBar.CalculatePercentage(50, 0));
            Assert.Equal(0, ProgressBar.CalculatePercentage(100, -1));
        }

        [Fact]
        public void ProgressBar_CalculatePercentage_WithOverflow_ClampedToHundred()
        {
            Assert.Equal(100, ProgressBar.CalculatePercentage(150, 100));
        }

        [Fact]
        public void ConsoleWriter_WriteProgressBar_WritesCorrectOutput()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            writer.WriteProgressBar(50, 100);

            var result = output.ToString();
            Assert.Contains("[", result);
            Assert.Contains("]", result);
            Assert.Contains("50%", result);
        }

        [Fact]
        public void ConsoleWriter_WriteProgressBarLine_WritesWithNewline()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);

            writer.WriteProgressBarLine(25, 100);

            var result = output.ToString();
            Assert.Contains("25%", result);
            Assert.EndsWith(Environment.NewLine, result);
        }

        [Fact]
        public void ConsoleWriter_WriteProgressBar_WithTheme_AppliesTheme()
        {
            using var output = new StringWriter();
            var writer = new ConsoleWriter(output: output);
            var theme = ConsoleThemes.Dark;

            writer.WriteProgressBar(50, 100, theme: theme);

            var result = output.ToString();
            Assert.Contains("50%", result);
            // Should contain ANSI escape sequences from theme
            Assert.Contains("\u001b[", result);
        }
    }
}
