using Discord.WebSocket;
using Squad.Bot.Logging;

namespace Squad.Bot.DsEvents
{
    public class OnUserStateChange
    {
        public static async Task OnUserVoiceStateUpdate(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            await Logger.LogEvent($"{nameof(OnUserStateChange)} has been executed by {user.Username}, {nameof(oldState)}: {oldState.VoiceChannel?.Id}, {nameof(newState)}: {newState.VoiceChannel?.Id}");
            await PrivateRooms(user, oldState, newState);
            await CollectData(user, newState);
        }

        private static async Task CollectData(SocketUser user, SocketVoiceState newState)
        {
            
        }

        private static async Task PrivateRooms(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            
        }
    }
}
