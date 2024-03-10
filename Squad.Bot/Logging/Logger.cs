using Discord;
using Microsoft.Extensions.Logging;

namespace Squad.Bot.Logging
{
    public class Logger
    {
        private readonly ILogger<Logger> _logger;

        public Logger(in ILogger<Logger> logger)
        {
            _logger = logger;
        }

        public void LogInfo(in string? message, params object?[] args)
        {
            _logger.LogInformation(message: message, args: args);
        }

        public void LogWarning(in string? message, in Exception? ex, params object?[] args)
        {
            _logger.LogWarning(message: message, exception: ex, args: args);
        }

        public void LogError(in string? message, in Exception? ex, params object?[] args)
        {
            _logger.LogError(message: message, exception: ex, args: args);
        }

        public void LogCritical(in string? message, in Exception? ex, params object?[] args)
        {
            _logger.LogCritical(message: message, exception: ex, args: args);
        }

        public void LogTrace(in string? message, in Exception? ex, params object?[] args)
        {
            _logger.LogTrace(message: message, exception: ex, args: args);
        }

        public void LogDebug(in string? message, params object?[] args)
        {
            _logger.LogDebug(message: message, args: args);
        }

        public Task LogDiscord(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                    LogCritical(message.Message, message.Exception, message.Source);
                    break;
                case LogSeverity.Error:
                    LogError(message.Message, message.Exception, message.Source);
                    break;
                case LogSeverity.Warning:
                    LogWarning(message.Message, ex: message.Exception, message.Source);
                    break;
                case LogSeverity.Info:
                    LogInfo(message.Message, message.Source);
                    break;
                case LogSeverity.Verbose:
                    LogTrace(message.Message, message.Exception, message.Source);
                    break;
                case LogSeverity.Debug:
                    LogDebug(message.Message, message.Source);
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
