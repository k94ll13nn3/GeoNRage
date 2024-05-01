using System.Globalization;
using Microsoft.AspNetCore.Components;
using Plotly.Blazor;
using Plotly.Blazor.LayoutLib;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.TreeMapLib;

namespace GeoNRage.App.Pages.Statistics;

public partial class MostVisitedLocationsChart : IModal
{
    private readonly IList<ITrace> _data = [];
    private readonly Config _config = new PlotlyConfig().Config;
    private readonly Layout _layout = new PlotlyConfig().Layout;

    [Parameter]
    public IEnumerable<LocationDto> Locations { get; set; } = [];

    public string Id => nameof(MostVisitedLocationsChart);

    protected override void OnInitialized()
    {
        _layout.Height = 800;
        _layout.Margin = new Margin { B = 0, T = 20, L = 0, R = 0 };

        ComputeChartData();
    }

    private void ComputeChartData()
    {
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

        _data.Clear();
        _data.Add(new TreeMap
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
