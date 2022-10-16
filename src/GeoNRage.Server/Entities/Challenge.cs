namespace GeoNRage.Server.Entities;

public class Challenge
{
    public int Id { get; set; }

    public string MapId { get; set; } = null!;

    public Map Map { get; set; } = null!;

    public int GameId { get; set; }

    public Game Game { get; set; } = null!;

    public string GeoGuessrId { get; set; } = null!;

    public int? TimeLimit { get; set; }

    public ICollection<PlayerScore> PlayerScores { get; set; } = new HashSet<PlayerScore>();

    public ICollection<Location> Locations { get; set; } = new HashSet<Location>();

    public string? CreatorId { get; set; }

    public Player? Creator { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
