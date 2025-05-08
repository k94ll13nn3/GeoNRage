using GeoNRage.App.Apis;
using GeoNRage.App.Layouts.Main;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

namespace GeoNRage.App.Pages.Admin;

public partial class MapAdminEdit : IModal
{
    private MapEditDto _map = new();
    private EditForm _form = null!;
    private string? _error;

    [CascadingParameter]
    public ModalRender ModalRender { get; set; } = null!;

    [Parameter]
    public MapDto Map { get; set; } = null!;

    [Inject]
    public IMapsApi MapsApi { get; set; } = null!;

    public string Id => nameof(MapAdminEdit);

    protected override void OnInitialized()
    {
        _map = new MapEditDto { Name = Map.Name, IsMapForGame = Map.IsMapForGame };
    }

    protected override void OnAfterRender(bool firstRender)
    {
        _form?.EditContext?.UpdateCssClassProvider();
    }

    private void Cancel()
    {
        ModalRender.Cancel();
    }

    private async Task UpdateMapAsync()
    {
        try
        {
            await MapsApi.UpdateAsync(Map.Id, _map);
            ModalRender.Close();
        }
        catch (ApiException e)
        {
            _error = $"Error: {(await e.GetContentAsAsync<ProblemDetails>())?.Detail}";
        }
    }
}
