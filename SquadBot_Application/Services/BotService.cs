using Discord;
using SquadBot_Application.Bot.Utilities;
using SquadBot_Application.Bot.Discord;
using SquadBot_Application.Logging;
using SquadBot_Application.Models;

namespace SquadBot_Application.Services
{
    public class BotService
    {
        private static Thread? thread;
        public static void StartThread()
        {
            thread ??= new(new ThreadStart(Main));

            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        public static void StopThread()
        {
            thread?.Interrupt();
        }
        private static void Main()
        {
            Config? config = ConfigService.GetConfig();

            try
            {
                TokenUtils.ValidateToken(TokenType.Bot, config.Token);
            }
            catch
            {
                Logger.LogError("The discord bot token was invalid, please check the value :" + config.Token);
                ApplicationHelper.AnnounceAndExit();
            }

            var bot = new BotApp(config);

            // Start the bot in async context from a sync context
            var closingException = bot.RunAsync().GetAwaiter().GetResult();

            if (closingException == null)
            {
                ApplicationHelper.AnnounceAndExit();
            }
            else
            {
                Logger.LogError("Caught crashing exception");
                Logger.LogException(closingException);
                Console.WriteLine();
                ApplicationHelper.AnnounceAndExit();
            }
        }
    }
}
