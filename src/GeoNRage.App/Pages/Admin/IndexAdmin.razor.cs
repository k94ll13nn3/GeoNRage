using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Admin;

public partial class IndexAdmin
{
    private AdminInfoDto? _adminInfo;

    [Inject]
    public IAdminApi AdminApi { get; set; } = null!;

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    public IEnumerable<TableInfoDto> Tables { get; set; } = null!;

    public bool ShowWarning { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _adminInfo = await AdminApi.GetAdminInfoAsync();
        Tables = _adminInfo.Tables;
        StateHasChanged();
    }
}
