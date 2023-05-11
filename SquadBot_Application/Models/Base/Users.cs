using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SquadBot_Application.Models.Base
{
    [Table("users")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "Field User.ID must contain userID")]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        public string Nick { get; set; } = string.Empty;

        public float XP { get; set; } = 0f;

        public ICollection<Warns> Warns { get; set; } = null!;
        public ICollection<BlackList> BlackList { get; set; } = null!;
    }
}
