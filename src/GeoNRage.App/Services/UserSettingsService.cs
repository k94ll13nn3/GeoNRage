using System.Text.Json;
using Microsoft.JSInterop;

namespace GeoNRage.App.Services;

[AutoConstructor]
public partial class UserSettingsService
{
    private const string KeyName = "state";

    private readonly IJSRuntime _jsRuntime;

    public event EventHandler? SettingsChanged;

    public async Task<UserSettings> Get()
    {
        string str = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", KeyName);
        return str != null ? JsonSerializer.Deserialize<UserSettings>(str) ?? new UserSettings(false) : new UserSettings(false);
    }

    public async Task Save(UserSettings settings)
    {
        string json = JsonSerializer.Serialize(settings);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", KeyName, json);
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }
}
