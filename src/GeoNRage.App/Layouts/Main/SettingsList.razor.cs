using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Layouts.Main;

public partial class SettingsList
{
    [Inject]
    public UserSettingsService UserSettingsService { get; set; } = null!;

    public bool AllMaps { get; set; }

    public Theme Theme { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AllMaps = (await UserSettingsService.GetAsync()).AllMaps;
        Theme = (await UserSettingsService.GetAsync()).Theme;
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
}
