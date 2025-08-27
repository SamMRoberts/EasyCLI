namespace EasyCLI.Tests
{
    /// <summary>
    /// Tests for Phase 2 CLI enhancement features.
    /// </summary>
    public class Phase2EnhancementTests
    {
        [Fact]
        public void AppConfig_HasDefaultValues()
        {
            var config = new AppConfig();

            Assert.Equal("https://api.example.com", config.ApiUrl);
            Assert.Equal(30, config.Timeout);
            Assert.True(config.EnableLogging);
            Assert.Equal("Info", config.LogLevel);
            Assert.Equal("console", config.OutputFormat);
            Assert.True(config.UseColors);
            Assert.NotNull(config.Source);
        }

        [Fact]
        public void ConfigurationSource_HasDefaultSources()
        {
            var source = new ConfigurationSource();

            Assert.Equal("default", source.ApiUrlSource);
            Assert.Equal("default", source.TimeoutSource);
            Assert.Equal("default", source.EnableLoggingSource);
            Assert.Equal("default", source.LogLevelSource);
            Assert.Equal("default", source.OutputFormatSource);
            Assert.Equal("default", source.UseColorsSource);
        }

        [Fact]
        public void ConfigManager_InitializesWithCorrectPaths()
        {
            var manager = new ConfigManager("testapp");
            var (globalPath, localPath) = manager.GetConfigPaths();

            Assert.Contains(".testapp", globalPath);
            Assert.Contains("config.json", globalPath);
            Assert.Contains(".testapp.json", localPath);
        }

        [Fact]
        public void EnvironmentDetector_DetectsBasicEnvironment()
        {
            var info = EnvironmentDetector.DetectEnvironment();

            Assert.NotNull(info);
            Assert.False(string.IsNullOrEmpty(info.Platform));
            Assert.NotNull(info.Metadata);
            Assert.True(info.Metadata.ContainsKey("WorkingDirectory"));
            Assert.True(info.Metadata.ContainsKey("UserName"));
            Assert.True(info.Metadata.ContainsKey("MachineName"));
        }

        [Fact]
        public void Logger_DeterminesCorrectLevel_FromQuietFlag()
        {
            var args = new[] { "--quiet", "somecommand" };
            var level = Logger.DetermineLogLevel(args);

            Assert.Equal(LogLevel.Quiet, level);
        }

        [Fact]
        public void Logger_DeterminesCorrectLevel_FromVerboseFlag()
        {
            var args = new[] { "--verbose", "somecommand" };
            var level = Logger.DetermineLogLevel(args);

            Assert.Equal(LogLevel.Verbose, level);
        }

        [Fact]
        public void Logger_DeterminesCorrectLevel_FromDebugFlag()
        {
            // Save current CI environment variable
            var originalCI = System.Environment.GetEnvironmentVariable("CI");

            try
            {
                // Clear CI variable for this test
                System.Environment.SetEnvironmentVariable("CI", null);

                var args = new[] { "--debug", "somecommand" };
                var level = Logger.DetermineLogLevel(args);

                Assert.Equal(LogLevel.Debug, level);
            }
            finally
            {
                // Restore original CI environment variable
                System.Environment.SetEnvironmentVariable("CI", originalCI);
            }
        }

        [Fact]
        public void Logger_DeterminesCorrectLevel_Default()
        {
            // Save current CI environment variable
            var originalCI = System.Environment.GetEnvironmentVariable("CI");

            try
            {
                // Clear CI variable for this test
                System.Environment.SetEnvironmentVariable("CI", null);

                var args = new[] { "somecommand" };
                var level = Logger.DetermineLogLevel(args);

                Assert.Equal(LogLevel.Normal, level);
            }
            finally
            {
                // Restore original CI environment variable
                System.Environment.SetEnvironmentVariable("CI", originalCI);
            }
        }

        [Fact]
        public void Logger_HandlesShortFlags()
        {
            var quietArgs = new[] { "-q", "command" };
            var verboseArgs = new[] { "-v", "command" };

            Assert.Equal(LogLevel.Quiet, Logger.DetermineLogLevel(quietArgs));
            Assert.Equal(LogLevel.Verbose, Logger.DetermineLogLevel(verboseArgs));
        }

        [Fact]
        public async Task ConfigManager_HandlesNonExistentFiles()
        {
            var manager = new ConfigManager("nonexistent-test-app");
            var config = await manager.LoadConfigAsync<AppConfig>();

            Assert.NotNull(config);
            Assert.Equal("https://api.example.com", config.ApiUrl); // Default value
        }

        [Fact]
        public void EnvironmentInfo_InitializesCorrectly()
        {
            var info = new EnvironmentInfo();

            Assert.False(info.IsGitRepository);
            Assert.Null(info.GitBranch);
            Assert.False(info.IsDockerEnvironment);
            Assert.False(info.IsContinuousIntegration);
            Assert.Null(info.CiProvider);
            Assert.False(info.HasConfigFile);
            Assert.Null(info.ConfigFile);
            Assert.False(info.IsInteractive);
            Assert.Empty(info.Platform);
            Assert.NotNull(info.Metadata);
            Assert.Empty(info.Metadata);
        }

        [Theory]
        [InlineData("GITHUB_ACTIONS", "GitHub Actions")]
        [InlineData("GITLAB_CI", "GitLab CI")]
        [InlineData("JENKINS_URL", "Jenkins")]
        [InlineData("TRAVIS", "Travis CI")]
        [InlineData("CIRCLECI", "CircleCI")]
        [InlineData("APPVEYOR", "AppVeyor")]
        [InlineData("AZURE_PIPELINES", "Azure Pipelines")]
        public void EnvironmentDetector_DetectsCiProviders(string envVar, string expectedProvider)
        {
            // Arrange: clear all known CI indicators first to isolate the one under test
            string[] ciVars = new[] { "CI", "CONTINUOUS_INTEGRATION", "BUILD_NUMBER", "GITHUB_ACTIONS", "GITLAB_CI", "JENKINS_URL", "TRAVIS", "CIRCLECI", "APPVEYOR", "AZURE_PIPELINES" };
            (string key, string? value)[] resets = ciVars.Select(v => (v, (string?)null)).ToArray();

            using (new EnvVarScope(resets))
            using (new EnvVarScope((envVar, "1")))
            {
                var info = EnvironmentDetector.DetectEnvironment();
                Assert.True(info.IsContinuousIntegration);
                Assert.Equal(expectedProvider, info.CiProvider);
            }
        }
    }
}
