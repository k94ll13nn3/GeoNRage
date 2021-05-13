namespace GeoNRage.Shared.Dtos
{
    public class GameMapDto
    {
        public int MapId { get; set; }

        public MapDto Map { get; set; } = null!;

        public string Name { get; set; } = string.Empty;

        public string? Link { get; set; }
    }
}
