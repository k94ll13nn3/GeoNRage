using System.Threading.Tasks;
using GeoNRage.Shared.Dtos;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface IPlayersApi
    {
        [Get("/api/players")]
        Task<PlayerDto[]> GetAllAsync();

        [Post("/api/players")]
        Task<PlayerDto> CreateAsync([Body] PlayerCreateOrEditDto map);

        [Post("/api/players/{id}")]
        Task<PlayerDto> UpdateAsync(int id, [Body] PlayerCreateOrEditDto map);

        [Delete("/api/players/{id}")]
        Task DeleteAsync(int id);
    }
}
