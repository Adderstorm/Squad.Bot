namespace Squad.Bot.Logging
{
    /// <summary>
    /// A class for logging messages with a specific severity and an optional exception.
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Creates a new LogMessage with the specified severity, message, and exception.
        /// </summary>
        /// <param name="severity">The severity of the message.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception associated with the message, if any.</param>
        public LogMessage(LogType severity, string message, Exception? exception = null)
        {
            Severity = severity;
            Message = message;
            Exception = exception;
        }

        /// <summary>
        /// The severity of the message.
        /// </summary>
        public LogType Severity { get; }

        /// <summary>
        /// The message to log.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The exception associated with the message, if any.
        /// </summary>
        public Exception? Exception { get; }

        /// <summary>
        /// Indicates whether the message has an associated exception.
        /// </summary>
        public bool HasException => Exception != null;
    }
}