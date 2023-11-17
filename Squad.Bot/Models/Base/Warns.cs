using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squad.Bot.Models.Base
{
    [Table("warns")]
    public class Warns
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Field Warns.User must contain user that you want to warn")]
        public Users User { get; set; } = null!;

        [Required]
        public Guilds Guilds { get; set; } = null!;

        public int ModeratorID { get; set; }

        [Required(ErrorMessage = "You must to specify a reason")]
        [MaxLength(150)]
        public string Reason { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
