using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoNRage.Shared.Dtos
{
    public class PlayerScoreWithChallengeDto
    {
        public int? Sum => Rounds.Any(r => r is not null) ? Rounds.Sum() : null;

        public IReadOnlyList<int?> Rounds { get; set; } = new List<int?>();

        public int ChallengeId { get; set; }

        public string MapId { get; set; } = null!;

        public int GameId { get; set; }

        public DateTime GameDate { get; set; }
    }
}
