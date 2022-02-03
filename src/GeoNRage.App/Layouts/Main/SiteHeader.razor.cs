using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace GeoNRage.App.Layouts.Main;

public partial class SiteHeader : IAsyncDisposable
{
    [Inject]
    public IJSRuntime JSRuntime { get; set; } = null!;

    public Theme Theme { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Theme = (await UserSettingsService.GetAsync()).Theme;
        IJSObjectReference jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./Layouts/Main/{nameof(SiteHeader)}.razor.js");
        await jsModule.InvokeVoidAsync("disableStyleSheet");
    }

    public async ValueTask DisposeAsync()
    {
        IJSObjectReference jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./Layouts/Main/{nameof(SiteHeader)}.razor.js");
        await jsModule.InvokeVoidAsync("enableStyleSheet");
        GC.SuppressFinalize(this);
    }

    internal override void OnSettingsChanged(object? sender, UserSettingsEventArgs e)
    {
        Theme = e.Theme;
        StateHasChanged();
    }
}
