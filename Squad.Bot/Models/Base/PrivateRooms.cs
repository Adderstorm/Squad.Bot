using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squad.Bot.Models.Base
{
    [Table("privateRooms")]
    public class PrivateRooms
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("serverId")]
        public Guilds Guilds { get; set; } = null!;

        [Column("categoryId")]
        public ulong CategoryID { get; set; }

        [Column("channelId")]
        public ulong ChannelID { get; set; }
    }
}
