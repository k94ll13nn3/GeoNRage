using System;
using System.Collections.Generic;

namespace GeoNRage.Shared.Dtos
{
    public class PlayerScoreWithChallengeDto
    {
        public int? Sum { get; set; }

        public bool Done { get; set; }

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
