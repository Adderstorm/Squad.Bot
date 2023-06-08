using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SquadBot_Application.Bot.Models.Bot_special
{
    public class ServersToLogData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Column("serverId")]
        public long ServerID { get; set; }
    }
}
