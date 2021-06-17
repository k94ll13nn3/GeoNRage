using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;
using Refit;

namespace GeoNRage.App.Pages
{
    public partial class ChallengePage
    {
        [Inject]
        public IChallengesApi ChallengesApi { get; set; } = null!;

        [Parameter]
        public int Id { get; set; }

        public bool ChallengeFound { get; set; } = true;

        public bool Loaded { get; set; }

        public ChallengeDto Challenge { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            ApiResponse<ChallengeDto> response = await ChallengesApi.GetAsync(Id);
            if (!response.IsSuccessStatusCode || response.Content is null)
            {
                ChallengeFound = false;
            }
            else
            {
                Loaded = true;
                ChallengeFound = true;
                Challenge = response.Content;
                StateHasChanged();
            }
        }

        private async Task RefreshAsync()
        {
            await ChallengesApi.ImportChallengeAsync(new() { GeoGuessrId = Challenge.GeoGuessrId });
            ApiResponse<ChallengeDto> response = await ChallengesApi.GetAsync(Id);
            Challenge = response.Content!;
            StateHasChanged();
        }
    }
}
