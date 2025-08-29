using EasyCLI.Configuration;
using EasyCLI.Shell;
using EasyCLI.Telemetry;

namespace EasyCLI.Tests
{
    /// <summary>
    /// Tests for telemetry functionality and privacy features.
    /// </summary>
    public class TelemetryTests
    {
        [Fact]
        public void NoOpTelemetryService_IsDisabledByDefault()
        {
            var service = new NoOpTelemetryService();

            Assert.False(service.IsEnabled);
            Assert.False(service.HasUserConsent);
        }

        [Fact]
        public void NoOpTelemetryService_PerformsNoOperations()
        {
            var service = new NoOpTelemetryService();

            // These should not throw or cause any side effects
            service.TrackCommand("test", true);
            service.TrackError("error");
            var flushTask = service.FlushAsync();

            Assert.True(flushTask.IsCompleted);
            Assert.Equal(TaskStatus.RanToCompletion, flushTask.Status);
        }

        [Fact]
        public void AppConfig_TelemetryConsentIsFalseByDefault()
        {
            var config = new AppConfig();

            Assert.False(config.TelemetryConsent);
        }

        [Fact]
        public void AppConfig_TelemetryConsentCanBeSet()
        {
            var config = new AppConfig
            {
                TelemetryConsent = true,
            };

            Assert.True(config.TelemetryConsent);
        }

        [Fact]
        public void CommandLineArgs_ParsesTelemetryConsentFlag()
        {
            var args = new CommandLineArgs(new[] { "--telemetry-consent" });

            Assert.True(args.IsTelemetryConsent);
        }

        [Fact]
        public void CommandLineArgs_TelemetryConsentIsFalseByDefault()
        {
            var args = new CommandLineArgs(Array.Empty<string>());

            Assert.False(args.IsTelemetryConsent);
        }

        [Fact]
        public void PrivacyNotices_HasNonEmptyDisclaimer()
        {
            Assert.False(string.IsNullOrWhiteSpace(PrivacyNotices.TelemetryDisclaimer));
            Assert.Contains("does not collect", PrivacyNotices.TelemetryDisclaimer);
            Assert.Contains("disabled unless you explicitly opt-in", PrivacyNotices.TelemetryDisclaimer);
        }

        [Fact]
        public void PrivacyNotices_DefinesWhatDataIsCollected()
        {
            var data = PrivacyNotices.WhatDataIsCollected;

            Assert.NotEmpty(data);
            Assert.Contains(data, item => item.Contains("Command names"));
            Assert.Contains(data, item => item.Contains("Error types"));
        }

        [Fact]
        public void PrivacyNotices_DefinesWhatDataIsNotCollected()
        {
            var data = PrivacyNotices.WhatDataIsNotCollected;

            Assert.NotEmpty(data);
            Assert.Contains(data, item => item.Contains("File names"));
            Assert.Contains(data, item => item.Contains("Personal information"));
        }

        [Fact]
        public void PrivacyNotices_GetTelemetryStatus_NoConsent()
        {
            var status = PrivacyNotices.GetTelemetryStatus(false, false);

            Assert.Contains("DISABLED", status);
            Assert.Contains("no consent", status);
            Assert.Contains("privacy protected", status);
        }

        [Fact]
        public void PrivacyNotices_GetTelemetryStatus_ConsentButDisabled()
        {
            var status = PrivacyNotices.GetTelemetryStatus(false, true);

            Assert.Contains("DISABLED", status);
            Assert.Contains("consent provided", status);
            Assert.Contains("service inactive", status);
        }

        [Fact]
        public void PrivacyNotices_GetTelemetryStatus_ConsentAndEnabled()
        {
            var status = PrivacyNotices.GetTelemetryStatus(true, true);

            Assert.Contains("ENABLED", status);
            Assert.Contains("collecting anonymous usage data", status);
        }

        [Fact]
        public void PrivacyNotices_DisplayPrivacyNotice_DoesNotThrow()
        {
            using var output = new StringWriter();
            var writer = new EasyCLI.Console.ConsoleWriter(enableColors: false, output);
            var theme = EasyCLI.Styling.ConsoleThemes.Dark;

            // Should not throw
            PrivacyNotices.DisplayPrivacyNotice(writer, theme);

            var result = output.ToString();
            Assert.Contains("Privacy Notice", result);
            Assert.Contains("does not collect", result);
        }

        [Fact]
        public void PrivacyNotices_DisplayPrivacyNotice_ThrowsOnNullWriter()
        {
            var theme = EasyCLI.Styling.ConsoleThemes.Dark;

            Assert.Throws<ArgumentNullException>(() =>
                PrivacyNotices.DisplayPrivacyNotice(null!, theme));
        }

        [Fact]
        public void PrivacyNotices_DisplayPrivacyNotice_ThrowsOnNullTheme()
        {
            using var output = new StringWriter();
            var writer = new EasyCLI.Console.ConsoleWriter(enableColors: false, output);

            Assert.Throws<ArgumentNullException>(() =>
                PrivacyNotices.DisplayPrivacyNotice(writer, null!));
        }

        [Fact]
        public void ConfigurationSource_TelemetryConsentSourceIsDefault()
        {
            var source = new ConfigurationSource();

            Assert.Equal("default", source.TelemetryConsentSource);
        }
    }
}