using SquadBot_Application.Models.AI;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace SquadBot_Application.Models.Base
{
    [Table("servers")]
    public class Servers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id")]
        public int Id { get; set; }

        [Column("serverName")]
        [Required(ErrorMessage = "Field serverName must contain the server name")]
        public string ServerName { get; set; } = string.Empty;

        [Column("blacklistId")]
        public ICollection<BlackList> BlackList { get; set; } = null!;

        [Column("totalMembersId")]
        public ICollection<TotalMembers> TotalMembers { get; set; } = null!;
    }
}
