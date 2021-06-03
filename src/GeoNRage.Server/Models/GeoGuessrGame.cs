using System.Collections.Generic;

namespace GeoNRage.Server.Models
{
    public class GeoGuessrGame
    {
        public string Map { get; set; } = string.Empty;

        public string MapName { get; set; } = string.Empty;

        public int TimeLimit { get; set; }

        public GeoGuessrPlayer Player { get; set; } = null!;

        public IList<GeoGuessrRound> Rounds { get; set; } = new List<GeoGuessrRound>();
    }
}
