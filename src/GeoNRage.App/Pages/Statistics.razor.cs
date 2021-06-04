using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos;
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

        internal IEnumerable<PlayerStatistic> Players { get; set; } = Enumerable.Empty<PlayerStatistic>();

        internal IEnumerable<LocationDto> Locations { get; set; } = Enumerable.Empty<LocationDto>();

        protected override async Task OnInitializedAsync()
        {
            Players = (await PlayersApi.GetAllFullAsync()).Select(CreateStatistic).ToList();
            Locations = await LocationsApi.GetAllAsync();
            Players = Sort(Players, nameof(PlayerStatistic.PlayerName), true);
            Locations = Sort(Locations, nameof(LocationDto.DisplayName), true);
        }

        private static IEnumerable<PlayerStatistic> Sort(IEnumerable<PlayerStatistic> players, string column, bool ascending)
        {
            return column switch
            {
                nameof(PlayerStatistic.PlayerName) => ascending ? players.OrderBy(p => p.PlayerName) : players.OrderByDescending(p => p.PlayerName),
                nameof(PlayerStatistic.NumberOf5000) => ascending ? players.OrderBy(p => p.NumberOf5000) : players.OrderByDescending(p => p.NumberOf5000),
                nameof(PlayerStatistic.NumberOf4999) => ascending ? players.OrderBy(p => p.NumberOf4999) : players.OrderByDescending(p => p.NumberOf4999),
                nameof(PlayerStatistic.ChallengesCompleted) => ascending ? players.OrderBy(p => p.ChallengesCompleted) : players.OrderByDescending(p => p.ChallengesCompleted),
                nameof(PlayerStatistic.BestGame) => ascending ? players.OrderBy(p => p.BestGame) : players.OrderByDescending(p => p.BestGame),
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
                _ => throw new ArgumentOutOfRangeException(nameof(column), "Invalid column name"),
            };
        }

        private static IEnumerable<LocationDto> Filter(IEnumerable<LocationDto> locations, string searchTerm)
        {
            return locations.Where(x =>
                (x.AdministrativeAreaLevel1 ?? "—").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || (x.AdministrativeAreaLevel2 ?? "—").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || (x.Country ?? "—").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || (x.Locality ?? "—").Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        private PlayerStatistic CreateStatistic(PlayerFullDto player)
        {
            List<PlayerScoreWithChallengeDto> results = player
                  .PlayerScores
                  .Where(p => (p.ChallengeTimeLimit ?? 300) == 300)
                  .GroupBy(p => p.GameId)
                  .Where(g => g.Count() == 3)
                  .OrderByDescending(g => g.OrderBy(c => c.ChallengeId).Select(p => p.Sum).Sum())
                  .First()
                  .OrderBy(c => c.ChallengeId)
                  .ToList();

            return new(
                player.Name,
                player.Id,
                player.PlayerScores.SelectMany(p => p.Rounds).Count(s => s == 5000),
                player.PlayerScores.SelectMany(p => p.Rounds).Count(s => s == 4999),
                player.PlayerScores.Count(p => p.Rounds.All(s => s is not null or 0)),
                results.Sum(p => p.Sum) ?? 0,
                results[0].GameId,
                (int)(player.PlayerScores.SelectMany(p => p.Rounds).Average() ?? 0));
        }

        internal record PlayerStatistic(string PlayerName, string PlayerId, int NumberOf5000, int NumberOf4999, int ChallengesCompleted, int BestGame, int BestGameId, int RoundAverage);
    }
}
