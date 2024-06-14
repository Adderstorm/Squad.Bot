using Discord;
using Discord.Interactions;

namespace Squad.Bot.FunctionalModules.Preconditions
{
    public class IsUserOwnerAttribute : PreconditionAttribute
    {
        public new const string ErrorMessage = "You are not the owner";
        public async override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            var user = await context.Guild.GetUserAsync(context.User.Id);

            var permissions = user.VoiceChannel?.GetPermissionOverwrite(user);

            if (permissions != null && permissions.Value.ManageChannel == PermValue.Allow)
                return PreconditionResult.FromSuccess();
            else
                return PreconditionResult.FromError(ErrorMessage);
        }
    }
}
