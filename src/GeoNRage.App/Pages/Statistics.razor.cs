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
        public IChallengesApi ChallengesApi { get; set; } = null!;

        [Inject]
        public IPlayersApi PlayersApi { get; set; } = null!;

        [Inject]
        public IMapsApi MapsApi { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public IEnumerable<PlayerDto> Players { get; set; } = Enumerable.Empty<PlayerDto>();

        public IEnumerable<MapDto> Maps { get; set; } = Enumerable.Empty<MapDto>();

        public Dictionary<string, List<(ChallengeDto challenge, PlayerScoreDto score)>> ScoresByPlayer { get; private set; } = new();

        protected override async Task OnInitializedAsync()
        {
            ChallengeDto[] challenges = await ChallengesApi.GetAllAsync();
            ScoresByPlayer = challenges
                .SelectMany(c => c.PlayerScores.Select(p => (challenge: c, score: p)))
                .GroupBy(p => p.score.PlayerId)
                .ToDictionary(g => g.Key, g => g.ToList());
            Players = await PlayersApi.GetAllAsync();
            Maps = await MapsApi.GetAllAsync();
        }
    }
}
