using SquadBot.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SquadBot.Models.AI
{
    [Table("membersActivity")]
    public class MembersActivity
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

        [Column("lastActivityDate")]
        public DateTime LastActivityDate { get; set; } = DateTime.Now;
    }
}
