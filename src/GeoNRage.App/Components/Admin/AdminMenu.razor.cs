using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Admin
{
    public partial class AdminMenu
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;
    }
}
