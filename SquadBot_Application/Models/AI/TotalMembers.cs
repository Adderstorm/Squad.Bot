﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SquadBot_Application.Models.Base;

namespace SquadBot_Application.Models.AI
{
    [Table("totalMembers")]
    public class TotalMembers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id")]
        public int Id { get; set; }

        [Column("serverId")]
        public Servers Server { get; set; } = null!;

        [Column("totalUsers")]
        [Required]
        public int TotalUsers { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
