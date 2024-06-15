using Refit;

namespace GeoNRage.App.Apis;

public interface IPlayersApi
{
    [Get("/api/players")]
    Task<PlayerDto[]> GetAllAsync();

    [Get("/api/players/admin-view")]
    Task<PlayerAdminViewDto[]> GetAllAsAdminViewAsync();

    [Get("/api/players/statistics")]
    [Headers($"{Constants.MapStatusHeaderName}:")]
    Task<PlayerStatisticDto[]> GetAllStatisticsAsync();

    [Get("/api/players/{id}/full")]
    [Headers($"{Constants.MapStatusHeaderName}:")]
    Task<ApiResponse<PlayerFullDto>> GetFullAsync(string id);

    [Get("/api/players/{id}/resume")]
    [Headers($"{Constants.MapStatusHeaderName}:")]
    Task<ApiResponse<PlayerResumeDto>> GetResumeAsync(string id);

    [Put("/api/players/{id}")]
    Task UpdateAsync(string id, [Body] PlayerEditDto dto);

    [Post("/api/players/{id}/update-geoguessr-profile")]
    Task UpdateGeoGuessrProfileAsync(string id);

    [Delete("/api/players/{id}")]
    Task DeleteAsync(string id);
}
