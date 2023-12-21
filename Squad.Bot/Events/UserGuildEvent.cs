using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.Logging;

namespace Squad.Bot.Events
{
    public class UserGuildEvent
    {

        private readonly SquadDBContext _dbContext;
        private readonly Logger _logger;

        public UserGuildEvent(SquadDBContext dbContext, Logger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // TODO: Change the stubs with the working code
        public async Task OnUserLeftGuild(SocketGuild guild, SocketUser user)
        {
            _logger.LogDebug("{nameof(OnUserLeftGuild)} has been executed by {user.Username} in {guild.Id}", nameof(OnUserLeftGuild), user.Username, guild.Id);
        }
        public async Task OnUserJoinGuild(SocketGuildUser user)
        {
            _logger.LogDebug("{nameof(OnUserJoinGuild)} has been executed by {user.Username} in {user.Guild.Id}", nameof(OnUserJoinGuild), user.Username, user.Guild.Id);
        }
    }
}
