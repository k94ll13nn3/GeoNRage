using System.Collections.Generic;
using System.Text.Json.Serialization;
using GeoNRage.Server.Models;

namespace GeoNRage.Server.GeoGuessr
{
    public class GoogleGeocodeResult
    {
        [JsonPropertyName("formatted_address")]
        public string FormattedAddress { get; set; } = null!;

        [JsonPropertyName("address_components")]
        public IList<GoogleGeocodeComponent> AddressComponents { get; set; } = new List<GoogleGeocodeComponent>();
    }
}
