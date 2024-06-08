namespace GeoNRage.Server.Entities;

public class Player
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string GeoGuessrName { get; set; } = null!;

    public Uri IconUrl { get; set; } = null!;

    public string? AssociatedPlayerId { get; set; }

    public Player? AssociatedPlayer { get; set; }

    public string Title { get; set; } = null!;

    public DateTime? LastGeoGuessrUpdate { get; set; }

    public ICollection<PlayerScore> PlayerScores { get; set; } = [];
}
