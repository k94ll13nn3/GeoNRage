using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace GeoNRage.App.Layouts.Main;

public partial class SiteHeader : IAsyncDisposable
{
    private IJSObjectReference? _jsModule;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = null!;

    public Theme Theme { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (_jsModule is not null)
        {
            await _jsModule.DisposeAsync();
            _jsModule = null;
        }
        GC.SuppressFinalize(this);
    }

    protected override async Task OnInitializedAsync()
    {
        Theme = (await UserSettingsService.GetAsync()).Theme;
        _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./Layouts/Main/{nameof(SiteHeader)}.razor.js");
        await UpdateStyleAsync();
    }

    internal override async void OnSettingsChanged(object? sender, UserSettingsEventArgs e)
    {
        Theme = e.Theme;
        await UpdateStyleAsync();
        StateHasChanged();
    }

    private async Task UpdateStyleAsync()
    {
        string? theme = Theme switch
        {
            Theme.Dark => "dark",
            Theme.Light => "light",
            _ => null
        };

        if (_jsModule is not null)
        {
            await _jsModule.InvokeVoidAsync("enableStyleSheet", theme);
        }
    }
}
