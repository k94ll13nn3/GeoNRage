using System.Text.Json;
using Microsoft.JSInterop;

namespace GeoNRage.App.Services;

[AutoConstructor]
public partial class UserSettingsService
{
    private const string KeyName = "settingsV2";

    private readonly IJSRuntime _jsRuntime;

    public event EventHandler<UserSettingsEventArgs>? SettingsChanged;

    public async Task<UserSettings> GetAsync()
    {
        string json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", KeyName);
        var defaultSettings = new UserSettings(false, Theme.System, true);
        return json != null ? JsonSerializer.Deserialize<UserSettings>(json) ?? defaultSettings : defaultSettings;
    }

    public async Task SaveAsync(UserSettings settings, string changedKey)
    {
        ArgumentNullException.ThrowIfNull(settings);

        string json = JsonSerializer.Serialize(settings);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", KeyName, json);
        SettingsChanged?.Invoke(this, new(settings.AllMaps, settings.Theme, settings.SeasonalStyle, changedKey));
    }
}
