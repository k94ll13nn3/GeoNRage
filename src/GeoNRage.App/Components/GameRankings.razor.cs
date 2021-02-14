using GeoNRage.Data.Entities;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components
{
    public partial class GameRankings
    {
        [Parameter]
        public Game Game { get; set; } = null!;
    }
}
