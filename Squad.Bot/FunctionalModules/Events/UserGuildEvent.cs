using Discord;
using Discord.WebSocket;
using Squad.Bot.Data;
using Squad.Bot.Logging;
using Squad.Bot.Models.AI;
using Squad.Bot.Models.Base;
using System;

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
            await CheckPossibleNullData(guildUser.Guild, guildUser);

            Users? user = await _dbContext.Users.FindAsync((Users x) => x.Id == guildUser.Id);
            Guilds? guild = await _dbContext.Guilds.FindAsync((Guilds x) => x.Id == guildUser.Guild.Id);

#pragma warning disable CS8601 // Perhaps the destination is a reference that allows a NULL value. / does not allow
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
#pragma warning restore CS8601 // Perhaps the destination is a reference that allows a NULL value. / does not allow

            await _dbContext.AddAsync(joinDate);
            await _dbContext.AddAsync(totalMembers);

            await _dbContext.SaveChangesAsync();
        }

        public async Task OnUserLeftGuild(SocketGuild socketGuild, SocketUser socketUser)
        {
            await CheckPossibleNullData(socketGuild, socketUser);

            Users? user = await _dbContext.Users.FindAsync((Users x) => x.Id == socketUser.Id);
            Guilds? guild = await _dbContext.Guilds.FindAsync((Guilds x) => x.Id == socketGuild.Id);

#pragma warning disable CS8601 // Perhaps the destination is a reference that allows a NULL value. / does not allow
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
#pragma warning restore CS8601 // Perhaps the destination is a reference that allows a NULL value. / does not allow
        }

        public async Task OnUserMessageReceived(SocketMessage message)
        {
            _logger.LogDebug("{nameOfFunc} has been executed by {Author.Username}:{Author.Id} in {Channel.Id} channel", nameof(OnUserMessageReceived), message.Author.Username, message.Author.Id, message.Channel.Id);
        }

        private async Task CheckPossibleNullData(SocketGuild socketGuild, SocketUser socketUser)
        {
            Users? user = await _dbContext.Users.FindAsync((Users x) => x.Id == socketUser.Id);
            Guilds? guild = await _dbContext.Guilds.FindAsync((Guilds x) => x.Id == socketGuild.Id);

            if (guild == null)
            {
                await CreateNewGuild(socketGuild);
            }
            if (user == null)
            {
                await CreateNewUser(socketUser);
            }
        }

        private async Task CreateNewGuild(SocketGuild socketGuild)
        {
            GuildEvent newGuild = new(_dbContext, _logger);

            await newGuild.OnGuildJoined(socketGuild);
        }

        private async Task CreateNewUser(SocketUser socketUser)
        {
            Users user = new()
            {
                Id = socketUser.Id,
                Nick = socketUser.GlobalName,
            };

            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
