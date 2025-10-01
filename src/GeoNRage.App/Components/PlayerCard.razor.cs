using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;
using Refit;

namespace GeoNRage.App.Components;

public partial class PlayerCard
{
    private PlayerResumeDto? _player;
    private bool _loaded;

    [Parameter]
    public string Id { get; set; } = null!;

    [Inject]
    public IPlayersApi PlayersApi { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        _loaded = false;
        StateHasChanged();
        ApiResponse<PlayerResumeDto> response = await PlayersApi.GetResumeAsync(Id);
        if (response.IsSuccessStatusCode && response.Content is not null)
        {
            _loaded = true;
            _player = response.Content;
            StateHasChanged();
        }
    }
}
