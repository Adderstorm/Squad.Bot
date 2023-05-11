﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SquadBot_Application.Models.Base
{
    [Table("privateRooms")]
    public class PrivateRooms
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("serverId")]
        public Servers Server { get; set; } = null!;

        [Column("categoryId")]
        public int CategoryID { get; set; }

        [Column("channelId")]
        public int ChannelID { get; set; }
    }
}
