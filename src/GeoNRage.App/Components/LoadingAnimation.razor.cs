using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components;

public partial class LoadingAnimation
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = null!;

    [Parameter]
    public bool Animated { get; set; } = true;

    [Parameter]
    public bool BigIcon { get; set; }
}
