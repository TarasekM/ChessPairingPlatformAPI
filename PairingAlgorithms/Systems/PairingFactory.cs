namespace PlatformaTurniejowaAPI.PairingAlgorithms.Systems
{
    public class PairingFactory
    {

        public enum PairingSystems
        {
            RoundRobin,
            PlayOff,
            Swiss,
        }

        public static Pairing GetPairing(PairingSystems System, int CurrentRound)
        {
            switch (System)
            {
                case PairingSystems.RoundRobin:
                    return new RoundRobin(CurrentRound);
                case PairingSystems.PlayOff:
                    return new PlayOff(CurrentRound);
                case PairingSystems.Swiss:
                    return new Swiss(CurrentRound);
                default:
                    return new RoundRobin(CurrentRound);
            }
        }
    }
}
