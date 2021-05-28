namespace GeoNRage.Server.Dtos.GeoGuessr
{
    public class GeoGuessrGame
    {
        public string Map { get; set; } = string.Empty;

        public string MapName { get; set; } = string.Empty;

        public GeoGuessrPlayer Player { get; set; } = null!;
    }
}
