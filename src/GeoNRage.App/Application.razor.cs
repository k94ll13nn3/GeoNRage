using Microsoft.AspNetCore.Components;

namespace GeoNRage.App
{
    public partial class Application
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;
    }
}
