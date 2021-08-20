using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components;

public partial class Expander
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool IsExpanded { get; set; }

    [Parameter]
    public string Title { get; set; } = null!;
}
