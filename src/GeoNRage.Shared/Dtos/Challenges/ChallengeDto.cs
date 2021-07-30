using System;

namespace GeoNRage.Shared.Dtos.Challenges
{
    public class ChallengeDto
    {
        public int Id { get; set; }

        public string MapId { get; set; } = null!;

        public string MapName { get; set; } = null!;

        public string GeoGuessrId { get; set; } = null!;

        public int? GameId { get; set; }

        public string? CreatorName { get; set; }

        public int MaxScore { get; set; }
    }
}
