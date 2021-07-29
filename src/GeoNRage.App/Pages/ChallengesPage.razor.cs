using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.App.Components.Shared;
using GeoNRage.App.Services;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;
using Refit;

namespace GeoNRage.App.Pages
{
    public partial class ChallengesPage
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public IChallengesApi ChallengesApi { get; set; } = null!;

        [Inject]
        public IMapsApi MapsApi { get; set; } = null!;

        [Inject]
        public IPlayersApi PlayersApi { get; set; } = null!;

        [Inject]
        public PopupService PopupService { get; set; } = null!;

        public IEnumerable<ChallengeDto> Challenges { get; set; } = null!;

        public IEnumerable<PlayerDto> Players { get; set; } = null!;

        public string GeoGuessrId { get; set; } = null!;

        public Table<ChallengeDto> ChallengesTable { get; set; } = null!;

        public string? Error { get; set; }

        public bool ShowAllMaps { get; set; }

        public bool ChallengeImported { get; set; }

        public ICollection<string> PlayersToHide { get; } = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            await FilterChallengesAsync();
            Players = await PlayersApi.GetAllAsync();
        }

        private static IEnumerable<ChallengeDto> Sort(IEnumerable<ChallengeDto> challenges, string column, bool ascending)
        {
            return column switch
            {
                nameof(ChallengeDto.MapName) => ascending ? challenges.OrderBy(c => c.MapName) : challenges.OrderByDescending(c => c.MapName),
                nameof(PlayerScoreWithGuessDto.Sum) => ascending ? challenges.OrderBy(c => c.PlayerScores.Max(p => p.Sum)) : challenges.OrderByDescending(c => c.PlayerScores.Max(p => p.Sum)),
                _ => throw new ArgumentOutOfRangeException(nameof(column), "Invalid column name"),
            };
        }

        private void Import()
        {
            PopupService.DisplayOkCancelPopup("Importation", "Valider l'importation du challenge ?", async () => await ImportAsync(), true);
        }

        private async Task ShowAllMapToggleAsync()
        {
            ShowAllMaps = !ShowAllMaps;
            await FilterChallengesAsync();
        }

        private async Task FilterChallengesAsync()
        {
            var mapsForGame = (await MapsApi.GetAllAsync()).Where(m => ShowAllMaps || m.IsMapForGame).Select(m => m.Id).ToList();
            Challenges = (await ChallengesApi.GetAllWithoutGameAsync()).Where(c => mapsForGame.Contains(c.MapId) && c.PlayerScores.All(p => !PlayersToHide.Contains(p.PlayerId)));
            ChallengesTable?.SetItems(Challenges);
        }

        private async Task ImportAsync()
        {
            try
            {
                Error = null;
                ChallengeImported = false;
                await ChallengesApi.ImportChallengeAsync(new() { GeoGuessrId = GeoGuessrId, OverrideData = false });
                await FilterChallengesAsync();
                GeoGuessrId = string.Empty;
                ChallengeImported = true;
            }
            catch (ValidationApiException e)
            {
                Error = string.Join(",", e.Content?.Errors.Select(x => string.Join(",", x.Value)) ?? Array.Empty<string>());
            }
            catch (ApiException e)
            {
                Error = e.Content;
            }
            finally
            {
                PopupService.HidePopup();
                StateHasChanged();
            }
        }
    }
}
