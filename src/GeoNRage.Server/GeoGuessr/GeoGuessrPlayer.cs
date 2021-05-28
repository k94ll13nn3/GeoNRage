using System.Collections.Generic;

namespace GeoNRage.Server.Dtos.GeoGuessr
{
    public class GeoGuessrPlayer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227")]
        public IList<GeoGuessrGuess> Guesses { get; set; } = new List<GeoGuessrGuess>();

        public string Id { get; set; } = string.Empty;

        public string Nick { get; set; } = string.Empty;
    }
}
