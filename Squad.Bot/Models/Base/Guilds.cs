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

        public string ServerName { get; set; } = string.Empty;

        public ICollection<BlackList>? BlackList { get; set; }

        public ICollection<TotalMembers>? TotalMembers { get; set; }

        public DateTime? DeletedAt { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
