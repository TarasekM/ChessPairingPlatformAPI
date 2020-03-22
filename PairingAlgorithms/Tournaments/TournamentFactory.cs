using System;
using System.Collections.Generic;
using PlatformaTurniejowaAPI.Models;

namespace PlatformaTurniejowaAPI.PairingAlgorithms.Tournaments
{
    class TournamentFactory
    {
        public static Tournament GetTournament(string Title, DateTime Date, int CurrentRound, string tournamentSystem){
            return tournamentSystem switch
            {
                "Kołowy" => new RoundRobinTournament(Title, Date, CurrentRound),
                "Pucharowy" => new PlayOffTournament(Title, Date, CurrentRound),
                "Szwajcarski" => new SwissTournament(Title, Date, CurrentRound),
                _ => new RoundRobinTournament(Title, Date, CurrentRound),
            };
        }

        public static Tournament GetTournamentFromModel(TournamentModel model, List<PlayerModel> playerModels)
        {
            return model.PairingSystem switch
            {
                "Kołowy" => new RoundRobinTournament(model, playerModels),
                "Pucharowy" => new PlayOffTournament(model, playerModels),
                "Szwajcarski" => new SwissTournament(model, playerModels),
                _ => new RoundRobinTournament(model, playerModels),
            };
        }
    }
}