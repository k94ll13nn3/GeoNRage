namespace GeoNRage.Server.Entities;

public class Map
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsMapForGame { get; set; }

    public ICollection<Challenge> Challenges { get; set; } = [];
}
