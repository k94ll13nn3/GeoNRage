using System;
using System.Collections.Generic;

namespace GeoNRage.Shared.Dtos
{
    public class ChallengeDto
    {
        public int Id { get; set; }

        public string MapId { get; set; } = null!;

        public string MapName { get; set; } = null!;

        public Uri? Link { get; set; }

        public int GameId { get; set; }

        public ICollection<PlayerScoreDto> PlayerScores { get; set; } = new HashSet<PlayerScoreDto>();
    }
}
