using GeoNRage.App.Apis;
using GeoNRage.App.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

namespace GeoNRage.App.Pages.Admin;

public partial class MapsAdmin
{
    [Inject]
    public IMapsApi MapsApi { get; set; } = null!;

    [Inject]
    public ModalService ModalService { get; set; } = null!;

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    public IEnumerable<MapDto> Maps { get; set; } = null!;

    public bool ShowEditForm { get; set; }

    public MapEditDto Map { get; set; } = new();

    public string? SelectedMapId { get; set; }

    public EditForm Form { get; set; } = null!;

    public string? Error { get; set; }

    public Table<MapDto> MapsTable { get; set; } = null!;

    public void EditMap(string mapId)
    {
        Error = null;
        ShowEditForm = true;
        Map = new MapEditDto { Name = Maps.First(m => m.Id == mapId).Name, IsMapForGame = Maps.First(m => m.Id == mapId).IsMapForGame };
        SelectedMapId = mapId;
    }

    public async Task DeleteMapAsync(string mapId)
    {
        ModalResult result = await ModalService.DisplayOkCancelPopupAsync("Suppression", $"Valider la suppression de la carte {mapId} ?");
        if (!result.Cancelled)
        {
            try
            {
                await MapsApi.DeleteAsync(mapId);
                Maps = await MapsApi.GetAllAsync();
                MapsTable.SetItems(Maps);
                StateHasChanged();
            }
            catch (ApiException e)
            {
                await ToastService.DisplayErrorToastAsync(e, "map-delete");
            }
        }
    }

    public async Task UpdateMapAsync()
    {
        try
        {
            Error = null;
            if (SelectedMapId is not null)
            {
                await MapsApi.UpdateAsync(SelectedMapId, Map);
            }

            ShowEditForm = false;
            SelectedMapId = null;
            Maps = await MapsApi.GetAllAsync();
            MapsTable.SetItems(Maps);
            StateHasChanged();
        }
        catch (ApiException e)
        {
            Error = $"Error: {(await e.GetContentAsAsync<ApiError>())?.Message}";
        }
    }

    protected override async Task OnInitializedAsync()
    {
        Maps = await MapsApi.GetAllAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        Form?.EditContext?.UpdateCssClassProvider();
    }
}
