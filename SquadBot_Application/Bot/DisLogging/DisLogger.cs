namespace SquadBot_Application.Bot.DisLogging
{
    public static class DisLogger
    {
        public static List<Task> LogTasks { get; set; } = new List<Task>();
        private static readonly object _lock = new();
        public static void LogCommand(string message) => LogToConsole(new DisLogMessage(DisLogType.CommandExecuted, message));
        public static void LogException(Exception exception, string? message = null) => LogToConsole(new DisLogMessage(DisLogType.Exception, message ?? "No extra information.", exception));
        public static void LogEvent(string message) => LogToConsole(new DisLogMessage(DisLogType.EventRegistered, message));
        public static void LogInfo(string message) => LogToConsole(new DisLogMessage(DisLogType.Info, message));
        private static void LogToConsole(DisLogMessage logMessage)
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
        }

        private static void PrintSeverityPrefix(DisLogType severity)
        {
            // Looks like '[Info]' but adds color to the inner text, and restore the old color
            Console.Write("[");
            var oldColor = Console.ForegroundColor;
            var severityColor = severity switch
            {
                DisLogType.Exception => ConsoleColor.Red,
                DisLogType.CommandExecuted => ConsoleColor.DarkBlue,
                DisLogType.EventRegistered => ConsoleColor.DarkBlue,
                DisLogType.Info => ConsoleColor.DarkYellow,
                _ => throw new NotImplementedException("That log type doesn't exist"),
            };
            Console.ForegroundColor = severityColor;
            Console.Write(severity.ToString());
            Console.ForegroundColor = oldColor;
            Console.Write("]");
        }
    }
}
