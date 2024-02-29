using Discord;
using Discord.Interactions;
using Squad.Bot.Data;
using Squad.Bot.Logging;
using Squad.Bot.Utilities;

namespace Squad.Bot.FunctionalModules.Modals.ModalsInteraction
{
    public class PrivateRoomsModals : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly Logger _logger;

        public PrivateRoomsModals(Logger logger)
        {
            _logger = logger;
        }

        [ModalInteraction("renameModal")]
        public async Task RenameModalInteraction(RenameModal modal)
        {
            await Context.Guild.GetUser(Context.User.Id).VoiceChannel.ModifyAsync(x => x.Name = modal.ChannelName.Replace("{game}", Context.User.Activities.First().Name).Replace("{username}", Context.Guild.CurrentUser.Nickname ?? Context.User.Username ?? Context.User.GlobalName));

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
                    Description = "Not a number in limit input or negative number",
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
    }
}
