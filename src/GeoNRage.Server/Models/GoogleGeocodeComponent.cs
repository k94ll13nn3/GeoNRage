using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GeoNRage.Server.Models
{
    public class GoogleGeocodeComponent
    {
        [JsonPropertyName("long_name")]
        public string Name { get; set; } = null!;

        public IList<string> Types { get; set; } = new List<string>();
    }
}
