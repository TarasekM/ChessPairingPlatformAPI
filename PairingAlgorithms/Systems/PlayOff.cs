using System;
using System.Collections.Generic;

namespace PlatformaTurniejowaAPI.PairingAlgorithms.Systems
{
    public class PlayOff : Pairing
    {
        private List<List<int>> CurrentPairs{get; set;}
    
        public PlayOff(int CurrentRound) :
            base(CurrentRound)
        {
            CurrentPairs = new List<List<int>>();
        }

        public override List<List<int>> Pair(List<Player> Players)
        {
            if (CurrentRound == 1){
                CurrentPairs = GetFirstPairs(Players);
            }else{
                CurrentPairs = GetParis(Players);
            }
            CurrentRound++;
            return CurrentPairs;
        }
        
        private List<List<int>> GetParis(List<Player> Players){
            List<int> IDs = GetIDs(Players);
            IDs = RemoveLosers(IDs);
            List<List<int>> Pairs = new List<List<int>>();
            for(int i = 0; i < IDs.Count; i+=2){
                int p1 = IDs[i];
                int p2 = IDs[i+1];
                Pairs.Add(
                    new List<int>(){ p1, p2 });
            }
            return Pairs;
        }

        private  List<List<int>> GetFirstPairs(List<Player> Players){
            List<List<int>> FirstPairs = new List<List<int>>();

            List<int> IDs = GetIDs(Players);
            IDs = FillWithByes(IDs);
            int mid = IDs.Count / 2;

            for (int i = 0; i < mid; i++){
                int p1 = IDs[i];
                int p2 = IDs[mid + i];
                FirstPairs.Add(
                    new List<int>(){ p1, p2 });
            }

            return FirstPairs;
        }

        private int GetBracketHeight(int playersCount){
            int i = 0;
            while (playersCount >= 1){
                i++;
                playersCount /= 2;
            }
            return i;
        }

        private List<int> FillWithByes(List<int> IDs){
            List<int> IDsCopy = IDs;
            int bracketHeight = GetBracketHeight(IDs.Count);
            int byeRange = (int) (Math.Pow(2, bracketHeight) - IDs.Count);
            
            for (int i = 0; i < byeRange; i++){
                IDsCopy.Add(0);
            }
            return IDsCopy;
        }

        private List<int> GetIDs(List<Player> Players){
            List<int> IDs = new List<int>();

            foreach (Player player in Players){
                IDs.Add(player.ID);    
            }
            return IDs;
        }
        
        private List<int> RemoveLosers(List<int> IDs){
            List<int> withoutLosers = new List<int>();
            foreach(List<int> Pair in CurrentPairs){
                for(int i = 0; i < Pair.Count; i++){
                    if (IDs.Contains(Pair[i])){
                        withoutLosers.Add(Pair[i]);
                    }
                }
            }
            return withoutLosers;
        }
    }
}
