using System.Runtime.InteropServices;
using System.Security;
using EasyCLI.Console;
using EasyCLI.Prompts;
using EasyCLI.Styling;

namespace EasyCLI.Tests
{
    public class SecureStringPromptTests
    {
        private sealed class CapturingHidden : IHiddenInputSource
        {
            private readonly string _value;
            public char? LastMask { get; private set; }
            public CapturingHidden(string value) => _value = value;
            public string ReadHidden(char? mask = null)
            {
                LastMask = mask;
                return _value;
            }
        }

        private sealed class DummyWriter : IConsoleWriter
        {
            public void Write(string message) { }
            public void WriteLine(string message) { }
            public void Write(string message, ConsoleStyle style) { }
            public void WriteLine(string message, ConsoleStyle style) { }
        }

        private sealed class DummyReader : IConsoleReader
        {
            public string ReadLine() => string.Empty; // not used for hidden input path
        }

        private static string SecureStringToString(SecureString secureString)
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(ptr) ?? string.Empty;
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(ptr);
                }
            }
        }

        [Fact]
        public void SecureStringPrompt_Passes_Default_Mask()
        {
            var hidden = new CapturingHidden("secret");
            var prompt = new SecureStringPrompt("Password", new DummyWriter(), new DummyReader(), hiddenSource: hidden, mask: '*');
            var result = prompt.GetValue();
            
            Assert.Equal("secret", SecureStringToString(result));
            Assert.Equal('*', hidden.LastMask);
            result.Dispose();
        }

        [Fact]
        public void SecureStringPrompt_Passes_Custom_Mask()
        {
            var hidden = new CapturingHidden("token");
            var prompt = new SecureStringPrompt("Token", new DummyWriter(), new DummyReader(), hiddenSource: hidden, mask: '#');
            var result = prompt.GetValue();
            
            Assert.Equal("token", SecureStringToString(result));
            Assert.Equal('#', hidden.LastMask);
            result.Dispose();
        }

        [Fact]
        public void SecureStringPrompt_Uses_Default_Value_When_Empty()
        {
            var defaultSecure = new SecureString();
            foreach (char c in "fallback")
            {
                defaultSecure.AppendChar(c);
            }
            defaultSecure.MakeReadOnly();

            var hidden = new CapturingHidden("");
            var prompt = new SecureStringPrompt("Password", new DummyWriter(), new DummyReader(), hiddenSource: hidden, @default: defaultSecure, mask: '*');
            var result = prompt.GetValue();
            
            Assert.Equal("fallback", SecureStringToString(result));
            Assert.Equal('*', hidden.LastMask);
            result.Dispose();
            defaultSecure.Dispose();
        }

        private sealed class EscHidden : IHiddenInputSource
        {
            public string ReadHidden(char? mask = null) => "\u001b"; // simulate ESC
        }

        [Fact]
        public void SecureStringPrompt_Cancel_ReturnsDefault()
        {
            var defaultSecure = new SecureString();
            foreach (char c in "fallback")
            {
                defaultSecure.AppendChar(c);
            }
            defaultSecure.MakeReadOnly();

            var prompt = new SecureStringPrompt("Password", new DummyWriter(), new DummyReader(), hiddenSource: new EscHidden(), @default: defaultSecure, mask: '*');
            var result = prompt.GetValue();
            
            Assert.Equal("fallback", SecureStringToString(result));
            result.Dispose();
            defaultSecure.Dispose();
        }

        [Fact]
        public void SecureStringPrompt_EmptyInput_ReturnsEmptySecureString()
        {
            var hidden = new CapturingHidden("");
            var prompt = new SecureStringPrompt("Password", new DummyWriter(), new DummyReader(), hiddenSource: hidden, @default: null, mask: '*');
            var result = prompt.GetValue();
            
            Assert.Equal("", SecureStringToString(result));
            result.Dispose();
        }

        [Fact]
        public void SecureStringPrompt_NonInteractiveMode_WithDefault_ReturnsDefault()
        {
            var defaultSecure = new SecureString();
            foreach (char c in "default123")
            {
                defaultSecure.AppendChar(c);
            }
            defaultSecure.MakeReadOnly();

            var options = new PromptOptions { NonInteractive = true };
            var prompt = new SecureStringPrompt("Password", new DummyWriter(), new DummyReader(), hiddenSource: null, options: options, @default: defaultSecure);
            var result = prompt.GetValue();
            
            Assert.Equal("default123", SecureStringToString(result));
            result.Dispose();
            defaultSecure.Dispose();
        }

        [Fact]
        public void SecureStringPrompt_NonInteractiveMode_WithoutDefault_ThrowsException()
        {
            var options = new PromptOptions { NonInteractive = true };
            var prompt = new SecureStringPrompt("Password", new DummyWriter(), new DummyReader(), hiddenSource: null, options: options, @default: null);
            
            var exception = Assert.Throws<InvalidOperationException>(() => prompt.GetValue());
            Assert.Contains("Cannot prompt for 'Password' in non-interactive mode", exception.Message);
        }

        [Fact]
        public void SecureStringPrompt_HandlesSpecialCharacters()
        {
            const string specialInput = "p@ssw0rd!$";
            var hidden = new CapturingHidden(specialInput);
            var prompt = new SecureStringPrompt("Password", new DummyWriter(), new DummyReader(), hiddenSource: hidden, mask: '*');
            var result = prompt.GetValue();
            
            Assert.Equal(specialInput, SecureStringToString(result));
            result.Dispose();
        }
    }
}