using Squad.Bot.DisBot.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squad.Bot.DisBot.Models.AI
{
    [Table("joinDate")]
    public class JoinDate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("userId")]
        public Users User { get; set; } = null!;

        [Required]
        [Column("serverId")]
        public Servers Server { get; set; } = null!;

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
