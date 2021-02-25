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
        public IGamesApi GamesApi { get; set; } = null!;

        public IEnumerable<GameDto> Games { get; set; } = Enumerable.Empty<GameDto>();

        public IEnumerable<PlayerDto> Players { get; set; } = Enumerable.Empty<PlayerDto>();

        public IList<int> GetScores(int playerId)
        {
            return Games
                .Select(g => g.Values.Where(v => v.PlayerId == playerId && g.Maps.Select(m => m.Id).Contains(v.MapId)))
                .SelectMany(v => v)
                .Select(v => v.Score)
                .ToList();
        }

        protected override async Task OnInitializedAsync()
        {
            Games = await GamesApi.GetAllAsync();

            Players = Games.SelectMany(g => g.Players).GroupBy(p => p.Id).Select(g => g.First());
        }
    }
}
