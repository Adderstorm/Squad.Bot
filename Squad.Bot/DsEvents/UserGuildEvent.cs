using Discord.WebSocket;
using Squad.Bot.Logging;
using System.Diagnostics.Tracing;

namespace Squad.Bot.DsEvents
{
    public class UserGuildEvent
    {
        public static async Task OnUserLeftGuild(SocketGuild guild, SocketUser user)
        {
            Logger.LogEvent($"{nameof(OnUserLeftGuild)} has been executed by {user.Username} in {guild.Id}");
        }
        public static async Task OnUserJoinGuild(SocketGuildUser user)
        {
            Logger.LogEvent($"{nameof(OnUserJoinGuild)} has been executed by {user.Username} in {user.Guild.Id}");
        }
    }
}
