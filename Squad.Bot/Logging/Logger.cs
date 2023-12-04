namespace Squad.Bot.Logging
{
    /// <summary>
    /// Provides a set of static methods for logging messages to the console.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// A list of tasks that are currently running for logging.
        /// </summary>
        public static List<Task> LogTasks { get; set; } = new List<Task>();

        /// <summary>
        /// A lock object for synchronizing access to the <see cref="LogTasks"/> list.
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Logs a message of type <see cref="LogType.CommandExecuted"/> to the console.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task LogCommand(string message) => LogToConsole(new LogMessage(LogType.CommandExecuted, message));

        /// <summary>
        /// Logs a message of type <see cref="LogType.Exception"/> to the console.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">An optional message to include with the exception.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task LogException(Exception exception, string? message = null) => LogToConsole(new LogMessage(LogType.Exception, message ?? "No extra information.", exception));

        /// <summary>
        /// Logs a message of type <see cref="LogType.EventRegistered"/> to the console.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task LogEvent(string message) => LogToConsole(new LogMessage(LogType.EventRegistered, message));

        /// <summary>
        /// Logs a message of type <see cref="LogType.Info"/> to the console.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task LogInfo(string message) => LogToConsole(new LogMessage(LogType.Info, message));

        /// <summary>
        /// Logs a message to the console.
        /// </summary>
        /// <param name="logMessage">The message to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task LogToConsole(LogMessage logMessage)
        {
            // Fire and forget
            LogTasks.Add(Task.Run(() =>
            {
                lock (_lock)
                {
                    PrintSeverityPrefix(logMessage.Severity);
                    Console.WriteLine($" - {logMessage.Message}");

                    if (logMessage.HasException)
                    {
                        Console.WriteLine();
                        Console.WriteLine(logMessage.Exception.Message);
                    }

                    LogTasks = LogTasks.Where(x => !x.IsCanceled && !x.IsCompleted && !x.IsCompletedSuccessfully && !x.IsFaulted).ToList();
                }
            }));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Prints a prefix for the given log severity to the console.
        /// </summary>
        /// <param name="severity">The severity of the log message.</param>
        private static void PrintSeverityPrefix(LogType severity)
        {
            // Looks like '[Info]' but adds color to the inner text, and restores the old color
            Console.Write("[");
            var oldColor = Console.ForegroundColor;
            var severityColor = severity switch
            {
                LogType.Exception => ConsoleColor.Red,
                LogType.CommandExecuted => ConsoleColor.DarkBlue,
                LogType.EventRegistered => ConsoleColor.DarkBlue,
                LogType.Info => ConsoleColor.DarkYellow,
                _ => throw new NotImplementedException("That log type doesn't exist"),
            };
            Console.ForegroundColor = severityColor;
            Console.Write(severity.ToString());
            Console.ForegroundColor = oldColor;
            Console.Write("]");
        }
    }
}