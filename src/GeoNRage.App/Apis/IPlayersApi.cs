using System.Threading.Tasks;
using GeoNRage.Shared.Dtos.Players;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface IPlayersApi
    {
        [Get("/api/players")]
        Task<PlayerDto[]> GetAllAsync();

        [Get("/api/players/statistics")]
        Task<PlayerStatisticDto[]> GetAllStatisticsAsync();

        [Get("/api/players/{id}/full")]
        Task<ApiResponse<PlayerFullDto>> GetFullAsync(string id);

        [Put("/api/players/{id}")]
        Task UpdateAsync(string id, [Body] PlayerEditDto dto);

        [Delete("/api/players/{id}")]
        Task DeleteAsync(string id);
    }
}
