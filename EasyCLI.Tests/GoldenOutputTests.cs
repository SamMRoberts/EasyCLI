using System;
using System.IO;
using System.Threading.Tasks;
using EasyCLI.Console;
using EasyCLI.Shell;
using EasyCLI.Styling;
using Xunit;

namespace EasyCLI.Tests
{
    /// <summary>
    /// Golden/snapshot tests for help output in different formats (styled, plain, JSON).
    /// These tests ensure consistent UX and stable output contracts.
    /// </summary>
    public class GoldenOutputTests
    {
        /// <summary>
        /// Captures output from a shell command and returns it as a string.
        /// </summary>
        private static async Task<string> CaptureShellOutput(string command, bool enableColors = false, ConsoleTheme? theme = null)
        {
            var input = new StringReader($"{command}\nexit\n");
            var output = new StringWriter();
            var writer = new ConsoleWriter(enableColors: enableColors, output: output);
            
            var shell = new CliShell(new ConsoleReader(input), writer, new ShellOptions { Prompt = "test>" });

            await shell.RunAsync();
            return output.ToString();
        }

        /// <summary>
        /// Helper to normalize output for snapshot comparison by removing timestamps and dynamic content.
        /// </summary>
        private static string NormalizeOutput(string output)
        {
            // Remove the prompt and exit-related output to focus on command output
            var lines = output.Split('\n');
            var result = string.Empty;
            bool captureStarted = false;
            
            foreach (var line in lines)
            {
                // Skip prompt lines and start capturing after the command
                if (line.Trim() == "test>exit" || line.Trim() == "test>")
                    continue;
                    
                if (line.Contains("test>"))
                {
                    captureStarted = true;
                    continue;
                }
                
                if (captureStarted)
                {
                    result += line + "\n";
                }
            }
            
            return result.Trim();
        }

        [Fact]
        public async Task Help_GeneralHelp_StyledOutput()
        {
            // Arrange & Act
            var output = await CaptureShellOutput("help", enableColors: false);
            var normalized = NormalizeOutput(output);

            // Assert - Check for key components of styled help
            Assert.Contains("SUPPORT:", normalized);
            Assert.Contains("Version:", normalized);
            Assert.Contains("Issues:  https://github.com/SamMRoberts/EasyCLI/issues", normalized);
            Assert.Contains("Docs:    https://github.com/SamMRoberts/EasyCLI", normalized);
            Assert.Contains("Use --plain or --json for stable output", normalized);
            Assert.Contains("https://github.com/SamMRoberts/EasyCLI/blob/main/docs/output-contract.md", normalized);
            
            // Verify command categories are present
            Assert.Contains("Core:", normalized);
            Assert.Contains("help", normalized);
            Assert.Contains("Show help or detailed help for a command", normalized);
        }

        [Fact]
        public async Task Help_EchoCommand_StyledOutput()
        {
            // Arrange & Act
            var output = await CaptureShellOutput("echo --help", enableColors: false);
            var normalized = NormalizeOutput(output);

            // Assert - Check for echo command help structure
            Assert.Contains("USAGE:", normalized);
            Assert.Contains("echo [options] <text...>", normalized);
            Assert.Contains("DESCRIPTION:", normalized);
            Assert.Contains("ARGUMENTS:", normalized);
            Assert.Contains("OPTIONS:", normalized);
            Assert.Contains("EXAMPLES:", normalized);
            Assert.Contains("SUPPORT:", normalized);
            
            // Verify specific echo options
            Assert.Contains("--help", normalized);
            Assert.Contains("--verbose", normalized);
            Assert.Contains("--dry-run", normalized);
            Assert.Contains("Show what would be done without executing", normalized);
        }

        [Fact]
        public async Task Help_ConfigCommand_StyledOutput()
        {
            // Arrange & Act
            var output = await CaptureShellOutput("config --help", enableColors: false);
            var normalized = NormalizeOutput(output);

            // Assert - Check for config command help structure
            Assert.Contains("config", normalized);
            Assert.Contains("USAGE:", normalized);
            Assert.Contains("DESCRIPTION:", normalized);
            Assert.Contains("SUPPORT:", normalized);
            
            // Config should have help footer
            Assert.Contains("Version:", normalized);
            Assert.Contains("Issues:  https://github.com/SamMRoberts/EasyCLI/issues", normalized);
        }

        [Fact]
        public async Task Help_GeneralHelp_PlainOutput()
        {
            // Note: This test verifies plain output format (colors disabled)
            // In a full implementation, this would test --plain flag behavior
            
            var output = await CaptureShellOutput("help", enableColors: false);
            var normalized = NormalizeOutput(output);

            // Assert - Verify plain output characteristics (no ANSI escape sequences)
            Assert.DoesNotContain("\u001b[", normalized); // No ANSI escape sequences
            Assert.Contains("Core:", normalized);
            Assert.Contains("SUPPORT:", normalized);
            
            // Should still contain all essential information
            Assert.Contains("Use --plain or --json for stable output", normalized);
        }

        [Fact]
        public async Task Help_EchoCommand_PlainOutput()
        {
            // Test echo help in plain format (no colors)
            var output = await CaptureShellOutput("echo --help", enableColors: false);
            var normalized = NormalizeOutput(output);

            // Assert - Verify no ANSI codes and all content present
            Assert.DoesNotContain("\u001b[", normalized); // No ANSI escape sequences (real ANSI codes start with ESC)
            Assert.Contains("USAGE:", normalized);
            Assert.Contains("OPTIONS:", normalized);
            Assert.Contains("EXAMPLES:", normalized);
        }

        [Fact]
        public async Task Help_Footer_ConsistentAcrossCommands()
        {
            // Test that help footer is consistent across different commands
            var helpOutput = await CaptureShellOutput("help", enableColors: false);
            var echoHelpOutput = await CaptureShellOutput("echo --help", enableColors: false);
            var configHelpOutput = await CaptureShellOutput("config --help", enableColors: false);

            var helpNormalized = NormalizeOutput(helpOutput);
            var echoNormalized = NormalizeOutput(echoHelpOutput);
            var configNormalized = NormalizeOutput(configHelpOutput);

            // Assert - All should contain the same footer elements
            string[] footerElements = {
                "SUPPORT:",
                "Version:",
                "Issues:  https://github.com/SamMRoberts/EasyCLI/issues",
                "Docs:    https://github.com/SamMRoberts/EasyCLI",
            };

            foreach (var element in footerElements)
            {
                Assert.Contains(element, helpNormalized);
                Assert.Contains(element, echoNormalized);
                Assert.Contains(element, configNormalized);
            }
        }

        [Fact]
        public async Task Help_OutputContract_ReferencesPresent()
        {
            // Test that output contract references are present in help
            var output = await CaptureShellOutput("help", enableColors: false);
            var normalized = NormalizeOutput(output);

            // Assert - Verify automation guidance is present
            Assert.Contains("Use --plain or --json for stable output", normalized);
            Assert.Contains("https://github.com/SamMRoberts/EasyCLI/blob/main/docs/output-contract.md", normalized);
            
            // Should mention scripting considerations
            var lowerOutput = normalized.ToLowerInvariant();
            Assert.True(
                lowerOutput.Contains("automation") || 
                lowerOutput.Contains("script") || 
                lowerOutput.Contains("stable"),
                "Help should contain automation/scripting guidance"
            );
        }

        [Fact]
        public async Task Echo_ConciseHelp_WhenNoArguments()
        {
            // Test that echo shows concise help when run without arguments
            var output = await CaptureShellOutput("echo", enableColors: false);
            var normalized = NormalizeOutput(output);

            // Assert - Should show concise help, not error
            Assert.DoesNotContain("Error: No text specified to echo", normalized);
            Assert.Contains("USAGE:", normalized);
            Assert.Contains("echo [options] <text...>", normalized);
            Assert.Contains("EXAMPLES:", normalized);
            Assert.Contains("For more information, run: echo --help", normalized);
        }

        [Fact]
        public async Task Help_StyledOutput_WithColors()
        {
            // Test that styled output includes ANSI escape sequences when colors are enabled
            var output = await CaptureShellOutput("help", enableColors: true);
            var normalized = NormalizeOutput(output);

            // Assert - Should contain ANSI escape sequences for styling
            Assert.Contains("\u001b[", normalized); // Contains ANSI escape sequences
            Assert.Contains("SUPPORT:", normalized);
            Assert.Contains("Core:", normalized);
        }

        [Fact]
        public async Task Help_CommandStructure_Consistent()
        {
            // Test that command help structure is consistent across different commands
            var echoHelp = await CaptureShellOutput("echo --help", enableColors: false);
            var configHelp = await CaptureShellOutput("config --help", enableColors: false);

            var echoNormalized = NormalizeOutput(echoHelp);
            var configNormalized = NormalizeOutput(configHelp);

            // Assert - Both should have consistent structure
            string[] expectedSections = {
                "USAGE:",
                "DESCRIPTION:",
                "SUPPORT:",
            };

            foreach (var section in expectedSections)
            {
                Assert.Contains(section, echoNormalized);
                Assert.Contains(section, configNormalized);
            }

            // Both should reference automation and scripting
            Assert.Contains("AUTOMATION & SCRIPTING:", echoNormalized);
            Assert.Contains("AUTOMATION & SCRIPTING:", configNormalized);
        }

        [Fact]
        public async Task Help_ScriptingGuidance_Present()
        {
            // Test that all help outputs include scripting guidance
            var commands = new[] { "help", "echo --help", "config --help" };

            foreach (var command in commands)
            {
                var output = await CaptureShellOutput(command, enableColors: false);
                var normalized = NormalizeOutput(output);

                // Assert - Should contain scripting guidance
                Assert.Contains("--plain or --json", normalized);
                Assert.Contains("scripts", normalized);
                Assert.Contains("exit codes", normalized);
                Assert.Contains("JSON format guaranteed stable", normalized);
            }
        }

        [Fact]
        public async Task Help_VersionInfo_Consistent()
        {
            // Test that version information is consistently formatted
            var helpOutput = await CaptureShellOutput("help", enableColors: false);
            var echoHelpOutput = await CaptureShellOutput("echo --help", enableColors: false);

            var helpNormalized = NormalizeOutput(helpOutput);
            var echoNormalized = NormalizeOutput(echoHelpOutput);

            // Assert - Version format should be consistent
            Assert.Contains("Version: 1.0.0.0", helpNormalized);
            Assert.Contains("Version: 1.0.0.0", echoNormalized);

            // GitHub URLs should be consistent
            Assert.Contains("https://github.com/SamMRoberts/EasyCLI/issues", helpNormalized);
            Assert.Contains("https://github.com/SamMRoberts/EasyCLI/issues", echoNormalized);
        }
    }
}