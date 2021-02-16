using System.Threading.Tasks;
using GeoNRage.Shared.Dtos;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface IGamesApi
    {
        [Get("/api/games")]
        Task<GameLightDto[]> GetAllAsync();

        [Get("/api/games/{id}")]
        Task<ApiResponse<GameDto>> GetAsync(int id);
    }
}
