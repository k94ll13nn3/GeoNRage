using System.Timers;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components;

public sealed partial class PaginatedTable<T> : IDisposable
{
    private IReadOnlyCollection<T> _items = null!;
    private int _itemCount;
    private string _filter = string.Empty;
    private string _sortColumn = string.Empty;
    private bool _sortAscending;
    private System.Timers.Timer _searchTimer = default!;
    private int _pageCount;
    private int _currentPage = 1;

    [Parameter]
    [EditorRequired]
    public RenderFragment<T> RowContent { get; set; } = null!;

    [Parameter]
    public RenderFragment? FooterContent { get; set; }

    [Parameter]
    [EditorRequired]
    public IEnumerable<TableHeader> Headers { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public Func<IPaginationQuery, Task<PaginationResult<T>>> DataFunction { get; set; } = null!;

    [Parameter]
    public int PageSize { get; set; } = 10;

    [Parameter]
    public int DebounceTimer { get; set; } = 250;

    [Parameter]
    public IReadOnlyCollection<string> SearchFields { get; set; } = [];

    [Parameter]
    public bool ShowRowCount { get; set; }

    [Parameter]
    public bool IsNarrow { get; set; } = true;

    [Parameter]
    public bool IsFullWidth { get; set; } = true;

    [Parameter]
    public bool SortAscending { get; set; } = true;

    public void Dispose()
    {
        _searchTimer.Dispose();
    }

    public async Task ReloadDataAsync()
    {
        await RefreshItemsAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        if (Headers.FirstOrDefault(h => h.DefaultSort) is TableHeader tableHeader)
        {
            _sortColumn = tableHeader.Property!;
        }

        _sortAscending = SortAscending;

        _searchTimer = new System.Timers.Timer(DebounceTimer);
        _searchTimer.Elapsed += OnSearchTimerElapsedAsync;
        _searchTimer.AutoReset = false;

        await RefreshItemsAsync();
    }

    private async Task ChangePageAsync(int newPage)
    {
        if (newPage >= 1 && newPage <= _pageCount)
        {
            _currentPage = newPage;

            await RefreshItemsAsync();
        }
    }

    private async Task SortAsync(string column, bool ascending)
    {
        _sortColumn = column;
        _sortAscending = ascending;
        await RefreshItemsAsync();
    }

    private bool IsColumnSorted(string column, bool ascending)
    {
        return column == _sortColumn && ascending == _sortAscending;
    }

    private void OnFilter(ChangeEventArgs args)
    {
        _filter = args?.Value as string ?? string.Empty;
        _searchTimer.Stop();
        _searchTimer.Start();
    }

    private async void OnSearchTimerElapsedAsync(object? sender, ElapsedEventArgs e)
    {
        await RefreshItemsAsync();
    }

    private async Task RefreshItemsAsync()
    {
        var paginationQuery = new PaginationQuery { PageSize = PageSize, Page = _currentPage };
        if (!string.IsNullOrWhiteSpace(_sortColumn))
        {
            paginationQuery.OrderBy = $"{_sortColumn} {(_sortAscending ? "asc" : "desc")}";
        }

        if (!string.IsNullOrWhiteSpace(_filter) && SearchFields.Count > 0)
        {
            paginationQuery.Filter = string.Join('|', SearchFields.Select(f => $"{f}=*{_filter}"));
        }

        PaginationResult<T> items = await DataFunction(paginationQuery);
        _items = items.Data;
        _itemCount = items.Count;
        _pageCount = (int)Math.Ceiling(1.0 * _itemCount / PageSize);

        StateHasChanged();
    }
}
