using System;
using System.Collections.Generic;

namespace PlatformaTurniejowaAPI.PairingAlgorithms.Systems
{
    public class Swiss : Pairing
    {
        public Swiss(int CurrentRound) :
            base(CurrentRound)
        {
        }

        public override List<List<int>> Pair(List<Player> Players)
        {
            return GetPairings(Players);
        }

        private List<List<int>> GetPairings(List<Player> Players)
        {
            List<Player> playersCopy = new List<Player>(Players);
            List<List<int>> Pairings = new List<List<int>>();
            Dictionary<int, Dictionary<int, float>> weightsForPairings = GetAllWeights(playersCopy);
            int maxTries = Players.Count;
            int i = 0;
            while (weightsForPairings.Count != 0  && i <= maxTries){
                List<int> bestPairing = GetBestPairing(weightsForPairings);
                List<int> Pair = GetPair(bestPairing[0], bestPairing[1], Players);
                if (Pair[0] == -1 && Pair[1] == -1)
                {
                    i++;
                    continue;
                }else {
                    Pairings.Add(Pair);
                }

                RemoveWeights(bestPairing[0], bestPairing[1], weightsForPairings);
                i++;
            }
            return Pairings;
        }

        private List<int> GetBestPairing(Dictionary<int, Dictionary<int, float>> weightsForPairings){
            int lowestChoiceID = FindLowestChoiceID(weightsForPairings);
            List<int> bestPairID = new List<int>(){ lowestChoiceID, -1 };
            float bestPairWeight = - (float) Math.Pow(2, 32);
            if (lowestChoiceID >= 0)
            {
                Dictionary<int, float> playersWeights = weightsForPairings[lowestChoiceID];
                foreach(KeyValuePair<int, float> playerWeight in playersWeights)
                {
                    if (bestPairWeight < playerWeight.Value)
                    {
                        bestPairWeight = playerWeight.Value;
                        bestPairID[1] = playerWeight.Key;
                    }
                }
            }

            return bestPairID;
        }

        private int FindLowestChoiceID(Dictionary<int, Dictionary<int, float>> weightsForPairings)
        {
            int lowestChoiceID = -1;
            int count = (int)(Math.Pow(2, 15));
            foreach (KeyValuePair<int, Dictionary<int, float>> playersWeights in weightsForPairings)
            {
                if (playersWeights.Value.Count < count)
                {
                    count = playersWeights.Value.Count;
                    lowestChoiceID = playersWeights.Key;
                }
            }
            return lowestChoiceID;
         }

            private void RemoveWeights(int ID1, int ID2, Dictionary<int, Dictionary<int, float>> weightsForPairings){
            weightsForPairings.Remove(ID1);
            weightsForPairings.Remove(ID2);
            foreach(KeyValuePair<int, Dictionary<int, float>> weights in weightsForPairings){
                weights.Value.Remove(ID1);
                weights.Value.Remove(ID2);
            }
        }

        private List<int> GetPair(int playerID, int opponentID, List<Player> Players){
            int white;
            int black;
            Player player = Players.Find(x => x.ID == playerID);
            Player opponent = Players.Find(x => x.ID == opponentID);
            if (player == null)
            {
                player = Player.GetByePlayer();
            }
            if (opponent == null)
            {
                opponent = Player.GetByePlayer();
            }
            if (player.GamesAsWhite >= opponent.GamesAsWhite){
                white = opponent.ID;
                black = player.ID;
            }else{
                white = player.ID;
                black = opponent.ID;
            }
            List<int> Pair = new List<int>(){
                white, black
            };
            return Pair;
        }

        private float GetScoreWeight(Player currPlayer, Player other){
            float scoreWeight = 0;
            if(other.Score < currPlayer.Score){
                scoreWeight = other.Score - currPlayer.Score;
            }else{
                scoreWeight = currPlayer.Score - other.Score;
            }
            return scoreWeight;
        }

        private float GetColorWeight(Player currPlayer, Player other){
            float blackWeight = Math.Abs(currPlayer.GamesAsBlack - other.GamesAsBlack);
            float whiteWeight = Math.Abs(currPlayer.GamesAsWhite - other.GamesAsWhite);
            float colorWeight = blackWeight + whiteWeight;
            return colorWeight;
        }

        private Dictionary<int, Dictionary<int, float>> GetAllWeights (List<Player> Players){
            Dictionary<int, Dictionary<int, float>> weightsForPairings = new Dictionary<int, Dictionary<int, float>>();
            foreach(Player currPlayer in Players){
                Dictionary<int, float> weightsForPairing = GetWeights(Players, currPlayer);
                weightsForPairings.Add(currPlayer.ID, weightsForPairing);
            }
            return weightsForPairings;
        }

        private Dictionary<int, float> GetWeights(List<Player> Players, Player currPlayer){
            /*
            Return dictionary where key is ID of player and value is weight for pair with that player
            <int player ID : float weight>
            */
            Dictionary<int, float> weightsForPairing = new Dictionary<int, float>();
            float scoreMultiplier = 1.5f;
            float colorMultiplier = (CurrentRound - 1)/8f;
            foreach(Player other in Players){
                float weight = 0;
                if (currPlayer.CanPlay(other)){
                    weight += GetScoreWeight(currPlayer, other) * scoreMultiplier;
                    weight += GetColorWeight(currPlayer, other) * colorMultiplier;
                    weightsForPairing.Add(other.ID, weight);
                }
            }
            return weightsForPairing;
        }
    }
}
