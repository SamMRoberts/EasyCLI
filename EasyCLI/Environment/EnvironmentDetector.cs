using System.Runtime.InteropServices;

namespace EasyCLI.Environment
{
    /// <summary>
    /// Detects and analyzes the current execution environment.
    /// </summary>
    public class EnvironmentDetector
    {
        /// <summary>
        /// Detects and returns comprehensive environment information.
        /// </summary>
        /// <returns>Environment information.</returns>
        public static EnvironmentInfo DetectEnvironment()
        {
            EnvironmentInfo info = new()
            {
                IsGitRepository = IsGitRepository(),
                GitBranch = GetGitBranch(),
                IsDockerEnvironment = IsDockerEnvironment(),
                IsContinuousIntegration = IsContinuousIntegration(),
                CiProvider = GetCiProvider(),
                HasConfigFile = HasConfigurationFile(),
                ConfigFile = FindConfigurationFile(),
                IsInteractive = IsInteractiveSession(),
                Platform = GetPlatform(),
            };

            PopulateMetadata(info);
            return info;
        }

        /// <summary>
        /// Checks if the current directory is a Git repository.
        /// </summary>
        /// <returns>True if in a Git repository.</returns>
        private static bool IsGitRepository()
        {
            return Directory.Exists(".git") || FindGitDirectory() != null;
        }

        /// <summary>
        /// Gets the current Git branch name.
        /// </summary>
        /// <returns>The Git branch name or null if not in a Git repository.</returns>
        private static string? GetGitBranch()
        {
            try
            {
                string? gitDir = FindGitDirectory();
                if (gitDir == null)
                {
                    return null;
                }

                string headFile = Path.Combine(gitDir, "HEAD");
                if (!File.Exists(headFile))
                {
                    return null;
                }

                string head = File.ReadAllText(headFile).Trim();
                if (head.StartsWith("ref: refs/heads/", StringComparison.Ordinal))
                {
                    return head["ref: refs/heads/".Length..];
                }

                // Detached HEAD state
                return head[..8]; // First 8 characters of commit hash
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Finds the .git directory by walking up the directory tree.
        /// </summary>
        /// <returns>Path to .git directory or null if not found.</returns>
        private static string? FindGitDirectory()
        {
            string? currentDir = Directory.GetCurrentDirectory();
            while (!string.IsNullOrEmpty(currentDir))
            {
                string gitDir = Path.Combine(currentDir, ".git");
                if (Directory.Exists(gitDir))
                {
                    return gitDir;
                }

                currentDir = Path.GetDirectoryName(currentDir);
            }

            return null;
        }

        /// <summary>
        /// Checks if running in a Docker container.
        /// </summary>
        /// <returns>True if in Docker.</returns>
        private static bool IsDockerEnvironment()
        {
            // Check common Docker indicators
            return System.Environment.GetEnvironmentVariable("DOCKER_CONTAINER") != null ||
                   File.Exists("/.dockerenv") ||
                   IsRunningInContainer();
        }

        /// <summary>
        /// Checks if running in a container by examining cgroup.
        /// </summary>
        /// <returns>True if in a container.</returns>
        private static bool IsRunningInContainer()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    string cgroupFile = "/proc/1/cgroup";
                    if (File.Exists(cgroupFile))
                    {
                        string content = File.ReadAllText(cgroupFile);
                        return content.Contains("docker") || content.Contains("containerd");
                    }
                }
            }
            catch
            {
                // Ignore errors in container detection
            }

            return false;
        }

        /// <summary>
        /// Checks if running in a CI environment.
        /// </summary>
        /// <returns>True if in CI.</returns>
        private static bool IsContinuousIntegration()
        {
            string[] ciIndicators =
            [
                "CI", "CONTINUOUS_INTEGRATION", "BUILD_NUMBER",
                "GITHUB_ACTIONS", "GITLAB_CI", "JENKINS_URL",
                "TRAVIS", "CIRCLECI", "APPVEYOR", "AZURE_PIPELINES"
            ];

            return ciIndicators.Any(indicator =>
                !string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable(indicator)));
        }

        /// <summary>
        /// Gets the CI provider name if running in CI.
        /// </summary>
        /// <returns>CI provider name or null.</returns>
        private static string? GetCiProvider()
        {
            Dictionary<string, string> providers = new()
            {
                { "GITHUB_ACTIONS", "GitHub Actions" },
                { "GITLAB_CI", "GitLab CI" },
                { "JENKINS_URL", "Jenkins" },
                { "TRAVIS", "Travis CI" },
                { "CIRCLECI", "CircleCI" },
                { "APPVEYOR", "AppVeyor" },
                { "AZURE_PIPELINES", "Azure Pipelines" },
            };

            foreach ((string envVar, string provider) in providers)
            {
                if (!string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable(envVar)))
                {
                    return provider;
                }
            }

            return IsContinuousIntegration() ? "Unknown CI" : null;
        }

        /// <summary>
        /// Checks if common configuration files exist.
        /// </summary>
        /// <returns>True if config files found.</returns>
        private static bool HasConfigurationFile()
        {
            return FindConfigurationFile() != null;
        }

        /// <summary>
        /// Finds configuration files in common locations.
        /// </summary>
        /// <returns>Path to first found config file or null.</returns>
        private static string? FindConfigurationFile()
        {
            string[] configFiles =
            [
                "appsettings.json", "config.json", "app.config.json",
                ".config.json", "settings.json", ".settings.json"
            ];

            return configFiles.FirstOrDefault(File.Exists);
        }

        /// <summary>
        /// Checks if running in an interactive session.
        /// </summary>
        /// <returns>True if interactive.</returns>
        private static bool IsInteractiveSession()
        {
            try
            {
                // Check if stdin is redirected or if running in CI
                return !System.Console.IsInputRedirected &&
!System.Console.IsOutputRedirected &&
                       !IsContinuousIntegration();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the platform information.
        /// </summary>
        /// <returns>Platform description.</returns>
        private static string GetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "macOS";
            }

            return RuntimeInformation.OSDescription;
        }

        /// <summary>
        /// Populates additional metadata about the environment.
        /// </summary>
        /// <param name="info">Environment info to populate.</param>
        private static void PopulateMetadata(EnvironmentInfo info)
        {
            ArgumentNullException.ThrowIfNull(info);

            info.Metadata["WorkingDirectory"] = Directory.GetCurrentDirectory();
            info.Metadata["UserName"] = System.Environment.UserName;
            info.Metadata["MachineName"] = System.Environment.MachineName;
            info.Metadata["ProcessorCount"] = System.Environment.ProcessorCount.ToString(System.Globalization.CultureInfo.InvariantCulture);

            // Add runtime information
            info.Metadata["RuntimeVersion"] = RuntimeInformation.FrameworkDescription;
            info.Metadata["ProcessArchitecture"] = RuntimeInformation.ProcessArchitecture.ToString();

            // Add CI-specific metadata if in CI
            if (info.IsContinuousIntegration)
            {
                string? buildNumber = System.Environment.GetEnvironmentVariable("BUILD_NUMBER") ??
                                 System.Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER");
                if (!string.IsNullOrEmpty(buildNumber))
                {
                    info.Metadata["BuildNumber"] = buildNumber;
                }
            }
        }

        /// <summary>
        /// Determines if the application should run in non-interactive mode.
        /// This considers both environment factors and explicit command-line flags.
        /// </summary>
        /// <param name="args">Command line arguments to check for explicit flags.</param>
        /// <returns>True if the application should run in non-interactive mode.</returns>
        public static bool IsNonInteractiveMode(Shell.CommandLineArgs? args = null)
        {
            // Check for explicit command-line flag first
            if (args?.IsNoInput == true)
            {
                return true;
            }

            // Fall back to environment-based detection
            return !IsInteractiveSession();
        }

        /// <summary>
        /// Creates PromptOptions configured for the current environment.
        /// </summary>
        /// <param name="args">Command line arguments to check for explicit flags.</param>
        /// <param name="baseOptions">Base options to extend, or null to use defaults.</param>
        /// <returns>PromptOptions configured for non-interactive mode if applicable.</returns>
        public static Prompts.PromptOptions CreatePromptOptions(Shell.CommandLineArgs? args = null, Prompts.PromptOptions? baseOptions = null)
        {
            Prompts.PromptOptions options = baseOptions ?? new Prompts.PromptOptions();
            options.NonInteractive = IsNonInteractiveMode(args);
            return options;
        }
    }
}
