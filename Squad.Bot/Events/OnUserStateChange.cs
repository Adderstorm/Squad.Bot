using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.Logging;

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

        private async Task CollectData(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            await Logger.LogInfo($"{user}, {newState}, {oldState}");
        }

        private async Task PrivateRooms(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == newState.VoiceChannel.Guild.Id);

            await Logger.LogInfo($"{user}, {newState}, {oldState}");
        }
    }
}
