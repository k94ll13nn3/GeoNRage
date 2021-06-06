namespace GeoNRage.Shared.Dtos
{
    public class LocationDto
    {
        public string? DisplayName { get; set; }

        public string? Locality { get; set; }

        public string? AdministrativeAreaLevel2 { get; set; }

        public string? AdministrativeAreaLevel1 { get; set; }

        public string? Country { get; set; }

        public int TimesSeen { get; set; }
    }
}
