using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Squad.Bot.Data;

namespace Squad.Bot.FunctionalModules.Preconditions
{
    public class IsUserInPRoom : PreconditionAttribute
    {
        public new string ErrorMessage = "You are not in a private room";
        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
#pragma warning disable CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
            SquadDBContext dbContext = services.GetService<SquadDBContext>();
#pragma warning restore CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.

            var savedPortal = dbContext.PrivateRooms.FirstOrDefault(x => x.Guilds.Id == context.Guild.Id);
            var user = await context.Guild.GetUserAsync(context.User.Id);

            if (context.Channel.Id == savedPortal.SettingsChannelID && user.VoiceChannel?.CategoryId == savedPortal.CategoryID)
                return PreconditionResult.FromSuccess();
            else
                return PreconditionResult.FromError(ErrorMessage);
        }
    }
}
