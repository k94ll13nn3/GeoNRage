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
        public PopupService PopupService { get; set; } = null!;

        public IEnumerable<ChallengeDto> Challenges { get; set; } = null!;

        public IEnumerable<MapDto> Maps { get; set; } = null!;

        public string GeoGuessrId { get; set; } = null!;

        public Table<ChallengeDto> ChallengesTable { get; set; } = null!;

        public string? Error { get; set; }

        public bool ShowAllMaps { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Maps = await MapsApi.GetAllAsync();
            var mapsForGame = Maps.Where(m => m.IsMapForGame).Select(m => m.Id).ToList();
            Challenges = (await ChallengesApi.GetAllWithoutGameAsync()).Where(c => mapsForGame.Contains(c.MapId));
        }

        private static IEnumerable<ChallengeDto> Sort(IEnumerable<ChallengeDto> challenges, string column, bool ascending)
        {
            return column switch
            {
                nameof(ChallengeDto.MapName) => ascending ? challenges.OrderBy(c => c.MapName) : challenges.OrderByDescending(c => c.MapName),
                nameof(PlayerScoreDto.Sum) => ascending ? challenges.OrderBy(c => c.PlayerScores.Max(p => p.Sum)) : challenges.OrderByDescending(c => c.PlayerScores.Max(p => p.Sum)),
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
            var mapsForGame = Maps.Where(m => ShowAllMaps || m.IsMapForGame).Select(m => m.Id).ToList();
            Challenges = (await ChallengesApi.GetAllWithoutGameAsync()).Where(c => mapsForGame.Contains(c.MapId));
            ChallengesTable.SetItems(Challenges);
            StateHasChanged();
        }

        private async Task ImportAsync()
        {
            try
            {
                Error = null;
                await ChallengesApi.ImportChallengeAsync(new() { GeoGuessrId = GeoGuessrId, OverrideData = false });
                Challenges = await ChallengesApi.GetAllWithoutGameAsync();
                ChallengesTable.SetItems(Challenges);
                GeoGuessrId = string.Empty;
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
