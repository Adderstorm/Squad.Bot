﻿using SquadBot_Application.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace SquadBot_Application.Models.AI
{
    [Table("messagesActivity")]
    public class MessagesActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("serverId")]
        [Required]
        public Servers Server { get; set; } = null!;

        [Column("totalMessages")]
        [Required]
        public int TotalMessages { get; set; } = 0;

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
