using System.Threading.Tasks;
using GeoNRage.Data.Entities;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface IGamesApi
    {
        [Get("/api/games")]
        Task<Game[]> GetAllAsync();

        [Get("/api/games/{id}")]
        Task<Game> GetAsync(int id);
    }
}
