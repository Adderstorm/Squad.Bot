using Discord;
using SquadBot.Discord;
using SquadBot.Utilities;
using SquadBot_Application.Constants;
using SquadBot_Application.Logging;
using SquadBot_Application.Models;
using SquadBot_Application.Services;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SquadBot
{
    internal class Program
    {
        public static void Main(string args)
        {
            Config? config;

            if (args == null)
                config = ConfigService.GetConfig();
            else
                config = JsonSerializer.Deserialize<Config>(args);

            try
            {
                TokenUtils.ValidateToken(TokenType.Bot, config.Token);
            }
            catch
            {
                Logger.LogError("The discord bot token was invalid, please check the value :" + config.Token);
                ApplicationHelper.AnnounceAndExit();
            }

            var bot = new Bot(config.Token, config);

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