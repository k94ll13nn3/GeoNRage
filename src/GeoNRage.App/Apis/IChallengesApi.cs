using System.Threading.Tasks;
using GeoNRage.Shared.Dtos;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface IChallengesApi
    {
        [Get("/api/challenges")]
        Task<ChallengeDto[]> GetAllAsync();
    }
}
