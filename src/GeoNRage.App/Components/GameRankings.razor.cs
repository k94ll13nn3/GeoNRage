using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components
{
    public partial class GameRankings
    {
        [Parameter]
        public GameDto Game { get; set; } = null!;
    }
}
