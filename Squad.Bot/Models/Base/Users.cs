using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squad.Bot.Models.Base
{
    [Table("users")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "Field User.ID must contain userID")]
        public ulong Id { get; set; }

        [Required]
        public string Nick { get; set; } = string.Empty;

        public float XP { get; set; } = 0f;

        public ICollection<Warns> Warns { get; set; } = null!;

        public ICollection<BlackList> BlackList { get; set; } = null!;
    }
}
