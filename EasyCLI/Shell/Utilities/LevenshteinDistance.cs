namespace EasyCLI.Shell.Utilities
{
    /// <summary>
    /// Utility class for calculating edit distance between strings to enable fuzzy matching for command and option suggestions.
    /// </summary>
    public static class LevenshteinDistance
    {
        /// <summary>
        /// Calculates the Levenshtein distance between two strings.
        /// The Levenshtein distance is the minimum number of single-character edits (insertions, deletions, or substitutions)
        /// required to change one string into another.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="target">The target string.</param>
        /// <returns>The Levenshtein distance between the two strings.</returns>
        public static int Calculate(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target) ? 0 : target.Length;
            }

            if (string.IsNullOrEmpty(target))
            {
                return source.Length;
            }

            int sourceLength = source.Length;
            int targetLength = target.Length;

            // Create a matrix to store distances
            int[,] matrix = new int[sourceLength + 1, targetLength + 1];

            // Initialize first row and column
            for (int i = 0; i <= sourceLength; i++)
            {
                matrix[i, 0] = i;
            }

            for (int j = 0; j <= targetLength; j++)
            {
                matrix[0, j] = j;
            }

            // Fill the matrix
            for (int i = 1; i <= sourceLength; i++)
            {
                for (int j = 1; j <= targetLength; j++)
                {
                    int cost = source[i - 1] == target[j - 1] ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(
                            matrix[i - 1, j] + 1,     // deletion
                            matrix[i, j - 1] + 1),    // insertion
                        matrix[i - 1, j - 1] + cost);   // substitution
                }
            }

            return matrix[sourceLength, targetLength];
        }

        /// <summary>
        /// Finds the best match from a collection of candidates based on Levenshtein distance.
        /// </summary>
        /// <param name="input">The input string to match against.</param>
        /// <param name="candidates">The collection of candidate strings.</param>
        /// <param name="maxDistance">The maximum distance to consider as a match (default: 3).</param>
        /// <returns>The best matching candidate, or null if no candidate is within the max distance.</returns>
        public static string? FindBestMatch(string input, IEnumerable<string> candidates, int maxDistance = 3)
        {
            if (string.IsNullOrEmpty(input) || candidates == null)
            {
                return null;
            }

            string? bestMatch = null;
            int bestDistance = int.MaxValue;

            foreach (string candidate in candidates)
            {
                if (string.IsNullOrEmpty(candidate))
                {
                    continue;
                }

                int distance = Calculate(input, candidate);

                // Prioritize exact matches and close matches
                if (distance < bestDistance && distance <= maxDistance)
                {
                    bestDistance = distance;
                    bestMatch = candidate;
                }
            }

            return bestMatch;
        }

        /// <summary>
        /// Finds multiple good matches from a collection of candidates based on Levenshtein distance.
        /// </summary>
        /// <param name="input">The input string to match against.</param>
        /// <param name="candidates">The collection of candidate strings.</param>
        /// <param name="maxDistance">The maximum distance to consider as a match (default: 3).</param>
        /// <param name="maxResults">The maximum number of results to return (default: 3).</param>
        /// <returns>A collection of the best matching candidates ordered by distance.</returns>
        public static IEnumerable<string> FindMultipleMatches(string input, IEnumerable<string> candidates, int maxDistance = 3, int maxResults = 3)
        {
            if (string.IsNullOrEmpty(input) || candidates == null)
            {
                return [];
            }

            IEnumerable<string> matches = candidates
                .Where(candidate => !string.IsNullOrEmpty(candidate))
                .Select(candidate => new { Candidate = candidate, Distance = Calculate(input, candidate) })
                .Where(item => item.Distance <= maxDistance)
                .OrderBy(item => item.Distance)
                .ThenBy(item => item.Candidate, StringComparer.OrdinalIgnoreCase)
                .Take(maxResults)
                .Select(item => item.Candidate);

            return matches;
        }
    }
}
