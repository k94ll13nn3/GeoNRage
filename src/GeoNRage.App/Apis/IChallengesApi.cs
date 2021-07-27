using System.Threading.Tasks;
using GeoNRage.Shared.Dtos;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface IChallengesApi
    {
        [Get("/api/challenges")]
        Task<ChallengeDto[]> GetAllAsync();

        [Get("/api/challenges/without-games")]
        Task<ChallengeDto[]> GetAllWithoutGameAsync();

        [Get("/api/challenges/{id}")]
        Task<ApiResponse<ChallengeDto>> GetAsync(int id);

        [Post("/api/challenges/import")]
        Task<int> ImportChallengeAsync([Body] ChallengeImportDto dto);

        [Delete("/api/challenges/{id}")]
        Task DeleteAsync(int id);
    }
}
