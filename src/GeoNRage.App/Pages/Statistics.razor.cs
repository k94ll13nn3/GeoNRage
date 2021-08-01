using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos.Locations;
using GeoNRage.Shared.Dtos.Players;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages
{
    public partial class Statistics
    {
        [Inject]
        public IPlayersApi PlayersApi { get; set; } = null!;

        [Inject]
        public ILocationsApi LocationsApi { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        internal IEnumerable<PlayerStatisticDto> Players { get; set; } = Enumerable.Empty<PlayerStatisticDto>();

        internal IEnumerable<LocationDto> Locations { get; set; } = Enumerable.Empty<LocationDto>();

        protected override async Task OnInitializedAsync()
        {
            Players = (await PlayersApi.GetAllStatisticsAsync());
            Locations = await LocationsApi.GetAllAsync();
            Players = Sort(Players, nameof(PlayerStatisticDto.Name), true);
            Locations = Sort(Locations, nameof(LocationDto.DisplayName), true);
        }

        private static IEnumerable<PlayerStatisticDto> Sort(IEnumerable<PlayerStatisticDto> players, string column, bool ascending)
        {
            return column switch
            {
                nameof(PlayerStatisticDto.Name) => ascending ? players.OrderBy(p => p.Name) : players.OrderByDescending(p => p.Name),
                nameof(PlayerStatisticDto.NumberOf5000) => ascending ? players.OrderBy(p => p.NumberOf5000) : players.OrderByDescending(p => p.NumberOf5000),
                nameof(PlayerStatisticDto.NumberOf4999) => ascending ? players.OrderBy(p => p.NumberOf4999) : players.OrderByDescending(p => p.NumberOf4999),
                nameof(PlayerStatisticDto.ChallengesCompleted) => ascending ? players.OrderBy(p => p.ChallengesCompleted) : players.OrderByDescending(p => p.ChallengesCompleted),
                nameof(PlayerStatisticDto.BestGame) => ascending ? players.OrderBy(p => p.BestGame) : players.OrderByDescending(p => p.BestGame),
                nameof(PlayerStatisticDto.RoundAverage) => ascending ? players.OrderBy(p => p.RoundAverage) : players.OrderByDescending(p => p.RoundAverage),
                _ => throw new ArgumentOutOfRangeException(nameof(column), "Invalid column name"),
            };
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
