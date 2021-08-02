using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.App.Components.Shared;
using GeoNRage.App.Services;
using GeoNRage.Shared.Dtos.Locations;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages
{
    public partial class LocationsStatistics : IDisposable
    {
        private bool _disposedValue;

        [Inject]
        public ILocationsApi LocationsApi { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public MapStatusService MapStatusService { get; set; } = null!;

        public Table<LocationDto> LocationsTable { get; set; } = null!;

        internal IEnumerable<LocationDto> Locations { get; set; } = Enumerable.Empty<LocationDto>();

        protected override async Task OnInitializedAsync()
        {
            Locations = await LocationsApi.GetAllAsync();
            MapStatusService.MapStatusChanged += OnMapStatusChanged;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    MapStatusService.MapStatusChanged -= OnMapStatusChanged;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private async void OnMapStatusChanged(object? sender, EventArgs e)
        {
            Locations = Enumerable.Empty<LocationDto>();
            StateHasChanged();
            Locations = await LocationsApi.GetAllAsync();
            LocationsTable?.SetItems(Locations);
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
                RemoveDiacritics(x.AdministrativeAreaLevel1 ?? "—").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || RemoveDiacritics(x.AdministrativeAreaLevel2 ?? "—").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || RemoveDiacritics(x.Country ?? "—").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || RemoveDiacritics(x.Locality ?? "—").Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        private static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
