using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.Logging;
using Squad.Bot.Utilities;

namespace Squad.Bot.Components
{
    public class PrivateRoomsComponents(SquadDBContext dbContext, Logger logger) : InteractionModuleBase<SocketInteractionContext>
    {

        private readonly SquadDBContext _dbContext = dbContext;
        private readonly Logger _logger = logger;

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

        // TODO: Untested code

        [ComponentInteraction("portal.hide")]
        public async Task Hide()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user) && IsUserOwner(user))
            {
                var everyoneRole = Context.Guild.Roles.First(x => x.IsEveryone);

                var ChannelPermissions = user.VoiceChannel.GetPermissionOverwrite(everyoneRole) ?? new();

                if (ChannelPermissions.ViewChannel == PermValue.Allow || 
                    ChannelPermissions.ViewChannel == PermValue.Inherit)
                {
                    ChannelPermissions = ChannelPermissions.Modify(viewChannel: PermValue.Deny);

                    await user.VoiceChannel.AddPermissionOverwriteAsync(everyoneRole, ChannelPermissions);

                    EmbedBuilder embed = new()
                    {
                        Title = "Show/Hide room for everyone",
                        Description = $"{Context.Guild.CurrentUser.Nickname ?? Context.User.Username ?? Context.User.GlobalName}, voice channel is hidden",
                        Color = CustomColors.Success
                    };

                    await RespondAsync(embed: embed.Build(), ephemeral: true);
                }
                else
                {
                    ChannelPermissions = ChannelPermissions.Modify(viewChannel: PermValue.Allow);

                    await user.VoiceChannel.AddPermissionOverwriteAsync(everyoneRole, ChannelPermissions);

                    EmbedBuilder embed = new()
                    {
                        Title = "Show/Hide room for everyone",
                        Description = $"{Context.Guild.CurrentUser.Nickname ?? Context.User.Username ?? Context.User.GlobalName}, voice channel is publicly available",
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
                // TODO: waiting when Discord API devs updates select menus(not sure)
                //var connectedUsers = Context.Guild.GetVoiceChannel(user.VoiceChannel.Id).ConnectedUsers;

                var selectMenus = new SelectMenuBuilder(customId: "portal.Kick.Select",
                                                        placeholder: "Select member to kick",
                                                        minValues: 1, maxValues: 1)
                    .WithType(ComponentType.UserSelect)
                    .WithDefaultValues(SelectMenuDefaultValue.FromUser(user));
                  //.WithUserTypes(connectedUsers);

                var components = new ComponentBuilder()
                    .WithSelectMenu(selectMenus);

                await RespondAsync(components: components.Build(), ephemeral: true);
            }
        }

        [ComponentInteraction("portal.owner")]
        public async Task Owner()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user) && IsUserOwner(user))
            {
                // TODO: waiting when Discord API devs updates select menus(not sure)
                //var connectedUsers = Context.Guild.GetVoiceChannel(user.VoiceChannel.Id).ConnectedUsers;

                var selectMenus = new SelectMenuBuilder(customId: "portal.Owner.Select",
                                                        placeholder: "Select new channel owner",
                                                        minValues: 1, maxValues: 1,
                                                        defaultValues: [SelectMenuDefaultValue.FromUser(user)])
                    .WithType(ComponentType.UserSelect);
                    //.WithUserTypes(connectedUsers);

                var components = new ComponentBuilder()
                    .WithSelectMenu(selectMenus);

                await RespondAsync(components: components.Build(), ephemeral: true);
            }
        }

        [ComponentInteraction("portal.lock")]
        public async Task Lock()
        {
            var user = Context.Guild.GetUser(Context.User.Id);


            if (IsUserInPRoom(Context, user) && IsUserOwner(user))
            {
                var everyoneRole = Context.Guild.Roles.First(x => x.IsEveryone);

                var ChannelPermissions = user.VoiceChannel.GetPermissionOverwrite(everyoneRole) ?? new();

                if (ChannelPermissions.Connect == PermValue.Allow || ChannelPermissions.Connect == PermValue.Inherit)
                {
                    ChannelPermissions = ChannelPermissions.Modify(connect: PermValue.Deny);

                    await user.VoiceChannel.AddPermissionOverwriteAsync(everyoneRole, ChannelPermissions);

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
                    ChannelPermissions = ChannelPermissions.Modify(connect: PermValue.Allow);

                    await user.VoiceChannel.AddPermissionOverwriteAsync(everyoneRole, ChannelPermissions);

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
            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user) && IsUserOwner(user))
            {
                await RespondWithModalAsync<RenameModal>("renameModal");
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

        [ComponentInteraction("portal.Owner.Select")]
        public async Task OwnerSelect(string[] selectedUsers)
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user) && IsUserOwner(user))
            {
                ulong selectedUserId;
                try
                {
                    selectedUserId = Convert.ToUInt64(selectedUsers.First());
                }
                catch (Exception ex)
                {
                    _logger.LogError(message: "An error occurred while converting string to int from discord's interaction.({funcname})", ex: ex, "OwnerSelect");

                    var embed = new EmbedBuilder
                    {
                        Title = "Error",
                        Description = "Oooppss, something went wrong...",
                        Color = CustomColors.Failure,
                    }
                    .WithAuthor(Context.User)
                    .AddField(name: "This could be due to the fact that discord broke something... or update)", value: ex.Message + ex.GetType());

                    await RespondAsync(embed: embed.Build(), ephemeral: true);
                    return;
                }

                var voiceChannel = Context.Guild.GetVoiceChannel(user.VoiceChannel.Id);
                var connectedUsers = voiceChannel.ConnectedUsers.ToHashSet();
                var selectedUser = Context.Guild.GetUser(selectedUserId);

                if (selectedUserId != user.Id && connectedUsers.Contains(selectedUser))
                {
                    var oldChannelPermissions = user.VoiceChannel.GetPermissionOverwrite(user) ?? new();

                    oldChannelPermissions = oldChannelPermissions.Modify(muteMembers: PermValue.Inherit,
                                                                         manageChannel: PermValue.Inherit);

                    var newChannelPermissions = user.VoiceChannel.GetPermissionOverwrite(selectedUser) ?? new();

                    newChannelPermissions = newChannelPermissions.Modify(muteMembers: PermValue.Allow,
                                                                         manageChannel: PermValue.Allow);

                    await user.VoiceChannel.AddPermissionOverwriteAsync(user, oldChannelPermissions);
                    await user.VoiceChannel.AddPermissionOverwriteAsync(selectedUser, newChannelPermissions);

                    var embed = new EmbedBuilder
                    {
                        Title = "Owner change success",
                        Description = $"New owner is {selectedUser.DisplayName ?? selectedUser.Username}",
                        Color = CustomColors.Success,
                    }.WithAuthor(selectedUser);

                    await RespondAsync(embed: embed.Build(), ephemeral: true);
                }
                else
                {
                    var embed = new EmbedBuilder
                    {
                        Title = "Owner change failure",
                        Description = "The selected user is currently the owner or the selected user is not in a private room",
                        Color = CustomColors.Failure,
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

        [ComponentInteraction("portal.Kick.Select")]
        public async Task KickSelect(string[] selectedUsers)
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user) && IsUserOwner(user))
            {
                ulong selectedUserId;
                try
                {
                    selectedUserId = Convert.ToUInt64(selectedUsers.First());
                }
                catch (Exception ex)
                {
                    _logger.LogError(message: "An error occurred while converting string to int from discord's interaction.({funcname})", ex: ex, "KickSelect");

                    var embed = new EmbedBuilder
                    {
                        Title = "Error",
                        Description = "Oooppss, something went wrong...",
                        Color = CustomColors.Failure,
                    }
                    .WithAuthor(Context.User)
                    .AddField(name: "This could be due to the fact that discord broke something... or update)", value: ex.Message + ex.GetType());

                    await RespondAsync(embed: embed.Build(), ephemeral: true);
                    return;
                }

                var voiceChannel = Context.Guild.GetVoiceChannel(user.VoiceChannel.Id);
                var connectedUsers = voiceChannel.ConnectedUsers.ToHashSet();
                var selectedUser = Context.Guild.GetUser(selectedUserId);

                if (selectedUserId != user.Id && connectedUsers.Contains(selectedUser))
                {
                    await selectedUser.ModifyAsync(x => x.Channel = null);

                    var embed = new EmbedBuilder
                    {
                        Title = "Kick user success",
                        Description = $"{selectedUser.DisplayName ?? selectedUser.Username}'s was kicked from your private channel",
                        Color = CustomColors.Success,
                    }.WithAuthor(selectedUser);

                    await RespondAsync(embed: embed.Build(), ephemeral: true);
                }
                else
                {
                    var embed = new EmbedBuilder
                    {
                        Title = "Kick user failure",
                        Description = "The selected user is currently the owner or the selected user is not in a private room",
                        Color = CustomColors.Failure,
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

        [ModalInteraction("renameModal")]
        public async Task RenameModalInteraction(RenameModal modal)
        {
            await Context.Guild.GetUser(Context.User.Id).VoiceChannel.ModifyAsync(x => x.Name = modal.ChannelName);

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
            ushort numberLimit = 5;
            try
            {
                numberLimit = Convert.ToUInt16(modal.Limit);
            }
            catch
            {
                var embedError = new EmbedBuilder
                {
                    Title = "Error",
                    Description = "Not a number in limit input ro negative number",
                    Color = CustomColors.Failure,
                };

                await RespondAsync(embed: embedError.Build(), ephemeral: true);
                return;
            }

            await Context.Guild.GetUser(Context.User.Id).VoiceChannel.ModifyAsync(x => x.UserLimit = numberLimit);

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
            public string Limit { get; set; } = null!;
        }
    }
}
