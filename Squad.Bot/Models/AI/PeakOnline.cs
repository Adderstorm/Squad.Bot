using Squad.Bot.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squad.Bot.Models.AI
{
    [Table("peakOnline")]
    public class PeakOnline
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guilds Guilds { get; set; } = null!;

        public int Peak { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
