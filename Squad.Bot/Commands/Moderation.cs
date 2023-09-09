using Discord;
using Discord.Interactions;
using Squad.Bot.Logging;

namespace Squad.Bot.Commands
{
    [Group("moderation", "commands for moderators")]
    public class Moderation : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("kick", "kick user")]
        [DefaultMemberPermissions(GuildPermission.KickMembers)]
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
                    Color = 0xE02B2B
                }.Build();
                await RespondAsync("",embed: embed);
            }
            else
            {
                try
                {
                    Embed embed = new EmbedBuilder
                    {
                        Title = "User Kicked!",
                        Description = $"**{member}** was kicked by **{Context.User.Username}**!",
                        Color = 0x9C84EF
                    }.AddField("Reason:", Reason)
                    .Build();
                    await Logger.LogInfo("Kicking");
                    await member.KickAsync(Reason);
                    await Logger.LogInfo("responding");
                    await RespondAsync("", embed: embed);
                    await Logger.LogInfo("Responded");
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
                        Color = 0xE02B2B
                    }.Build();

                    await RespondAsync("", embed: embed);
                }
            }
        }
        [SlashCommand("nick", "changes nick")]
        [DefaultMemberPermissions(GuildPermission.ManageNicknames)]
        [EnabledInDm(false)]
        public async Task Nick(IUser user, string nickname)
        {
            await Logger.LogCommand($"{nameof(Nick)} has executed by {Context.User.Username} in {Context.Channel.Id}");
            var member = Context.Guild.GetUser(user.Id);
            try
            {
                var embed = new EmbedBuilder
                {
                    Title = "Changed Nickname!",
                    Description = $"**{member.Nickname}'s** new nickname is **{nickname}**!",
                    Color = 0x9C84EF
                }.Build();
                await Logger.LogInfo("Changing nick");
                await member.ModifyAsync(x => x.Nickname = nickname);
                await Logger.LogInfo("Responding embed");
                await RespondAsync(embed: embed,options:new RequestOptions() { Timeout=35000});
                await Logger.LogInfo("nick has changed");
            }
            catch(Exception ex)
            {
                await Logger.LogException(ex);
                var embed = new EmbedBuilder
                {
                    Title = "Error!",
                    Description = "An error occurred while trying to change the nickname of the user. Make sure my role is above the role of the user you want to change the nickname.",
                    Color = 0xE02B2B
                }.Build();
                await RespondAsync(embed: embed);
            }
        }
        [SlashCommand("ban", "ban user")]
        [DefaultMemberPermissions(GuildPermission.BanMembers)]
        [EnabledInDm(false)]
        public async Task Ban(IUser user, string Reason, bool notify = true)
        {
            var member = Context.Guild.GetUser(user.Id);
            if (member.GuildPermissions.Administrator)
            {
                Embed embed = new EmbedBuilder
                {
                    Title = "Error!",
                    Description = "User has Admin permissions.",
                    Color = 0xE02B2B
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
                        Color = 0x9C84EF
                    }.AddField("Reason:", Reason)
                    .Build();
                    await Context.Guild.AddBanAsync(user, reason: Reason);
                    await RespondAsync(embed: embed);
                    try
                    {
                        if(notify)
                            await member.SendMessageAsync($"You were banned by **{Context.User.Username}**!\nReason: {Reason}");
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
                        Color = 0xE02B2B
                    }.Build();
                    await RespondAsync(embed: embed);
                }
            }
        }
        [SlashCommand("purge", "The amount of messages that should be deleted.")]
        [DefaultMemberPermissions(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [EnabledInDm(false)]
        public async Task Purge(short amount = 0)
        {
            Embed embed;
            if (amount >= 100)
            {
                embed = new EmbedBuilder
                {
                    Title = "Error!",
                    Description = "I can't delete more than 100 messages",
                    Color = 0xE02B2B
                }.Build();
                await RespondAsync(embed: embed);
                return;
            }

            var messages = Context.Channel.GetMessagesAsync(limit: amount).Flatten();

            embed = new EmbedBuilder
            {
                Description = $"{Context.User} cleared {amount} messages!",
                Color = 0x9C84EF
            }.Build();
            await RespondAsync(embed: embed, ephemeral: true);

            await foreach(var message in messages)
                await Context.Channel.DeleteMessageAsync(message.Id);
        }
    }
}
