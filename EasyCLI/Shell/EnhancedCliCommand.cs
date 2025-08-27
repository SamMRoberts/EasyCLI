using EasyCLI.Configuration;
using EasyCLI.Environment;
using EasyCLI.Logging;

namespace EasyCLI.Shell
{
    /// <summary>
    /// Enhanced base CLI command with configuration management, environment detection, and structured logging.
    /// </summary>
    public abstract class EnhancedCliCommand : BaseCliCommand
    {
        /// <summary>
        /// Gets the logger for this command execution.
        /// </summary>
        protected Logger? Logger { get; private set; }

        /// <summary>
        /// Gets the configuration manager.
        /// </summary>
        protected ConfigManager? ConfigManager { get; private set; }

        /// <summary>
        /// Gets the environment information.
        /// </summary>
        protected EnvironmentInfo? Environment { get; private set; }

        /// <summary>
        /// Gets the loaded configuration.
        /// </summary>
        protected AppConfig? Config { get; private set; }

        /// <summary>
        /// Executes the enhanced command with configuration, environment detection, and logging setup.
        /// </summary>
        /// <param name="args">The parsed command line arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Exit code (0 for success).</returns>
        protected override async Task<int> ExecuteCommand(CommandLineArgs args, ShellExecutionContext context, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(context);

            try
            {
                // Set up enhanced features before execution
                await SetupEnhancedFeaturesAsync(context, args);

                // Call the enhanced command implementation
                return await ExecuteEnhancedCommand(args, context, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger?.LogError($"Failed during enhanced command execution: {ex.Message}");
                Logger?.LogDebug($"Stack trace: {ex}");
                return ExitCodes.GeneralError;
            }
        }

        /// <summary>
        /// Executes the enhanced command logic. Override this method instead of ExecuteCommand.
        /// </summary>
        /// <param name="args">The parsed command line arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Exit code (0 for success).</returns>
        protected abstract Task<int> ExecuteEnhancedCommand(CommandLineArgs args, ShellExecutionContext context, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the name of the application for configuration purposes.
        /// Override to customize the application name used for config files.
        /// </summary>
        /// <returns>The application name.</returns>
        protected virtual string GetApplicationName()
        {
            return "easycli";
        }

        /// <summary>
        /// Creates the configuration object. Override to use custom configuration types.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <returns>A new configuration instance.</returns>
        protected virtual async Task<AppConfig> CreateConfigurationAsync(ShellExecutionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return ConfigManager == null
                ? new AppConfig()
                : await ConfigManager.LoadConfigAsync<AppConfig>(Logger != null && Logger.Level >= LogLevel.Verbose ? context.Writer : null);
        }

        /// <summary>
        /// Called after environment detection but before command execution.
        /// Override to handle environment-specific setup.
        /// </summary>
        /// <param name="environment">The detected environment information.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected virtual Task OnEnvironmentDetectedAsync(EnvironmentInfo environment, ShellExecutionContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called after configuration is loaded. Override to handle configuration-specific setup.
        /// </summary>
        /// <param name="config">The loaded configuration.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected virtual Task OnConfigurationLoadedAsync(AppConfig config, ShellExecutionContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets up enhanced features including logging, environment detection, and configuration.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SetupEnhancedFeaturesAsync(ShellExecutionContext context, CommandLineArgs args)
        {
            // 1. Set up logging first - use the parsed args to check flags
            LogLevel logLevel = DetermineLogLevelFromParsedArgs(args);
            Logger = new Logger(context.Writer, logLevel, GetTheme(context));

            Logger.LogDebug($"Starting enhanced command: {Name}");
            Logger.LogDebug($"Log level: {logLevel}");

            // 2. Set up configuration management
            string appName = GetApplicationName();
            ConfigManager = new ConfigManager(appName);
            Logger.LogVerbose($"Initialized config manager for app: {appName}");

            // 3. Detect environment
            Environment = EnvironmentDetector.DetectEnvironment();
            Logger.LogDebug($"Environment detected - Platform: {Environment.Platform}, Interactive: {Environment.IsInteractive}, CI: {Environment.IsContinuousIntegration}");

            if (Environment.IsGitRepository)
            {
                Logger.LogVerbose($"Git repository detected on branch: {Environment.GitBranch}");
            }

            if (Environment.IsDockerEnvironment)
            {
                Logger.LogVerbose("Docker environment detected");
            }

            if (Environment.IsContinuousIntegration)
            {
                Logger.LogVerbose($"CI environment detected: {Environment.CiProvider}");
            }

            // 4. Load configuration
            Config = await CreateConfigurationAsync(context);
            Logger.LogVerbose("Configuration loaded successfully");

            // 5. Allow derived classes to respond to environment and config
            await OnEnvironmentDetectedAsync(Environment, context);
            await OnConfigurationLoadedAsync(Config, context);

            Logger.LogDebug("Enhanced features setup complete");
        }

        /// <summary>
        /// Determines log level from parsed command line arguments.
        /// </summary>
        /// <param name="args">Parsed command line arguments.</param>
        /// <returns>The appropriate log level.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static members should appear before non-static members", Justification = "Logical grouping")]
        private static LogLevel DetermineLogLevelFromParsedArgs(CommandLineArgs args)
        {
            // Check environment first
            bool isCI = System.Environment.GetEnvironmentVariable("CI") != null;
            if (isCI && !args.IsVerbose)
            {
                // Default to quiet in CI unless explicitly verbose
                return LogLevel.Quiet;
            }

            // Check command line flags
            return args.IsQuiet ? LogLevel.Quiet : args.HasFlag("debug") ? LogLevel.Debug : args.IsVerbose ? LogLevel.Verbose : LogLevel.Normal;
        }
    }
}
