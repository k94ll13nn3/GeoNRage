using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.App.Core;
using GeoNRage.App.Services;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

namespace GeoNRage.App.Pages.Admin
{
    public partial class GamesAdmin
    {
        [Inject]
        public IMapsApi MapsApi { get; set; } = null!;

        [Inject]
        public IPlayersApi PlayersApi { get; set; } = null!;

        [Inject]
        public IGamesApi GamesApi { get; set; } = null!;

        [Inject]
        public PopupService PopupService { get; set; } = null!;

        public IEnumerable<MapDto> Maps { get; set; } = null!;

        public IEnumerable<PlayerDto> Players { get; set; } = null!;

        public IEnumerable<GameDto> Games { get; set; } = null!;

        public bool ShowEditForm { get; set; }

        public GameCreateOrEditDto Game { get; set; } = new();

        public int? SelectedGameId { get; set; }

        public string? Error { get; set; }

        public EditForm Form { get; set; } = null!;

        public void EditGame(int gameId)
        {
            ShowEditForm = true;
            GameDto gameToEdit = Games.First(m => m.Id == gameId);
            Game = new GameCreateOrEditDto
            {
                Name = gameToEdit.Name,
                Date = gameToEdit.Date,
                Challenges = gameToEdit.Challenges.Select(c => new ChallengeCreateOrEditDto { Id = c.Id, GeoGuessrId = c.GeoGuessrId, MapId = c.MapId }).ToList(),
                PlayerIds = gameToEdit.Players.Select(p => p.Id).ToList()
            };

            SelectedGameId = gameId;
        }

        public void DeleteGame(int gameId)
        {
            PopupService.DisplayOkCancelPopup("Suppression", $"Valider la suppression de la partie {gameId} ?", () => OnConfirmDeleteAsync(gameId), false);
        }

        public async Task CreateOrUpdateGameAsync()
        {
            Error = string.Empty;
            try
            {
                if (SelectedGameId is not null)
                {
                    await GamesApi.UpdateAsync(SelectedGameId.Value, Game);
                }
                else
                {
                    await GamesApi.CreateAsync(Game);
                }

                ShowEditForm = false;
                SelectedGameId = null;
                Games = await GamesApi.GetAllAsync();
                StateHasChanged();
            }
            catch (ValidationApiException e)
            {
                Error = string.Join(",", e.Content?.Errors.Select(x => string.Join(",", x.Value)) ?? Array.Empty<string>());
            }
            catch (ApiException e)
            {
                Error = e.Content;
            }
        }

        public void ShowGameCreation()
        {
            ShowEditForm = true;
            Game = new GameCreateOrEditDto { Date = DateTime.Now };
        }

        protected override async Task OnInitializedAsync()
        {
            Maps = await MapsApi.GetAllAsync();
            Players = await PlayersApi.GetAllAsync();
            Games = await GamesApi.GetAllAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            Form?.EditContext?.UpdateCssClassProvider();
        }

        private async void OnConfirmDeleteAsync(int gameId)
        {
            try
            {
                await GamesApi.DeleteAsync(gameId);
                Games = await GamesApi.GetAllAsync();
                StateHasChanged();
            }
            catch (ApiException e)
            {
                PopupService.DisplayPopup("Erreur", e.Content ?? string.Empty);
            }
        }
    }
}
