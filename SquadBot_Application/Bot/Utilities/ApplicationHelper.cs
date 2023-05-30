using SquadBot_Application.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadBot_Application.Bot.Utilities
{
    public static class ApplicationHelper
    {
        public static void AnnounceAndExit()
        {
            Logger.LogInfo("Awaiting all log tasks...");
            Task.WhenAll(Logger.LogTasks).GetAwaiter().GetResult();
            Logger.LogInfo("Application closing safely...");
            Environment.Exit(0);
        }
    }
}
