using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GeoNRage.Server.Models
{
    public record GoogleGeocodeComponent([property: JsonPropertyName("long_name")] string Name, IList<string> Types);

    public record GoogleGeocodeResult(
        [property: JsonPropertyName("formatted_address")] string FormattedAddress,
        [property: JsonPropertyName("address_components")] IList<GoogleGeocodeComponent> AddressComponents);

    public record GoogleGeocode(IList<GoogleGeocodeResult> Results);
}
