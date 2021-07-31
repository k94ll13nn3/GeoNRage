namespace GeoNRage.Shared.Dtos.Locations
{
    public record LocationDto(
        string? DisplayName,
        string? Locality,
        string? AdministrativeAreaLevel2,
        string? AdministrativeAreaLevel1,
        string? Country,
        int TimesSeen,
        decimal Latitude,
        decimal Longitude);
}
