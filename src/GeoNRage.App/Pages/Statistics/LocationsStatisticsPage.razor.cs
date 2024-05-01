using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Statistics;

public partial class LocationsStatisticsPage
{
    [Inject]
    public ILocationsApi LocationsApi { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ModalService ModalService { get; set; } = null!;

    internal IEnumerable<LocationDto> Locations { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        Locations = await LocationsApi.GetAllAsync();
        StateHasChanged();
    }

    internal override async void OnSettingsChanged(object? sender, UserSettingsEventArgs e)
    {
        if (e.ChangedKey != nameof(UserSettings.AllMaps))
        {
            return;
        }

        Locations = [];
        StateHasChanged();
        Locations = await LocationsApi.GetAllAsync();
        StateHasChanged();
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

    private async Task ShowCountryChartAsync()
    {
        await ModalService.DisplayModalAsync<MostVisitedLocationsChart>(new()
        {
            [nameof(MostVisitedLocationsChart.Locations)] = Locations,
        },
        ModalOptions.Default with { ShowCloseButton = true, Size = ModalSize.ExtraLarge });
    }
}
