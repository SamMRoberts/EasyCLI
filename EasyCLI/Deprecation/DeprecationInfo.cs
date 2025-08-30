namespace EasyCLI.Deprecation
{
    /// <summary>
    /// Contains comprehensive information about a deprecated feature.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DeprecationInfo"/> class.
    /// </remarks>
    /// <param name="reason">The reason for deprecation.</param>
    /// <param name="message">A custom deprecation message.</param>
    /// <param name="removalVersion">The version when the feature will be removed.</param>
    /// <param name="alternative">Guidance on what to use instead.</param>
    /// <param name="moreInfoUrl">Optional URL for more information.</param>
    public class DeprecationInfo(
        DeprecationReason reason,
        string message,
        DeprecationVersion? removalVersion = null,
        string? alternative = null,
        string? moreInfoUrl = null)
    {
        /// <summary>
        /// Creates a simple deprecation info for a replaced feature.
        /// </summary>
        /// <param name="alternative">What to use instead.</param>
        /// <param name="removalVersion">When it will be removed.</param>
        /// <returns>A DeprecationInfo instance.</returns>
        public static DeprecationInfo Replaced(string alternative, DeprecationVersion? removalVersion = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(alternative);
            return new DeprecationInfo(
                DeprecationReason.Replaced,
                "replaced by a better alternative",
                removalVersion,
                alternative);
        }

        /// <summary>
        /// Creates a simple deprecation info for an obsolete feature.
        /// </summary>
        /// <param name="removalVersion">When it will be removed.</param>
        /// <param name="reason">Optional reason for obsolescence.</param>
        /// <returns>A DeprecationInfo instance.</returns>
        public static DeprecationInfo Obsolete(DeprecationVersion? removalVersion = null, string? reason = null)
        {
            return new DeprecationInfo(
                DeprecationReason.Obsolete,
                reason ?? "no longer needed",
                removalVersion);
        }

        /// <summary>
        /// Creates a deprecation info for a moved feature.
        /// </summary>
        /// <param name="newLocation">Where the feature has moved to.</param>
        /// <param name="removalVersion">When the old location will be removed.</param>
        /// <returns>A DeprecationInfo instance.</returns>
        public static DeprecationInfo Moved(string newLocation, DeprecationVersion? removalVersion = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(newLocation);
            return new DeprecationInfo(
                DeprecationReason.Moved,
                "moved to a new location",
                removalVersion,
                newLocation);
        }

        /// <summary>
        /// Gets the reason for deprecation.
        /// </summary>
        public DeprecationReason Reason { get; } = reason;

        /// <summary>
        /// Gets the deprecation message.
        /// </summary>
        public string Message { get; } = message ?? throw new ArgumentNullException(nameof(message));

        /// <summary>
        /// Gets the version when this feature will be removed, if specified.
        /// </summary>
        public DeprecationVersion? RemovalVersion { get; } = removalVersion;

        /// <summary>
        /// Gets guidance on what to use instead of this deprecated feature.
        /// </summary>
        public string? Alternative { get; } = alternative;

        /// <summary>
        /// Gets an optional URL for more information about the deprecation.
        /// </summary>
        public string? MoreInfoUrl { get; } = moreInfoUrl;

        /// <summary>
        /// Gets a value indicating whether this deprecation has a planned removal version.
        /// </summary>
        public bool HasRemovalVersion => RemovalVersion != null;

        /// <summary>
        /// Gets a formatted deprecation message for display to users.
        /// </summary>
        /// <param name="featureName">The name of the deprecated feature.</param>
        /// <returns>A formatted message suitable for display.</returns>
        public string GetFormattedMessage(string featureName)
        {
            ArgumentException.ThrowIfNullOrEmpty(featureName);

            List<string> parts =
            [
                $"'{featureName}' is deprecated",
            ];

            if (!string.IsNullOrEmpty(Message))
            {
                parts.Add($"({Message})");
            }

            if (HasRemovalVersion)
            {
                parts.Add($"and will be removed in version {RemovalVersion}");
            }

            string result = string.Join(" ", parts) + ".";

            if (!string.IsNullOrEmpty(Alternative))
            {
                result += $" Use '{Alternative}' instead.";
            }

            if (!string.IsNullOrEmpty(MoreInfoUrl))
            {
                result += $" More info: {MoreInfoUrl}";
            }

            return result;
        }
    }
}
