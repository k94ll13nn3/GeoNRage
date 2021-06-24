using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.App.Components.Shared;
using GeoNRage.App.Services;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;
using Refit;

namespace GeoNRage.App.Pages.Admin
{
    public partial class ChallengesAdmin
    {
        [Inject]
        public IGamesApi GamesApi { get; set; } = null!;

        [Inject]
        public IChallengesApi ChallengesApi { get; set; } = null!;

        [Inject]
        public PopupService PopupService { get; set; } = null!;

        public IEnumerable<ChallengeDto> Challenges { get; set; } = null!;

        public string? Error { get; set; }

        public Table<ChallengeDto> ChallengesTable { get; set; } = null!;

        public void ImportChallenge(ChallengeDto challenge)
        {
            PopupService.DisplayOkCancelPopup("Restoration", "Valider la restoration du challenge ?", async () => await ImportChallengeAsync(challenge), true);
        }

        protected override async Task OnInitializedAsync()
        {
            Challenges = await ChallengesApi.GetAllAsync();
        }

        private void DeleteChallenge(int challengeId)
        {
            PopupService.DisplayOkCancelPopup("Suppression", $"Valider la suppression du challenge {challengeId} ?", () => OnConfirmDeleteAsync(challengeId), false);
        }

        private async void OnConfirmDeleteAsync(int challengeId)
        {
            try
            {
                await ChallengesApi.DeleteAsync(challengeId);
                Challenges = await ChallengesApi.GetAllAsync();
                ChallengesTable.SetItems(Challenges);
                StateHasChanged();
            }
            catch (ApiException e)
            {
                PopupService.DisplayPopup("Erreur", e.Content ?? string.Empty);
            }
        }

        private async Task ImportChallengeAsync(ChallengeDto challenge)
        {
            _ = challenge ?? throw new ArgumentNullException(nameof(challenge));

            try
            {
                Error = null;
                await GamesApi.ImportChallengeAsync(challenge.GameId, new ChallengeImportDto { GeoGuessrId = challenge.GeoGuessrId, OverrideData = true, PersistData = true });
            }
            catch (ApiException e)
            {
                Error = e.Content;
            }
            finally
            {
                PopupService.HidePopup();
                Challenges = await ChallengesApi.GetAllAsync();
                ChallengesTable.SetItems(Challenges);
            }
        }
    }
}
