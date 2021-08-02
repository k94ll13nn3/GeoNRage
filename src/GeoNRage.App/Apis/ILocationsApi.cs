using System.Threading.Tasks;
using GeoNRage.App.Core;
using GeoNRage.Shared.Dtos.Locations;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface ILocationsApi
    {
        [Get("/api/locations")]
        [Headers($"{MapStatusHandler.HeaderName}:")]
        Task<LocationDto[]> GetAllAsync();
    }
}
