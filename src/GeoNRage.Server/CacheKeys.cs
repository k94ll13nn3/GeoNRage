using GeoNRage.Server.Services;

namespace GeoNRage.Server;

public static class CacheKeys
{
    public static string PlayerServiceGetAllAsync => $"{nameof(PlayerService)}.{nameof(PlayerService.GetAllAsync)}";
}
