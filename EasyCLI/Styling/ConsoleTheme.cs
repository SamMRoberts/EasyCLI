namespace EasyCLI
{
    /// <summary>
    /// Theme map for semantic styles with easy overrides.
    /// </summary>
    public class ConsoleTheme
    {
        public ConsoleStyle Success { get; set; } = ConsoleStyles.Success;
        public ConsoleStyle Warning { get; set; } = ConsoleStyles.Warning;
        public ConsoleStyle Error { get; set; } = ConsoleStyles.Error;
        public ConsoleStyle Heading { get; set; } = ConsoleStyles.Heading;
        public ConsoleStyle Info { get; set; } = ConsoleStyles.Info;
        public ConsoleStyle Hint { get; set; } = ConsoleStyles.Hint;
    }
}
