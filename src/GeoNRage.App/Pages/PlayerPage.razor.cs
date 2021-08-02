using System;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos.Players;
using Microsoft.AspNetCore.Components;
using Refit;

namespace GeoNRage.App.Pages
{
    public partial class PlayerPage
    {
        [Parameter]
        public string Id { get; set; } = null!;

        [Inject]
        public IPlayersApi PlayersApi { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public PlayerFullDto Player { get; set; } = null!;

        public bool PlayerFound { get; set; } = true;

        public bool Loaded { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Loaded = false;
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
                StateHasChanged();
            }
        }

        internal override async void OnMapStatusChanged(object? sender, EventArgs e)
        {
            await OnInitializedAsync();
        }
    }
}
