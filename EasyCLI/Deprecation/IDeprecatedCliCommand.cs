using EasyCLI.Shell;

namespace EasyCLI.Deprecation
{
    /// <summary>
    /// Interface for commands that are deprecated.
    /// </summary>
    public interface IDeprecatedCliCommand : ICliCommand
    {
        /// <summary>
        /// Gets the deprecation information for this command.
        /// </summary>
        DeprecationInfo DeprecationInfo { get; }
    }
}
