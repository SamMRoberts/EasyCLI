namespace EasyCLI.Shell.Commands
{
    /// <summary>
    /// Enhanced configuration command demonstrating phase 2 CLI features.
    /// </summary>
    public class ConfigCommand : EnhancedCliCommand
    {
        private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        /// <summary>
        /// Gets the command name.
        /// </summary>
        public override string Name => "config";

        /// <summary>
        /// Gets the command description.
        /// </summary>
        public override string Description => "Manage application configuration and show environment information";

        private static readonly string[] Item = ["Property", "Value"];

        /// <summary>
        /// Configures the help information for this command.
        /// </summary>
        /// <param name="help">The help information to configure.</param>
        protected override void ConfigureHelp(CommandHelp help)
        {
            ArgumentNullException.ThrowIfNull(help);

            help.Usage = "config [subcommand] [options]";
            help.Description = "Manage configuration files and display environment information. Supports global and local configuration files.";

            help.Arguments.Add(new CommandArgument("subcommand", "The configuration subcommand: show, get, set, env", false));

            help.Options.Add(new CommandOption("global", "g", "Operate on global configuration"));
            help.Options.Add(new CommandOption("local", "l", "Operate on local configuration"));
            help.Options.Add(new CommandOption("json", "j", "Output in JSON format"));

            help.Examples.Add(new CommandExample("config show", "Show current configuration"));
            help.Examples.Add(new CommandExample("config env", "Show environment information"));
            help.Examples.Add(new CommandExample("config get api_url", "Get a specific configuration value"));
            help.Examples.Add(new CommandExample("config set api_url https://api.prod.com --global", "Set a global configuration value"));
            help.Examples.Add(new CommandExample("config --json", "Show configuration in JSON format"));
        }

        /// <summary>
        /// Executes the configuration command logic.
        /// </summary>
        /// <param name="args">The parsed command line arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Exit code (0 for success).</returns>
        protected override async Task<int> ExecuteEnhancedCommand(CommandLineArgs args, ShellExecutionContext context, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(context);

            string subcommand = args.GetArgument(0)?.ToLowerInvariant() ?? "show";
            ConsoleTheme theme = GetTheme(context);

            Logger?.LogDebug($"Executing config subcommand: {subcommand}");

            return subcommand switch
            {
                "show" => await ShowConfigurationAsync(args, context, theme),
                "env" => await ShowEnvironmentAsync(args, context, theme),
                "get" => await GetConfigurationValueAsync(args, context, theme),
                "set" => await SetConfigurationValueAsync(args, context, theme),
                "paths" => await ShowConfigurationPathsAsync(args, context, theme),
                _ => await ShowUnknownSubcommandAsync(subcommand, context, theme),
            };
        }

        /// <summary>
        /// Shows the current configuration.
        /// </summary>
        private Task<int> ShowConfigurationAsync(CommandLineArgs args, ShellExecutionContext context, ConsoleTheme theme)
        {
            if (Config == null)
            {
                Logger?.LogError("Configuration not loaded");
                return Task.FromResult(ExitCodes.GeneralError);
            }

            if (args.HasFlag("json"))
            {
                string json = System.Text.Json.JsonSerializer.Serialize(Config, JsonOptions);
                context.Writer.WriteLine(json);
                return Task.FromResult(ExitCodes.Success);
            }

            context.Writer.WriteHeadingLine("Current Configuration", theme);
            context.Writer.WriteLine("");

            string[][] configData =
            [
                ["Setting", "Value", "Source"],
                ["API URL", Config.ApiUrl, Config.Source.ApiUrlSource],
                ["Timeout", Config.Timeout.ToString(System.Globalization.CultureInfo.InvariantCulture), Config.Source.TimeoutSource],
                ["Enable Logging", Config.EnableLogging.ToString(), Config.Source.EnableLoggingSource],
                ["Log Level", Config.LogLevel, Config.Source.LogLevelSource],
                ["Output Format", Config.OutputFormat, Config.Source.OutputFormatSource],
                ["Use Colors", Config.UseColors.ToString(), Config.Source.UseColorsSource],
            ];

            context.Writer.WriteTableSimple(
                configData.Take(1).First(),
                configData.Skip(1).ToArray(),
                headerStyle: theme.Heading,
                borderStyle: theme.Hint);

            Logger?.LogDebug("Configuration displayed successfully");
            return Task.FromResult(ExitCodes.Success);
        }

        /// <summary>
        /// Shows environment information.
        /// </summary>
        private Task<int> ShowEnvironmentAsync(CommandLineArgs args, ShellExecutionContext context, ConsoleTheme theme)
        {
            if (Environment == null)
            {
                Logger?.LogError("Environment information not available");
                return Task.FromResult(ExitCodes.GeneralError);
            }

            if (args.HasFlag("json"))
            {
                string json = System.Text.Json.JsonSerializer.Serialize(Environment, JsonOptions);
                context.Writer.WriteLine(json);
                return Task.FromResult(ExitCodes.Success);
            }

            context.Writer.WriteHeadingLine("Environment Information", theme);
            context.Writer.WriteLine("");

            // Basic environment info
            List<string[]> envData =
            [Item,
                ["Platform", Environment.Platform],
                ["Interactive", Environment.IsInteractive.ToString()],
                ["CI Environment", Environment.IsContinuousIntegration.ToString()],
            ];

            if (Environment.IsContinuousIntegration && !string.IsNullOrEmpty(Environment.CiProvider))
            {
                envData.Add(["CI Provider", Environment.CiProvider]);
            }

            if (Environment.IsGitRepository)
            {
                envData.Add(["Git Repository", "Yes"]);
                if (!string.IsNullOrEmpty(Environment.GitBranch))
                {
                    envData.Add(["Git Branch", Environment.GitBranch]);
                }
            }

            if (Environment.IsDockerEnvironment)
            {
                envData.Add(["Docker Container", "Yes"]);
            }

            if (Environment.HasConfigFile && !string.IsNullOrEmpty(Environment.ConfigFile))
            {
                envData.Add(["Config File", Environment.ConfigFile]);
            }

            context.Writer.WriteTableSimple(
                envData.First(),
                envData.Skip(1).ToArray(),
                headerStyle: theme.Heading,
                borderStyle: theme.Hint);

            // Show metadata if verbose
            if (Logger?.Level >= Logging.LogLevel.Verbose && Environment.Metadata.Count != 0)
            {
                context.Writer.WriteLine("");
                context.Writer.WriteHeadingLine("Additional Metadata", theme);
                context.Writer.WriteLine("");

                string[][] metadataData = Environment.Metadata
                    .Select(kv => new[] { kv.Key, kv.Value })
                    .Prepend(["Key", "Value"])
                    .ToArray();

                context.Writer.WriteTableSimple(
                    metadataData.First(),
                    metadataData.Skip(1).ToArray(),
                    headerStyle: theme.Heading,
                    borderStyle: theme.Hint);
            }

            Logger?.LogDebug("Environment information displayed successfully");
            return Task.FromResult(ExitCodes.Success);
        }

        /// <summary>
        /// Gets a specific configuration value.
        /// </summary>
        private Task<int> GetConfigurationValueAsync(CommandLineArgs args, ShellExecutionContext context, ConsoleTheme theme)
        {
            string? key = args.GetArgument(1);
            if (string.IsNullOrEmpty(key))
            {
                Logger?.LogError("Configuration key not specified");
                ShowSuggestion(context, "Use 'config get <key>' to get a configuration value");
                return Task.FromResult(ExitCodes.InvalidArguments);
            }

            if (Config == null)
            {
                Logger?.LogError("Configuration not loaded");
                return Task.FromResult(ExitCodes.GeneralError);
            }

            string? value = key.ToLowerInvariant() switch
            {
                "api_url" => Config.ApiUrl,
                "timeout" => Config.Timeout.ToString(System.Globalization.CultureInfo.InvariantCulture),
                "enable_logging" => Config.EnableLogging.ToString(),
                "log_level" => Config.LogLevel,
                "output_format" => Config.OutputFormat,
                "use_colors" => Config.UseColors.ToString(),
                _ => null,
            };

            if (value == null)
            {
                Logger?.LogError($"Unknown configuration key: {key}");
                ShowSuggestion(context, "Use 'config show' to see all available configuration keys");
                return Task.FromResult(ExitCodes.InvalidArguments);
            }

            context.Writer.WriteLine(value);
            Logger?.LogDebug($"Retrieved configuration value for {key}: {value}");
            return Task.FromResult(ExitCodes.Success);
        }

        /// <summary>
        /// Sets a configuration value.
        /// </summary>
        private Task<int> SetConfigurationValueAsync(CommandLineArgs args, ShellExecutionContext context, ConsoleTheme theme)
        {
            string? key = args.GetArgument(1);
            string? value = args.GetArgument(2);

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                Logger?.LogError("Configuration key and value must be specified");
                ShowSuggestion(context, "Use 'config set <key> <value>' to set a configuration value");
                return Task.FromResult(ExitCodes.InvalidArguments);
            }

            Logger?.LogWarning("Configuration setting not yet implemented in this demo");
            ShowSuggestion(context, "This feature will be available in a future version");
            return Task.FromResult(ExitCodes.Success);
        }

        /// <summary>
        /// Shows configuration file paths.
        /// </summary>
        private Task<int> ShowConfigurationPathsAsync(CommandLineArgs args, ShellExecutionContext context, ConsoleTheme theme)
        {
            if (ConfigManager == null)
            {
                Logger?.LogError("Configuration manager not available");
                return Task.FromResult(ExitCodes.GeneralError);
            }

            (string globalPath, string localPath) = ConfigManager.GetConfigPaths();

            context.Writer.WriteHeadingLine("Configuration File Paths", theme);
            context.Writer.WriteLine("");

            string[][] pathData =
            [
                ["Type", "Path", "Exists"],
                ["Global", globalPath, File.Exists(globalPath).ToString()],
                ["Local", localPath, File.Exists(localPath).ToString()]
            ];

            context.Writer.WriteTableSimple(
                pathData.First(),
                pathData.Skip(1).ToArray(),
                headerStyle: theme.Heading,
                borderStyle: theme.Hint);

            Logger?.LogDebug("Configuration paths displayed successfully");
            return Task.FromResult(ExitCodes.Success);
        }

        /// <summary>
        /// Handles unknown subcommands.
        /// </summary>
        private Task<int> ShowUnknownSubcommandAsync(string subcommand, ShellExecutionContext context, ConsoleTheme theme)
        {
            Logger?.LogError($"Unknown config subcommand: {subcommand}");
            context.Writer.WriteErrorLine($"Unknown subcommand: {subcommand}", theme);

            string[] suggestions = ["show", "env", "get", "set", "paths"];
            string? closest = suggestions.FirstOrDefault(s => s.StartsWith(subcommand.Length >= 2 ? subcommand[..2] : subcommand, StringComparison.Ordinal));

            if (!string.IsNullOrEmpty(closest))
            {
                ShowSuggestion(context, $"Did you mean '{closest}'?");
            }
            else
            {
                ShowSuggestion(context, "Available subcommands: " + string.Join(", ", suggestions));
            }

            return Task.FromResult(ExitCodes.InvalidArguments);
        }
    }
}
