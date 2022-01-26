using System.Globalization;
using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;
using Plotly.Blazor;
using Plotly.Blazor.LayoutLib;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.TreeMapLib;

namespace GeoNRage.App.Pages.Statistics;

public partial class LocationsStatisticsPage
{
    [Inject]
    public ILocationsApi LocationsApi { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public PopupService PopupService { get; set; } = null!;

    public bool ShowCountryChart { get; set; }

    internal IEnumerable<LocationDto> Locations { get; set; } = Enumerable.Empty<LocationDto>();

    public PlotlyChart? Chart { get; set; }

    public Config Config { get; set; } = new();

    public Layout Layout { get; set; } = new();

    public IList<ITrace> Data { get; } = new List<ITrace>();

    protected override async Task OnInitializedAsync()
    {
        Locations = await LocationsApi.GetAllAsync();
        GenerateCountryChart();
        StateHasChanged();
    }

    internal override async void OnSettingsChanged(object? sender, UserSettingsEventArgs e)
    {
        if (e.ChangedKey != nameof(UserSettings.AllMaps))
        {
            return;
        }

        Locations = Enumerable.Empty<LocationDto>();
        ShowCountryChart = false;
        Chart = null;
        StateHasChanged();
        Locations = await LocationsApi.GetAllAsync();
        GenerateCountryChart();
        StateHasChanged();
    }

    private async Task ShowCountryChartAsync(bool show)
    {
        ShowCountryChart = show;
        if (ShowCountryChart)
        {
            ShowCountryChart = true;
            if (Chart is not null)
            {
                await Chart.React();
            }
        }
        else
        {
            Chart = null;
        }
    }

    private static IEnumerable<LocationDto> Sort(IEnumerable<LocationDto> locations, string column, bool ascending)
    {
        return column switch
        {
            nameof(LocationDto.AdministrativeAreaLevel1) => ascending ? locations.OrderBy(p => p.AdministrativeAreaLevel1) : locations.OrderByDescending(p => p.AdministrativeAreaLevel1),
            nameof(LocationDto.AdministrativeAreaLevel2) => ascending ? locations.OrderBy(p => p.AdministrativeAreaLevel2) : locations.OrderByDescending(p => p.AdministrativeAreaLevel2),
            nameof(LocationDto.Country) => ascending ? locations.OrderBy(p => p.Country) : locations.OrderByDescending(p => p.Country),
            nameof(LocationDto.DisplayName) => ascending ? locations.OrderBy(p => p.DisplayName) : locations.OrderByDescending(p => p.DisplayName),
            nameof(LocationDto.Locality) => ascending ? locations.OrderBy(p => p.Locality) : locations.OrderByDescending(p => p.Locality),
            nameof(LocationDto.TimesSeen) => ascending ? locations.OrderBy(p => p.TimesSeen) : locations.OrderByDescending(p => p.TimesSeen),
            _ => throw new ArgumentOutOfRangeException(nameof(column), "Invalid column name"),
        };
    }

    private static IEnumerable<LocationDto> Filter(IEnumerable<LocationDto> locations, string searchTerm)
    {
        return locations.Where(x =>
            x.AdministrativeAreaLevel1.FormatNullWithDash().RemoveDiacritics().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || x.AdministrativeAreaLevel2.FormatNullWithDash().RemoveDiacritics().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || x.Country.FormatNullWithDash().RemoveDiacritics().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || x.Locality.FormatNullWithDash().RemoveDiacritics().Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }

    private void GenerateCountryChart()
    {
        Config = new PlotlyConfig().Config;

        Layout = new PlotlyConfig().Layout;
        Layout.Height = 800;
        Layout.Margin = new Margin { B = 0, T = 20, L = 0, R = 0 };

        var labels = new List<object> { "Tout" };
        var values = new List<object> { "" };
        var parents = new List<object> { "" };
        int sum = 0;
        foreach (IGrouping<string, LocationDto> country in Locations.GroupBy(l => l.Country ?? "-").OrderByDescending(g => g.Count()).Take(15))
        {
            labels.Add(country.Key);
            values.Add(country.Count().ToString(CultureInfo.InvariantCulture));
            parents.Add("Tout");
            sum += country.Count();
            foreach (IGrouping<string, LocationDto> level1 in country.GroupBy(l => l.AdministrativeAreaLevel1 ?? $"- ({country.Key})"))
            {
                labels.Add(level1.Key);
                values.Add(level1.Count().ToString(CultureInfo.InvariantCulture));
                parents.Add(country.Key);
                foreach (IGrouping<string, LocationDto> level2 in level1.GroupBy(l => l.AdministrativeAreaLevel2 ?? $"- ({level1.Key})"))
                {
                    labels.Add($"{level2.Key} ({level1.Key})");
                    values.Add(level2.Count().ToString(CultureInfo.InvariantCulture));
                    parents.Add(level1.Key);
                }
            }
        }

        values[0] = sum.ToString(CultureInfo.InvariantCulture); // Set total count to visited countries

        Data.Clear();
        Data.Add(new TreeMap
        {
            Name = "TreeMap",
            Labels = labels,
            Parents = parents,
            TextInfo = TextInfoFlag.Label | TextInfoFlag.Value,
            Values = values,
            BranchValues = BranchValuesEnum.Total
        });
    }
}
