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
        public NavigationManager NavigationManager { get; set; } = null!;

        internal IEnumerable<PlayerStatistic> Players { get; set; } = Enumerable.Empty<PlayerStatistic>();

        protected override async Task OnInitializedAsync()
        {
            Players = (await PlayersApi.GetAllFullAsync()).Select(CreateStatistic).ToList();
            Sort(true, nameof(PlayerStatistic.PlayerName));
        }

        private void Sort(bool ascending, string column)
        {
            switch (column)
            {
                case nameof(PlayerStatistic.PlayerName):
                    Players = ascending ? Players.OrderBy(p => p.PlayerName) : Players.OrderByDescending(p => p.PlayerName);
                    break;

                case nameof(PlayerStatistic.NumberOf5000):
                    Players = ascending ? Players.OrderBy(p => p.NumberOf5000) : Players.OrderByDescending(p => p.NumberOf5000);
                    break;

                case nameof(PlayerStatistic.NumberOf4999):
                    Players = ascending ? Players.OrderBy(p => p.NumberOf4999) : Players.OrderByDescending(p => p.NumberOf4999);
                    break;

                case nameof(PlayerStatistic.ChallengesCompleted):
                    Players = ascending ? Players.OrderBy(p => p.ChallengesCompleted) : Players.OrderByDescending(p => p.ChallengesCompleted);
                    break;

                case nameof(PlayerStatistic.BestGame):
                    Players = ascending ? Players.OrderBy(p => p.BestGame) : Players.OrderByDescending(p => p.BestGame);
                    break;
            }

            StateHasChanged();
        }

        private PlayerStatistic CreateStatistic(PlayerFullDto player)
        {
            List<PlayerScoreWithChallengeDto> results = player
                  .PlayerScores
                  .GroupBy(p => p.Challenge.GameId)
                  .OrderByDescending(g => g.OrderBy(c => c.Challenge.Id).Take(3).Select(p => p.Sum).Sum())
                  .First()
                  .OrderBy(c => c.Challenge.Id)
                  .Take(3)
                  .ToList();

            return new(
                player.Name,
                player.Id,
                player.PlayerScores.SelectMany(p => p.Rounds).Count(s => s == 5000),
                player.PlayerScores.SelectMany(p => p.Rounds).Count(s => s == 4999),
                player.PlayerScores.Count(p => p.Rounds.All(s => s is not null or 0)),
                results.Sum(p => p.Sum) ?? 0,
                results[0].Challenge.GameId);
        }

        internal record PlayerStatistic(string PlayerName, string PlayerId, int NumberOf5000, int NumberOf4999, int ChallengesCompleted, int BestGame, int BestGameId);
    }
}
