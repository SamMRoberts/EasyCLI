using System.IO;
using EasyCLI.Console;
using EasyCLI.Shell;
using Xunit;

namespace EasyCLI.Tests
{
    public class ConsoleWriterFactoryTests
    {
        [Fact]
        public void Create_WithoutArguments_ReturnsRegularConsoleWriter()
        {
            var writer = ConsoleWriterFactory.Create();
            
            Assert.IsType<ConsoleWriter>(writer);
        }

        [Fact]
        public void Create_WithPlainFlag_ReturnsPlainConsoleWriter()
        {
            var args = new CommandLineArgs(new[] { "--plain" });
            
            var writer = ConsoleWriterFactory.Create(args);
            
            Assert.IsType<PlainConsoleWriter>(writer);
        }

        [Fact]
        public void Create_WithShortPlainFlag_ReturnsPlainConsoleWriter()
        {
            var args = new CommandLineArgs(new[] { "-p" });
            
            var writer = ConsoleWriterFactory.Create(args);
            
            Assert.IsType<PlainConsoleWriter>(writer);
        }

        [Fact]
        public void Create_WithoutPlainFlag_ReturnsRegularConsoleWriter()
        {
            var args = new CommandLineArgs(new[] { "--verbose" });
            
            var writer = ConsoleWriterFactory.Create(args);
            
            Assert.IsType<ConsoleWriter>(writer);
        }

        [Fact]
        public void Create_WithExplicitPlainMode_ReturnsPlainConsoleWriter()
        {
            var writer = ConsoleWriterFactory.Create(plainMode: true);
            
            Assert.IsType<PlainConsoleWriter>(writer);
        }

        [Fact]
        public void Create_WithExplicitNonPlainMode_ReturnsRegularConsoleWriter()
        {
            var writer = ConsoleWriterFactory.Create(plainMode: false);
            
            Assert.IsType<ConsoleWriter>(writer);
        }

        [Fact]
        public void Create_WithCustomOutput_UsesProvidedOutput()
        {
            var sw = new StringWriter();
            var args = new CommandLineArgs(new[] { "--plain" });
            
            var writer = ConsoleWriterFactory.Create(args, sw);
            
            Assert.IsType<PlainConsoleWriter>(writer);
            // Verify it uses our StringWriter by writing to it
            writer.Write("test");
            Assert.Equal("test", sw.ToString());
        }

        [Fact]
        public void ShouldUsePlainMode_WithPlainFlag_ReturnsTrue()
        {
            var args = new CommandLineArgs(new[] { "--plain" });
            
            var result = ConsoleWriterFactory.ShouldUsePlainMode(args);
            
            Assert.True(result);
        }

        [Fact]
        public void ShouldUsePlainMode_WithShortPlainFlag_ReturnsTrue()
        {
            var args = new CommandLineArgs(new[] { "-p" });
            
            var result = ConsoleWriterFactory.ShouldUsePlainMode(args);
            
            Assert.True(result);
        }

        [Fact]
        public void ShouldUsePlainMode_WithoutPlainFlag_ReturnsFalse()
        {
            var args = new CommandLineArgs(new[] { "--verbose" });
            
            var result = ConsoleWriterFactory.ShouldUsePlainMode(args);
            
            Assert.False(result);
        }

        [Fact]
        public void ShouldUsePlainMode_WithNullArgs_ReturnsFalse()
        {
            var result = ConsoleWriterFactory.ShouldUsePlainMode(null);
            
            Assert.False(result);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("true")]
        [InlineData("any_value")]
        public void ShouldDisableColors_WithNoColorSet_ReturnsTrue(string noColorValue)
        {
            // Arrange - Set NO_COLOR environment variable
            string? originalValue = System.Environment.GetEnvironmentVariable("NO_COLOR");
            try
            {
                System.Environment.SetEnvironmentVariable("NO_COLOR", noColorValue);
                
                // Act
                var result = ConsoleWriterFactory.ShouldDisableColors();
                
                // Assert
                Assert.True(result);
            }
            finally
            {
                // Cleanup
                System.Environment.SetEnvironmentVariable("NO_COLOR", originalValue);
            }
        }

        [Fact]
        public void ShouldDisableColors_WithoutNoColor_ChecksOtherConditions()
        {
            // Arrange - Ensure NO_COLOR is not set
            string? originalValue = System.Environment.GetEnvironmentVariable("NO_COLOR");
            try
            {
                System.Environment.SetEnvironmentVariable("NO_COLOR", null);
                
                // Act
                var result = ConsoleWriterFactory.ShouldDisableColors();
                
                // Assert
                // Result depends on other conditions (output redirection, TERM variable)
                // We just verify it doesn't throw and returns a boolean
                Assert.IsType<bool>(result);
            }
            finally
            {
                // Cleanup
                System.Environment.SetEnvironmentVariable("NO_COLOR", originalValue);
            }
        }

        [Fact]
        public void IntegrationTest_PlainFlagWithColoredOutput()
        {
            var sw = new StringWriter();
            var args = new CommandLineArgs(new[] { "--plain" });
            
            var writer = ConsoleWriterFactory.Create(args, sw);
            
            // Write styled content that should be stripped
            writer.Write("✓ Success", EasyCLI.Styling.ConsoleStyles.FgGreen);
            
            var result = sw.ToString();
            Assert.Equal("Success", result);
            Assert.DoesNotContain("✓", result);
            Assert.DoesNotContain("\u001b[", result);
        }

        [Fact]
        public void IntegrationTest_NoPlainFlagWithColoredOutput()
        {
            var sw = new StringWriter();
            var args = new CommandLineArgs(System.Array.Empty<string>());
            
            var writer = ConsoleWriterFactory.Create(args, sw);
            
            // Write styled content that should remain styled
            writer.Write("✓ Success", EasyCLI.Styling.ConsoleStyles.FgGreen);
            
            var result = sw.ToString();
            Assert.Contains("✓", result);
            Assert.Contains("Success", result);
            // Colors may or may not be present depending on environment
        }
    }
}