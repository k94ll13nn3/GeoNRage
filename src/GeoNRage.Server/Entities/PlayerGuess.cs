namespace GeoNRage.Server.Entities;

public class PlayerGuess
{
    public int ChallengeId { get; set; }

    public string PlayerId { get; set; } = null!;

    public PlayerScore PlayerScore { get; set; } = null!;

    public int RoundNumber { get; set; }

    public int? Score { get; set; }

    public bool TimedOut { get; set; }

    public bool TimedOutWithGuess { get; set; }

    public int? Time { get; set; }

    public double? Distance { get; set; }
}
