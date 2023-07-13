using Refit;

namespace GeoNRage.App.Apis;

public interface ILogApi
{
    [Post("/api/logs")]
    Task<HttpResponseMessage> PostLogAsync([Body] ErrorLog error);
}
