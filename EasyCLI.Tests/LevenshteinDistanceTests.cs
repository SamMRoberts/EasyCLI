using EasyCLI.Shell.Utilities;
using Xunit;

namespace EasyCLI.Tests
{
    public class LevenshteinDistanceTests
    {
        [Theory]
        [InlineData("", "", 0)]
        [InlineData("hello", "", 5)]
        [InlineData("", "world", 5)]
        [InlineData("cat", "cat", 0)]
        [InlineData("cat", "bat", 1)]
        [InlineData("cat", "cut", 1)]
        [InlineData("cat", "car", 1)]
        [InlineData("kitten", "sitting", 3)]
        [InlineData("saturday", "sunday", 3)]
        [InlineData("abc", "def", 3)]
        public void Calculate_ReturnsCorrectDistance(string source, string target, int expectedDistance)
        {
            // Act
            int actualDistance = LevenshteinDistance.Calculate(source, target);

            // Assert
            Assert.Equal(expectedDistance, actualDistance);
        }

        [Theory]
        [InlineData("hep", new[] { "help", "history", "pwd", "cd" }, "help")]
        [InlineData("histroy", new[] { "help", "history", "pwd", "cd" }, "history")]
        [InlineData("cler", new[] { "clear", "complete", "config" }, "clear")]
        [InlineData("conig", new[] { "clear", "complete", "config" }, "config")]
        [InlineData("abcdefgh", new[] { "help", "history", "pwd" }, null)] // No close match - distance too high
        public void FindBestMatch_ReturnsExpectedMatch(string input, string[] candidates, string? expectedMatch)
        {
            // Act
            string? actualMatch = LevenshteinDistance.FindBestMatch(input, candidates);

            // Assert
            Assert.Equal(expectedMatch, actualMatch);
        }

        [Fact]
        public void FindBestMatch_WithNullOrEmptyInput_ReturnsNull()
        {
            // Arrange
            string[] candidates = new[] { "help", "history" };

            // Act & Assert
            Assert.Null(LevenshteinDistance.FindBestMatch(null!, candidates));
            Assert.Null(LevenshteinDistance.FindBestMatch("", candidates));
        }

        [Fact]
        public void FindBestMatch_WithNullCandidates_ReturnsNull()
        {
            // Act
            string? result = LevenshteinDistance.FindBestMatch("test", null!);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void FindMultipleMatches_ReturnsOrderedMatches()
        {
            // Arrange
            string input = "hep";
            string[] candidates = ["help", "heap", "hope", "history", "pwd"];

            // Act
            var matches = LevenshteinDistance.FindMultipleMatches(input, candidates, maxDistance: 2).ToList();

            // Assert
            Assert.Contains("help", matches); // Distance 1
            Assert.Contains("heap", matches); // Distance 1
            Assert.DoesNotContain("history", matches); // Distance > 2
            Assert.DoesNotContain("pwd", matches); // Distance > 2
        }

        [Theory]
        [InlineData("--hlep", new[] { "--help", "--verbose", "--quiet" }, "--help")]
        [InlineData("--verbos", new[] { "--help", "--verbose", "--quiet" }, "--verbose")]
        [InlineData("-h", new[] { "--help", "--verbose", "--quiet" }, null)] // Exact match not found, distance too high
        public void FindBestMatch_WithCommandLineOptions_ReturnsExpectedMatch(string input, string[] options, string? expectedMatch)
        {
            // Act
            string? actualMatch = LevenshteinDistance.FindBestMatch(input, options);

            // Assert
            Assert.Equal(expectedMatch, actualMatch);
        }

        [Fact]
        public void FindBestMatch_RespectsMaxDistance()
        {
            // Arrange
            string input = "abcdef";
            string[] candidates = ["xyz", "123"];
            int maxDistance = 2;

            // Act
            string? result = LevenshteinDistance.FindBestMatch(input, candidates, maxDistance);

            // Assert
            Assert.Null(result); // All candidates should be beyond max distance
        }

        [Fact]
        public void FindMultipleMatches_RespectsMaxResults()
        {
            // Arrange
            string input = "test";
            string[] candidates = ["test1", "test2", "test3", "test4", "test5"];
            int maxResults = 3;

            // Act
            var matches = LevenshteinDistance.FindMultipleMatches(input, candidates, maxResults: maxResults).ToList();

            // Assert
            Assert.True(matches.Count <= maxResults);
        }
    }
}

