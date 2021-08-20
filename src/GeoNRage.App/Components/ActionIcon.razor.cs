using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components;

public partial class ActionIcon
{
    [Parameter]
    public Uri Link { get; set; } = null!;

    [Parameter]
    public string Icon { get; set; } = null!;

    [Parameter]
    public bool IsExternal { get; set; }

    [Parameter]
    public bool IsSmall { get; set; } = true;

    [Parameter]
    public string Tooltip { get; set; } = null!;
}
