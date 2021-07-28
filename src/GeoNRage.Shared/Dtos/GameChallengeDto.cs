using System.Collections.Generic;

namespace GeoNRage.Shared.Dtos
{
    public class GameChallengeDto
    {
        public int Id { get; set; }

        public string MapId { get; set; } = null!;

        public string MapName { get; set; } = null!;

        public string GeoGuessrId { get; set; } = null!;

        public ICollection<PlayerScoreDto> PlayerScores { get; set; } = new HashSet<PlayerScoreDto>();
    }
}
