using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Squad.Bot.Data;
using Squad.Bot.Models.Base;

namespace Squad.Bot.FunctionalModules.Preconditions
{
    public class IsUserOwner : PreconditionAttribute
    {
        private string USER_NOT_OWNER = "You are not the owner";
        public async override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            var user = await context.Guild.GetCurrentUserAsync();

            var permissions = user.VoiceChannel.GetPermissionOverwrite(user);

            if (permissions != null && permissions.Value.ManageChannel == PermValue.Allow)
                return PreconditionResult.FromSuccess();
            else
                return PreconditionResult.FromError(USER_NOT_OWNER);
        }
    }
}
