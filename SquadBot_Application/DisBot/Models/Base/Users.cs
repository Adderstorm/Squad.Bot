using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squad.Bot.DisBot.Models.Base
{
    [Table("users")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "Field User.ID must contain userID")]
        [Column("id")]
        public int Id { get; set; }

        [Column("nick")]
        [Required]
        public string Nick { get; set; } = string.Empty;

        [Column("XP")]
        public float XP { get; set; } = 0f;

        [Column("warnsId")]
        public ICollection<Warns> Warns { get; set; } = null!;

        [Column("blacklistId")]
        public ICollection<BlackList> BlackList { get; set; } = null!;
    }
}
