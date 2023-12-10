using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.Logging;
using Squad.Bot.Models.Base;
using Squad.Bot.Utilities;

namespace Squad.Bot.ComponentsInteraction
{
    public class PrivateRoomsComponents(SquadDBContext dbContext) : InteractionModuleBase<SocketInteractionContext>
    {

        private readonly SquadDBContext _dbContext = dbContext;

        [ComponentInteraction("portal.delete")]
        public async Task Delete()
        {
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

            if(savedPortal != new Models.Base.PrivateRooms())
            {
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
                _dbContext.PrivateRooms.Remove(savedPortal);
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
                await _dbContext.SaveChangesAsync();
            }
            
            // Get the category, voice, and text channels associated with the private room
            var categoryChannel = Context.Guild.GetCategoryChannel(savedPortal.CategoryID);
            var voiceChannel = Context.Guild.GetVoiceChannel(savedPortal.ChannelID);
            var settingsChannel = Context.Guild.GetTextChannel(savedPortal.SettingsChannelID);

            // Delete the category, voice, and text channels on Discord server
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
        [ComponentInteraction("portal.rename:*")]
        public async Task Rename(string channelName)
        {
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user))
            {
                // TODO: add user owner check
                await user.VoiceChannel.ModifyAsync(x => x.Name = channelName);

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
                    Title = "Oooppss, something went wrong...",
                    Description = "This could be due to the fact that you are not in a private room",
                    Color = CustomColors.Failure,
                };

                await RespondAsync(embed: embed.Build(), ephemeral: true);
            }
        }

        [ComponentInteraction("portal.hide")]
        public async Task Hide()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            // TODO: Fill embeds with information
            if (IsUserInPRoom(Context, user))
            {
                if(user.VoiceChannel.PermissionOverwrites.FirstOrDefault().Permissions.ViewChannel == PermValue.Allow || user.VoiceChannel.PermissionOverwrites.FirstOrDefault().Permissions.ViewChannel == PermValue.Inherit)
                {
                    ulong everyoneRoleId = Context.Guild.Roles.First(x => x.Name == "@everyone").Id;

                    var voiceOverwrites = new PermissionOverwriteHelper(everyoneRoleId, PermissionTarget.Role)
                    {
                        Permissions = PermissionOverwriteHelper.SetOverwritePermissions(viewChannel: PermValue.Deny, connect: PermValue.Deny)
                    };
                    await user.VoiceChannel.ModifyAsync(tsp => { tsp.PermissionOverwrites = voiceOverwrites.CreateOverwrites(); });
                    
                    EmbedBuilder embed = new()
                    {
                        Title = "Show/Hide room for everyone",
                        Description = "",
                        Color = CustomColors.Success
                    };

                    await RespondAsync(embed: embed.Build(), ephemeral: true);
                }
                else
                {
                    var voiceOverwrites = new PermissionOverwriteHelper(Context.User.Id, PermissionTarget.User)
                    {
                        Permissions = PermissionOverwriteHelper.SetOverwritePermissions(viewChannel: PermValue.Allow, connect: PermValue.Deny)
                    };
                    await user.VoiceChannel.ModifyAsync(tsp => { tsp.PermissionOverwrites = voiceOverwrites.CreateOverwrites(); });

                    EmbedBuilder embed = new()
                    {
                        Title = "Show/Hide room for everyone",
                        Description = $"{Context.Guild.CurrentUser.Nickname}, ",
                        Color = CustomColors.Success
                    };

                    await RespondAsync(embed: embed.Build(), ephemeral: true);
                }
            }
            else
            {
                var embed = new EmbedBuilder
                {
                    Title = "Oooppss, something went wrong...",
                    Description = "This could be due to the fact that you are not in a private room",
                    Color = CustomColors.Failure,
                };

                await RespondAsync(embed: embed.Build(), ephemeral: true);
            }
        }

        [ComponentInteraction("portal.kick:*")]
        public async Task Kick(IUser userToKick)
        {
            await Logger.LogInfo($"kick {userToKick}");

            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user))
            {

            }
        }

        [ComponentInteraction("portal.limit:*")]
        public async Task Limit(ushort limit)
        {
            await Logger.LogInfo($"limit {limit}");

            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user))
            {

            }
        }

        [ComponentInteraction("portal.owner:*")]
        public async Task Owner(IUser newOwner)
        {
            await Logger.LogInfo($"owner {newOwner}");

            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user))
            {

            }
        }

        [ComponentInteraction("portal.lock")]
        public async Task Lock()
        {
            await Logger.LogInfo($"lock");

            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user))
            {

            }
        }
        private bool IsUserInPRoom(SocketInteractionContext context, SocketGuildUser user)
        {
            var savedPortal = _dbContext.PrivateRooms.FirstOrDefault(x => x.Guilds.Id == Context.Guild.Id);

            if (context.Channel.Id == savedPortal.SettingsChannelID && user.VoiceChannel.CategoryId == savedPortal.CategoryID)
                return true;
            else
                return false;
        }
    }
}
