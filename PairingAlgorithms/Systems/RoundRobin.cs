using System;
using System.Collections.Generic;

namespace PlatformaTurniejowaAPI.PairingAlgorithms.Systems
{
    public class RoundRobin : Pairing
    {
        private List<int> FirstHalf { get; set; }
        private List<int> SecondHalf { get; set; }

        public RoundRobin(int CurrentRound) :
            base(CurrentRound)
        {
            FirstHalf = new List<int>();
            SecondHalf = new List<int>();
        }

        public override List<List<int>> Pair(List<Player> Players)
        {

            MaxRound = Players.Count - 1;
            SetUpTables(Players);

            // Return if max rounds has been played
            if (CurrentRound > MaxRound)
            {
                return GetPairings(Players);
            }
            for(int i = 0; i < CurrentRound - 1; i++)
            {
                Rotate();
            }
            List<List<int>> Pairings = GetPairings(Players);

            CurrentRound++;
            return Pairings;
        }

        private void SetUpTables(List<Player> Players)
        {
            int m = (int)(Math.Ceiling((decimal)Players.Count / 2));
            int r = Players.Count;
            int index = 0;
            for (; index < m; index++)
            {
                FirstHalf.Add(Players[index].ID);
            }

            for (; index < r; index++)
            {
                SecondHalf.Add(Players[index].ID);
            }

            // skip first round if there are odd number of players
            if (Players.Contains(Player.GetByePlayer()))
            {
                Rotate();
                CurrentRound++;
            }
        }

        private void Rotate()
        {
            int lastFromFirstHalf = FirstHalf[FirstHalf.Count - 1];
            int firstFromSecondHalf = SecondHalf[0];

            for(int i = FirstHalf.Count - 1; i > 1; i--)
            {
                FirstHalf[i] = FirstHalf[i - 1];
            }
            FirstHalf[1] = firstFromSecondHalf;
            SecondHalf.Remove(firstFromSecondHalf);
            SecondHalf.Add(lastFromFirstHalf);
        }

        private List<List<int>> GetPairings(List<Player> Players)
        {
            List<List<int>> Pairings = new List<List<int>>();

            for (int i = 0; i < FirstHalf.Count; i++)
            {
                List<int> Pair = GetPair(i);
                Pairings.Add(Pair);
            }
            return Pairings;
        }

        private List<int> GetPair(int n)
        {
            int white;
            int black;
            if (n % 2 == 1 || (CurrentRound % 2 == 0 && n == 0))
            {
                white = SecondHalf[n];
                black = FirstHalf[n];

            }else
            {
                white = FirstHalf[n];
                black = SecondHalf[n];
            }

            List<int> Pair = new List<int>()
            {
                white,
                black
            };

            return Pair;
        }
        
    }
}
