using Discord.WebSocket;
using Squad.Bot.Data;
using Squad.Bot.Logging;
using Squad.Bot.Models.AI;
using Squad.Bot.Models.Base;

namespace Squad.Bot.FunctionalModules.Events
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
        public async Task OnUserJoinGuild(SocketGuildUser guildUser)
        {
            Users? user = await _dbContext.Users.FindAsync((Users x) => x.Id == guildUser.Id);
            Guilds? guild = await _dbContext.Guilds.FindAsync((Guilds x) => x.Id == guildUser.Guild.Id);
            if (user == null)
            {
                user = new()
                {
                    Id = guildUser.Id,
                    Nick = guildUser.Nickname,
                };
            }
            else
            {
                NewMembers newMember = new()
                {
                    Guilds = guild,

                };
            }
        }

        public async Task OnUserLeftGuild(SocketGuild guild, SocketUser user)
        {

        }

        public async Task OnUserMessageReceived(SocketMessage message)
        {
            _logger.LogDebug("{nameOfFunc} has been executed by {Author.Username}:{Author.Id} in {Channel.Id} channel", nameof(OnUserMessageReceived), message.Author.Username, message.Author.Id, message.Channel.Id);
        }
    }
}
