using Refit;

namespace GeoNRage.App.Apis;

public interface IGamesApi
{
    [Get("/api/games/admin-view")]
    Task<GameAdminViewDto[]> GetAllAsAdminViewAsync();

    [Get("/api/games")]
    Task<GameDto[]> GetAllAsync();

    [Get("/api/games/{id}")]
    Task<ApiResponse<GameDetailDto>> GetAsync(int id);

    [Post("/api/games")]
    Task CreateAsync([Body] GameCreateOrEditDto dto);

    [Put("/api/games/{id}")]
    Task UpdateAsync(int id, [Body] GameCreateOrEditDto dto);

    [Post("/api/games/{id}/add-player")]
    Task AddPlayerAsync(int id, [Body(BodySerializationMethod.Serialized)] string playerId);

    [Post("/api/games/{id}/update-challenges")]
    Task UpdateChallengesAsync(int id);

    [Delete("/api/games/{id}")]
    Task DeleteAsync(int id);
}
