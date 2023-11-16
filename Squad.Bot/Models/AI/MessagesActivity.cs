using Squad.Bot.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squad.Bot.Models.AI
{
    [Table("messagesActivity")]
    public class MessagesActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("serverId")]
        [Required]
        public Guilds Guilds { get; set; } = null!;

        [Column("totalMessages")]
        [Required]
        public int TotalMessages { get; set; } = 0;

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
