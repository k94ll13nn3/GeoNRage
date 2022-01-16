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

    public async Task ChangeMapStatusAsync(ChangeEventArgs e)
    {
        AllMaps = e?.Value is true;
        await UserSettingsService.SaveAsync(await UserSettingsService.GetAsync() with { AllMaps = AllMaps });
    }

    public async Task ChangeThemeAsync(ChangeEventArgs e)
    {
        Theme = e?.Value is true ? Theme.Dark : Theme.Light;
        await UserSettingsService.SaveAsync(await UserSettingsService.GetAsync() with { Theme = Theme });
    }
}
