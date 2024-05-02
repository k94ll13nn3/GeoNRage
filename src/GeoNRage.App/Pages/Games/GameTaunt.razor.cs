using GeoNRage.App.Layouts.Main;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Games;

public partial class GameTaunt : IModal
{
    private string? _selectedPlayerId;
    private string? _selectedImageId;

    [Parameter]
    public EventCallback<(string player, string image)> OnTaunt { get; set; }

    [Parameter]
    public IEnumerable<GamePlayerDto> Players { get; set; } = null!;

    [CascadingParameter]
    public ModalRender ModalRender { get; set; } = null!;

    public string Id => nameof(GameTaunt);

    private async Task TauntAsync()
    {
        await OnTaunt.InvokeAsync((_selectedPlayerId!, _selectedImageId!));
    }

    private void Close()
    {
        ModalRender.Close();
    }
}
