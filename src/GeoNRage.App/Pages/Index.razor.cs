using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace GeoNRage.App.Pages
{
    public partial class Index : IAsyncDisposable
    {
        private HubConnection _hubConnection = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public async ValueTask DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/apphub"))
                .Build();

            _hubConnection.On<IEnumerable<GameBase>>("ReceiveGames", games =>
            {
                if (games.Any())
                {
                    NavigationManager.NavigateTo($"/games/{games.First().Id}");
                }

                StateHasChanged();
            });

            await _hubConnection.StartAsync();
            await _hubConnection.InvokeAsync("LoadGames");
        }
    }
}
