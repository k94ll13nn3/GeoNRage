using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.App.Components.Shared;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages
{
    public partial class ChallengesPage
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public IChallengesApi ChallengesApi { get; set; } = null!;

        public IEnumerable<ChallengeDto> Challenges { get; set; } = null!;

        public string GeoGuessrId { get; set; } = null!;

        public Table<ChallengeDto> ChallengesTable { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            Challenges = await ChallengesApi.GetAllWithoutGameAsync();
        }

        private static IEnumerable<ChallengeDto> Sort(IEnumerable<ChallengeDto> challenges, string column, bool ascending)
        {
            return column switch
            {
                nameof(PlayerScoreDto.Sum) => ascending ? challenges.OrderBy(c => c.PlayerScores.Max(p => p.Sum)) : challenges.OrderByDescending(c => c.PlayerScores.Max(p => p.Sum)),
                _ => throw new ArgumentOutOfRangeException(nameof(column), "Invalid column name"),
            };
        }

        private async Task ImportAsync()
        {
            await ChallengesApi.ImportChallengeAsync(new() { GeoGuessrId = GeoGuessrId });
            Challenges = await ChallengesApi.GetAllWithoutGameAsync();
            ChallengesTable.SetItems(Challenges);
            StateHasChanged();
            GeoGuessrId = string.Empty;
        }
    }
}
