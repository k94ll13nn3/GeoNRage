using GeoNRage.App.Apis;
using GeoNRage.App.Components;
using Microsoft.AspNetCore.Components;
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

    public Table<MapDto> MapsTable { get; set; } = null!;

    public async Task EditMapAsync(MapDto map)
    {
        ModalResult result = await ModalService.DisplayModalAsync<MapAdminEdit>(new()
        {
            [nameof(MapAdminEdit.Map)] = map,
        }, ModalOptions.Default);

        if (!result.Cancelled)
        {
            Maps = await MapsApi.GetAllAsync();
            MapsTable.SetItems(Maps);
        }
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

    protected override async Task OnInitializedAsync()
    {
        Maps = await MapsApi.GetAllAsync();
    }
}
