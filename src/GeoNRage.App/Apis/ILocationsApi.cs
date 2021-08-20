using Refit;

namespace GeoNRage.App.Apis;

public interface ILocationsApi
{
    [Get("/api/locations")]
    [Headers($"{MapStatusHandler.HeaderName}:")]
    Task<LocationDto[]> GetAllAsync();
}
