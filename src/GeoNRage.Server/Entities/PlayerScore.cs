namespace GeoNRage.Server.Entities;

internal sealed class PlayerScore
{
    public int ChallengeId { get; set; }

    public Challenge Challenge { get; set; } = null!;

    public string PlayerId { get; set; } = null!;

    public Player Player { get; set; } = null!;

    public ICollection<PlayerGuess> PlayerGuesses { get; set; } = [];
}
