using System.Text.Json.Serialization;

namespace GeoNRage.Server.Models;

internal sealed record GoogleGeocodeComponent([property: JsonPropertyName("long_name")] string Name, IList<string> Types);

internal sealed record GoogleGeocodeResult(
    [property: JsonPropertyName("formatted_address")] string FormattedAddress,
    [property: JsonPropertyName("address_components")] IList<GoogleGeocodeComponent> AddressComponents);

internal sealed record GoogleGeocode(IList<GoogleGeocodeResult> Results);
