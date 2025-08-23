using EasyCLI.Prompts;
using EasyCLI.Prompts.Validators;
using EasyCLI.Tests.Fakes;
using Xunit;

namespace EasyCLI.Tests
{
    public class ValidatorTests
    {
        [Fact]
        public void IntRangeValidator_EnforcesBounds()
        {
            var reader = new FakeConsoleReader(new[] { "999", "50" });
            var writer = new FakeConsoleWriter();
            var v = new IntRangeValidator(1, 100);
            var prompt = new IntPrompt("Percent", writer, reader, validator: v);
            var val = prompt.GetValue();
            Assert.Equal(50, val);
            Assert.Contains("between 1 and 100", writer.Output);
        }

        [Fact]
        public void RegexValidator_RejectsInvalid()
        {
            var reader = new FakeConsoleReader(new[] { "bad email", "user@example.com" });
            var writer = new FakeConsoleWriter();
            var rv = new RegexValidator(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", "Invalid email");
            var prompt = new StringPrompt("Email", writer, reader, validator: rv);
            var val = prompt.GetValue();
            Assert.Equal("user@example.com", val);
            Assert.Contains("Invalid email", writer.Output);
        }

        [Fact]
        public void NonEmptyValidator_DemandsInput()
        {
            var reader = new FakeConsoleReader(new[] { "", " ", "value" });
            var writer = new FakeConsoleWriter();
            var nv = new NonEmptyValidator();
            var prompt = new StringPrompt("Name", writer, reader, validator: nv);
            var val = prompt.GetValue();
            Assert.Equal("value", val);
            Assert.Contains("cannot be empty", writer.Output);
        }

        [Fact]
        public void PredicateValidator_CustomLogic()
        {
            var reader = new FakeConsoleReader(new[] { "abc", "123" });
            var writer = new FakeConsoleWriter();
            var pv = new PredicateValidator<int>(raw => int.TryParse(raw, out var n) && n % 3 == 0 ? (true, n, null) : (false, 0, "Must be divisible by 3"));
            var prompt = new IntPrompt("Multiple of 3", writer, reader, validator: pv);
            var val = prompt.GetValue();
            Assert.Equal(123, val);
            Assert.Contains("divisible by 3", writer.Output);
        }
    }
}
