﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SquadBot_Application.Models.Base
{
    [Table("blacklist")]
    public class BlackList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "You must to put a user for add in blacklist")]
        [Column("userId")]
        public Users User { get; set; } = null!;

        [Required]
        [Column("serverId")]
        public Servers Server { get; set; } = null!;
        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
