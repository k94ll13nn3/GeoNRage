using GeoNRage.Server.Services;

namespace GeoNRage.Server;

public static class CacheKeys
{
    public static string PlayerServiceGetAllAsync => $"{nameof(PlayerService)}.{nameof(PlayerService.GetAllAsync)}";

    public static string PlayerServiceGetGamesWithPlayersScoreAsync => $"{nameof(PlayerService)}.{nameof(PlayerService.GetGamesWithPlayersScoreAsync)}";

    public static string MapServiceGetAllAsync => $"{nameof(MapService)}.{nameof(MapService.GetAllAsync)}";
}
