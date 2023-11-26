using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.Logging;

namespace Squad.Bot.Events
{
    public class UserGuildEvent
    {

        private readonly SquadDBContext _dbContext;

        public UserGuildEvent(SquadDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task OnUserLeftGuild(SocketGuild guild, SocketUser user)
        {
            await Logger.LogEvent($"{nameof(OnUserLeftGuild)} has been executed by {user.Username} in {guild.Id}");
        }
        public async Task OnUserJoinGuild(SocketGuildUser user)
        {
            await Logger.LogEvent($"{nameof(OnUserJoinGuild)} has been executed by {user.Username} in {user.Guild.Id}");
        }
    }
}
