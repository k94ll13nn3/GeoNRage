namespace GeoNRage.Shared.Dtos
{
    public class GameMapCreateOrEditDto
    {
        public int MapId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Link { get; set; }
    }
}
