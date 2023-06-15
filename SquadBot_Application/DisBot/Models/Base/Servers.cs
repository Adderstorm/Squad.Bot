using Squad.Bot.DisBot.Models.AI;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squad.Bot.DisBot.Models.Base
{
    [Table("servers")]
    public class Servers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id")]
        public long Id { get; set; }

        [Column("serverName")]
        [Required(ErrorMessage = "Field serverName must contain the server name")]
        public string ServerName { get; set; } = string.Empty;

        [Column("blacklistId")]
        public ICollection<BlackList> BlackList { get; set; } = null!;

        [Column("totalMembersId")]
        public ICollection<TotalMembers> TotalMembers { get; set; } = null!;

        [Column("addedAt")]
        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
