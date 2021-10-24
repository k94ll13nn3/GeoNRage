using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Admin;

public partial class IndexAdmin
{
    [Inject]
    public IAdminApi AdminApi { get; set; } = null!;

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    public AdminInfoDto AdminInfo { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        AdminInfo = await AdminApi.GetAdminInfoAsync();
    }

    private async Task ClearLogsAsync()
    {
        await AdminApi.ClearLogsAsync();
        AdminInfo = await AdminApi.GetAdminInfoAsync();
        ToastService.DisplayToast("Logs vidés.", TimeSpan.FromSeconds(3), ToastType.Success, "clear-logs-admin", true);
        StateHasChanged();
    }
}
