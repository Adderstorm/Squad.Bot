using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Squad.Bot.Data;
using Squad.Bot.Utilities;

namespace Squad.Bot.FunctionalModules.Commands
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
        public async Task Invite(string voiceChannelName = "➕・Create", string settingsChannelName = "⚙️・Settings", string categoryName = "Portal")
        {
        N:
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

            if (savedPortal?.CategoryID != null && savedPortal?.ChannelID != null)
            {
                if (Context.Guild.GetCategoryChannel(savedPortal.CategoryID) == null && Context.Guild.GetVoiceChannel(savedPortal.ChannelID) == null && Context.Guild.GetTextChannel(savedPortal.SettingsChannelID) == null)
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

                    await RespondAsync(text: $"{Context.Guild.CurrentUser.Nickname ?? Context.User.Username ?? Context.User.GlobalName}, private rooms already created",
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
                    Permissions = new(viewChannel: PermValue.Allow)
                };
                var voiceOverwrites = new PermissionOverwriteHelper(everyoneRoleId, PermissionTarget.Role)
                {
                    Permissions = new(connect: PermValue.Allow, speak: PermValue.Deny)
                };
                var settingsOverwrites = new PermissionOverwriteHelper(everyoneRoleId, PermissionTarget.Role)
                {
                    Permissions = new()
                };

                var category = await Context.Guild.CreateCategoryChannelAsync(categoryName, tcp => { tcp.PermissionOverwrites = categoryOverwrites.CreateOptionalOverwrites(); });

                var voiceChannel = await Context.Guild.CreateVoiceChannelAsync(voiceChannelName, tcp =>
                {
                    tcp.CategoryId = category.Id;
                    tcp.PermissionOverwrites = voiceOverwrites.CreateOptionalOverwrites();
                });

                var settingsChannel = await Context.Guild.CreateTextChannelAsync(settingsChannelName, tcp =>
                {
                    tcp.CategoryId = category.Id;
                    tcp.Topic = "manage private rooms";
                    tcp.PermissionOverwrites = settingsOverwrites.CreateOptionalOverwrites();
                });

                await _dbContext.PrivateRooms.AddAsync(new Models.Base.PrivateRooms
                {
                    CategoryID = category.Id,
                    ChannelID = voiceChannel.Id,
                    SettingsChannelID = settingsChannel.Id,
                    Guilds = await _dbContext.Guilds.FirstAsync(x => x.Id == Context.Guild.Id)
                });
                await _dbContext.SaveChangesAsync();

                //Buttons
                var rename = new ButtonBuilder().WithCustomId("portal.rename").WithLabel("✏️").WithStyle(ButtonStyle.Secondary);
                var hide = new ButtonBuilder().WithCustomId("portal.hide").WithLabel("🔒").WithStyle(ButtonStyle.Secondary);
                var limit = new ButtonBuilder().WithCustomId("portal.limit").WithLabel("🫂").WithStyle(ButtonStyle.Secondary);
                var kick = new ButtonBuilder().WithCustomId("portal.kick").WithLabel("🚫").WithStyle(ButtonStyle.Secondary);
                var owner = new ButtonBuilder().WithCustomId("portal.owner").WithLabel("👤").WithStyle(ButtonStyle.Secondary);
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
                                  "\n🔒 — lock your room",
                    Color = CustomColors.Default,
                }.WithAuthor(name: "Private room management", iconUrl: "https://cdn.discordapp.com/emojis/963689541724688404.webp?size=128&quality=lossless");

                await settingsChannel.SendMessageAsync(embed: embed.Build(), components: components.Build());

                var successEmbed = new EmbedBuilder
                {
                    Title = $"✅ Created private rooms.",
                    Color = CustomColors.Success,
                }.WithAuthor(name: Context.Guild.CurrentUser.Nickname ?? Context.User.Username ?? Context.User.GlobalName, iconUrl: Context.Guild.CurrentUser.GetGuildAvatarUrl());

                await RespondAsync(embed: successEmbed.Build());
            }
        }
    }
}
