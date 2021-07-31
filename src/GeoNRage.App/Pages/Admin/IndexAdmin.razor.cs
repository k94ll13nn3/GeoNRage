using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos.Admin;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Admin
{
    public partial class IndexAdmin
    {
        [Inject]
        public IAdminApi AdminApi { get; set; } = null!;

        public AdminInfoDto AdminInfo { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            AdminInfo = await AdminApi.GetAdminInfoAsync();
        }
    }
}
