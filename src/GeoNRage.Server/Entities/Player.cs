namespace GeoNRage.Server.Entities;

public class Player
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public Uri? IconUrl { get; set; }

    public ICollection<PlayerScore> PlayerScores { get; set; } = new HashSet<PlayerScore>();
}
