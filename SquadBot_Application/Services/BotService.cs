using Discord;
using SquadBot_Application.Bot.Utilities;
using SquadBot_Application.Bot.Discord;
using SquadBot_Application.Logging;
using SquadBot_Application.Models;
using SquadBot_Application.Bot.Data;
using SquadBot_Application.Bot.Models.Bot_special;

namespace SquadBot_Application.Services
{
    public class BotService
    {
        private static Thread? thread;

        private static BotDBContext? _dbContext;

        public BotService(BotDBContext? dBContext) 
        {
            _dbContext = dBContext;
        }

        public static void StartThread()
        {
            thread ??= new(new ThreadStart(BotMain));

            thread.Priority = ThreadPriority.Highest;
            if (thread.ThreadState == ThreadState.Unstarted)
                thread.Start();
            else
                throw new Exception("Bot thread already started.");
        }

        public static void StopThread()
        {
            if (thread == null || thread.ThreadState == ThreadState.Unstarted)
                throw new Exception("Bot thread is not running or empty");

            thread?.Interrupt();
        }
        public static ThreadState GetThreadState()
        {
            if (thread == null || thread.ThreadState == ThreadState.Unstarted)
                throw new Exception("Thread are null or unstarted");

            return thread.ThreadState;
        }

        public static void SetServerToLog(long serverId)
        {
            if (_dbContext.ServersToLogData.FirstOrDefault(p => p.ServerID == serverId) == null)
                _dbContext.ServersToLogData.Add(new ServersToLogData { ServerID = serverId });
            else
                throw new ArgumentException("Server already has added");
        }

        private static void BotMain()
        {
            Config? config = new();
            try
            {
                config = ConfigService.GetConfig();
                TokenUtils.ValidateToken(TokenType.Bot, config.Token);
            }
            catch(Exception ex) when (ex is ArgumentException || ex is ArgumentNullException)
            {
                Logger.LogError("The discord bot token was invalid, please check the value :" + config.Token,ex);
                //ApplicationHelper.AnnounceAndExit();
            }
            catch (Exception ex)
            {
                Logger.LogError("Config file doesn't exist, please paste the configuration settings", ex);
                //ApplicationHelper.AnnounceAndExit();
            }

            var bot = new BotApp(config);

            Logger.LogInfo("Bot gonna start now");
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
