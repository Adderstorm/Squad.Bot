using SquadBot_Application.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SquadBot_Application.Models.AI
{
    [Table("totalLeaves")]
    public class TotalLeaves
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("userId")]
        [Required]
        public Users User { get; set; } = null!;

        [Column("serverId")]
        [Required]
        public Servers Server { get; set; } = null!;

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
