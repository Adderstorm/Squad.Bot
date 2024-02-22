using Squad.Bot.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squad.Bot.Models.AI
{
    public class MembersActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Users User { get; set; } = null!;

        [Required]
        public Guilds Guilds { get; set; } = null!;

        public DateTime LastActivityDate { get; set; } = DateTime.UtcNow;
    }
}
