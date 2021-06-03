using System.Threading.Tasks;
using GeoNRage.Shared.Dtos;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface ILocationsApi
    {
        [Get("/api/locations")]
        Task<LocationDto[]> GetAllAsync();
    }
}
