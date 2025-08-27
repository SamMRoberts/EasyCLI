using System.Text.Json;
using EasyCLI.Console;

namespace EasyCLI.Configuration
{
    /// <summary>
    /// Manages configuration loading and merging from multiple sources.
    /// </summary>
    public class ConfigManager
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        private readonly string _globalConfigPath;
        private readonly string _localConfigPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigManager"/> class.
        /// </summary>
        /// <param name="appName">The application name for config directory.</param>
        public ConfigManager(string appName = "easycli")
        {
            ArgumentNullException.ThrowIfNull(appName);

            _globalConfigPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile),
                $".{appName}",
                "config.json");

            _localConfigPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                $".{appName}.json");
        }

        /// <summary>
        /// Loads configuration from global and local sources.
        /// </summary>
        /// <typeparam name="T">The configuration type.</typeparam>
        /// <param name="writer">Optional console writer for logging.</param>
        /// <returns>The merged configuration.</returns>
        public async Task<T> LoadConfigAsync<T>(IConsoleWriter? writer = null)
            where T : class, new()
        {
            T config = new();

            // Load global config first
            if (File.Exists(_globalConfigPath))
            {
                writer?.WriteHintLine($"Loading global config from {_globalConfigPath}");
                string globalJson = await File.ReadAllTextAsync(_globalConfigPath);
                T? globalConfig = JsonSerializer.Deserialize<T>(globalJson);
                if (globalConfig != null)
                {
                    config = MergeConfigs(config, globalConfig, "global");
                }
            }

            // Load local config (overrides global)
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

            return config;
        }

        /// <summary>
        /// Saves configuration to the specified location.
        /// </summary>
        /// <typeparam name="T">The configuration type.</typeparam>
        /// <param name="config">The configuration to save.</param>
        /// <param name="global">Whether to save to global location.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SaveConfigAsync<T>(T config, bool global = false)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(config);

            string path = global ? _globalConfigPath : _localConfigPath;
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
        /// <returns>A tuple containing global and local config paths.</returns>
        public (string Global, string Local) GetConfigPaths()
        {
            return (_globalConfigPath, _localConfigPath);
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
    }
}
