using System.Collections.Generic;

namespace PlatformaTurniejowaAPI.PairingAlgorithms.Systems
{
    public abstract class Pairing : IPairing
    {
        public Pairing(int CurrentRound)
        {
            this.CurrentRound = CurrentRound;
        }

        public int CurrentRound { get; set; }

        public int MaxRound { get; set; }

        public abstract List<List<int>> Pair(List<Player> Players);
    }
}
