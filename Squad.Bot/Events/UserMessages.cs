using Discord.WebSocket;
using Squad.Bot.Logging;

namespace Squad.Bot.Events
{
    public class UserMessages
    {
        public static async Task OnUserMessageReceived(SocketMessage message)
        {
            await Logger.LogEvent($"{nameof(OnUserMessageReceived)} has been executed by {message.Author.Username}:{message.Author.Id} in {message.Channel.Id} channel");
        }
    }
}
