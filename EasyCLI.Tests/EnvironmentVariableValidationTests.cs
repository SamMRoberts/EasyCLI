using EasyCLI.Configuration;
using EasyCLI.Console;
using EasyCLI.Tests.Fakes;
using System.Text;

namespace EasyCLI.Tests
{
    /// <summary>
    /// Tests for environment variable validation and warnings.
    /// </summary>
    public class EnvironmentVariableValidationTests
    {
        [Fact]
        public async Task ConfigManager_WarnsAboutUnknownEasyCLIVariable()
        {
            using var scope = new EnvVarScope(("EASYCLI_UNKNOWN_SETTING", "test_value"));
            
            var writer = new FakeConsoleWriter();
            var manager = new ConfigManager("validation-test");
            
            // Load config which should trigger validation
            var config = await manager.LoadConfigAsync<AppConfig>(writer);
            
            // Check that a warning was written
            string outputText = writer.Output;
            Assert.Contains("Unknown environment variable 'EASYCLI_UNKNOWN_SETTING'", outputText);
            Assert.Contains("docs/env-vars.md", outputText);
        }

        [Fact]
        public async Task ConfigManager_SuggestsClosestMatchForTypo()
        {
            using var scope = new EnvVarScope(("EASYCLI_API_UL", "https://example.com"));
            
            var writer = new FakeConsoleWriter();
            var manager = new ConfigManager("typo-test");
            
            var config = await manager.LoadConfigAsync<AppConfig>(writer);
            
            string outputText = writer.Output;
            Assert.Contains("Unknown environment variable 'EASYCLI_API_UL'", outputText);
            Assert.Contains("Did you mean 'EASYCLI_API_URL'?", outputText);
        }

        [Fact]
        public async Task ConfigManager_NoWarningsForValidEasyCLIVariables()
        {
            using var scope = new EnvVarScope(
                ("EASYCLI_API_URL", "https://valid.example.com"),
                ("EASYCLI_TIMEOUT", "60"),
                ("EASYCLI_ENABLE_LOGGING", "false"));
            
            var writer = new FakeConsoleWriter();
            var manager = new ConfigManager("valid-test");
            
            var config = await manager.LoadConfigAsync<AppConfig>(writer);
            
            string outputText = writer.Output;
            Assert.DoesNotContain("Unknown environment variable", outputText);
            
            // Verify the values were actually applied
            Assert.Equal("https://valid.example.com", config.ApiUrl);
            Assert.Equal(60, config.Timeout);
            Assert.Equal(false, config.EnableLogging);
        }

        [Fact]
        public async Task ConfigManager_IgnoresNonEasyCLIVariables()
        {
            using var scope = new EnvVarScope(
                ("SOME_OTHER_VAR", "value"),
                ("PATH", "/usr/bin"),
                ("HOME", "/home/user"));
            
            var writer = new FakeConsoleWriter();
            var manager = new ConfigManager("ignore-test");
            
            var config = await manager.LoadConfigAsync<AppConfig>(writer);
            
            string outputText = writer.Output;
            Assert.DoesNotContain("Unknown environment variable", outputText);
        }

        [Fact]
        public async Task ConfigManager_NoWarningsWhenWriterIsNull()
        {
            using var scope = new EnvVarScope(("EASYCLI_INVALID_VAR", "test_value"));
            
            var manager = new ConfigManager("null-writer-test");
            
            // Should not throw or cause issues when writer is null
            var config = await manager.LoadConfigAsync<AppConfig>(writer: null);
            
            // Should still work and apply valid variables
            Assert.NotNull(config);
        }

        [Fact]
        public async Task ConfigManager_ValidatesAllKnownEasyCLIVariables()
        {
            using var scope = new EnvVarScope(
                ("EASYCLI_API_URL", "https://test.example.com"),
                ("EASYCLI_TIMEOUT", "45"),
                ("EASYCLI_ENABLE_LOGGING", "true"),
                ("EASYCLI_LOG_LEVEL", "Debug"),
                ("EASYCLI_OUTPUT_FORMAT", "json"),
                ("EASYCLI_USE_COLORS", "false"));
            
            var writer = new FakeConsoleWriter();
            var manager = new ConfigManager("all-vars-test");
            
            var config = await manager.LoadConfigAsync<AppConfig>(writer);
            
            string outputText = writer.Output;
            Assert.DoesNotContain("Unknown environment variable", outputText);
            
            // Verify all values were applied correctly
            Assert.Equal("https://test.example.com", config.ApiUrl);
            Assert.Equal(45, config.Timeout);
            Assert.Equal(true, config.EnableLogging);
            Assert.Equal("Debug", config.LogLevel);
            Assert.Equal("json", config.OutputFormat);
            Assert.Equal(false, config.UseColors);
        }

        [Fact]
        public async Task ConfigManager_EnvironmentVariablePrecedence()
        {
            using var scope = new EnvVarScope(
                ("EASYCLI_API_URL", "https://env.example.com"),
                ("EASYCLI_TIMEOUT", "90"));
            
            // Create a temporary config file with different values
            string tempDir = Path.GetTempPath();
            string configDir = Path.Combine(tempDir, $"easycli-precedence-test-{Guid.NewGuid()}");
            Directory.CreateDirectory(configDir);
            
            try
            {
                // Create a local config file
                string localConfigPath = Path.Combine(Directory.GetCurrentDirectory(), ".easycli-precedence-test.json");
                await File.WriteAllTextAsync(localConfigPath, @"{
                    ""api_url"": ""https://file.example.com"",
                    ""timeout"": 30,
                    ""enable_logging"": true
                }");
                
                var manager = new ConfigManager("easycli-precedence-test");
                var config = await manager.LoadConfigAsync<AppConfig>();
                
                // Environment variables should override file values
                Assert.Equal("https://env.example.com", config.ApiUrl);
                Assert.Equal(90, config.Timeout);
                Assert.Equal(true, config.EnableLogging);
                
                // Clean up local config file
                if (File.Exists(localConfigPath))
                {
                    File.Delete(localConfigPath);
                }
            }
            finally
            {
                if (Directory.Exists(configDir))
                {
                    Directory.Delete(configDir, true);
                }
            }
        }
    }
}