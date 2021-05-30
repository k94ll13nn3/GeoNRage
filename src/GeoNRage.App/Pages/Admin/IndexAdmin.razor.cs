using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Admin
{
    public partial class IndexAdmin
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        protected override void OnInitialized()
        {
            NavigationManager.NavigateTo("/admin/maps");
        }
    }
}
