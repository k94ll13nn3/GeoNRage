using GeoNRage.App.Apis;
using GeoNRage.App.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

namespace GeoNRage.App.Pages.Admin;

public partial class PlayersAdmin
{
    [Inject]
    public IPlayersApi PlayersApi { get; set; } = null!;

    [Inject]
    public ModalService ModalService { get; set; } = null!;

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    public IEnumerable<PlayerAdminViewDto> Players { get; set; } = null!;

    public bool ShowEditForm { get; set; }

    public PlayerEditDto Player { get; set; } = new();

    public string? SelectedPlayerId { get; set; }

    public EditForm Form { get; set; } = null!;

    public string? Error { get; set; }

    public Table<PlayerAdminViewDto> PlayersTable { get; set; } = null!;

    public void EditPlayer(string playerId)
    {
        Error = null;
        ShowEditForm = true;
        Player = new PlayerEditDto { Name = Players.First(m => m.Id == playerId).Name, AssociatedPlayerId = Players.First(m => m.Id == playerId).AssociatedPlayerId };
        SelectedPlayerId = playerId;
    }

    public async Task DeletePlayerAsync(string playerId)
    {
        ModalResult result = await ModalService.DisplayOkCancelPopupAsync("Suppression", $"Valider la suppression du joueur {playerId} ?");
        if (!result.Cancelled)
        {
            try
            {
                await PlayersApi.DeleteAsync(playerId);
                Players = await PlayersApi.GetAllAsAdminViewAsync();
                PlayersTable.SetItems(Players);
                StateHasChanged();
            }
            catch (ApiException e)
            {
                await ToastService.DisplayErrorToastAsync(e, "player-delete");
            }
        }
    }

    public async Task UpdateGeoGuessrProfileAsync(string playerId)
    {
        ModalResult result = await ModalService.DisplayOkCancelPopupAsync("Mise à jour", $"Valider la mise à jour du joueur {playerId} ?");
        if (!result.Cancelled)
        {
            try
            {
                await PlayersApi.UpdateGeoGuessrProfileAsync(playerId);
                Players = await PlayersApi.GetAllAsAdminViewAsync();
                PlayersTable.SetItems(Players);
                StateHasChanged();
            }
            catch (ApiException e)
            {
                await ToastService.DisplayErrorToastAsync(e, "player-update-geoguessr");
            }
        }
    }

    public async Task UpdatePlayerAsync()
    {
        try
        {
            Error = null;
            if (SelectedPlayerId is not null)
            {
                await PlayersApi.UpdateAsync(SelectedPlayerId, Player);
            }

            ShowEditForm = false;
            SelectedPlayerId = null;
            Players = await PlayersApi.GetAllAsAdminViewAsync();
            PlayersTable.SetItems(Players);
            StateHasChanged();
        }
        catch (ApiException e)
        {
            Error = $"Error: {(await e.GetContentAsAsync<ApiError>())?.Message}";
        }
    }

    protected override async Task OnInitializedAsync()
    {
        Players = await PlayersApi.GetAllAsAdminViewAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        Form?.EditContext?.UpdateCssClassProvider();
    }
}
