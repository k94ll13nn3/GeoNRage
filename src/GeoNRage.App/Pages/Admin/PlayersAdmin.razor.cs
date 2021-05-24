using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.App.Core;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Refit;

namespace GeoNRage.App.Pages.Admin
{
    public partial class PlayersAdmin
    {
        [Inject]
        public IPlayersApi PlayersApi { get; set; } = null!;

        [Inject]
        public IJSRuntime JSRuntime { get; set; } = null!;

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

        public async Task DeletePlayerAsync(string playerId)
        {
            if (!await JSRuntime.InvokeAsync<bool>("confirm", $"Valider la suppression du joueur {playerId} ?"))
            {
                return;
            }

            await PlayersApi.DeleteAsync(playerId);
            Players = await PlayersApi.GetAllAsync();
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
            }
            catch (ApiException e)
            {
                Error = e.Content;
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
    }
}
