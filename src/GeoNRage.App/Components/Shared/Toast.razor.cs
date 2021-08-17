using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Shared;

public partial class Toast
{
    [Parameter]
    public string Message { get; set; } = null!;

    [Parameter]
    public EventCallback OnCloseCallback { get; set; }
}
