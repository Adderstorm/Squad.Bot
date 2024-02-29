using Discord;
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

            if (guild == null)
            {
                GuildEvent newGuild = new(_dbContext, _logger);

                await newGuild.OnGuildJoined(guildUser.Guild);

                guild = await _dbContext.Guilds.FindAsync((Guilds x) => x.Id == guildUser.Guild.Id);
            }
            if (user == null)
            {
                user = new()
                {
                    Id = guildUser.Id,
                    Nick = guildUser.GlobalName,
                };

                await _dbContext.AddAsync(user);
            }

#pragma warning disable CS8601 // Возможно, назначение-ссылка, допускающее значение NULL. / не допускает
            JoinDate joinDate = new()
            {
                Guilds = guild,
                User = user,
            };
            TotalMembers totalMembers = new()
            {
                Guilds = guild,
                TotalUsers = guildUser.Guild.MemberCount,
            };
#pragma warning restore CS8601 // Возможно, назначение-ссылка, допускающее значение NULL. / не допускает

            await _dbContext.AddAsync(joinDate);
            await _dbContext.AddAsync(totalMembers);

            await _dbContext.SaveChangesAsync();
        }

        public async Task OnUserLeftGuild(SocketGuild socketGuild, SocketUser socketUser)
        {
            Users? user = await _dbContext.Users.FindAsync((Users x) => x.Id == socketUser.Id);
            Guilds? guild = await _dbContext.Guilds.FindAsync((Guilds x) => x.Id == socketGuild.Id);

            if (guild == null)
            {
                GuildEvent newGuild = new(_dbContext, _logger);

                await newGuild.OnGuildJoined(socketGuild);

                guild = await _dbContext.Guilds.FindAsync((Guilds x) => x.Id == socketGuild.Id);
            }
            if (user == null)
            {
                user = new()
                {
                    Id = socketUser.Id,
                    Nick = socketUser.GlobalName,
                };

                await _dbContext.AddAsync(user);
            }

#pragma warning disable CS8601 // Возможно, назначение-ссылка, допускающее значение NULL. / не допускает
            LeftDate leftDate = new()
            {
                User = user,
                Guilds = guild,
            };
            TotalMembers totalMembers = new()
            {
                Guilds = guild,
                TotalUsers = socketGuild.MemberCount,
            };
#pragma warning restore CS8601 // Возможно, назначение-ссылка, допускающее значение NULL. / не допускает
        }

        public async Task OnUserMessageReceived(SocketMessage message)
        {
            _logger.LogDebug("{nameOfFunc} has been executed by {Author.Username}:{Author.Id} in {Channel.Id} channel", nameof(OnUserMessageReceived), message.Author.Username, message.Author.Id, message.Channel.Id);
        }
    }
}
