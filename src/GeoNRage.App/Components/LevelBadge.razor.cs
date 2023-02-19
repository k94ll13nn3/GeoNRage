using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;
using Refit;

namespace GeoNRage.App.Components;

public partial class LevelBadge
{
    [Parameter]
    [EditorRequired]
    public string Id { get; set; } = null!;

    [Parameter]
    public int Size { get; set; } = 40;

    [Inject]
    public IPlayersApi PlayersApi { get; set; } = null!;

    public PlayerExperienceDto? PlayerExperience { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ApiResponse<PlayerExperienceDto> response = await PlayersApi.GetExperienceAsync(Id);
        if (response.IsSuccessStatusCode && response.Content is not null)
        {
            PlayerExperience = response.Content;
        }
    }
}
