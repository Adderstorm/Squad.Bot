using Squad.Bot.DisBot.DisLogging;

namespace Squad.Bot.DisBot.Utilities
{
    public static class ApplicationHelper
    {
        public static void AnnounceAndExit()
        {
            DisLogger.LogInfo("Awaiting all log tasks...");
            Task.WhenAll(DisLogger.LogTasks).GetAwaiter().GetResult();
            DisLogger.LogInfo("Application closing safely...");
            Environment.Exit(0);
        }
    }
}
