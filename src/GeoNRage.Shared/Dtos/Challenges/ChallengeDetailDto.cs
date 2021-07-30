using System.Collections.Generic;
using System.Linq;

namespace GeoNRage.Shared.Dtos.Challenges
{
    public class ChallengeDetailDto
    {
        public int Id { get; set; }

        public string MapName { get; set; } = null!;

        public string GeoGuessrId { get; set; } = null!;

        public IEnumerable<PlayerScoreWithGuessDto> PlayerScores { get; set; } = new HashSet<PlayerScoreWithGuessDto>();
    }

    public class PlayerScoreWithGuessDto
    {
        public string PlayerId { get; set; } = null!;

        public string PlayerName { get; set; } = null!;

        public IEnumerable<PlayerGuessDto> PlayerGuesses { get; set; } = new HashSet<PlayerGuessDto>();

        public int? Sum => PlayerGuesses.Select(g => g.Score).Sum();

        public PlayerGuessDto? this[int roundNumber] => PlayerGuesses.FirstOrDefault(x => x.RoundNumber == roundNumber);
    }

    public class PlayerGuessDto
    {
        public int RoundNumber { get; set; }

        public int? Score { get; set; }

        public int? Time { get; set; }

        public double? Distance { get; set; }
    }
}
