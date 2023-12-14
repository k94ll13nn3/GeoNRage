using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Layouts.Main;

public partial class SettingsList
{
    [Inject]
    public UserSettingsService UserSettingsService { get; set; } = null!;

    public bool AllMaps { get; set; }

    public Theme Theme { get; set; }

    public bool SeasonalStyle { get; set; }

    protected override async Task OnInitializedAsync()
    {
        UserSettings userSettings = await UserSettingsService.GetAsync();
        AllMaps = userSettings.AllMaps;
        Theme = userSettings.Theme;
        SeasonalStyle = userSettings.SeasonalStyle;
    }

    public async Task ChangeMapStatusAsync(bool allMaps)
    {
        AllMaps = allMaps;
        await UserSettingsService.SaveAsync(await UserSettingsService.GetAsync() with { AllMaps = AllMaps }, nameof(UserSettings.AllMaps));
    }

    public async Task ChangeThemeAsync(Theme theme)
    {
        Theme = theme;
        await UserSettingsService.SaveAsync(await UserSettingsService.GetAsync() with { Theme = Theme }, nameof(UserSettings.Theme));
    }

    public async Task ChangeSeasonalStyleAsync(bool seasonalStyle)
    {
        SeasonalStyle = seasonalStyle;
        await UserSettingsService.SaveAsync(await UserSettingsService.GetAsync() with { SeasonalStyle = SeasonalStyle }, nameof(UserSettings.SeasonalStyle));
    }
}
