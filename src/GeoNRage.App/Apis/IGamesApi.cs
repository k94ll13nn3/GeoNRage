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
        Task<GameDto> CreateAsync([Body] GameCreateDto dto);

        [Post("/api/games/{id}")]
        Task<GameDto> UpdateAsync(int id, [Body] GameEditDto dto);

        [Post("/api/games/{id}/addPlayer/{playerId}")]
        Task AddPlayerAsync(int id, string playerId);

        [Delete("/api/games/{id}")]
        Task DeleteAsync(int id);
    }
}
