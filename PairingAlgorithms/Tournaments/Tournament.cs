using System;
using System.Collections.Generic;
using PlatformaTurniejowaAPI.PairingAlgorithms.Systems;
using PlatformaTurniejowaAPI.Models;

namespace PlatformaTurniejowaAPI.PairingAlgorithms.Tournaments
{

    public abstract class Tournament //TODO Make it abstract || Add methods like printScoreboard / remove Players etc.
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public Pairing PairingSystem { get; set; }
        public List<Player> Players { get; set; }
        public int CurrentRound { get; set; }

        public Tournament(string Title, DateTime Date, int CurrentRound, PairingFactory.PairingSystems PairingSystem)
        {
            this.PairingSystem = PairingFactory.GetPairing(PairingSystem, CurrentRound);
            this.Title = Title;
            this.Date = Date;
            this.CurrentRound = CurrentRound;
        }

        public Tournament(TournamentModel model, List<PlayerModel> playerModels, PairingFactory.PairingSystems PairingSystem)
        {
            this.Title = model.Title;
            this.Date = model.Date;
            this.PairingSystem = PairingFactory.GetPairing(PairingSystem, model.CurrentRound);
            this.CurrentRound = model.CurrentRound;
            this.Players = new List<Player>();
            foreach(PlayerModel playerModel in playerModels)
            {
                List<PlayerModel> opponents = new List<PlayerModel>();
                foreach(Opponent opponent in playerModel.Opponents)
                {
                    PlayerModel playerModelOpponent = playerModels.Find(p => p.GUID == opponent.OpponentID);
                    opponents.Add(playerModelOpponent);
                }
                this.Players.Add(new Player(playerModel, opponents));
            }
        }

        public List<List<int>> Pair()
        {
            List<List<int>> Pairings = PairingSystem.Pair(Players);
            foreach (List<int> Pair in Pairings)
            {
                int white = Pair[0];
                int black = Pair[1];
                Player whitePlayer = Players.Find(p => p.ID == white);
                Player blackPlayer = Players.Find(p => p.ID == black);
                if (whitePlayer == null)
                {
                    whitePlayer = Player.GetByePlayer();
                }
                if(blackPlayer == null)
                {
                    blackPlayer = Player.GetByePlayer();
                }
                whitePlayer.Pair(blackPlayer);
            }
            return Pairings;
        }

        public void StartTournament()
        {
            if (Players.Count % 2 == 1)
            {
                Players.Add(Player.GetByePlayer());
            }
        }

        public abstract void GetScoreboard();


        public abstract void RemovePlayer();

        public abstract void AddPlayer();
    }
}
