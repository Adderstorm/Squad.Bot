using Discord.WebSocket;
using Squad.Bot.DisBot.DisLogging;
using System.Diagnostics.Tracing;

namespace Squad.Bot.DisBot.DsEvents
{
    public class UserGuildEvent
    {
        public static async Task OnUserLeftGuild(SocketGuild guild, SocketUser user)
        {
            DisLogger.LogEvent($"{nameof(OnUserLeftGuild)} has been executed by {user.Username} in {guild.Id}");
        }
        public static async Task OnUserJoinGuild(SocketGuildUser user)
        {
            DisLogger.LogEvent($"{nameof(OnUserJoinGuild)} has been executed by {user.Username} in {user.Guild.Id}");
        }
    }
}
