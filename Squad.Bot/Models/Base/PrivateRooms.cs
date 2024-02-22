using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squad.Bot.Models.Base
{
    public class PrivateRooms
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guilds Guilds { get; set; } = null!;

        public ulong CategoryID { get; set; }

        public ulong ChannelID { get; set; }
        public ulong SettingsChannelID { get; set; }
    }
}
