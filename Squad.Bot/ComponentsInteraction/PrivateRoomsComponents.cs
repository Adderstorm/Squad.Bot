using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.Logging;

namespace Squad.Bot.ComponentsInteraction
{
    public class PrivateRoomsComponents : InteractionModuleBase<SocketInteractionContext>
    {

        private readonly SquadDBContext _dbContext;

        public PrivateRoomsComponents(SquadDBContext dbContext)
        {
            _dbContext = dbContext;
        }


        [ComponentInteraction("portal:delete")]
        public async Task Delete()
        {
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
            _dbContext.PrivateRooms.Remove(savedPortal);
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.

            await _dbContext.SaveChangesAsync();

            //TODO: Понять как удалить существующие каналы на серваке дискорда
        }

        [ComponentInteraction("portal:rename")]
        public async Task Rename(string channelName)
        {
            await Logger.LogInfo($"rename {channelName}");
        }

        [ComponentInteraction("portal:hide")]
        public async Task Hide()
        {
            await Logger.LogInfo("hide");
        }
        [ComponentInteraction("portal:kick")]
        public async Task Kick(IUser user)
        {
            await Logger.LogInfo($"kick {user}");
        }
        [ComponentInteraction("portal:limit")]
        public async Task Limit(ushort limit)
        {
            await Logger.LogInfo($"limit {limit}");
        }
        [ComponentInteraction("portal:owner")]
        public async Task Owner(IUser newOwner)
        {
            await Logger.LogInfo($"owner {newOwner}");
        }
    }
}
