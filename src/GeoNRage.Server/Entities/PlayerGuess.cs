namespace GeoNRage.Server.Entities
{
    public class PlayerGuess
    {
        public int Id { get; set; }

        public int ChallengeId { get; set; }

        public string PlayerId { get; set; } = null!;

        public PlayerScore PlayerScore { get; set; } = null!;

        public int Order { get; set; }

        public int? Score { get; set; }
    }
}
