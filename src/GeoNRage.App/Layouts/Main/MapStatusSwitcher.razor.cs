using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Layouts.Main;

public partial class MapStatusSwitcher
{
    [Inject]
    public UserSettingsService UserSettingsService { get; set; } = null!;

    public bool AllMaps { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AllMaps = (await UserSettingsService.Get()).AllMaps;
    }

    public async Task ChangeMapStatus(ChangeEventArgs e)
    {
        AllMaps = e?.Value is true;
        await UserSettingsService.Save(await UserSettingsService.Get() with { AllMaps = AllMaps });
    }
}
