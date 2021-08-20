using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Admin;

public partial class AdminMenu
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;
}
