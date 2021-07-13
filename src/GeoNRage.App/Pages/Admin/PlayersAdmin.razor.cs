using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.App.Core;
using GeoNRage.App.Services;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

namespace GeoNRage.App.Pages.Admin
{
    public partial class PlayersAdmin
    {
        [Inject]
        public IPlayersApi PlayersApi { get; set; } = null!;

        [Inject]
        public PopupService PopupService { get; set; } = null!;

        public IEnumerable<PlayerDto> Players { get; set; } = null!;

        public bool ShowEditForm { get; set; }

        public PlayerCreateDto Player { get; set; } = new();

        public string? SelectedPlayerId { get; set; }

        public EditForm Form { get; set; } = null!;

        public string? Error { get; set; }

        public void EditPlayer(string playerId)
        {
            Error = null;
            ShowEditForm = true;
            Player = new PlayerCreateDto { Name = Players.First(m => m.Id == playerId).Name, Id = playerId };
            SelectedPlayerId = playerId;
        }

        public void DeletePlayer(string playerId)
        {
            PopupService.DisplayOkCancelPopup("Suppression", $"Valider la suppression du joueur {playerId} ?", () => OnConfirmDeleteAsync(playerId), false);
        }

        public async Task CreateOrUpdatePlayerAsync()
        {
            try
            {
                Error = null;
                if (SelectedPlayerId is not null)
                {
                    await PlayersApi.UpdateAsync(SelectedPlayerId, Player);
                }
                else
                {
                    await PlayersApi.CreateAsync(Player);
                }

                ShowEditForm = false;
                SelectedPlayerId = null;
                Players = await PlayersApi.GetAllAsync();
                StateHasChanged();
            }
            catch (ApiException e)
            {
                Error = $"Error: {e.Content}";
            }
        }

        public void ShowPlayerCreation()
        {
            Error = null;
            ShowEditForm = true;
            Player = new PlayerCreateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            Players = await PlayersApi.GetAllAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            Form?.EditContext?.UpdateCssClassProvider();
        }

        private async void OnConfirmDeleteAsync(string playerId)
        {
            try
            {
                await PlayersApi.DeleteAsync(playerId);
                Players = await PlayersApi.GetAllAsync();
                StateHasChanged();
            }
            catch (ApiException e)
            {
                PopupService.DisplayPopup("Erreur", e.Content ?? string.Empty);
            }
        }
    }
}
