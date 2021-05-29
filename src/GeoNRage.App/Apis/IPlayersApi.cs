using System.Threading.Tasks;
using GeoNRage.Shared.Dtos;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface IPlayersApi
    {
        [Get("/api/players")]
        Task<PlayerDto[]> GetAllAsync();

        [Get("/api/players/full")]
        Task<PlayerFullDto[]> GetAllFullAsync();

        [Get("/api/players/{id}/full")]
        Task<ApiResponse<PlayerFullDto>> GetFullAsync(string id);

        [Post("/api/players")]
        Task<PlayerDto> CreateAsync([Body] PlayerCreateDto dto);

        [Post("/api/players/{id}")]
        Task<PlayerDto> UpdateAsync(string id, [Body] PlayerEditDto dto);

        [Delete("/api/players/{id}")]
        Task DeleteAsync(string id);
    }
}
