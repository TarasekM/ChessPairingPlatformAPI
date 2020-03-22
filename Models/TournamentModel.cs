using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlatformaTurniejowaAPI.Models
{
    public class TournamentModel
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string PairingSystem { get; set; }

        public int CurrentRound { get; set; }

        [ForeignKey("PlayerModel")]
        public List<PlayerModel> Players { get; set; }

    }
}
