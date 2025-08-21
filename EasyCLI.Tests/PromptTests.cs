using EasyCLI.Prompts;
using EasyCLI.Tests.Fakes;
using Xunit;

namespace EasyCLI.Tests
{
    public class PromptTests
    {
        [Fact]
        public void StringPrompt_ReturnsTypedValue()
        {
            var reader = new FakeConsoleReader(new[] { "hello" });
            var writer = new FakeConsoleWriter();
            var p = new StringPrompt("Name", writer, reader);
            var val = p.Get();
            Assert.Equal("hello", val);
            Assert.Contains("Name", writer.Output);
        }

        [Fact]
        public void StringPrompt_UsesDefault_OnEmpty()
        {
            var reader = new FakeConsoleReader(new[] { "" });
            var writer = new FakeConsoleWriter();
            var p = new StringPrompt("Project", writer, reader, @default: "EasyCLI");
            var val = p.Get();
            Assert.Equal("EasyCLI", val);
            Assert.Contains("[EasyCLI]", writer.Output);
        }

        [Fact]
        public void IntPrompt_RePrompts_OnInvalid()
        {
            var reader = new FakeConsoleReader(new[] { "abc", "42" });
            var writer = new FakeConsoleWriter();
            var p = new IntPrompt("Age", writer, reader);
            var val = p.Get();
            Assert.Equal(42, val);
            // Should show an error line for invalid attempt
            Assert.Contains("Invalid value", writer.Output);
        }

        [Fact]
        public void YesNoPrompt_ParsesYes()
        {
            var reader = new FakeConsoleReader(new[] { "y" });
            var writer = new FakeConsoleWriter();
            var p = new YesNoPrompt("Proceed", writer, reader);
            var val = p.Get();
            Assert.True(val);
        }

        private sealed class RangeValidator : IPromptValidator<int>
        {
            private readonly int _min,_max;
            public RangeValidator(int min,int max){_min=min;_max=max;}
            public PromptValidationResult Validate(string raw, out int value)
            {
                value = default;
                if (!int.TryParse(raw, out var num))
                    return PromptValidationResult.Fail("Not a number");
                if (num < _min || num > _max)
                    return PromptValidationResult.Fail($"Value must be between {_min} and {_max}");
                value = num; return PromptValidationResult.Success();
            }
        }

        [Fact]
        public void IntPrompt_WithValidator_EnforcesRange()
        {
            var reader = new FakeConsoleReader(new[] { "999", "50" });
            var writer = new FakeConsoleWriter();
            var validator = new RangeValidator(1, 100);
            var p = new IntPrompt("Percent", writer, reader, validator: validator);
            var val = p.Get();
            Assert.Equal(50, val);
            Assert.Contains("between 1 and 100", writer.Output);
        }
    }
}
