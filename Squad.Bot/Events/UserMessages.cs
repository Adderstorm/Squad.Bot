using Discord.WebSocket;
using Squad.Bot.Data;
using Squad.Bot.Logging;

namespace Squad.Bot.Events
{
    public class UserMessages
    {

        private readonly SquadDBContext _dbContext;

        public UserMessages(SquadDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnUserMessageReceived(SocketMessage message)
        {
            await Logger.LogEvent($"{nameof(OnUserMessageReceived)} has been executed by {message.Author.Username}:{message.Author.Id} in {message.Channel.Id} channel");
        }
    }
}
