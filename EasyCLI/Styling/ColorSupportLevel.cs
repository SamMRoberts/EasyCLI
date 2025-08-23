namespace EasyCLI.Styling
{
    /// <summary>
    /// Represents detected terminal color capability.
    /// </summary>
    public enum ColorSupportLevel
    {
        /// <summary>
        /// No color support detected.
        /// </summary>
        None = 0,

        /// <summary>
        /// Basic 16-color ANSI support.
        /// </summary>
        Basic16 = 1,

        /// <summary>
        /// 256-color ANSI indexed support.
        /// </summary>
        Indexed256 = 2,

        /// <summary>
        /// TrueColor (24-bit) ANSI support.
        /// </summary>
        TrueColor = 3,
    }
}
