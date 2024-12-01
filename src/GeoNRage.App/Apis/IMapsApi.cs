using Refit;

namespace GeoNRage.App.Apis;

public interface IMapsApi
{
    [Get("/api/maps")]
    Task<MapDto[]> GetAllAsync();

    [Put("/api/maps/{id}")]
    Task UpdateAsync(string id, [Body] MapEditDto dto);

    [Delete("/api/maps/{id}")]
    Task DeleteAsync(string id);
}
