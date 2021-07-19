using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;
using Refit;

namespace GeoNRage.App.Pages
{
    public partial class PlayerPage
    {
        [Parameter]
        public string Id { get; set; } = null!;

        [Inject]
        public IMapsApi MapsApi { get; set; } = null!;

        [Inject]
        public IPlayersApi PlayersApi { get; set; } = null!;

        [Inject]
        public IChallengesApi ChallengesApi { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public IEnumerable<MapDto> Maps { get; set; } = null!;

        public IEnumerable<string> MapsForGame { get; set; } = null!;

        public IEnumerable<ChallengeDto> ChallengesNotDone { get; set; } = null!;

        public IEnumerable<(int id, int score)> GameHistoric { get; set; } = null!;

        public PlayerFullDto Player { get; set; } = null!;

        public IEnumerable<PlayerScoreWithChallengeDto> FilteredScores { get; set; } = null!;

        public bool PlayerFound { get; set; } = true;

        public bool Loaded { get; set; }

        public bool ShowAllMaps { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Maps = await MapsApi.GetAllAsync();
            MapsForGame = Maps.Where(m => m.IsMapForGame).Select(m => m.Id).ToList();
            ChallengeDto[] challenges = await ChallengesApi.GetAllAsync();

            ApiResponse<PlayerFullDto> response = await PlayersApi.GetFullAsync(Id);
            if (!response.IsSuccessStatusCode || response.Content is null)
            {
                PlayerFound = false;
            }
            else
            {
                Loaded = true;
                PlayerFound = true;
                Player = response.Content;

                FilteredScores = Player
                    .PlayerScores
                    .Where(p => (p.ChallengeTimeLimit ?? 300) == 300 && (p.GameId != -1 || p.MapIsMapForGame));

                IEnumerable<int> challengesDoneIds = Player.PlayerScores.Where(p => p.Rounds.All(r => r is not null)).Select(p => p.ChallengeId);
                ChallengesNotDone = challenges.Where(c => !challengesDoneIds.Contains(c.Id)).OrderByDescending(c => c.GameDate);
                GameHistoric = Player
                    .PlayerScores
                    .Where(p => (p.ChallengeTimeLimit ?? 300) == 300 && p.GameId != -1)
                    .GroupBy(p => p.GameId)
                    .Where(g => g.Count() == 3)
                    .OrderBy(g => g.First().GameDate)
                    .Select(g => (id: g.Key, score: g.Sum(p => p.Sum) ?? 0));

                StateHasChanged();
            }
        }

        private void ShowAllMapToggle()
        {
            ShowAllMaps = !ShowAllMaps;
            StateHasChanged();
        }
    }
}
