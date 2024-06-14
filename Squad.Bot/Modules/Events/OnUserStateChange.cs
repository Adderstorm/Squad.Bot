using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.Logging;
using Squad.Bot.Models.AI;
using Squad.Bot.Models.Base;
using Squad.Bot.Utilities;
using System.Collections;

namespace Squad.Bot.FunctionalModules.Events
{
    public class OnUserStateChange
    {
        private readonly SquadDBContext _dbContext;
        private readonly Logger _logger;

        private readonly Hashtable talkTime = [];

        public OnUserStateChange(SquadDBContext dbContext, Logger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task CollectTalkTimeData(SocketUser socketUser, SocketVoiceState oldState, SocketVoiceState newState)
        {
            // Working with old state
            if (newState.VoiceChannel == null && talkTime.ContainsKey(socketUser.Id) && !socketUser.IsBot)
            {
#pragma warning disable CS8605 // Unpacking is the conversion of a probable NULL value.
                DateTime firstConnectTime = (DateTime)talkTime[socketUser.Id];
#pragma warning restore CS8605 // Unpacking is the conversion of a probable NULL value.
                talkTime.Remove(socketUser.Id);

                TimeSpan minutesLeft = DateTime.Now.Subtract(firstConnectTime);

                await CheckPossibleNullData(oldState.VoiceChannel.Guild, socketUser);

                Guilds? guild = await _dbContext.Guilds.FirstOrDefaultAsync(x => x.Id == oldState.VoiceChannel.Guild.Id);
                Users? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == socketUser.Id);
                MembersActivity? membersActivity = await _dbContext.MembersActivity.FirstOrDefaultAsync(x => x.User == user);

#pragma warning disable CS8601 // Perhaps the destination is a reference that allows a NULL value. / does not allow
                MemberVoiceActivity voiceActivity = new()
                {
                    User = user,
                    Guilds = guild,
                    TotalMinutes = minutesLeft.TotalMinutes,
                };
                MembersActivity membersActivityNew = new()
                {
                    Guilds = guild,
                    User = user,
                };

                if(membersActivity == null)
                {
                    membersActivity = new()
                    {
                        User = user,
                        Guilds = guild,
                        LastActivityDate = DateTime.UtcNow,
                    };
                }
                else
                    membersActivity.LastActivityDate = DateTime.UtcNow;
#pragma warning restore CS8601 // Perhaps the destination is a reference that allows a NULL value. / does not allow

                await _dbContext.AddAsync(voiceActivity);

                if (membersActivity == null)
                    await _dbContext.AddAsync(membersActivityNew);
                else
                    _dbContext.Update(membersActivity);

                await _dbContext.SaveChangesAsync();
            }
            // Working with new state
            else if (oldState.VoiceChannel == null && !talkTime.ContainsKey(socketUser.Id) && !socketUser.IsBot)
            {
                talkTime.Add(socketUser.Id, DateTime.Now);
            }
            // Shit happening here or not :)
        }

        public async Task PrivateRooms(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            PrivateRooms? savedPortal;
            if (oldState.VoiceChannel == null)
                savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == newState.VoiceChannel.Guild.Id);
            else if (newState.VoiceChannel == null)
                savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == oldState.VoiceChannel.Guild.Id);
            else
                savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == newState.VoiceChannel.Guild.Id);

            if (savedPortal == null)
            {
                return;
            }

            if (newState.VoiceChannel != null && oldState.VoiceChannel != null && newState.VoiceChannel.Id == savedPortal.ChannelID && oldState.VoiceChannel.Category.Id == savedPortal.CategoryID && IsUserOwner(oldState, user))
            {
                var member = newState.VoiceChannel.Guild.GetUser(user.Id);
                await member.ModifyAsync(x => x.Channel = oldState.VoiceChannel);
                return;
            }
            else if (newState.VoiceChannel != null && newState.VoiceChannel.Id == savedPortal.ChannelID)
            {
                var permissions = new PermissionOverwriteHelper(user.Id, PermissionTarget.User)
                {
                    Permissions = new(muteMembers: PermValue.Allow,
                                      manageChannel: PermValue.Allow)
                };
                var guildUser = newState.VoiceChannel.Guild.GetUser(user.Id);
                var newVoiceChannel = await newState.VoiceChannel.Guild.CreateVoiceChannelAsync(savedPortal.DefaultRoomChannelName.Replace("{game}", user.Activities.First().Name)
                                                                                                                                  .Replace("{username}", guildUser.Guild.CurrentUser.Nickname ??
                                                                                                                                                         user.Username ??
                                                                                                                                                         user.GlobalName), tcp =>
                {
                    tcp.CategoryId = savedPortal.CategoryID;
                    tcp.PermissionOverwrites = permissions.CreateOptionalOverwrites();
                });

                var member = newState.VoiceChannel.Guild.GetUser(user.Id);
                await member.ModifyAsync(x => x.Channel = newVoiceChannel);
            }
            else if (oldState.VoiceChannel != null && oldState.VoiceChannel.Id != savedPortal.ChannelID && oldState.VoiceChannel.Category.Id == savedPortal.CategoryID)
            {
                var voiceChannel = oldState.VoiceChannel.Guild.GetVoiceChannel(oldState.VoiceChannel.Id);
                if (voiceChannel.ConnectedUsers.Count == 0)
                {
                    await oldState.VoiceChannel.DeleteAsync();
                }
            }
        }

        private static bool IsUserOwner(in SocketVoiceState state, in SocketUser user)
        {
            var permissions = state.VoiceChannel.GetPermissionOverwrite(user);
            if (permissions != null && permissions.Value.ManageChannel == PermValue.Allow)
                return true;
            else
                return false;
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
