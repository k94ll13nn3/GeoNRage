namespace GeoNRage.Server.Entities
{
    public class PlayerScore
    {
        public int ChallengeId { get; set; }

        public Challenge Challenge { get; set; } = null!;

        public string PlayerId { get; set; } = null!;

        public Player Player { get; set; } = null!;

        public int Round1 { get; set; }

        public int Round2 { get; set; }

        public int Round3 { get; set; }

        public int Round4 { get; set; }

        public int Round5 { get; set; }
    }
}
