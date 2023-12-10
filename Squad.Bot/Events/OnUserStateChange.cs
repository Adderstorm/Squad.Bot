using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.Logging;
using Squad.Bot.Utilities;
using Discord;
using Squad.Bot.Models.Base;

namespace Squad.Bot.Events
{
    public class OnUserStateChange
    {

        private readonly SquadDBContext _dbContext;

        public OnUserStateChange(SquadDBContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task OnUserVoiceStateUpdate(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            await Logger.LogEvent($"{nameof(OnUserStateChange)} has been executed by {user.Username}, {nameof(oldState)}: {oldState.VoiceChannel?.Id}, {nameof(newState)}: {newState.VoiceChannel?.Id}");
            await PrivateRooms(user, oldState, newState);
            await CollectData(user, oldState, newState);
        }

        // TODO: Change the stubs with the working code and !!! delete static field in CollectData
        private static async Task CollectData(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            await Logger.LogInfo($"{user}, {newState}, {oldState}");
        }

        private async Task PrivateRooms(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            PrivateRooms? savedPortal = new();
            if(oldState.VoiceChannel == null)
                savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == newState.VoiceChannel.Guild.Id);
            else if(newState.VoiceChannel == null)
                savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == oldState.VoiceChannel.Guild.Id);


            if (newState.VoiceChannel != null && newState.VoiceChannel.Id == savedPortal.ChannelID)
            {
                var permissions = new PermissionOverwriteHelper(user.Id, PermissionTarget.User)
                {
                    Permissions = PermissionOverwriteHelper.SetOverwritePermissions(muteMembers: PermValue.Allow,
                                                                                    deafenMembers: PermValue.Allow,
                                                                                    manageChannel: PermValue.Deny)
                };
                var newVoiceChannel = await newState.VoiceChannel.Guild.CreateVoiceChannelAsync($"{user.Username}'s channel", tcp => {
                                                                                                                                         tcp.CategoryId = savedPortal.CategoryID;
                                                                                                                                         tcp.PermissionOverwrites = permissions.CreateOverwrites();
                                                                                                                                     });

                var member = newState.VoiceChannel.Guild.GetUser(user.Id);
                await member.ModifyAsync(x => x.Channel = newVoiceChannel);
            }
            else if(oldState.VoiceChannel != null && oldState.VoiceChannel.Id != savedPortal.ChannelID && oldState.VoiceChannel.Category.Id == savedPortal.CategoryID)
            {
                var voiceChannel = oldState.VoiceChannel.Guild.GetVoiceChannel(oldState.VoiceChannel.Id);
                if (voiceChannel.ConnectedUsers.Count == 0)
                {
                    await oldState.VoiceChannel.DeleteAsync();                    
                }
            }
        }
    }
}
