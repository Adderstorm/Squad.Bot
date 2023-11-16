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

        public PrivateRoomsCommands(SquadDBContext context)
        {
            _dbContext = context;
        }


        [SlashCommand("invite", "Create a portal for the private rooms")]
        [DefaultMemberPermissions(GuildPermission.Administrator)]
        public async Task Invite(string channelName = "[➕] Create", string categoryName = "Portals")
        {
            var savedPortal = await _dbContext.PrivateRooms.FirstOrDefaultAsync(x => x.Guilds.Id == Context.Guild.Id);

            if (savedPortal?.CategoryID != null && savedPortal?.ChannelID != null)
            {
                if(Context.Guild.GetCategoryChannel(savedPortal.CategoryID) == null && Context.Guild.GetChannel(savedPortal.ChannelID) == null)
                {
                    _dbContext.PrivateRooms.Remove(savedPortal);
                }
                else
                {
                    var component = new ComponentBuilder()
                                            .WithButton(label: "Delete", customId: "portal:delete", style: ButtonStyle.Danger);

                    var message = await Context.Channel.SendMessageAsync(text: $"{Context.User.Username}, private rooms already created", components: component.Build());
                }
            }

            await Logger.LogInfo($"invite {channelName} {categoryName}");
        }

        [SlashCommand("rename", "Rename your own channel name")]
        public async Task Rename(string channelName)
        {
            await Logger.LogInfo($"rename {channelName}");
        }

        [SlashCommand("hide", "Hide or show your own voice channel")]
        public async Task Hide()
        {
            await Logger.LogInfo("hide");
        }

        [SlashCommand("kick", "Kick an abussive member")]
        public async Task Kick(IUser user)
        {
            await Logger.LogInfo($"kick {user}");
        }

        [SlashCommand("limit", "Set a limit for the voice channel")]
        public async Task Limit(ushort limit = 5)
        {
            await Logger.LogInfo($"limit {limit}");
        }

        [SlashCommand("owner", "Set a new channel owner")]
        public async Task Owner(IUser newOwner)
        {
            await Logger.LogInfo($"owner {newOwner}");
        }
        [ComponentInteraction("portal:delete")]
        public async Task Delete()
        {
            
        }
    }
}
