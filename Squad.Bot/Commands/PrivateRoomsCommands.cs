using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.Logging;

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
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

            if (savedPortal?.CategoryID != null && savedPortal?.ChannelID != null)
            {
                if(Context.Guild.GetCategoryChannel(savedPortal.CategoryID) == null && Context.Guild.GetVoiceChannel(savedPortal.ChannelID) == null && Context.Guild.GetTextChannel(savedPortal.SettingsChannelID) == null)
                {
                    _dbContext.PrivateRooms.Remove(savedPortal);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    var component = new ComponentBuilder()
                                            .WithButton(label: "Delete", customId: "portal:delete", style: ButtonStyle.Danger);

                    var message = await Context.Channel.SendMessageAsync(text: $"{Context.User.Username}, private rooms already created", 
                                                                         components: component.Build(), 
                                                                         flags: MessageFlags.Ephemeral);
                }
            }
            else
            {
                var category = await Context.Guild.CreateCategoryChannelAsync(categoryName);

                //TODO: Понять как поменять права голосовых каналов
                var voiceChannel = await Context.Guild.CreateVoiceChannelAsync(voiceChannelName, tcp => {
                                                                                                            tcp.CategoryId = category.Id;
                                                                                                        });

                var textChannel = await Context.Guild.CreateTextChannelAsync(settingsChannelName, tcp => {
                                                                                                            tcp.CategoryId = category.Id;
                                                                                                            tcp.Topic = "manage private rooms";
                                                                                                         });

                //Buttons
                var rename = new ButtonBuilder().WithCustomId("portal:rename").WithLabel("✏️").WithStyle(ButtonStyle.Secondary);
                var hide = new ButtonBuilder().WithCustomId("portal:hide").WithLabel("🔒").WithStyle(ButtonStyle.Secondary);
                var limit = new ButtonBuilder().WithCustomId("portal:limit").WithLabel("🫂").WithStyle(ButtonStyle.Secondary);
                var kick = new ButtonBuilder().WithCustomId("portal:kick").WithLabel("🚫").WithStyle(ButtonStyle.Secondary);
                var owner = new ButtonBuilder().WithCustomId("portal:owner").WithLabel("👤").WithStyle(ButtonStyle.Secondary);

                //Component with buttons
                var components = new ComponentBuilder().WithButton(rename).WithButton(hide).WithButton(owner).WithButton(limit).WithButton(kick);

                //Final embed
                var embed = new EmbedBuilder().WithAuthor(new EmbedAuthorBuilder().WithName("Private room management")
                                                                                  .WithIconUrl("https://cdn.discordapp.com/emojis/963689541724688404.webp?size=128&quality=lossless"))
                                              .WithDescription("You can change the configuration of your room using interactions." +
                                                               "\n" +
                                                               "\n✏️ — change the name of the room" +
                                                               "\n🔒 — hide/show the room" +
                                                               "\n🫂 — change the user limit" +
                                                               "\n🚫 — kick the participant out of the room" +
                                                               "\n👤 — change the owner of the room")
                                              .WithColor(Color.Default);

                await textChannel.SendMessageAsync(embed: embed.Build());

                var successEmbed = new EmbedBuilder
                {
                    Title = $"✅ **{Context.User.Username}** created private rooms.",
                    Color = 0x9C84EF
                };

                await RespondAsync(embed: successEmbed.Build());
            }
        }

        [SlashCommand("rename", "Change the name of the room")]
        public async Task Rename(string channelName, SocketInteractionContext? context = null)
        {
            await Logger.LogInfo($"rename {channelName}");
        }

        [SlashCommand("hide", "Hide/show the room")]
        public async Task Hide(SocketInteractionContext? context = null)
        {
            await Logger.LogInfo("hide");
        }

        [SlashCommand("kick", "Kick the participant out of the room")]
        public async Task Kick(IUser user, SocketInteractionContext? context = null)
        {
            await Logger.LogInfo($"kick {user}");
        }

        [SlashCommand("limit", "Change the user limit")]
        public async Task Limit(ushort limit = 5, SocketInteractionContext? context = null)
        {
            await Logger.LogInfo($"limit {limit}");
        }

        [SlashCommand("owner", "Change the owner of the room")]
        public async Task Owner(IUser newOwner, SocketInteractionContext? context = null)
        {
            await Logger.LogInfo($"owner {newOwner}");
        }
    }
}
