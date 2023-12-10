using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.Logging;
using Squad.Bot.Utilities;
using System.Threading.Tasks;

namespace Squad.Bot.Commands
{
    [Group("private_rooms", "Help you manage private rooms")]
    public class PrivateRoomsCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly SquadDBContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateRoomsCommands"/> class.
        /// </summary>
        /// <param name="context">The interaction context.</param>
        public PrivateRoomsCommands(SquadDBContext context)
        {
            _dbContext = context;
        }


        /// <summary>
        /// Create a portal for the private rooms
        /// </summary>
        /// <param name="voiceChannelName">The name of the voice channel.</param>
        /// <param name="settingsChannelName">The name of the settings channel.</param>
        /// <param name="categoryName">The name of the category.</param>
        [SlashCommand("invite", "Create a portal for the private rooms")]
        [DefaultMemberPermissions(GuildPermission.Administrator)]
        public async Task Invite(string voiceChannelName = "[➕] Create", string settingsChannelName = "[⚙️] Settings", string categoryName = "Portal")
        {
            N:
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

            if (savedPortal?.CategoryID != null && savedPortal?.ChannelID != null)
            {
                if(Context.Guild.GetCategoryChannel(savedPortal.CategoryID) == null && Context.Guild.GetVoiceChannel(savedPortal.ChannelID) == null && Context.Guild.GetTextChannel(savedPortal.SettingsChannelID) == null)
                {
                    _dbContext.PrivateRooms.Remove(savedPortal);
                    await _dbContext.SaveChangesAsync();
                    // UNSAFE: goto may cause many problems in future
                    goto N;
                }
                else
                {
                    var component = new ComponentBuilder()
                                            .WithButton(label: "Delete", customId: "portal.delete", style: ButtonStyle.Danger);

                    await RespondAsync(text: $"{Context.User.Username}, private rooms already created", 
                                                                         components: component.Build(), 
                                                                         ephemeral: true);
                }
            }
            else
            {
                ulong everyoneRoleId = Context.Guild.Roles.First(x => x.Name == "@everyone").Id;

                // Permissions overwrites
                var categoryOverwrites = new PermissionOverwriteHelper(everyoneRoleId, PermissionTarget.Role)
                {
                    Permissions = PermissionOverwriteHelper.SetOverwritePermissions(viewChannel: PermValue.Allow)
                };
                var voiceOverwrites = new PermissionOverwriteHelper(everyoneRoleId, PermissionTarget.Role)
                {
                    Permissions = PermissionOverwriteHelper.SetOverwritePermissions(connect: PermValue.Allow, speak: PermValue.Deny)
                };
                var settingsOverwrites = new PermissionOverwriteHelper(everyoneRoleId, PermissionTarget.Role)
                {
                    Permissions = PermissionOverwriteHelper.SetOverwritePermissions()
                };

                var category = await Context.Guild.CreateCategoryChannelAsync(categoryName, tcp => { tcp.PermissionOverwrites = categoryOverwrites.CreateOverwrites(); });

                var voiceChannel = await Context.Guild.CreateVoiceChannelAsync(voiceChannelName, tcp => {tcp.CategoryId = category.Id;
                                                                                                         tcp.PermissionOverwrites = voiceOverwrites.CreateOverwrites();});

                var settingsChannel = await Context.Guild.CreateTextChannelAsync(settingsChannelName, tcp => {tcp.CategoryId = category.Id;
                                                                                                          tcp.Topic = "manage private rooms";
                                                                                                          tcp.PermissionOverwrites = settingsOverwrites.CreateOverwrites();});

                await _dbContext.PrivateRooms.AddAsync(new Models.Base.PrivateRooms { CategoryID = category.Id, 
                                                                                      ChannelID = voiceChannel.Id, 
                                                                                      SettingsChannelID = settingsChannel.Id, 
                                                                                      Guilds = await _dbContext.Guilds.FirstAsync(x => x.Id == Context.Guild.Id) });
                await _dbContext.SaveChangesAsync();

                //Buttons
                var rename = new ButtonBuilder().WithCustomId("portal.rename:*").WithLabel("✏️").WithStyle(ButtonStyle.Secondary);
                var hide = new ButtonBuilder().WithCustomId("portal.hide").WithLabel("🔒").WithStyle(ButtonStyle.Secondary);
                var limit = new ButtonBuilder().WithCustomId("portal.limit:*").WithLabel("🫂").WithStyle(ButtonStyle.Secondary);
                var kick = new ButtonBuilder().WithCustomId("portal.kick:*").WithLabel("🚫").WithStyle(ButtonStyle.Secondary);
                var owner = new ButtonBuilder().WithCustomId("portal.owner:*").WithLabel("👤").WithStyle(ButtonStyle.Secondary);
                var lock_ = new ButtonBuilder().WithCustomId("portal.lock").WithLabel("👤").WithStyle(ButtonStyle.Secondary);

                //Component with buttons
                var components = new ComponentBuilder().WithButton(rename).WithButton(hide).WithButton(owner).WithButton(limit).WithButton(kick).WithButton(lock_);

                //Final embed
                var embed = new EmbedBuilder()
                {
                    Description = "You can change the configuration of your room using interactions." +
                                  "\n" +
                                  "\n✏️ — change the name of the room" +
                                  "\n👁 — hide/show the room" +
                                  "\n🫂 — change the user limit" +
                                  "\n🚫 — kick the participant out of the room" +
                                  "\n👤 — change the owner of the room" +
                                  "\n🔒 — change the owner of the room",
                    Color = CustomColors.Default,
                }.WithAuthor(name: "Private room management", iconUrl: "https://cdn.discordapp.com/emojis/963689541724688404.webp?size=128&quality=lossless");

                await settingsChannel.SendMessageAsync(embed: embed.Build(), components: components.Build());

                var successEmbed = new EmbedBuilder
                {
                    Title = $"✅ Created private rooms.",
                    Color = CustomColors.Success,
                }.WithAuthor(name: Context.Guild.CurrentUser.Nickname, iconUrl: Context.Guild.CurrentUser.GetGuildAvatarUrl());

                await RespondAsync(embed: successEmbed.Build());
            }
        }

        // TODO: Change the stubs with the working code
        [SlashCommand("rename", "Change the name of the room")]
        public async Task Rename(string channelName)
        {
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
                    Title = "Something went wrong...",
                    Description = "This could be due to the fact that you were not writing in the portal settings channel or you are not in a private room",
                    Color = CustomColors.Failure,
                };
            }
        }

        [SlashCommand("hide", "Hide/show the room")]
        public async Task Hide()
        {
            await Logger.LogInfo("hide");

            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user))
            {

            }
        }

        [SlashCommand("kick", "Kick the participant out of the room")]
        public async Task Kick(IUser userToKick)
        {
            await Logger.LogInfo($"kick {userToKick}");

            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user))
            {

            }
        }

        [SlashCommand("limit", "Change the user limit")]
        public async Task Limit(ushort limit = 5)
        {
            await Logger.LogInfo($"limit {limit}");

            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user))
            {

            }
        }

        [SlashCommand("owner", "Change the owner of the room")]
        public async Task Owner(IUser newOwner)
        {
            await Logger.LogInfo($"owner {newOwner}");

            var user = Context.Guild.GetUser(Context.User.Id);

            if (IsUserInPRoom(Context, user))
            {

            }
        }

        [SlashCommand("lock", "Change the owner of the room")]
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
