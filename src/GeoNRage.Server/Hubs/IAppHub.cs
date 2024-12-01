namespace GeoNRage.Server.Hubs;

internal interface IAppHub
{
    Task NewPlayerAdded();

    Task Taunted(string imageId, string? name);

    Task ReceiveValue(int challengeId, string playerId, int round, int score);
}
