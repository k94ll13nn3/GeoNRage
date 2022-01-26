using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GeoNRage.App.Layouts.Main;

public partial class SiteHeader
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

    internal override void OnSettingsChanged(object? sender, UserSettingsEventArgs e)
    {
        Theme = e.Theme;
        StateHasChanged();
    }
}
