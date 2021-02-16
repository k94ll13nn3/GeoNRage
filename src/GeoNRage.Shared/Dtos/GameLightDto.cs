namespace GeoNRage.Shared.Dtos
{
    public class GameLightDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool Locked { get; set; }

        public int Rounds { get; set; }
    }
}
