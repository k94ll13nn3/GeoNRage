using GeoNRage.Shared.Dtos.Games;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Games
{
    public partial class GameRankings
    {
        [Parameter]
        public GameDetailDto Game { get; set; } = null!;
    }
}
