using SquadBot.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SquadBot.Models.AI
{
    [Table("peakOnline")]
    public class PeakOnline
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("serverId")]
        [Required]
        public Servers Server { get; set; } = null!;

        [Column("peak")]
        public int Peak { get; set; } = 0;

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
