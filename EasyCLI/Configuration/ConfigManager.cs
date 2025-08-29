using System.Text.Json;
using EasyCLI.Console;

namespace EasyCLI.Configuration
{
    /// <summary>
    /// Manages configuration loading and merging from multiple sources with XDG Base Directory spec compliance.
    /// Configuration precedence: flags > env > local > user > system.
    /// </summary>
    public class ConfigManager
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        private readonly string _systemConfigPath;
        private readonly string _userConfigPath;
        private readonly string _localConfigPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigManager"/> class.
        /// </summary>
        /// <param name="appName">The application name for config directory.</param>
        public ConfigManager(string appName = "easycli")
        {
            ArgumentNullException.ThrowIfNull(appName);

            // System-wide configuration (lowest precedence)
            _systemConfigPath = Path.Combine("/etc", appName, "config.json");

            // User configuration following XDG Base Directory specification
            string xdgConfigHome = System.Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")
                ?? Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), ".config");
            _userConfigPath = Path.Combine(xdgConfigHome, appName, "config.json");

            // Local project configuration (highest precedence after env vars and flags)
            _localConfigPath = Path.Combine(Directory.GetCurrentDirectory(), $".{appName}.json");
        }

        /// <summary>
        /// Loads configuration from all sources in order of precedence.
        /// Configuration precedence: flags > env > local > user > system.
        /// </summary>
        /// <typeparam name="T">The configuration type.</typeparam>
        /// <param name="writer">Optional console writer for logging.</param>
        /// <returns>The merged configuration.</returns>
        public async Task<T> LoadConfigAsync<T>(IConsoleWriter? writer = null)
            where T : class, new()
        {
            T config = new();

            // 1. Load system config first (lowest precedence)
            if (File.Exists(_systemConfigPath))
            {
                writer?.WriteHintLine($"Loading system config from {_systemConfigPath}");
                string systemJson = await File.ReadAllTextAsync(_systemConfigPath);
                T? systemConfig = JsonSerializer.Deserialize<T>(systemJson);
                if (systemConfig != null)
                {
                    config = MergeConfigs(config, systemConfig, "system");
                }
            }

            // 2. Load user config (XDG-compliant)
            if (File.Exists(_userConfigPath))
            {
                writer?.WriteHintLine($"Loading user config from {_userConfigPath}");
                string userJson = await File.ReadAllTextAsync(_userConfigPath);
                T? userConfig = JsonSerializer.Deserialize<T>(userJson);
                if (userConfig != null)
                {
                    config = MergeConfigs(config, userConfig, "user");
                }
            }

            // 3. Load local config (project-specific)
            if (File.Exists(_localConfigPath))
            {
                writer?.WriteHintLine($"Loading local config from {_localConfigPath}");
                string localJson = await File.ReadAllTextAsync(_localConfigPath);
                T? localConfig = JsonSerializer.Deserialize<T>(localJson);
                if (localConfig != null)
                {
                    config = MergeConfigs(config, localConfig, "local");
                }
            }

            // 4. Apply environment variable overrides
            config = ApplyEnvironmentVariables(config);

            // Note: Flag overrides are handled at the application level
            return config;
        }

        /// <summary>
        /// Saves configuration to the specified location.
        /// </summary>
        /// <typeparam name="T">The configuration type.</typeparam>
        /// <param name="config">The configuration to save.</param>
        /// <param name="scope">The configuration scope (user, local, or system).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SaveConfigAsync<T>(T config, ConfigScope scope = ConfigScope.User)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(config);

            string path = scope switch
            {
                ConfigScope.System => _systemConfigPath,
                ConfigScope.User => _userConfigPath,
                ConfigScope.Local => _localConfigPath,
                _ => _userConfigPath,
            };

            string? directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                _ = Directory.CreateDirectory(directory);
            }

            string json = JsonSerializer.Serialize(config, JsonOptions);
            await File.WriteAllTextAsync(path, json);
        }

        /// <summary>
        /// Gets the paths used for configuration files.
        /// </summary>
        /// <returns>A tuple containing system, user, and local config paths.</returns>
        public (string System, string User, string Local) GetConfigPaths()
        {
            return (_systemConfigPath, _userConfigPath, _localConfigPath);
        }

        /// <summary>
        /// Gets detailed information about configuration sources and their existence.
        /// </summary>
        /// <returns>Configuration source information.</returns>
        public ConfigSourceInfo GetConfigSourceInfo()
        {
            return new ConfigSourceInfo
            {
                SystemPath = _systemConfigPath,
                SystemExists = File.Exists(_systemConfigPath),
                UserPath = _userConfigPath,
                UserExists = File.Exists(_userConfigPath),
                LocalPath = _localConfigPath,
                LocalExists = File.Exists(_localConfigPath),
                XdgConfigHome = System.Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"),
            };
        }

        private static T MergeConfigs<T>(T baseConfig, T sourceConfig, string sourceName)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(baseConfig);
            ArgumentNullException.ThrowIfNull(sourceConfig);

            // Use reflection to merge properties
            System.Reflection.PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    object? sourceValue = property.GetValue(sourceConfig);
                    if (sourceValue != null)
                    {
                        property.SetValue(baseConfig, sourceValue);

                        // Update source tracking if config supports it
                        if (baseConfig is AppConfig appConfig && appConfig.Source != null)
                        {
                            UpdateSourceTracking(appConfig.Source, property.Name, sourceName);
                        }
                    }
                }
            }

            return baseConfig;
        }

        private static void UpdateSourceTracking(ConfigurationSource source, string propertyName, string sourceName)
        {
            ArgumentNullException.ThrowIfNull(source);

            System.Reflection.PropertyInfo? sourceProperty = typeof(ConfigurationSource)
                .GetProperty($"{propertyName}Source");

            sourceProperty?.SetValue(source, sourceName);
        }

        /// <summary>
        /// Applies environment variable overrides to configuration.
        /// Environment variables follow the pattern: EASYCLI_{PROPERTY_NAME}.
        /// </summary>
        /// <typeparam name="T">The configuration type.</typeparam>
        /// <param name="config">The configuration to update.</param>
        /// <returns>The updated configuration.</returns>
        private static T ApplyEnvironmentVariables<T>(T config)
            where T : class
        {
            System.Reflection.PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (System.Reflection.PropertyInfo? property in properties.Where(p => p.CanWrite))
            {
                // Convert property name to environment variable format
                // e.g., ApiUrl -> EASYCLI_API_URL
                string envVarName = $"EASYCLI_{ConvertToSnakeCase(property.Name).ToUpperInvariant()}";
                string? envValue = System.Environment.GetEnvironmentVariable(envVarName);

                if (!string.IsNullOrEmpty(envValue))
                {
                    try
                    {
                        object? convertedValue = ConvertEnvironmentValue(envValue, property.PropertyType);
                        if (convertedValue != null)
                        {
                            property.SetValue(config, convertedValue);

                            // Update source tracking if config supports it
                            if (config is AppConfig appConfig && appConfig.Source != null)
                            {
                                UpdateSourceTracking(appConfig.Source, property.Name, "environment");
                            }
                        }
                    }
                    catch
                    {
                        // Ignore conversion errors - use existing value
                    }
                }
            }

            return config;
        }

        /// <summary>
        /// Converts a property name to snake_case for environment variables.
        /// </summary>
        private static string ConvertToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            StringBuilder result = new();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsUpper(c) && i > 0)
                {
                    _ = result.Append('_');
                }
                _ = result.Append(char.ToLower(c, System.Globalization.CultureInfo.InvariantCulture));
            }

            return result.ToString();
        }

        /// <summary>
        /// Converts an environment variable value to the target property type.
        /// </summary>
        private static object? ConvertEnvironmentValue(string value, Type targetType)
        {
            if (targetType == typeof(string))
            {
                return value;
            }

            if (targetType == typeof(bool))
            {
                return bool.TryParse(value, out bool boolResult) ? boolResult :
                       value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
                       value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                       value.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            if (targetType == typeof(int))
            {
                return int.TryParse(value, out int intResult) ? intResult : null;
            }

            if (targetType == typeof(double))
            {
                return double.TryParse(value, out double doubleResult) ? doubleResult : null;
            }

            // Add more type conversions as needed
            return null;
        }
    }
}
