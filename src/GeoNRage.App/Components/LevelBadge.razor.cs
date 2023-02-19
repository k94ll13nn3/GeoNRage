using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components;

public partial class LevelBadge
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = null!;

    [Parameter]
    public int Size { get; set; } = 40;
}
