namespace EasyCLI.Shell
{
    /// <summary>
    /// Exception thrown when there is a naming conflict during command registration.
    /// </summary>
    public class CommandNamingException : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNamingException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public CommandNamingException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNamingException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CommandNamingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
