using Squad.Bot.Models.AI;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squad.Bot.Models.Base
{
    [Table("servers")]
    public class Guilds
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; set; }

        [Required(ErrorMessage = "Field serverName must contain the server name")]
        public string ServerName { get; set; } = string.Empty;

        public ICollection<BlackList> BlackList { get; set; } = null!;

        public ICollection<TotalMembers> TotalMembers { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
