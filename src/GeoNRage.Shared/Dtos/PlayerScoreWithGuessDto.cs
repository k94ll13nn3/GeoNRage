using System.Collections.Generic;
using System.Linq;

namespace GeoNRage.Shared.Dtos
{
    public class PlayerScoreWithGuessDto
    {
        public string PlayerId { get; set; } = null!;

        public string PlayerName { get; set; } = null!;

        public ICollection<PlayerGuessDto> PlayerGuesses { get; set; } = new HashSet<PlayerGuessDto>();

        public int? Sum => PlayerGuesses.Select(g => g.Score).Sum();

        public PlayerGuessDto? this[int roundNumber] => PlayerGuesses.FirstOrDefault(x => x.RoundNumber == roundNumber);
    }
}
