using Refit;

namespace GeoNRage.App.Apis;

public interface ILocationsApi
{
    [Get("/api/locations")]
    [Headers($"{Constants.MapStatusHeaderName}:")]
    Task<LocationDto[]> GetAllAsync();
}
