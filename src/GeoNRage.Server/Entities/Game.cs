namespace GeoNRage.Server.Entities;

internal sealed class Game
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public ICollection<Challenge> Challenges { get; set; } = [];
}
