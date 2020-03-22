using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlatformaTurniejowaAPI.Models
{
    public class PlayerModel
    {

        [Key]
        public Guid GUID { get; set; }

        [Required]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public int TotalGames { get; set; }

        public int GamesAsWhite { get; set; }

        public int GamesAsBlack { get; set; }

        public float Score { get; set; }

        [ForeignKey("Opponent")]
        public List<Opponent> Opponents { get; set; }

        [ForeignKey("Tournament")]
        public Guid TournamentGuid { get; set; }

    }
}
