using System.Collections.Generic;

namespace GeoNRage.Server.Models
{
    public class GeoGuessrPlayer
    {
        public IList<GeoGuessrGuess> Guesses { get; set; } = new List<GeoGuessrGuess>();

        public string Id { get; set; } = string.Empty;

        public string Nick { get; set; } = string.Empty;
    }
}
