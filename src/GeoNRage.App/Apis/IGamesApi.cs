using System.Threading.Tasks;
using GeoNRage.Shared.Dtos;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface IGamesApi
    {
        [Get("/api/games")]
        Task<GameDto[]> GetAllAsync();

        [Get("/api/games/light")]
        Task<GameLightDto[]> GetAllLightAsync();

        [Get("/api/games/{id}")]
        Task<ApiResponse<GameDto>> GetAsync(int id);

        [Post("/api/games")]
        Task<GameDto> CreateAsync([Body] GameCreateOrEditDto map);

        [Post("/api/games/{id}")]
        Task<GameDto> UpdateAsync(int id, [Body] GameCreateOrEditDto map);

        [Post("/api/games/{id}/lock")]
        Task LockAsync(int id);

        [Post("/api/games/{id}/reset")]
        Task ResetAsync(int id);

        [Delete("/api/games/{id}")]
        Task DeleteAsync(int id);
    }
}
