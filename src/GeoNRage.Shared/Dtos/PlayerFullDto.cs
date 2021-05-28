using System.Collections.Generic;

namespace GeoNRage.Shared.Dtos
{
    public class PlayerFullDto : PlayerDto
    {
        public ICollection<PlayerScoreWithChallengeDto> PlayerScores { get; set; } = new HashSet<PlayerScoreWithChallengeDto>();
    }
}
