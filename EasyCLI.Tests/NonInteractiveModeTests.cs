using EasyCLI.Environment;
using EasyCLI.Prompts;
using EasyCLI.Shell;
using EasyCLI.Tests.Fakes;
using Xunit;

namespace EasyCLI.Tests
{
    public class NonInteractiveModeTests
    {
        [Fact]
        public void CommandLineArgs_IsNoInput_ReturnsTrueWhenNoInputFlagPresent()
        {
            var argsLong = new CommandLineArgs(new[] { "--no-input" });
            var argsAlias = new CommandLineArgs(new[] { "--non-interactive" });

            Assert.True(argsLong.IsNoInput);
            Assert.True(argsAlias.IsNoInput);
        }

        [Fact]
        public void CommandLineArgs_IsNoInput_ReturnsFalseWhenNoInputFlagNotPresent()
        {
            var args = new CommandLineArgs(new[] { "--verbose", "--plain" });

            Assert.False(args.IsNoInput);
        }

        [Fact]
        public void EnvironmentDetector_IsNonInteractiveMode_ReturnsTrueWhenNoInputFlagPresent()
        {
            var args = new CommandLineArgs(new[] { "--no-input" });

            bool result = EnvironmentDetector.IsNonInteractiveMode(args);

            Assert.True(result);
        }

        [Fact]
        public void EnvironmentDetector_CreatePromptOptions_SetsNonInteractiveWhenNoInputFlagPresent()
        {
            var args = new CommandLineArgs(new[] { "--no-input" });

            var options = EnvironmentDetector.CreatePromptOptions(args);

            Assert.True(options.NonInteractive);
        }

        [Fact]
        public void EnvironmentDetector_CreatePromptOptions_ExtendsBaseOptions()
        {
            var args = new CommandLineArgs(new[] { "--no-input" });
            var baseOptions = new PromptOptions { EnablePaging = false };

            var options = EnvironmentDetector.CreatePromptOptions(args, baseOptions);

            Assert.True(options.NonInteractive);
            Assert.False(options.EnablePaging); // Preserved from base options
        }

        [Fact]
        public void StringPrompt_NonInteractive_ReturnsDefaultWhenAvailable()
        {
            var reader = new FakeConsoleReader(new[] { "should not be read" });
            var writer = new FakeConsoleWriter();
            var options = new PromptOptions { NonInteractive = true };
            var prompt = new StringPrompt("Name", writer, reader, options, @default: "DefaultValue");

            var result = prompt.GetValue();

            Assert.Equal("DefaultValue", result);
            // Should not have written the prompt since we're in non-interactive mode
            Assert.DoesNotContain("Name", writer.Output);
        }

        [Fact]
        public void StringPrompt_NonInteractive_ThrowsWhenNoDefaultAvailable()
        {
            var reader = new FakeConsoleReader(new[] { "should not be read" });
            var writer = new FakeConsoleWriter();
            var options = new PromptOptions { NonInteractive = true };
            var prompt = new StringPrompt("Name", writer, reader, options);

            var exception = Assert.Throws<InvalidOperationException>(() => prompt.GetValue());

            Assert.Contains("Cannot prompt for 'Name' in non-interactive mode", exception.Message);
            Assert.Contains("Use --no-input only when all prompts have default values", exception.Message);
        }

        [Fact]
        public void IntPrompt_NonInteractive_ReturnsDefaultWhenAvailable()
        {
            var reader = new FakeConsoleReader(new[] { "should not be read" });
            var writer = new FakeConsoleWriter();
            var options = new PromptOptions { NonInteractive = true };
            var prompt = new IntPrompt("Age", writer, reader, options, @default: 25);

            var result = prompt.GetValue();

            Assert.Equal(25, result);
        }

        [Fact]
        public void IntPrompt_NonInteractive_ThrowsWhenNoDefaultAvailable()
        {
            var reader = new FakeConsoleReader(new[] { "should not be read" });
            var writer = new FakeConsoleWriter();
            var options = new PromptOptions { NonInteractive = true };
            var prompt = new IntPrompt("Age", writer, reader, options);

            var exception = Assert.Throws<InvalidOperationException>(() => prompt.GetValue());

            Assert.Contains("Cannot prompt for 'Age' in non-interactive mode", exception.Message);
        }

        [Fact]
        public void YesNoPrompt_NonInteractive_ReturnsDefaultWhenAvailable()
        {
            var reader = new FakeConsoleReader(new[] { "should not be read" });
            var writer = new FakeConsoleWriter();
            var options = new PromptOptions { NonInteractive = true };
            var prompt = new YesNoPrompt("Continue", writer, reader, options, @default: true);

            var result = prompt.GetValue();

            Assert.True(result);
        }

        [Fact]
        public void YesNoPrompt_NonInteractive_ThrowsWhenNoDefaultAvailable()
        {
            var reader = new FakeConsoleReader(new[] { "should not be read" });
            var writer = new FakeConsoleWriter();
            var options = new PromptOptions { NonInteractive = true };
            var prompt = new YesNoPrompt("Continue", writer, reader, options);

            var exception = Assert.Throws<InvalidOperationException>(() => prompt.GetValue());

            Assert.Contains("Cannot prompt for 'Continue (y/n)' in non-interactive mode", exception.Message);
        }

        [Fact]
        public void StringPrompt_Interactive_WorksNormally()
        {
            var reader = new FakeConsoleReader(new[] { "UserInput" });
            var writer = new FakeConsoleWriter();
            var options = new PromptOptions { NonInteractive = false };
            var prompt = new StringPrompt("Name", writer, reader, options, @default: "DefaultValue");

            var result = prompt.GetValue();

            Assert.Equal("UserInput", result);
            Assert.Contains("Name", writer.Output);
        }

        [Fact]
        public void StringPrompt_Interactive_UsesDefaultOnEmptyInput()
        {
            var reader = new FakeConsoleReader(new[] { "" });
            var writer = new FakeConsoleWriter();
            var options = new PromptOptions { NonInteractive = false };
            var prompt = new StringPrompt("Name", writer, reader, options, @default: "DefaultValue");

            var result = prompt.GetValue();

            Assert.Equal("DefaultValue", result);
        }
    }
}
