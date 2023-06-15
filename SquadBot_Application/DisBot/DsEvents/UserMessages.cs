using Discord.WebSocket;
using Squad.Bot.DisBot.DisLogging;

namespace Squad.Bot.DisBot.DsEvents
{
    public class UserMessages
    {
        public static async Task OnUserMessageReceived(SocketMessage message)
        {
            DisLogger.LogEvent($"{nameof(OnUserMessageReceived)} has been executed by {message.Author.Username}:{message.Author.Id} in {message.Channel.Id} channel");
        }
    }
}
