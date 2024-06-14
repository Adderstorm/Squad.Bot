using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Squad.Bot.Data;

namespace Squad.Bot.FunctionalModules.Preconditions
{
    public class IsUserInPRoomAttribute : PreconditionAttribute
    {
        public new const string ErrorMessage = "You are not in a private room";
        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
#pragma warning disable CS8600 // Converting a literal that allows a NULL value or a possible NULL value to a type that does not allow a NULL value.
            SquadDBContext dbContext = services.GetService<SquadDBContext>();
#pragma warning restore CS8600 // Converting a literal that allows a NULL value or a possible NULL value to a type that does not allow a NULL value.

            var savedPortal = dbContext.PrivateRooms.FirstOrDefault(x => x.Guilds.Id == context.Guild.Id);
            var user = await context.Guild.GetUserAsync(context.User.Id);

#pragma warning disable S2259 // Null pointers should not be dereferenced
            if (context.Channel.Id == savedPortal.SettingsChannelID && user.VoiceChannel?.CategoryId == savedPortal.CategoryID)
                return PreconditionResult.FromSuccess();
            else
                return PreconditionResult.FromError(ErrorMessage);
#pragma warning restore S2259 // Null pointers should not be dereferenced
        }
    }
}
