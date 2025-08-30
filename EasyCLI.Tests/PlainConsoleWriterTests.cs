using System.IO;
using EasyCLI.Console;
using EasyCLI.Styling;
using Xunit;

namespace EasyCLI.Tests
{
    public class PlainConsoleWriterTests
    {
        [Fact]
        public void PlainConsoleWriter_Write_StripsAnsiEscapeSequences()
        {
            var sw = new StringWriter();
            var innerWriter = new ConsoleWriter(enableColors: true, output: sw);
            var plainWriter = new PlainConsoleWriter(innerWriter);

            plainWriter.Write("Hello", ConsoleStyles.FgRed);

            var result = sw.ToString();
            Assert.Equal("Hello", result);
            Assert.DoesNotContain("\u001b[31m", result);
            Assert.DoesNotContain("\u001b[0m", result);
        }

        [Fact]
        public void PlainConsoleWriter_WriteLine_StripsAnsiEscapeSequences()
        {
            var sw = new StringWriter();
            var innerWriter = new ConsoleWriter(enableColors: true, output: sw);
            var plainWriter = new PlainConsoleWriter(innerWriter);

            plainWriter.WriteLine("Hello", ConsoleStyles.FgRed);

            var result = sw.ToString();
            Assert.Equal("Hello" + System.Environment.NewLine, result);
            Assert.DoesNotContain("\u001b[31m", result);
            Assert.DoesNotContain("\u001b[0m", result);
        }

        [Fact]
        public void PlainConsoleWriter_Write_StripsCommonSymbols()
        {
            var sw = new StringWriter();
            var innerWriter = new ConsoleWriter(enableColors: false, output: sw);
            var plainWriter = new PlainConsoleWriter(innerWriter);

            plainWriter.Write("✓ Success ⚠ Warning ✗ Error");

            var result = sw.ToString();
            Assert.Equal("Success  Warning  Error", result);
            Assert.DoesNotContain("✓", result);
            Assert.DoesNotContain("⚠", result);
            Assert.DoesNotContain("✗", result);
        }

        [Fact]
        public void PlainConsoleWriter_Write_StripsBoxDrawingCharacters()
        {
            var sw = new StringWriter();
            var innerWriter = new ConsoleWriter(enableColors: false, output: sw);
            var plainWriter = new PlainConsoleWriter(innerWriter);

            plainWriter.Write("┌─────┐");

            var result = sw.ToString();
            Assert.Equal("-", result);
            Assert.DoesNotContain("┌", result);
            Assert.DoesNotContain("─", result);
            Assert.DoesNotContain("┐", result);
        }

        [Fact]
        public void PlainConsoleWriter_Write_HandlesMixedDecorations()
        {
            var sw = new StringWriter();
            var innerWriter = new ConsoleWriter(enableColors: true, output: sw);
            var plainWriter = new PlainConsoleWriter(innerWriter);

            var styledText = ConsoleStyles.FgGreen.Apply("✓ Test passed");
            plainWriter.Write(styledText);

            var result = sw.ToString();
            Assert.Equal("Test passed", result);
            Assert.DoesNotContain("✓", result);
            Assert.DoesNotContain("\u001b[", result);
        }

        [Fact]
        public void PlainConsoleWriter_Write_PreservesPlainText()
        {
            var sw = new StringWriter();
            var innerWriter = new ConsoleWriter(enableColors: false, output: sw);
            var plainWriter = new PlainConsoleWriter(innerWriter);

            plainWriter.Write("Plain text without decorations");

            var result = sw.ToString();
            Assert.Equal("Plain text without decorations", result);
        }

        [Fact]
        public void PlainConsoleWriter_Write_HandlesEmptyString()
        {
            var sw = new StringWriter();
            var innerWriter = new ConsoleWriter(enableColors: false, output: sw);
            var plainWriter = new PlainConsoleWriter(innerWriter);

            plainWriter.Write("");

            var result = sw.ToString();
            Assert.Equal("", result);
        }

        [Fact]
        public void PlainConsoleWriter_Write_HandlesNullString()
        {
            var sw = new StringWriter();
            var innerWriter = new ConsoleWriter(enableColors: false, output: sw);
            var plainWriter = new PlainConsoleWriter(innerWriter);

            // If Write does not accept null, this should throw. Otherwise, check the result.
            // Uncomment the following line if Write should throw an ArgumentNullException:
            // Assert.Throws<ArgumentNullException>(() => plainWriter.Write(null));

            // If Write should handle null gracefully, use:
            plainWriter.Write((string?)null);
            var result = sw.ToString();
            Assert.Equal("", result);
        }

        [Fact]
        public void PlainConsoleWriter_Write_HandlesOnlyDecorations()
        {
            var sw = new StringWriter();
            var innerWriter = new ConsoleWriter(enableColors: false, output: sw);
            var plainWriter = new PlainConsoleWriter(innerWriter);

            plainWriter.Write("✓⚠✗");

            var result = sw.ToString();
            Assert.Equal("", result); // All symbols should be stripped
        }

        [Fact]
        public void PlainConsoleWriter_WriteLine_WithoutStyling()
        {
            var sw = new StringWriter();
            var innerWriter = new ConsoleWriter(enableColors: false, output: sw);
            var plainWriter = new PlainConsoleWriter(innerWriter);

            plainWriter.WriteLine("Simple text");

            var result = sw.ToString();
            Assert.Equal("Simple text" + System.Environment.NewLine, result);
        }

        [Fact]
        public void PlainConsoleWriter_Write_WithoutStyling()
        {
            var sw = new StringWriter();
            var innerWriter = new ConsoleWriter(enableColors: false, output: sw);
            var plainWriter = new PlainConsoleWriter(innerWriter);

            plainWriter.Write("Simple text");

            var result = sw.ToString();
            Assert.Equal("Simple text", result);
        }

        [Fact]
        public void PlainConsoleWriter_ComparedToStyledOutput()
        {
            // Test with styled output
            var swStyled = new StringWriter();
            var styledWriter = new ConsoleWriter(enableColors: true, output: swStyled);
            styledWriter.Write("✓ Success", ConsoleStyles.FgGreen);

            // Test with plain output
            var swPlain = new StringWriter();
            var innerWriter = new ConsoleWriter(enableColors: true, output: swPlain);
            var plainWriter = new PlainConsoleWriter(innerWriter);
            plainWriter.Write("✓ Success", ConsoleStyles.FgGreen);

            var styledResult = swStyled.ToString();
            var plainResult = swPlain.ToString();

            // Styled output should contain ANSI codes and symbols
            Assert.Contains("\u001b[32m", styledResult); // Green color
            Assert.Contains("✓", styledResult);

            // Plain output should be clean
            Assert.Equal("Success", plainResult);
            Assert.DoesNotContain("\u001b[", plainResult);
            Assert.DoesNotContain("✓", plainResult);
        }

        [Fact]
        public void PlainConsoleWriter_StripsAllSymbolCategories()
        {
            var sw = new StringWriter();
            var innerWriter = new ConsoleWriter(enableColors: false, output: sw);
            var plainWriter = new PlainConsoleWriter(innerWriter);

            // Test symbols from different categories
            var testString = "✓Success ✗Error ⚠Warning ℹInfo " +  // Status symbols
                            "•Bullet ◉Circle ▶Arrow " +             // Bullet and directional symbols
                            "⚡Lightning ✨Sparkles 🔥Fire " +         // Effect symbols
                            "🎯Target 🏆Trophy " +                   // Achievement symbols
                            "📝Memo 📊Chart " +                      // Document symbols
                            "🔧Wrench ⚙Gear " +                      // Tool symbols
                            "🎨Art 🔄Process " +                     // Creative and time symbols
                            "🚀Rocket 🎉Party " +                    // Celebration symbols
                            "🔔Bell";                                // Notification symbols

            plainWriter.Write(testString);

            var result = sw.ToString();
            
            // Should contain only the text, no symbols
            var expected = "Success Error Warning Info " +
                          "Bullet Circle Arrow " +
                          "Lightning Sparkles Fire " +
                          "Target Trophy " +
                          "Memo Chart " +
                          "Wrench Gear " +
                          "Art Process " +
                          "Rocket Party " +
                          "Bell";
            
            Assert.Equal(expected, result);
            
            // Verify specific symbols are removed
            Assert.DoesNotContain("✓", result);
            Assert.DoesNotContain("✗", result);
            Assert.DoesNotContain("⚠", result);
            Assert.DoesNotContain("ℹ", result);
            Assert.DoesNotContain("•", result);
            Assert.DoesNotContain("◉", result);
            Assert.DoesNotContain("▶", result);
            Assert.DoesNotContain("⚡", result);
            Assert.DoesNotContain("✨", result);
            Assert.DoesNotContain("🔥", result);
            Assert.DoesNotContain("🎯", result);
            Assert.DoesNotContain("🏆", result);
            Assert.DoesNotContain("📝", result);
            Assert.DoesNotContain("📊", result);
            Assert.DoesNotContain("🔧", result);
            Assert.DoesNotContain("⚙", result);
            Assert.DoesNotContain("🎨", result);
            Assert.DoesNotContain("🔄", result);
            Assert.DoesNotContain("🚀", result);
            Assert.DoesNotContain("🎉", result);
            Assert.DoesNotContain("🔔", result);
        }
    }
}
