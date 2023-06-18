using Discord.WebSocket;
using Squad.Bot.Logging;

namespace Squad.Bot.DsEvents
{
    public class UserMessages
    {
        public static async Task OnUserMessageReceived(SocketMessage message)
        {
            Logger.LogEvent($"{nameof(OnUserMessageReceived)} has been executed by {message.Author.Username}:{message.Author.Id} in {message.Channel.Id} channel");
        }
    }
}
