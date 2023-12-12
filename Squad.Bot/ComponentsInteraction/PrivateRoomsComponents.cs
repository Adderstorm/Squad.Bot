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
            }.WithAuthor(name: Context.Guild.CurrentUser.Nickname ?? Context.User.Username ?? Context.User.GlobalName, iconUrl: Context.Guild.CurrentUser.GetGuildAvatarUrl());

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }

        // TODO: Bug: the bot revokes all channel rights when they are redefined
        // TODO: Change the stubs with the working code(need to learn Modals)
        [ComponentInteraction("portal.hide")]
        public async Task Hide()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            // TODO: Fill embeds with information
            if (IsUserInPRoom(Context, user) && IsUserOwner(user))
            {
                var userChannelPermissions = user.VoiceChannel.PermissionOverwrites.FirstOrDefault().Permissions;

                if(userChannelPermissions.ViewChannel == PermValue.Allow || userChannelPermissions.ViewChannel == PermValue.Inherit)
                {
                    ulong everyoneRoleId = Context.Guild.Roles.First(x => x.Name == "@everyone").Id;

                    var voiceOverwrites = new PermissionOverwriteHelper(everyoneRoleId, PermissionTarget.Role)
                    {
                        Permissions = PermissionOverwriteHelper.SetOverwritePermissions(viewChannel: PermValue.Deny)
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
                        Permissions = PermissionOverwriteHelper.SetOverwritePermissions(viewChannel: PermValue.Allow)
                    };
                    await user.VoiceChannel.ModifyAsync(tsp => { tsp.PermissionOverwrites = voiceOverwrites.CreateOverwrites(); });

                    EmbedBuilder embed = new()
                    {
                        Title = "Show/Hide room for everyone",
                        Description = $"{Context.Guild.CurrentUser.Nickname ?? Context.User.Username ?? Context.User.GlobalName}, ",
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
                    Description = "This could be due to the fact that you are not in a private room or not the owner",
                    Color = CustomColors.Failure,
                };

                await RespondAsync(embed: embed.Build(), ephemeral: true);
            }
        }
        
        [ComponentInteraction("portal.kick")]
        public async Task Kick()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user) && IsUserOwner(user))
            {
                
            }
        }

        [ComponentInteraction("portal.owner")]
        public async Task Owner()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user) && IsUserOwner(user))
            {

            }
        }

        [ComponentInteraction("portal.lock")]
        public async Task Lock()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user) && IsUserOwner(user))
            {
                var userChannelPermissions = user.VoiceChannel.PermissionOverwrites.FirstOrDefault().Permissions;

                if (userChannelPermissions.Connect == PermValue.Allow || userChannelPermissions.Connect == PermValue.Inherit)
                {
                    ulong everyoneRoleId = Context.Guild.Roles.First(x => x.Name == "@everyone").Id;

                    var voiceOverwrites = new PermissionOverwriteHelper(everyoneRoleId, PermissionTarget.Role)
                    {
                        Permissions = PermissionOverwriteHelper.SetOverwritePermissions(connect: PermValue.Deny)
                    };
                    await user.VoiceChannel.ModifyAsync(tsp => { tsp.PermissionOverwrites = voiceOverwrites.CreateOverwrites(); });

                    EmbedBuilder embed = new()
                    {
                        Title = "Lock/unlock room for everyone",
                        Description = "",
                        Color = CustomColors.Success
                    };

                    await RespondAsync(embed: embed.Build(), ephemeral: true);
                }
                else
                {
                    ulong everyoneRoleId = Context.Guild.Roles.First(x => x.Name == "@everyone").Id;

                    var voiceOverwrites = new PermissionOverwriteHelper(everyoneRoleId, PermissionTarget.Role)
                    {
                        Permissions = PermissionOverwriteHelper.SetOverwritePermissions(connect: PermValue.Allow)
                    };
                    await user.VoiceChannel.ModifyAsync(tsp => { tsp.PermissionOverwrites = voiceOverwrites.CreateOverwrites(); });

                    EmbedBuilder embed = new()
                    {
                        Title = "Lock/unlock room for everyone",
                        Description = "",
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
                    Description = "This could be due to the fact that you are not in a private room or not the owner",
                    Color = CustomColors.Failure,
                };

                await RespondAsync(embed: embed.Build(), ephemeral: true);
            }
        }

        [ComponentInteraction("portal.limit")]
        public async Task Limit()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user) && IsUserOwner(user))
            {
                await RespondWithModalAsync<LimitModal>("changeLimit");
            }
            else
            {
                var embed = new EmbedBuilder
                {
                    Title = "Oooppss, something went wrong...",
                    Description = "This could be due to the fact that you are not in a private room or not the owner",
                    Color = CustomColors.Failure,
                };

                await RespondAsync(embed: embed.Build(), ephemeral: true);
            }
        }

        [ComponentInteraction("portal.rename")]
        public async Task Rename()
        {
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user) && IsUserOwner(user))
            {
                await RespondWithModalAsync<RenameModal>("RenameModal");
            }
            else
            {
                var embed = new EmbedBuilder
                {
                    Title = "Oooppss, something went wrong...",
                    Description = "This could be due to the fact that you are not in a private room or not the owner",
                    Color = CustomColors.Failure,
                };

                await RespondAsync(embed: embed.Build(), ephemeral: true);
            }
        }

        [ModalInteraction("renameModal")]
        public async Task RenameModalInteraction(RenameModal modal)
        {

            await Context.Guild.CurrentUser.VoiceChannel.ModifyAsync(x => x.Name = modal.ChannelName);

            var embed = new EmbedBuilder
            {
                Title = "Channel was successfully renamed",
                Color = CustomColors.Success,
            };

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }

        [ModalInteraction("changeLimit")]
        public async Task ChangeLimitInteraction(LimitModal modal)
        {
            await Context.Guild.CurrentUser.VoiceChannel.ModifyAsync(x => x.UserLimit = modal.Limit);

            var embed = new EmbedBuilder
            {
                Title = "Limit was successfully changed",
                Color = CustomColors.Success,
            };

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
        private bool IsUserInPRoom(SocketInteractionContext context, SocketGuildUser user)
        {
            var savedPortal = _dbContext.PrivateRooms.FirstOrDefault(x => x.Guilds.Id == Context.Guild.Id);

            if (context.Channel.Id == savedPortal.SettingsChannelID && user.VoiceChannel.CategoryId == savedPortal.CategoryID)
                return true;
            else
                return false;
        }
        private static bool IsUserOwner(SocketGuildUser user)
        {
            var permissions = user.VoiceChannel.GetPermissionOverwrite(user);
            if (permissions != null && permissions.Value.ManageChannel == PermValue.Allow)
                return true;
            else
                return false;
        }

        public class RenameModal : IModal
        {
            public string Title => "Rename channel";

            [RequiredInput]
            [InputLabel("New name")]
            [ModalTextInput("rename.new_name", style: TextInputStyle.Paragraph, placeholder: "You can use {game}, {username} or any other text name you want", maxLength: 100)]
            public string ChannelName { get; set; } = null!;
        }

        public class LimitModal : IModal
        {
            public string Title => "Change channel limit";

            [RequiredInput]
            [InputLabel("New limit")]
            [ModalTextInput("limit.new_limit", maxLength: 2)]
            public int Limit { get; set; }
        }
    }
}
