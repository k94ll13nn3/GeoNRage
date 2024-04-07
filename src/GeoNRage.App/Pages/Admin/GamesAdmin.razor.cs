using GeoNRage.App.Apis;
using GeoNRage.App.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

namespace GeoNRage.App.Pages.Admin;

public partial class GamesAdmin
{
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

    public IEnumerable<MapDto> Maps { get; set; } = null!;

    public IEnumerable<PlayerDto> Players { get; set; } = null!;

    public IEnumerable<GameAdminViewDto> Games { get; set; } = null!;

    public bool ShowEditForm { get; set; }

    public GameCreateOrEditDto Game { get; set; } = new();

    public int? SelectedGameId { get; set; }

    public string? Error { get; set; }

    public EditForm Form { get; set; } = null!;

    public Table<GameAdminViewDto> GamesTable { get; set; } = null!;

    public void EditGame(int gameId)
    {
        ShowEditForm = true;
        GameAdminViewDto gameToEdit = Games.First(m => m.Id == gameId);
        Game = new GameCreateOrEditDto
        {
            Name = gameToEdit.Name,
            Date = gameToEdit.Date,
            Challenges = gameToEdit.Challenges.Select(c => new GameChallengeCreateOrEditDto { Id = c.Id, GeoGuessrId = c.GeoGuessrId, MapId = c.MapId }).ToList(),
            PlayerIds = gameToEdit.PlayerIds.ToList()
        };

        SelectedGameId = gameId;
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
                GamesTable.SetItems(Games);
            }
            catch (ApiException e)
            {
                await ToastService.DisplayErrorToastAsync(e, "game-delete");
            }
        }
    }

    public async Task CreateOrUpdateGameAsync()
    {
        Error = string.Empty;
        try
        {
            if (SelectedGameId is not null)
            {
                await GamesApi.UpdateAsync(SelectedGameId.Value, Game);
            }
            else
            {
                await GamesApi.CreateAsync(Game);
            }

            ShowEditForm = false;
            SelectedGameId = null;
            await LoadGamesAsync();
            StateHasChanged();
            GamesTable.SetItems(Games);
        }
        catch (ValidationApiException e)
        {
            Error = $"Error: {string.Join(",", e.Content?.Errors.Select(x => string.Join(Environment.NewLine, x.Value)) ?? [])}";
        }
        catch (ApiException e)
        {
            Error = $"Error: {(await e.GetContentAsAsync<ApiError>())?.Message}";
        }
    }

    public void ShowGameCreation()
    {
        Error = string.Empty;
        ShowEditForm = true;
        Game = new GameCreateOrEditDto { Date = DateTime.Now };
    }

    protected override async Task OnInitializedAsync()
    {
        Maps = (await MapsApi.GetAllAsync()).OrderByDescending(m => m.IsMapForGame);
        Players = await PlayersApi.GetAllAsync();
        await LoadGamesAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        Form?.EditContext?.UpdateCssClassProvider();
    }

    private async Task LoadGamesAsync()
    {
        Games = await GamesApi.GetAllAsAdminViewAsync();
    }
}
