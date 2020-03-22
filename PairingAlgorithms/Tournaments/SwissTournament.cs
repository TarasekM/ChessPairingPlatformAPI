using System;
using System.Collections.Generic;
using PlatformaTurniejowaAPI.PairingAlgorithms.Systems;
using PlatformaTurniejowaAPI.Models;

namespace PlatformaTurniejowaAPI.PairingAlgorithms.Tournaments
{
    class SwissTournament: Tournament
    {
        public SwissTournament(string Title, DateTime Date, int CurrentRound) :
            base(Title, Date, CurrentRound, PairingFactory.PairingSystems.Swiss)
            {
                
            }

        public SwissTournament(TournamentModel model, List<PlayerModel> playerModels) :
            base(model, playerModels, PairingFactory.PairingSystems.Swiss)
        {

        }

        public override void AddPlayer()
        {
            throw new NotImplementedException();
        }

        override public void GetScoreboard(){
            throw new NotImplementedException();
        }

        public override void RemovePlayer()
        {
            throw new NotImplementedException();
        }
    }

}