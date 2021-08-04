using System.Collections.Generic;
using System.Linq;
using GeoNRage.Shared.Dtos.Games;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Games
{
    public partial class GameRankings
    {
        [Parameter]
        public IEnumerable<GameChallengeDto> Challenges { get; set; } = null!;

        [Parameter]
        public IEnumerable<GamePlayerDto> Players { get; set; } = null!;

        [Parameter]
        public IReadOnlyDictionary<(int challengeId, string playerId, int round), int?> Scores { get; set; } = null!;

        public string? GetPlayerName(string playerId)
        {
            return Players.FirstOrDefault(p => p.Id == playerId)?.Name;
        }
    }
}
