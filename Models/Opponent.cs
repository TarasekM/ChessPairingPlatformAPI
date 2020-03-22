using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlatformaTurniejowaAPI.Models
{
    public class Opponent
    {
        [Key]
        public Guid ID { get; set; }

        [ForeignKey("PlayerModel")]
        public Guid OpponentID { get; set; }

    }
}
