namespace Squad.Bot.Logging
{
    /// <summary>
    /// Enum containing the different types of log entries that can be recorded.
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Indicates that a command was executed.
        /// </summary>
        CommandExecuted,

        /// <summary>
        /// Indicates that a new event was registered.
        /// </summary>
        EventRegistered,

        /// <summary>
        /// Indicates that an exception occurred.
        /// </summary>
        Exception,

        /// <summary>
        /// Indicates that general information was recorded.
        /// </summary>
        Info
    }
}