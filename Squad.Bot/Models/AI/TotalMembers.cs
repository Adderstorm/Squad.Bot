using Squad.Bot.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squad.Bot.Models.AI
{
    [Table("totalMembers")]
    public class TotalMembers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public Guilds Guilds { get; set; } = null!;

        [Required]
        public int TotalUsers { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
