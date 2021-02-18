using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Games
{
    public partial class GameRankings
    {
        [Parameter]
        public GameDto Game { get; set; } = null!;
    }
}
