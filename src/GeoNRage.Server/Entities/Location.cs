namespace GeoNRage.Server.Entities
{
    public class Location
    {
        public int ChallengeId { get; set; }

        public Challenge Challenge { get; set; } = null!;

        public int RoundNumber { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string? DisplayName { get; set; }

        public string? Locality { get; set; }

        public string? AdministrativeAreaLevel2 { get; set; }

        public string? AdministrativeAreaLevel1 { get; set; }

        public string? Country { get; set; }
    }
}
