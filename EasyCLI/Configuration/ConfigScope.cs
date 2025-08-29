namespace EasyCLI.Configuration
{
    /// <summary>
    /// Defines the scope for configuration storage.
    /// </summary>
    public enum ConfigScope
    {
        /// <summary>
        /// System-wide configuration (/etc/appname/config.json).
        /// </summary>
        System,

        /// <summary>
        /// User-specific configuration (XDG-compliant: $XDG_CONFIG_HOME/appname/config.json or ~/.config/appname/config.json).
        /// </summary>
        User,

        /// <summary>
        /// Local project configuration (./.appname.json).
        /// </summary>
        Local,
    }
}
