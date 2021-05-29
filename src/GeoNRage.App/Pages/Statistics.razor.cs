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
        public IPlayersApi PlayersApi { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public IEnumerable<PlayerFullDto> Players { get; set; } = Enumerable.Empty<PlayerFullDto>();

        protected override async Task OnInitializedAsync()
        {
            Players = await PlayersApi.GetAllFullAsync();
        }
    }
}
