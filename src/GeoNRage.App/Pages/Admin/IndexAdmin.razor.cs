using GeoNRage.App.Apis;
using GeoNRage.App.Components;
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

    public IEnumerable<LogEntryDto> Logs { get; set; } = null!;

    public Table<LogEntryDto> LogsTable { get; set; } = null!;

    public bool ShowWarning { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _adminInfo = await AdminApi.GetAdminInfoAsync();
        Logs = _adminInfo.Logs.Where(l => l.Level == "Error").OrderByDescending(l => l.Timestamp).ToList();
        Tables = _adminInfo.Tables;
        StateHasChanged();
    }

    private async Task ClearLogsAsync()
    {
        await AdminApi.ClearLogsAsync();
        await OnInitializedAsync();
        ToastService.DisplayToast("Logs vidés.", TimeSpan.FromSeconds(3), ToastType.Success, "clear-logs-admin", true);
        LogsTable.SetItems(Logs);
        ShowWarning = false;
    }

    private void ToggleLogView()
    {
        if (_adminInfo is not null)
        {
            ShowWarning = !ShowWarning;
            Logs = (ShowWarning ? _adminInfo.Logs : _adminInfo.Logs.Where(l => l.Level == "Error")).OrderByDescending(l => l.Timestamp).ToList();
            LogsTable.SetItems(Logs);
        }
    }
}
