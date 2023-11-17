using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squad.Bot.Models.Base
{
    [Table("blacklist")]
    public class BlackList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "You must to put a user for add in blacklist")]
        public Users User { get; set; } = null!;

        [Required]
        public Guilds Guilds { get; set; } = null!;

        [Required(ErrorMessage = "You must to specify a reason")]
        [MaxLength(150)]
        public string Reason { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
