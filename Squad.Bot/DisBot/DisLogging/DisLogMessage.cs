namespace Squad.Bot.DisBot.DisLogging
{
    public class DisLogMessage
    {
        public DisLogMessage(DisLogType severity, string message, Exception? exception = null)
        {
            Severity = severity;
            Message = message;
            Exception = exception;
        }
        public DisLogType Severity { get; }
        public string Message { get; }
        public Exception? Exception { get; }
        public bool HasException => Exception != null;
    }
}
