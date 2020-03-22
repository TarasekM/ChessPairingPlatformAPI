using System.Collections.Generic;

namespace PlatformaTurniejowaAPI.PairingAlgorithms.Systems
{
    public interface IPairing
    {
        List<List<int>> Pair(List<Player> Players);
    }
}
