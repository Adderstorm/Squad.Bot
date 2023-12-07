using Discord;
using Discord.Interactions;
using Squad.Bot.Logging;
using Squad.Bot.Utilities;

namespace Squad.Bot.Commands
{
    /// <summary>
    /// A class containing commands for moderation.
    /// </summary>
    [Group("moderation", "commands for moderators")]
    public class Moderation : InteractionModuleBase<SocketInteractionContext>
    {
        /// <summary>
        /// Kicks a user from the server.
        /// </summary>
        /// <param name="user">The user to kick.</param>
        /// <param name="reason">The reason for the kick.</param>
        /// <remarks>
        /// This command requires the <c>Kick Members</c> permission in the server.
        /// If the user has the <c>Administrator</c> permission, the command will fail.
        /// </remarks>
        /// <example>
        /// !kick @JohnDoe Spamming
        /// </example>
        [SlashCommand("kick", "kick user")]
        [DefaultMemberPermissions(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [EnabledInDm(false)]
        public async Task Kick(IUser user, string Reason)
        {
            var member = Context.Guild.GetUser(user.Id);
            if (member.GuildPermissions.Administrator)
            {
                Embed embed = new EmbedBuilder
                {
                    Title = "Error!",
                    Description = "User has Admin permissions.",
                    Color = CustomColors.Failure,
                }.Build();
                await RespondAsync("", embed: embed);
            }
            else
            {
                try
                {
                    var embed = new EmbedBuilder
                    {
                        Title = "User Kicked!",
                        Description = $"**{member}** was kicked by **{Context.User.Username}**!",
                        Color = CustomColors.Success,
                    }.AddField("Reason:", Reason);

                    await member.KickAsync(Reason);
                    await RespondAsync("", embed: embed.Build());
                    try
                    {
                        await member.SendMessageAsync($"You were kicked by **{Context.User.Username}**!\nReason: {Reason}");
                    }
                    catch { /*Couldn't send a message in the private messages of the user*/}
                }
                catch (Exception ex)
                {
                    await Logger.LogException(ex);
                    var embed = new EmbedBuilder
                    {
                        Title = "Error!",
                        Description = "An error occurred while trying to kick the user. Make sure my role is above the role of the user you want to kick.",
                        Color = CustomColors.Failure,
                    };

                    await RespondAsync("", embed: embed.Build());
                }
            }
        }

        /// <summary>
        /// Changes the nickname of a user in the server.
        /// </summary>
        /// <param name="user">The user to change the nickname of.</param>
        /// <param name="nickname">The new nickname for the user.</param>
        /// <remarks>
        /// This command requires the <c>Manage Nicknames</c> permission in the server.
        /// If the user has the <c>Administrator</c> permission, the command will fail.
        /// </remarks>
        /// <example>
        /// !nick @JohnDoe John
        /// </example>
        [SlashCommand("nick", "changes nick")]
        [DefaultMemberPermissions(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [EnabledInDm(false)]
        public async Task Nick(IUser user, string nickname)
        {

            var member = Context.Guild.GetUser(user.Id);
            try
            {
                var embed = new EmbedBuilder
                {
                    Title = "Changed Nickname!",
                    Description = $"**{member.Nickname}'s** new nickname is **{nickname}**!",
                    Color = CustomColors.Success
                }.Build();

                await member.ModifyAsync(x => x.Nickname = nickname);
                await RespondAsync(embed: embed, options: new RequestOptions() { Timeout = 35000 });
            }
            catch (Exception ex)
            {
                await Logger.LogException(ex);
                var embed = new EmbedBuilder
                {
                    Title = "Error!",
                    Description = "An error occurred while trying to change the nickname of the user. Make sure my role is above the role of the user you want to change the nickname.",
                    Color = CustomColors.Failure
                };
                await RespondAsync(embed: embed.Build());
            }
        }

        /// <summary>
        /// Bans a user from the server.
        /// </summary>
        /// <param name="user">The user to ban.</param>
        /// <param name="reason">The reason for the ban.</param>
        /// <param name="notify">Whether to notify the user.</param>
        /// <remarks>
        /// This command requires the <c>Ban Members</c> permission in the server.
        /// If the user has the <c>Administrator</c> permission, the command will fail.
        /// </remarks>
        /// <example>
        /// !ban @JohnDoe Spamming true
        /// </example>
        [SlashCommand("ban", "ban user")]
        [DefaultMemberPermissions(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [EnabledInDm(false)]
        public async Task Ban(IUser user, string reason, bool notify = true)
        {
            var member = Context.Guild.GetUser(user.Id);
            if (member.GuildPermissions.Administrator)
            {
                Embed embed = new EmbedBuilder
                {
                    Title = "Error!",
                    Description = "User has Admin permissions.",
                    Color = CustomColors.Failure
                }.Build();
                await RespondAsync("", embed: embed);
            }
            else
            {
                try
                {
                    Embed embed = new EmbedBuilder
                    {
                        Title = "User Banned!",
                        Description = $"**{member}** was banned by **{Context.User.Username}**!",
                        Color = CustomColors.Success
                    }.AddField("Reason:", reason)
                    .Build();
                    await Context.Guild.AddBanAsync(user, reason: reason);
                    await RespondAsync(embed: embed);
                    try
                    {
                        if (notify)
                            await member.SendMessageAsync($"You were banned by **{Context.User.Username}**!\nReason: {reason}");
                    }
                    catch { /*Couldn't send a message in the private messages of the user*/}
                }
                catch (Exception ex)
                {
                    await Logger.LogException(ex);
                    var embed = new EmbedBuilder
                    {
                        Title = "Error!",
                        Description = "An error occurred while trying to ban the user. Make sure my role is above the role of the user you want to ban.",
                        Color = CustomColors.Failure
                    }.Build();
                    await RespondAsync(embed: embed);
                }
            }
        }

        /// <summary>
        /// Deletes a specified number of messages from the current channel.
        /// </summary>
        /// <param name="amount">The number of messages to delete.</param>
        /// <remarks>
        /// This command requires the <c>Manage Messages</c> permission in the server.
        /// If the user has the <c>Administrator</c> permission, the command will fail.
        /// </remarks>
        /// <example>
        /// !purge 10
        /// </example>
        [SlashCommand("purge", "The amount of messages that should be deleted.")]
        [DefaultMemberPermissions(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [EnabledInDm(false)]
        public async Task Purge(short amount = 0)
        {
            SocketInteractionContext ctx = new(Context.Client, Context.Interaction);
            Embed embed;
            if (amount >= 100)
            {
                embed = new EmbedBuilder
                {
                    Title = "Error!",
                    Description = "I can't delete more than 100 messages",
                    Color = CustomColors.Failure
                }.Build();
                await RespondAsync(embed: embed);
                return;
            }

            var messages = Context.Channel.GetMessagesAsync(limit: amount).Flatten();

            embed = new EmbedBuilder
            {
                Title = $"{Context.User} cleared {amount} messages!",
                Description = "Don't wait till bot respond to you, it will take a few seconds",
                Color = CustomColors.Success
            }.Build();
            await RespondAsync(embed: embed, ephemeral: true);

            await foreach (var message in messages)
                await ctx.Channel.DeleteMessageAsync(message.Id);
        }
    }
}