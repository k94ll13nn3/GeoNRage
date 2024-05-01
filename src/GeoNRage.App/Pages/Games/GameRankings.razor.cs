using GeoNRage.App.Layouts.Main;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Games;

public partial class GameRankings : IModal
{
    [Parameter]
    public IEnumerable<GameChallengeDto> Challenges { get; set; } = null!;

    [Parameter]
    public IEnumerable<GamePlayerDto> Players { get; set; } = null!;

    [Parameter]
    public IReadOnlyDictionary<(int challengeId, string playerId, int round), int?> Scores { get; set; } = null!;

    [CascadingParameter]
    public ModalRender ModalRender { get; set; } = null!;

    public string Id => nameof(GameRankings);

    public string? GetPlayerName(string playerId)
    {
        return Players.FirstOrDefault(p => p.Id == playerId)?.Name;
    }

    private void Close()
    {
        ModalRender.Close();
    }
}
