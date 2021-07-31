using System.Threading.Tasks;
using GeoNRage.Shared.Dtos.Admin;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface IAdminApi
    {
        [Get("/api/admin/info")]
        Task<AdminInfoDto> GetAdminInfoAsync();
    }
}
