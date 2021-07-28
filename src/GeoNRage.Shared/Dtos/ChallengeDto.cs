using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoNRage.Shared.Dtos
{
    public class ChallengeDto
    {
        public int Id { get; set; }

        public string MapId { get; set; } = null!;

        public string MapName { get; set; } = null!;

        public string GeoGuessrId { get; set; } = null!;

        public int GameId { get; set; }

        public string GameName { get; set; } = null!;

        public DateTime GameDate { get; set; }

        public ICollection<PlayerScoreWithGuessDto> PlayerScores { get; set; } = new HashSet<PlayerScoreWithGuessDto>();

        public int TimeLimit { get; set; }

        public int? LocationsCount { get; set; }

        public PlayerScoreWithGuessDto? this[string playerId] => PlayerScores.FirstOrDefault(x => x.PlayerId == playerId);

        public string? CreatorName { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
