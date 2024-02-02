using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.Logging;
using Squad.Bot.Utilities;
using Discord;
using Squad.Bot.Models.Base;

namespace Squad.Bot.FunctionalModules.Events
{
    public class OnUserStateChange
    {

        private readonly SquadDBContext _dbContext;
        private readonly Logger _logger;

        public OnUserStateChange(SquadDBContext dbContext, Logger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task OnUserVoiceStateUpdate(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            await PrivateRooms(user, oldState, newState);
            await CollectData(user, oldState, newState);
        }

        // TODO: Change the stub with the working code and !!!(if need) delete static field in CollectData
        private async Task CollectData(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            _logger.LogInfo("{user}, {newState}, {oldState}", user, newState, oldState);
        }

        private async Task PrivateRooms(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            PrivateRooms? savedPortal = new();
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
                var newVoiceChannel = await newState.VoiceChannel.Guild.CreateVoiceChannelAsync($"{guildUser.Nickname ?? user.GlobalName ?? user.Username}'s channel", tcp =>
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

        private static bool IsUserOwner(SocketVoiceState state, SocketUser user)
        {
            var permissions = state.VoiceChannel.GetPermissionOverwrite(user);
            if (permissions != null && permissions.Value.ManageChannel == PermValue.Allow)
                return true;
            else
                return false;
        }
    }
}
