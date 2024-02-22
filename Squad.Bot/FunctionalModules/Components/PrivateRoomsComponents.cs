using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.FunctionalModules.Modals;
using Squad.Bot.FunctionalModules.Preconditions;
using Squad.Bot.Logging;
using Squad.Bot.Utilities;

namespace Squad.Bot.FunctionalModules.Components
{
    public partial class PrivateRooms : InteractionModuleBase<SocketInteractionContext>
    {

        private readonly SquadDBContext _dbContext;
        private readonly Logger _logger;

        public PrivateRooms(SquadDBContext dbContext, Logger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [ComponentInteraction("portal.delete")]
        public async Task Delete()
        {
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

            if (savedPortal != new Models.Base.PrivateRooms())
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
            if (voiceChannel != null)
                await voiceChannel.DeleteAsync();
            if (settingsChannel != null)
                await settingsChannel.DeleteAsync();
            if (categoryChannel != null)
                await categoryChannel.DeleteAsync();

            var embed = new EmbedBuilder()
            {
                Title = "Portals was successfully deleted",
                Description = "Now you can use again /private_rooms invite",
                Color = CustomColors.Success,
            }.WithAuthor(Context.Guild.CurrentUser);

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }

        [ComponentInteraction("portal.hide")]
        [IsUserInPRoom]
        [IsUserOwner]
        public async Task Hide()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

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

        [ComponentInteraction("portal.kick")]
        [IsUserInPRoom]
        [IsUserOwner]
        public async Task Kick()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            // waiting when Discord API devs updates select menus(not sure)
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

        [ComponentInteraction("portal.owner")]
        [IsUserInPRoom]
        [IsUserOwner]
        public async Task Owner()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            // waiting when Discord API devs updates select menus(not sure)
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

        [ComponentInteraction("portal.lock")]
        [IsUserInPRoom]
        [IsUserOwner]
        public async Task Lock()
        {
            var user = Context.Guild.GetUser(Context.User.Id);

            var everyoneRole = Context.Guild.Roles.First(x => x.IsEveryone);

            var ChannelPermissions = user.VoiceChannel.GetPermissionOverwrite(everyoneRole) ?? new();

            if (ChannelPermissions.Connect == PermValue.Allow || ChannelPermissions.Connect == PermValue.Inherit)
            {
                ChannelPermissions = ChannelPermissions.Modify(connect: PermValue.Deny);

                await user.VoiceChannel.AddPermissionOverwriteAsync(everyoneRole, ChannelPermissions);
            }
            else
            {
                ChannelPermissions = ChannelPermissions.Modify(connect: PermValue.Allow);

                await user.VoiceChannel.AddPermissionOverwriteAsync(everyoneRole, ChannelPermissions);
            }

            EmbedBuilder embed = new()
            {
                Title = "Lock/unlock room for everyone",
                Description = "",
                Color = CustomColors.Success
            };

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }

        [ComponentInteraction("portal.limit")]
        [IsUserInPRoom]
        [IsUserOwner]
        public async Task Limit()
        {
            await RespondWithModalAsync<LimitModal>("changeLimit");
        }

        [ComponentInteraction("portal.rename")]
        [IsUserInPRoom]
        [IsUserOwner]
        public async Task Rename()
        {
            await RespondWithModalAsync<RenameModal>("renameModal");
        }

        [ComponentInteraction("portal.Owner.Select")]
        [IsUserInPRoom]
        [IsUserOwner]
        public async Task OwnerSelect(string[] selectedUsers)
        {
            var user = Context.Guild.GetUser(Context.User.Id);

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
                .WithAuthor(Context.Guild.CurrentUser)
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
                    Description = "The selected user is currently the owner or not in a private room",
                    Color = CustomColors.Failure,
                };

                await RespondAsync(embed: embed.Build(), ephemeral: true);
            }
        }

        [ComponentInteraction("portal.Kick.Select")]
        [IsUserInPRoom]
        [IsUserOwner]
        public async Task KickSelect(string[] selectedUsers)
        {
            var user = Context.Guild.GetUser(Context.User.Id);

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
    }
}
