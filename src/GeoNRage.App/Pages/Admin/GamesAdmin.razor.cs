using GeoNRage.App.Apis;
using GeoNRage.App.Components;
using Microsoft.AspNetCore.Components;
using Refit;

namespace GeoNRage.App.Pages.Admin;

public partial class GamesAdmin
{
    private IEnumerable<GameAdminViewDto> _games = [];
    private Table<GameAdminViewDto> _gamesTable = null!;

    [Inject]
    public IMapsApi MapsApi { get; set; } = null!;

    [Inject]
    public IPlayersApi PlayersApi { get; set; } = null!;

    [Inject]
    public IGamesApi GamesApi { get; set; } = null!;

    [Inject]
    public ModalService ModalService { get; set; } = null!;

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    public async Task CreateOrEditGameAsync(GameAdminViewDto? game)
    {
        ModalResult result = await ModalService.DisplayModalAsync<GameAdminCreateOrEdit>(new()
        {
            [nameof(GameAdminCreateOrEdit.Game)] = game,
        }, ModalOptions.Default);

        if (!result.Cancelled)
        {
            await LoadGamesAsync();
            _gamesTable.SetItems(_games);
        }
    }

    public async Task DeleteGameAsync(int gameId)
    {
        ModalResult result = await ModalService.DisplayOkCancelPopupAsync("Suppression", $"Valider la suppression de la partie {gameId} ?");
        if (!result.Cancelled)
        {
            try
            {
                await GamesApi.DeleteAsync(gameId);
                await LoadGamesAsync();
                StateHasChanged();
                _gamesTable.SetItems(_games);
            }
            catch (ApiException e)
            {
                await ToastService.DisplayErrorToastAsync(e, "game-delete");
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadGamesAsync();
    }

    private async Task LoadGamesAsync()
    {
        _games = await GamesApi.GetAllAsAdminViewAsync();
    }
}
