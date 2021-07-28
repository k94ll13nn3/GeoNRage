using System.Collections.Generic;
using System.Linq;

namespace GeoNRage.Shared.Dtos
{
    public class GameDto : GameLightDto
    {
        public ICollection<GameChallengeDto> Challenges { get; set; } = new HashSet<GameChallengeDto>();

        public ICollection<PlayerDto> Players { get; set; } = new HashSet<PlayerDto>();

        public int? this[int challengeId, string playerId, int round]
        {
            get
            {
                PlayerScoreDto? playerScore = Challenges.FirstOrDefault(c => c.Id == challengeId)?.PlayerScores.FirstOrDefault(p => p.PlayerId == playerId);
                if (playerScore is not null && round >= 1 && round <= playerScore.Rounds.Count)
                {
                    return playerScore.Rounds[round - 1];
                }
                else
                {
                    return null;
                }
            }

            set
            {
                PlayerScoreDto? playerScore = Challenges.FirstOrDefault(c => c.Id == challengeId)?.PlayerScores.FirstOrDefault(p => p.PlayerId == playerId);
                if (playerScore is not null)
                {
                    switch (round)
                    {
                        case 1:
                            playerScore.Round1 = value;
                            break;

                        case 2:
                            playerScore.Round2 = value;
                            break;

                        case 3:
                            playerScore.Round3 = value;
                            break;

                        case 4:
                            playerScore.Round4 = value;
                            break;

                        case 5:
                            playerScore.Round5 = value;
                            break;
                    }
                }
            }
        }
    }
}
