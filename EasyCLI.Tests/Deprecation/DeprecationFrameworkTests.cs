using EasyCLI.Deprecation;
using EasyCLI.Extensions;
using EasyCLI.Console;
using Xunit;

namespace EasyCLI.Tests.Deprecation
{
    public class DeprecationFrameworkTests
    {
        [Fact]
        public void DeprecationInfo_Replaced_CreatesCorrectInfo()
        {
            // Arrange
            var alternative = "--new-option";
            var version = new DeprecationVersion("2.0.0");

            // Act
            var info = DeprecationInfo.Replaced(alternative, version);

            // Assert
            Assert.Equal(DeprecationReason.Replaced, info.Reason);
            Assert.Equal("replaced by a better alternative", info.Message);
            Assert.Equal(alternative, info.Alternative);
            Assert.Equal(version, info.RemovalVersion);
            Assert.True(info.HasRemovalVersion);
        }

        [Fact]
        public void DeprecationInfo_Obsolete_CreatesCorrectInfo()
        {
            // Arrange
            var version = new DeprecationVersion("3.0.0");
            var reason = "no longer supported";

            // Act
            var info = DeprecationInfo.Obsolete(version, reason);

            // Assert
            Assert.Equal(DeprecationReason.Obsolete, info.Reason);
            Assert.Equal(reason, info.Message);
            Assert.Equal(version, info.RemovalVersion);
            Assert.True(info.HasRemovalVersion);
        }

        [Fact]
        public void DeprecationInfo_Moved_CreatesCorrectInfo()
        {
            // Arrange
            var newLocation = "new-command";
            var version = new DeprecationVersion("2.5.0");

            // Act
            var info = DeprecationInfo.Moved(newLocation, version);

            // Assert
            Assert.Equal(DeprecationReason.Moved, info.Reason);
            Assert.Equal("moved to a new location", info.Message);
            Assert.Equal(newLocation, info.Alternative);
            Assert.Equal(version, info.RemovalVersion);
            Assert.True(info.HasRemovalVersion);
        }

        [Fact]
        public void DeprecationInfo_GetFormattedMessage_WithAllParts_FormatsCorrectly()
        {
            // Arrange
            var info = new DeprecationInfo(
                DeprecationReason.Replaced,
                "use new API",
                new DeprecationVersion("2.0.0"),
                "--new-flag",
                "https://example.com/migration");

            // Act
            var message = info.GetFormattedMessage("--old-flag");

            // Assert
            Assert.Equal(
                "'--old-flag' is deprecated (use new API) and will be removed in version 2.0.0. Use '--new-flag' instead. More info: https://example.com/migration",
                message);
        }

        [Fact]
        public void DeprecationInfo_GetFormattedMessage_WithMinimalInfo_FormatsCorrectly()
        {
            // Arrange
            var info = new DeprecationInfo(DeprecationReason.Obsolete, "no longer needed");

            // Act
            var message = info.GetFormattedMessage("old-command");

            // Assert
            Assert.Equal("'old-command' is deprecated (no longer needed).", message);
        }

        [Fact]
        public void DeprecationVersion_NextMajor_CalculatesCorrectly()
        {
            // Act
            var nextMajor = DeprecationVersion.NextMajor("1.5.3");

            // Assert
            Assert.Equal("2.0.0", nextMajor.Version);
            Assert.True(nextMajor.IsMajorVersion);
        }

        [Fact]
        public void DeprecationVersion_NextMinor_CalculatesCorrectly()
        {
            // Act
            var nextMinor = DeprecationVersion.NextMinor("1.5.3");

            // Assert
            Assert.Equal("1.6.0", nextMinor.Version);
        }

        [Fact]
        public void DeprecationExtensions_WriteDeprecationWarning_WritesCorrectFormat()
        {
            // Arrange
            using var output = new StringWriter();
            var writer = new ConsoleWriter(enableColors: false, output: output);
            var info = DeprecationInfo.Replaced("--new-option");

            // Act
            writer.WriteDeprecationWarning("--old-option", info);

            // Assert
            var result = output.ToString();
            Assert.Contains("⚠ Deprecation Warning:", result);
            Assert.Contains("'--old-option' is deprecated", result);
        }
    }
}