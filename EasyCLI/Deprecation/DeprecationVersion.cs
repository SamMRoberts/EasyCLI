namespace EasyCLI.Deprecation
{
    /// <summary>
    /// Represents version information for deprecation tracking.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DeprecationVersion"/> class.
    /// </remarks>
    /// <param name="version">The version string (e.g., "2.0.0").</param>
    public class DeprecationVersion(string version)
    {
        /// <summary>
        /// Gets the version string.
        /// </summary>
        public string Version { get; } = version ?? throw new ArgumentNullException(nameof(version));

        /// <summary>
        /// Gets a value indicating whether this represents a major version change.
        /// </summary>
        public bool IsMajorVersion => Version.Split('.')[0] != "0";

        /// <summary>
        /// Creates a DeprecationVersion for the next major version.
        /// </summary>
        /// <param name="currentVersion">The current version.</param>
        /// <returns>A DeprecationVersion representing the next major version.</returns>
        public static DeprecationVersion NextMajor(string currentVersion)
        {
            ArgumentException.ThrowIfNullOrEmpty(currentVersion);

            string[] parts = currentVersion.Split('.');
            if (parts.Length == 0)
            {
                throw new ArgumentException("Invalid version format", nameof(currentVersion));
            }

            return int.TryParse(parts[0], out int major)
                ? new DeprecationVersion($"{major + 1}.0.0")
                : throw new ArgumentException("Invalid version format", nameof(currentVersion));
        }

        /// <summary>
        /// Creates a DeprecationVersion for the next minor version.
        /// </summary>
        /// <param name="currentVersion">The current version.</param>
        /// <returns>A DeprecationVersion representing the next minor version.</returns>
        public static DeprecationVersion NextMinor(string currentVersion)
        {
            ArgumentException.ThrowIfNullOrEmpty(currentVersion);

            string[] parts = currentVersion.Split('.');
            if (parts.Length < 2)
            {
                throw new ArgumentException("Invalid version format", nameof(currentVersion));
            }

            return int.TryParse(parts[0], out int major) && int.TryParse(parts[1], out int minor)
                ? new DeprecationVersion($"{major}.{minor + 1}.0")
                : throw new ArgumentException("Invalid version format", nameof(currentVersion));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Version;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is DeprecationVersion other && Version == other.Version;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Version.GetHashCode();
        }
    }
}
