using Refit;

namespace GeoNRage.App.Apis;

public interface IChallengesApi
{
    [Get("/api/challenges")]
    [Headers($"{Constants.MapStatusHeaderName}:")]
    Task<ChallengeDto[]> GetAllAsync(bool onlyWithoutGame = false, [Query(CollectionFormat.Multi)] string[]? playersToExclude = null);

    [Get("/api/challenges/admin-view")]
    Task<ChallengeAdminViewDto[]> GetAllAsAdminViewAsync();

    [Get("/api/challenges/{id}")]
    Task<ApiResponse<ChallengeDetailDto>> GetAsync(int id);

    [Post("/api/challenges/import")]
    Task<int> ImportChallengeAsync([Body] ChallengeImportDto dto);

    [Delete("/api/challenges/{id}")]
    Task DeleteAsync(int id);
}
