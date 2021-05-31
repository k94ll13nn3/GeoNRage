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

        public IEnumerable<ChallengeDto> ChallengesNotDone { get; set; } = null!;

        public IEnumerable<int> GameHistoric { get; set; } = null!;

        public PlayerFullDto Player { get; set; } = null!;

        public bool PlayerFound { get; set; } = true;

        public bool Loaded { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Maps = await MapsApi.GetAllAsync();
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

                ChallengesNotDone = challenges.ExceptBy(Player.PlayerScores.Where(p => p.Rounds.All(r => r is not null)).Select(p => p.ChallengeId), c => c.Id);
                GameHistoric = Player
                    .PlayerScores
                    .GroupBy(p => p.GameDate)
                    .Where(g => g.Count() == 3)
                    .OrderBy(g => g.Key)
                    .Select(g => g.Sum(p => p.Sum) ?? 0);

                StateHasChanged();
            }
        }
    }
}
