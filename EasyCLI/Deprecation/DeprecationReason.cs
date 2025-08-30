namespace EasyCLI.Deprecation
{
    /// <summary>
    /// Represents the reason for deprecation.
    /// </summary>
    public enum DeprecationReason
    {
        /// <summary>
        /// Feature is being replaced by a better alternative.
        /// </summary>
        Replaced,

        /// <summary>
        /// Feature is no longer needed or used.
        /// </summary>
        Obsolete,

        /// <summary>
        /// Feature has security concerns.
        /// </summary>
        Security,

        /// <summary>
        /// Feature has performance issues.
        /// </summary>
        Performance,

        /// <summary>
        /// Feature conflicts with new design principles.
        /// </summary>
        DesignChange,

        /// <summary>
        /// Feature is being moved to a different location.
        /// </summary>
        Moved,

        /// <summary>
        /// Other reason not covered by the above.
        /// </summary>
        Other,
    }
}
