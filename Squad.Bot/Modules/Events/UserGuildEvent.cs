using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
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

        public async Task OnUserJoinGuild(SocketGuildUser guildUser)
        {
            await CheckPossibleNullData(guildUser.Guild, guildUser);

            Users? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == guildUser.Id);
            Guilds? guild = await _dbContext.Guilds.FirstOrDefaultAsync(x => x.Id == guildUser.Guild.Id);

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

            Users? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == socketUser.Id);
            Guilds? guild = await _dbContext.Guilds.FirstOrDefaultAsync(x => x.Id == socketGuild.Id);

#pragma warning disable CS8601 // Perhaps the destination is a reference that allows a NULL value. / P.S. does not allow
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
#pragma warning restore CS8601 // Perhaps the destination is a reference that allows a NULL value. / P.S. does not allow
        }

        public async Task OnUserMessageReceived(SocketMessage message)
        {
            if (message.Channel is SocketGuild socketGuild)
            {
                await CheckPossibleNullData(socketGuild, message.Author);

                Users? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == message.Author.Id);
                Guilds? guild = await _dbContext.Guilds.FirstOrDefaultAsync(x => x.Id == socketGuild.Id);
                MembersActivity? membersActivity = await _dbContext.MembersActivity.FirstOrDefaultAsync(x => x.User == user);

                if (message.Source == MessageSource.User)
                {
#pragma warning disable CS8601 // Perhaps the destination is a reference that allows a NULL value. / P.S. does not allow
                    MemberMessagesActivity memberMessagesActivity = new()
                    {
                        User = user,
                        Guilds = guild,
                    };
                    MembersActivity membersActivityNew = new()
                    {
                        Guilds = guild,
                        User = user,
                    };

                    membersActivity.LastActivityDate = DateTime.UtcNow;
#pragma warning restore CS8601 // Perhaps the destination is a reference that allows a NULL value. / P.S. does not allow

                    await _dbContext.AddAsync(memberMessagesActivity);

                    if(membersActivity == default)
                        await _dbContext.AddAsync(membersActivityNew);
                    else
                        _dbContext.Update(membersActivity);
                }

#pragma warning disable CS8601 // Perhaps the destination is a reference that allows a NULL value. / P.S. does not allow
                MessagesActivity messagesActivity = new()
                {
                    Guilds = guild,
                };
#pragma warning restore CS8601 // Perhaps the destination is a reference that allows a NULL value. / P.S. does not allow

                await _dbContext.AddAsync(messagesActivity);

                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Checks if the given SocketGuild and SocketUser have data in the database, if not, creates new data.
        /// </summary>
        /// <param name="socketGuild">The SocketGuild to check for.</param>
        /// <param name="socketUser">The SocketUser to check for.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task CheckPossibleNullData(SocketGuild socketGuild, SocketUser socketUser)
        {
            Users? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == socketUser.Id);
            Guilds? guild = await _dbContext.Guilds.FirstOrDefaultAsync(x => x.Id == socketGuild.Id);

            if (guild == default)
            {
                await CreateNewGuild(socketGuild);
            }
            if (user == default)
            {
                await CreateNewUser(socketUser);
            }
        }

        /// <summary>
        /// Creates new data for a new guild.
        /// </summary>
        /// <param name="socketGuild">The new guild.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task CreateNewGuild(SocketGuild socketGuild)
        {
            GuildEvent newGuild = new(_dbContext, _logger);

            await newGuild.OnGuildJoined(socketGuild);
        }

        /// <summary>
        /// Creates new data for a new user.
        /// </summary>
        /// <param name="socketUser">The new user.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
