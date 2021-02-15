using System.Threading.Tasks;
using GeoNRage.Data.Dtos;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface IMapsApi
    {
        [Get("/api/maps")]
        Task<MapDto[]> GetAllAsync();

        [Post("/api/maps")]
        Task<MapDto> CreateAsync([Body] MapCreateOrEditDto map);

        [Post("/api/maps/{id}")]
        Task<MapDto> UpdateAsync(int id, [Body] MapCreateOrEditDto map);

        [Delete("/api/maps/{id}")]
        Task DeleteAsync(int id);
    }
}
