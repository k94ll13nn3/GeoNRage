namespace GeoNRage.Server.Models
{
    public class GeoGuessrGameGuess
    {
        public string Token { get; set; } = string.Empty;

        public decimal Lat { get; set; }

        public decimal Lng { get; set; }

        public bool TimedOut { get; set; }
    }
}
