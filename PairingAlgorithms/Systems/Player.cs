using System;
using System.Collections.Generic;
using PlatformaTurniejowaAPI.Models;

namespace PlatformaTurniejowaAPI.PairingAlgorithms.Systems {
    public class Player
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public float Score { get; set; }
        public float FullBuchholz { get; set; }
        public float MedianBuchholz { get; set; }

        public int TotalGames { get; set; }
        public int GamesAsWhite { get; set; }
        public int GamesAsBlack { get; set; }

        public bool HaveOpponent { get; set; }
        public int? CurrentOpponentID { get; set; }
        public List<Player> Opponents { get; set; }

        enum Colors
        {
            BLACK,
            WHITE
        }

        public Player()
        {
            Score = 0;
            TotalGames = 0;
            GamesAsWhite = 0;
            GamesAsBlack = 0;
            HaveOpponent = false;
            CurrentOpponentID = null;
            Opponents = new List<Player>();
        }

        public Player(int ID, string Name)
        {
            this.ID = ID;
            this.Name = Name;
            Score = 0;
            TotalGames = 0;
            GamesAsWhite = 0;
            GamesAsBlack = 0;
            HaveOpponent = false;
            CurrentOpponentID = null;
            Opponents = new List<Player>();
        }

        public Player(PlayerModel model, List<PlayerModel> playerModels)
        {
            ID = model.ID;
            Name = model.Name;
            Score = model.Score;
            TotalGames = model.TotalGames;
            GamesAsBlack = model.GamesAsBlack;
            GamesAsWhite = model.GamesAsWhite;
            CurrentOpponentID = null;
            HaveOpponent = false;
            Opponents = new List<Player>();
            foreach (PlayerModel playerModel in playerModels)
            {
                Opponents.Add(InitializePlayerFromModel(playerModel));
            }
        }

        // Do not initialize Opponents field for now to stop recursion
        private Player InitializePlayerFromModel(PlayerModel model)
        {
            Player player = new Player
            {
                ID = model.ID,
                Name = model.Name,
                Score = model.Score,
                TotalGames = model.TotalGames,
                GamesAsBlack = model.GamesAsBlack,
                GamesAsWhite = model.GamesAsWhite,
                CurrentOpponentID = null,
                HaveOpponent = false
            };
            return player;
        }

        public static Player GetByePlayer()
        {
            return new Player(0, "Bye");
        }

        public bool Pair(Player other)
        {
            /// <summary>method <c>Pair</c> Pairs player with other one. Updates Players fields.
            /// Note: this player always "plays" white and other player always "plays" black.
            /// </summary>

            if (Opponents.Contains(other))
            {
                return false;
            }
            HaveOpponent = true;
            CurrentOpponentID = other.ID;
            Opponents.Add(other);
            TotalGames++;
            GamesAsWhite++;

            other.HaveOpponent = true;
            other.CurrentOpponentID = ID;
            other.Opponents.Add(this);
            other.TotalGames++;
            other.GamesAsBlack++;
            return true;
        }

        public bool CanPlay(Player other)
        {
            if (this.PlayedWith(other) || this.HaveOpponent || other.HaveOpponent)
            {
                return false;
            }

            if (other.CanPlayAs(Player.Opossite(this.PrefferedColor())))
            {
                return true;
            }
            if (this.CanPlayAs(Player.Opossite(other.PrefferedColor())))
            {
                return true;
            }
            return false;
        }

        private Colors PrefferedColor()
        {
            int whiteColorDifference = this.GamesAsWhite - this.GamesAsBlack;
            int blackColorDifference = this.GamesAsBlack - this.GamesAsWhite;
            Colors prefferedColor;
            if (whiteColorDifference > blackColorDifference)
            {
                prefferedColor = Colors.BLACK;
            }
            else
            {
                prefferedColor = Colors.WHITE;
            }
            return prefferedColor;
        }

        private bool CanPlayAs(Colors color)
        {
            switch (color)
            {
                case Colors.WHITE:
                    int whiteColorDifference = this.GamesAsWhite - this.GamesAsBlack;
                    return whiteColorDifference < 2;
                case Colors.BLACK:
                    int blackColorDifference = this.GamesAsBlack - this.GamesAsWhite;
                    return blackColorDifference < 2;
                default:
                    return false;
            }
        }

        static Colors Opossite(Colors color)
        {
            switch (color)
            {
                case Colors.BLACK:
                    return Colors.WHITE;
                case Colors.WHITE:
                    return Colors.BLACK;
                default:
                    return Colors.BLACK;
            }
        }

        private bool PlayedWith(Player other)
        {
            if (Opponents.Contains(other))
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return ID + ". " + Name;
        }

        public override bool Equals(object obj)
        {
            Player other = (Player)obj;
            if (
                this.ID == other.ID &&
                this.Name == other.Name &&
                this.Score == other.Score &&
                this.TotalGames == other.TotalGames &&
                this.GamesAsWhite == other.GamesAsWhite &&
                this.GamesAsBlack == other.GamesAsBlack &&
                this.HaveOpponent == other.HaveOpponent
                )
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Name, Score, TotalGames, GamesAsWhite, GamesAsBlack, HaveOpponent, CurrentOpponentID);
        }
    }
}
