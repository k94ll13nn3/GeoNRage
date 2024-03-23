using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Layouts.Main;

public partial class ThemeSwitcher
{
    private Theme _theme;

    [Inject]
    public UserSettingsService UserSettingsService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        UserSettings userSettings = await UserSettingsService.GetAsync();
        _theme = userSettings.Theme;
    }

    public async Task ToggleThemeAsync()
    {
        _theme = _theme switch
        {
            Theme.System => Theme.Dark,
            Theme.Dark => Theme.Light,
            _ => Theme.System,
        };
        await UserSettingsService.SaveAsync(await UserSettingsService.GetAsync() with { Theme = _theme }, nameof(UserSettings.Theme));
    }
}
