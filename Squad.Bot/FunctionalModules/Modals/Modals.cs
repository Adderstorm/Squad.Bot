using Discord;
using Discord.Interactions;

namespace Squad.Bot.FunctionalModules.Modals
{
    public class RenameModal : IModal
    {
        public string Title => "Rename channel";

        [RequiredInput]
        [InputLabel("New name")]
        [ModalTextInput("rename.new_name", style: TextInputStyle.Paragraph, placeholder: "You can use {game}, {username} or any other text name you want", maxLength: 100)]
        public string ChannelName { get; set; } = null!;
    }

    public class LimitModal : IModal
    {
        public string Title => "Change channel limit";

        [RequiredInput]
        [InputLabel("New limit")]
        [ModalTextInput("limit.new_limit", maxLength: 2)]
        public string Limit { get; set; } = null!;
    }
}
