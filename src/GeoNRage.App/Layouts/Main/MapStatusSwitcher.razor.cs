using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Layouts.Main;

public partial class MapStatusSwitcher
{
    [Inject]
    public UserSettingsService UserSettingsService { get; set; } = null!;

    public bool AllMaps { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AllMaps = (await UserSettingsService.GetAsync()).AllMaps;
    }

    public async Task ChangeMapStatusAsync(ChangeEventArgs e)
    {
        AllMaps = e?.Value is true;
        await UserSettingsService.SaveAsync(await UserSettingsService.GetAsync() with { AllMaps = AllMaps });
    }
}
