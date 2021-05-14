using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages
{
    public partial class Statistics
    {
        [Inject]
        public IGamesApi GamesApi { get; set; } = null!;

        [Inject]
        public IPlayersApi PlayersApi { get; set; } = null!;

        public IEnumerable<GameDto> Games { get; set; } = Enumerable.Empty<GameDto>();

        public IEnumerable<PlayerDto> Players { get; set; } = Enumerable.Empty<PlayerDto>();

        public IList<int> GetScores(string playerId)
        {
            List<int> scores = new();
            foreach (GameDto game in Games)
            {
                foreach (ChallengeDto challenge in game.Challenges)
                {
                    PlayerScoreDto? playerScore = challenge.PlayerScores.FirstOrDefault(x => x.PlayerId == playerId);
                    if (playerScore is not null)
                    {
                        scores.Add(playerScore.Round1);
                        scores.Add(playerScore.Round2);
                        scores.Add(playerScore.Round3);
                        scores.Add(playerScore.Round4);
                        scores.Add(playerScore.Round5);
                    }
                }
            }

            return scores;
        }

        protected override async Task OnInitializedAsync()
        {
            Games = await GamesApi.GetAllAsync();
            Players = await PlayersApi.GetAllAsync();
        }
    }
}
