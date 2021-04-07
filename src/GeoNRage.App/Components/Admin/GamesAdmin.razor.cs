using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GeoNRage.App.Components.Admin
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
        public IJSRuntime JSRuntime { get; set; } = null!;

        public IEnumerable<MapDto> Maps { get; set; } = null!;

        public IEnumerable<PlayerDto> Players { get; set; } = null!;

        public IEnumerable<GameDto> Games { get; set; } = null!;

        public bool ShowEditForm { get; set; }

        public GameCreateOrEditDto Game { get; set; } = null!;

        public int? SelectedGameId { get; set; }

        public void EditGame(int gameId)
        {
            ShowEditForm = true;
            GameDto gameToEdit = Games.First(m => m.Id == gameId);
            Game = new GameCreateOrEditDto
            {
                Name = gameToEdit.Name,
                Date = gameToEdit.Date,
                MapIds = gameToEdit.Maps.Select(m => m.Id).ToList(),
                PlayerIds = gameToEdit.Players.Select(p => p.Id).ToList(),
            };

            SelectedGameId = gameId;
        }

        public async Task DeleteGameAsync(int gameId)
        {
            if (!await JSRuntime.InvokeAsync<bool>("confirm", $"Valider la suppression de la partie {gameId} ?"))
            {
                return;
            }

            await GamesApi.DeleteAsync(gameId);
            Games = await GamesApi.GetAllAsync();
        }

        public async Task LockGameAsync(int gameId)
        {
            if (!await JSRuntime.InvokeAsync<bool>("confirm", $"Valider le verrouillage de la partie {gameId} ?"))
            {
                return;
            }

            await GamesApi.LockAsync(gameId);
            Games = await GamesApi.GetAllAsync();
        }

        public async Task UnlockGameAsync(int gameId)
        {
            if (!await JSRuntime.InvokeAsync<bool>("confirm", $"Valider le déverrouillage de la partie {gameId} ?"))
            {
                return;
            }

            await GamesApi.UnlockAsync(gameId);
            Games = await GamesApi.GetAllAsync();
        }

        public async Task ResetGameAsync(int gameId)
        {
            if (!await JSRuntime.InvokeAsync<bool>("confirm", $"Valider la réinitialisation de la partie {gameId} ?"))
            {
                return;
            }

            await GamesApi.ResetAsync(gameId);
            Games = await GamesApi.GetAllAsync();
        }

        public async Task CreateOrUpdateGameAsync()
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
    }
}
