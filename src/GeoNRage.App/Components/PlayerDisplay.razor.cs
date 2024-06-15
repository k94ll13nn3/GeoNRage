using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components;

public partial class PlayerDisplay
{
    [Parameter]
    [EditorRequired]
    public string Id { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public string Name { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public Uri IconUrl { get; set; } = null!;

    [Parameter]
    public bool WithLink { get; set; }

    [Parameter]
    public bool WithMargin { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;
}
