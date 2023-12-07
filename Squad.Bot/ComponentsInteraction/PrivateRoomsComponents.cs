using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.Logging;
using Squad.Bot.Utilities;

namespace Squad.Bot.ComponentsInteraction
{
    public class PrivateRoomsComponents(SquadDBContext dbContext) : InteractionModuleBase<SocketInteractionContext>
    {

        private readonly SquadDBContext _dbContext = dbContext;

        [ComponentInteraction("portal:delete")]
        public async Task Delete()
        {
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

            if(savedPortal != null)
            {
                _dbContext.PrivateRooms.Remove(savedPortal);
                await _dbContext.SaveChangesAsync();
            }
            
            // Get the category, voice, and text channels associated with the private room
            var categoryChannel = Context.Guild.GetCategoryChannel(savedPortal.CategoryID);
            var voiceChannel = Context.Guild.GetVoiceChannel(savedPortal.ChannelID);
            var settingsChannel = Context.Guild.GetTextChannel(savedPortal.SettingsChannelID);

            // Delete the category, voice, and text channels
            if(voiceChannel != null)
                await voiceChannel.DeleteAsync();
            if(settingsChannel != null)
                await settingsChannel.DeleteAsync();
            if(categoryChannel != null)
                await categoryChannel.DeleteAsync();

            var embed = new EmbedBuilder()
            {
                Title = "Portals was successfully deleted",
                Description = "Now you can use again /private_rooms invite",
                Color = CustomColors.Success,
            }.WithAuthor(name: Context.Guild.CurrentUser.Nickname, iconUrl: Context.Guild.CurrentUser.GetGuildAvatarUrl());

            await ModifyOriginalResponseAsync(x => { x.Content = null; x.Embed = embed.Build(); });
        }

        // TODO: Change the stubs with the working code
        [ComponentInteraction("portal:rename")]
        public async Task Rename(string channelName)
        {
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

            if (Context.Guild.CurrentUser.VoiceChannel.CategoryId == savedPortal.CategoryID)
            {
                // TODO: add user owner check
                await Context.Guild.CurrentUser.VoiceChannel.ModifyAsync(x => x.Name = channelName);

                var embed = new EmbedBuilder
                {
                    Title = "Channel was successfully renamed",
                    Color = CustomColors.Success,
                };

                await RespondAsync(embed: embed.Build(), ephemeral: true);
            }
            else
            {
                var embed = new EmbedBuilder
                {
                    Title = "Something went wrong...",
                    Description = "This could be due to the fact that you are not in a private room",
                    Color = CustomColors.Failure,
                };
            }
        }

        [ComponentInteraction("portal:hide")]
        public async Task Hide()
        {
            await Logger.LogInfo("hide");
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

            if (Context.Channel.Id == savedPortal.SettingsChannelID && Context.Guild.CurrentUser.VoiceChannel.CategoryId == savedPortal.CategoryID)
            {

            }
        }

        [ComponentInteraction("portal:kick")]
        public async Task Kick(IUser user)
        {
            await Logger.LogInfo($"kick {user}");
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

            if (Context.Channel.Id == savedPortal.SettingsChannelID && Context.Guild.CurrentUser.VoiceChannel.CategoryId == savedPortal.CategoryID)
            {

            }
        }

        [ComponentInteraction("portal:limit")]
        public async Task Limit(ushort limit)
        {
            await Logger.LogInfo($"limit {limit}");
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

            if (Context.Channel.Id == savedPortal.SettingsChannelID && Context.Guild.CurrentUser.VoiceChannel.CategoryId == savedPortal.CategoryID)
            {

            }
        }

        [ComponentInteraction("portal:owner")]
        public async Task Owner(IUser newOwner)
        {
            await Logger.LogInfo($"owner {newOwner}");
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

            if (Context.Channel.Id == savedPortal.SettingsChannelID && Context.Guild.CurrentUser.VoiceChannel.CategoryId == savedPortal.CategoryID)
            {

            }
        }
    }
}
