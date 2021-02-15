using System.Threading.Tasks;
using GeoNRage.Data.Entities;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface IPlayersApi
    {
        [Get("/api/players")]
        Task<Player[]> GetAllAsync();
    }
}
