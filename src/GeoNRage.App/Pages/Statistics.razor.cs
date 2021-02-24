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

        public Dictionary<(int id, string name), IList<int>> PlayersWithScores { get; } = new();

        protected override async Task OnInitializedAsync()
        {
            Games = await GamesApi.GetAllAsync();

            foreach ((int id, string name) in Games.SelectMany(g => g.Players.Select(p => (p.Id, p.Name))))
            {
                PlayersWithScores[(id, name)] = Games
                    .Select(g => g.Values.Where(v => v.PlayerId == id && g.Maps.Select(m => m.Id).Contains(v.MapId)))
                    .SelectMany(v => v)
                    .Select(v => v.Score)
                    .ToList();
            }
        }
    }
}
