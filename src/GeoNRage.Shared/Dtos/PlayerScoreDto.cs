using System.Collections.Generic;
using System.Linq;

namespace GeoNRage.Shared.Dtos
{
    public class PlayerScoreDto
    {
        public string PlayerId { get; set; } = null!;

        public string PlayerName { get; set; } = null!;

        public int? Round1 { get; set; }

        public int? Round2 { get; set; }

        public int? Round3 { get; set; }

        public int? Round4 { get; set; }

        public int? Round5 { get; set; }

        public int? Sum => Rounds.Any(r => r is not null) ? Rounds.Sum() : null;

        public IReadOnlyList<int?> Rounds => new[] { Round1, Round2, Round3, Round4, Round5 };
    }
}
