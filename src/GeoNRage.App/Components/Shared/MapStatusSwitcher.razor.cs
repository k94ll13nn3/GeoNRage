using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GeoNRage.App.Components.Shared;

public partial class MapStatusSwitcher
{
    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;

    [Inject]
    public MapStatusService MapStatusService { get; set; } = null!;

    public bool AllMaps { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AllMaps = (await JsRuntime.InvokeAsync<string>("localStorage.getItem", MapStatusHandler.HeaderName)) == true.ToString();
        MapStatusService.SetMapStatus(AllMaps);
    }

    public async Task ChangeMapStatus(ChangeEventArgs e)
    {
        await JsRuntime.InvokeVoidAsync("localStorage.setItem", MapStatusHandler.HeaderName, e?.Value?.ToString());
        AllMaps = e?.Value is true;
        MapStatusService.SetMapStatus(AllMaps);
    }
}
