using System.Threading.Tasks;
using GeoNRage.Shared.Dtos;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface IMapsApi
    {
        [Get("/api/maps")]
        Task<MapDto[]> GetAllAsync();

        [Post("/api/maps")]
        Task<MapDto> CreateAsync([Body] MapCreateDto dto);

        [Post("/api/maps/{id}")]
        Task<MapDto> UpdateAsync(string id, [Body] MapEditDto dto);

        [Delete("/api/maps/{id}")]
        Task DeleteAsync(string id);
    }
}
