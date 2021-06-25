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
    public partial class MapsAdmin
    {
        [Inject]
        public IMapsApi MapsApi { get; set; } = null!;

        [Inject]
        public PopupService PopupService { get; set; } = null!;

        public IEnumerable<MapDto> Maps { get; set; } = null!;

        public bool ShowEditForm { get; set; }

        public MapCreateDto Map { get; set; } = new();

        public string? SelectedMapId { get; set; }

        public EditForm Form { get; set; } = null!;

        public string? Error { get; set; }

        public void EditMap(string mapId)
        {
            Error = null;
            ShowEditForm = true;
            Map = new MapCreateDto { Name = Maps.First(m => m.Id == mapId).Name, Id = mapId, IsMapForGame = Maps.First(m => m.Id == mapId).IsMapForGame };
            SelectedMapId = mapId;
        }

        public void DeleteMap(string mapId)
        {
            PopupService.DisplayOkCancelPopup("Suppression", $"Valider la suppression de la carte {mapId} ?", () => OnConfirmDeleteAsync(mapId), false);
        }

        public async Task CreateOrUpdateMapAsync()
        {
            try
            {
                Error = null;
                if (SelectedMapId is not null)
                {
                    await MapsApi.UpdateAsync(SelectedMapId, Map);
                }
                else
                {
                    await MapsApi.CreateAsync(Map);
                }

                ShowEditForm = false;
                SelectedMapId = null;
                Maps = await MapsApi.GetAllAsync();
                StateHasChanged();
            }
            catch (ApiException e)
            {
                Error = e.Content;
            }
        }

        public void ShowMapCreation()
        {
            Error = null;
            ShowEditForm = true;
            Map = new MapCreateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            Maps = await MapsApi.GetAllAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            Form?.EditContext?.UpdateCssClassProvider();
        }

        private async void OnConfirmDeleteAsync(string mapId)
        {
            try
            {
                await MapsApi.DeleteAsync(mapId);
                Maps = await MapsApi.GetAllAsync();
                StateHasChanged();
            }
            catch (ApiException e)
            {
                PopupService.DisplayPopup("Erreur", e.Content ?? string.Empty);
            }
        }
    }
}
