namespace GeoNRage.Server.Entities
{
    public class GameMap
    {
        public int GameId { get; set; }

        public Game Game { get; set; } = null!;

        public int MapId { get; set; }

        public Map Map { get; set; } = null!;

        public string Name { get; set; } = string.Empty;

        public string? Link { get; set; }
    }
}
