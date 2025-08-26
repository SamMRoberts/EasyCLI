namespace EasyCLI.Shell
{
    /// <summary>
    /// Parses command line arguments into structured data.
    /// </summary>
    public class CommandLineArgs
    {
        private readonly Dictionary<string, string?> _options = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _flags = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<string> _arguments = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineArgs"/> class.
        /// </summary>
        /// <param name="args">The command line arguments to parse.</param>
        public CommandLineArgs(string[] args)
        {
            ArgumentNullException.ThrowIfNull(args);
            ParseArguments(args);
        }

        /// <summary>
        /// Gets the positional arguments.
        /// </summary>
        public IReadOnlyList<string> Arguments => _arguments.AsReadOnly();

        /// <summary>
        /// Gets a value indicating whether help was requested.
        /// </summary>
        public bool IsHelpRequested => HasFlag("help") || HasFlag("h");

        /// <summary>
        /// Gets a value indicating whether verbose mode was requested.
        /// </summary>
        public bool IsVerbose => HasFlag("verbose") || HasFlag("v");

        /// <summary>
        /// Gets a value indicating whether quiet mode was requested.
        /// </summary>
        public bool IsQuiet => HasFlag("quiet") || HasFlag("q");

        /// <summary>
        /// Gets a value indicating whether dry-run mode was requested.
        /// </summary>
        public bool IsDryRun => HasFlag("dry-run") || HasFlag("n");

        /// <summary>
        /// Gets a value indicating whether force mode was requested.
        /// </summary>
        public bool IsForce => HasFlag("force") || HasFlag("f");

        /// <summary>
        /// Gets a value indicating whether yes/confirmation mode was requested.
        /// </summary>
        public bool IsYes => HasFlag("yes") || HasFlag("y");

        /// <summary>
        /// Checks if a flag is present.
        /// </summary>
        /// <param name="name">The flag name (without dashes).</param>
        /// <returns>True if the flag is present.</returns>
        public bool HasFlag(string name)
        {
            return _flags.Contains(name);
        }

        /// <summary>
        /// Gets the value of an option.
        /// </summary>
        /// <param name="name">The option name (without dashes).</param>
        /// <returns>The option value, or null if not present.</returns>
        public string? GetOption(string name)
        {
            return _options.TryGetValue(name, out string? value) ? value : null;
        }

        /// <summary>
        /// Gets the value of an option with a default value.
        /// </summary>
        /// <param name="name">The option name (without dashes).</param>
        /// <param name="defaultValue">The default value if option is not present.</param>
        /// <returns>The option value or the default value.</returns>
        public string GetOption(string name, string defaultValue)
        {
            return GetOption(name) ?? defaultValue;
        }

        /// <summary>
        /// Gets a positional argument by index.
        /// </summary>
        /// <param name="index">The argument index.</param>
        /// <returns>The argument value, or null if index is out of range.</returns>
        public string? GetArgument(int index)
        {
            return index >= 0 && index < _arguments.Count ? _arguments[index] : null;
        }

        /// <summary>
        /// Validates that required arguments are present.
        /// </summary>
        /// <param name="requiredCount">The number of required arguments.</param>
        /// <returns>True if all required arguments are present.</returns>
        public bool ValidateArgumentCount(int requiredCount)
        {
            return _arguments.Count >= requiredCount;
        }

        /// <summary>
        /// Validates that an argument count is within the specified range.
        /// </summary>
        /// <param name="minCount">The minimum argument count.</param>
        /// <param name="maxCount">The maximum argument count.</param>
        /// <returns>True if argument count is within range.</returns>
        public bool ValidateArgumentCount(int minCount, int maxCount)
        {
            return _arguments.Count >= minCount && _arguments.Count <= maxCount;
        }

        /// <summary>
        /// Returns a string representation of the parsed arguments for debugging.
        /// </summary>
        /// <returns>A string describing the parsed arguments.</returns>
        public override string ToString()
        {
            StringBuilder sb = new();
            _ = sb.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"Arguments:");
            for (int i = 0; i < _arguments.Count; i++)
            {
                _ = sb.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"  [{i}]: {_arguments[i]}");
            }

            if (_flags.Count > 0)
            {
                _ = sb.AppendLine("Flags:");
                foreach (string flag in _flags)
                {
                    _ = sb.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"  --{flag}");
                }
            }

            if (_options.Count > 0)
            {
                _ = sb.AppendLine("Options:");
                foreach ((string key, string? value) in _options)
                {
                    _ = sb.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"  --{key}={value}");
                }
            }

            return sb.ToString();
        }

        private void ParseArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (arg.StartsWith("--", StringComparison.Ordinal))
                {
                    // Long option
                    string name = arg[2..];
                    if (name.Contains('=', StringComparison.Ordinal))
                    {
                        // --option=value format
                        string[] parts = name.Split('=', 2);
                        _options[parts[0]] = parts[1];
                    }
                    else if (i + 1 < args.Length && !args[i + 1].StartsWith('-'))
                    {
                        // --option value format (if next arg is not another option)
                        _options[name] = args[++i];
                    }
                    else
                    {
                        // --flag format
                        _ = _flags.Add(name);
                    }
                }
                else if (arg.StartsWith('-') && arg.Length > 1)
                {
                    // Short option(s)
                    string shortOpts = arg[1..];
                    for (int j = 0; j < shortOpts.Length; j++)
                    {
                        string shortOpt = shortOpts[j].ToString(System.Globalization.CultureInfo.InvariantCulture);
                        if (j == shortOpts.Length - 1 && i + 1 < args.Length && !args[i + 1].StartsWith('-'))
                        {
                            // Last short option in group can have a value
                            _options[shortOpt] = args[++i];
                        }
                        else
                        {
                            _ = _flags.Add(shortOpt);
                        }
                    }
                }
                else
                {
                    // Positional argument
                    _arguments.Add(arg);
                }
            }
        }
    }
}
