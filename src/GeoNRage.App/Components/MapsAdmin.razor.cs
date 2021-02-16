using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GeoNRage.App.Components
{
    public partial class MapsAdmin
    {
        [Inject]
        public IMapsApi MapsApi { get; set; } = null!;

        [Inject]
        public IJSRuntime JSRuntime { get; set; } = null!;

        public IEnumerable<MapDto> Maps { get; set; } = null!;

        public bool ShowEditForm { get; set; }

        public MapCreateOrEditDto Map { get; set; } = null!;

        public int? SelectedMapId { get; set; }

        public void EditMap(int mapId)
        {
            ShowEditForm = true;
            Map = new MapCreateOrEditDto { Name = Maps.First(m => m.Id == mapId).Name };
            SelectedMapId = mapId;
        }

        public async Task DeleteMapAsync(int mapId)
        {
            if (!await JSRuntime.InvokeAsync<bool>("confirm", $"Valider la suppression de la carte {mapId} ?"))
            {
                return;
            }

            await MapsApi.DeleteAsync(mapId);
            Maps = await MapsApi.GetAllAsync();
        }

        public async Task CreateOrUpdateMapAsync()
        {
            if (SelectedMapId is not null)
            {
                await MapsApi.UpdateAsync(SelectedMapId.Value, Map);
            }
            else
            {
                await MapsApi.CreateAsync(Map);
            }

            ShowEditForm = false;
            SelectedMapId = null;
            Maps = await MapsApi.GetAllAsync();
        }

        public void ShowMapCreation()
        {
            ShowEditForm = true;
            Map = new MapCreateOrEditDto();
        }

        protected override async Task OnInitializedAsync()
        {
            Maps = await MapsApi.GetAllAsync();
        }
    }
}
