using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.Logging;

namespace Squad.Bot.ComponentsInteraction
{
    public class PrivateRoomsComponents(SquadDBContext dbContext) : InteractionModuleBase<SocketInteractionContext>
    {

        private readonly SquadDBContext _dbContext = dbContext;

        [ComponentInteraction("portal:delete")]
        public async Task Delete()
        {
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
            _dbContext.PrivateRooms.Remove(savedPortal);
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.

            await _dbContext.SaveChangesAsync();

            
            // Get the category, voice, and text channels associated with the private room
            var categoryChannel = Context.Guild.GetCategoryChannel(savedPortal.CategoryID);
            var voiceChannel = Context.Guild.GetVoiceChannel(savedPortal.ChannelID);
            var settingsChannel = Context.Guild.GetTextChannel(savedPortal.SettingsChannelID);

            // Delete the category, voice, and text channels
            await categoryChannel.DeleteAsync();
            await voiceChannel.DeleteAsync();
            await settingsChannel.DeleteAsync();

            var embed = new EmbedBuilder()
                .WithAuthor(Context.User.Username)
                .WithColor(0x9C84EF)
                .WithDescription("Portals was successfully deleted");

            await ModifyOriginalResponseAsync(x => { x.Content = null; x.Embed = embed.Build(); });
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
