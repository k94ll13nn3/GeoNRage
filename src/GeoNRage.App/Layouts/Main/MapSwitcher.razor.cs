using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Layouts.Main;

public partial class MapSwitcher
{
    private bool _allMaps;

    [Inject]
    public UserSettingsService UserSettingsService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        UserSettings userSettings = await UserSettingsService.GetAsync();
        _allMaps = userSettings.AllMaps;
    }

    public async Task ToggleMapAsync()
    {
        _allMaps = !_allMaps;
        await UserSettingsService.SaveAsync(await UserSettingsService.GetAsync() with { AllMaps = _allMaps }, nameof(UserSettings.AllMaps));
    }
}
