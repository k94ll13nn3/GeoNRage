﻿using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.App.Services;
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

        [Inject]
        public PopupService PopupService { get; set; } = null!;

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

        private void Refresh()
        {
            PopupService.DisplayOkCancelPopup("Restoration", "Valider la restoration du challenge ?", async () => await RefreshAsync(), true);
        }

        private async Task RefreshAsync()
        {
            try
            {
                await ChallengesApi.ImportChallengeAsync(new() { GeoGuessrId = Challenge.GeoGuessrId, OverrideData = true });
                PopupService.HidePopup();
                ApiResponse<ChallengeDto> response = await ChallengesApi.GetAsync(Id);
                Challenge = response.Content!;
                StateHasChanged();
            }
            catch (ApiException e)
            {
                PopupService.DisplayPopup("Erreur", e.Content ?? "Echec de l'opération");
            }
        }
    }
}