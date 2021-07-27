using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoNRage.Shared.Dtos
{
    public class PlayerScoreWithChallengeDto
    {
        public int? Sum => PlayerGuesses.Select(g => g.Score).Sum();

        public ICollection<PlayerGuessDto> PlayerGuesses { get; set; } = new HashSet<PlayerGuessDto>();

        public int ChallengeId { get; set; }

        public int? ChallengeTimeLimit { get; set; }

        public string MapId { get; set; } = null!;

        public string MapName { get; set; } = null!;

        public int GameId { get; set; }

        public DateTime GameDate { get; set; }

        public bool MapIsMapForGame { get; set; }
    }
}
