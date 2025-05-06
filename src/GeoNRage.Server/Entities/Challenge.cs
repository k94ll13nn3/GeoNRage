namespace GeoNRage.Server.Entities;

internal sealed class Challenge
{
    public int Id { get; set; }

    public string MapId { get; set; } = null!;

    public Map Map { get; set; } = null!;

    public int GameId { get; set; }

    public Game Game { get; set; } = null!;

    public string GeoGuessrId { get; set; } = null!;

    public int? TimeLimit { get; set; }

    public ICollection<PlayerScore> PlayerScores { get; set; } = [];

    public ICollection<Location> Locations { get; set; } = [];

    public string? CreatorId { get; set; }

    public Player? Creator { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
