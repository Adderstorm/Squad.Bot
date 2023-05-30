using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SquadBot_Application.Bot.Models.Base
{
    [Table("warns")]
    public class Warns
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("userId")]
        [Required(ErrorMessage = "Field Warns.User must contain user that you want to warn")]
        public Users User { get; set; } = null!;

        [Required]
        [Column("serverId")]
        public Servers Server { get; set; } = null!;

        [Column("moderatorId")]
        public int ModeratorID { get; set; }

        [Column("reason")]
        [Required(ErrorMessage = "You must to specify a reason")]
        [MaxLength(150)]
        public string Reason { get; set; } = string.Empty;

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
