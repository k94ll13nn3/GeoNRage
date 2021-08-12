using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Shared;

public partial class PageNotFound
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;
}
