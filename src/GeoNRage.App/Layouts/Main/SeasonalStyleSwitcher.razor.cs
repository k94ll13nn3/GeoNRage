using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Layouts.Main;

public partial class SeasonalStyleSwitcher
{
    private bool _seasonalStyle;

    [Inject]
    public UserSettingsService UserSettingsService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        UserSettings userSettings = await UserSettingsService.GetAsync();
        _seasonalStyle = userSettings.SeasonalStyle;
    }

    public async Task ToggleSeasonalStyleAsync()
    {
        _seasonalStyle = !_seasonalStyle;
        await UserSettingsService.SaveAsync(await UserSettingsService.GetAsync() with { SeasonalStyle = _seasonalStyle }, nameof(UserSettings.SeasonalStyle));
    }
}
