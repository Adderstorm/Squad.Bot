namespace Squad.Bot.Logging
{
    public static class Logger
    {
        public static List<Task> LogTasks { get; set; } = new List<Task>();
        private static readonly object _lock = new();
        public static Task LogCommand(string message) => LogToConsole(new LogMessage(LogType.CommandExecuted, message));
        public static Task LogException(Exception exception, string? message = null) => LogToConsole(new LogMessage(LogType.Exception, message ?? "No extra information.", exception));
        public static Task LogEvent(string message) => LogToConsole(new LogMessage(LogType.EventRegistered, message));
        public static Task LogInfo(string message) => LogToConsole(new LogMessage(LogType.Info, message));
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
                        Console.WriteLine(logMessage.Exception.Message.ToString());
                    }

                    LogTasks = LogTasks.Where(x => !x.IsCanceled && !x.IsCompleted && !x.IsCompletedSuccessfully && !x.IsFaulted).ToList();
                }
            }));
            return Task.CompletedTask;
        }

        private static void PrintSeverityPrefix(LogType severity)
        {
            // Looks like '[Info]' but adds color to the inner text, and restore the old color
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
