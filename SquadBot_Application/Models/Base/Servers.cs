using SquadBot_Application.Models.AI;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SquadBot_Application.Models.Base
{
    [Table("servers")]
    public class Servers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Field serverName must contain the server name")]
        public string ServerName { get; set; } = string.Empty;

        public ICollection<BlackList> BlackList { get; set; } = null!;
        public TotalMembers TotalMembers { get; set; } = null!;

    }
}
