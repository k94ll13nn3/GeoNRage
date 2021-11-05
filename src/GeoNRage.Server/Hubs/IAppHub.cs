namespace GeoNRage.Server.Hubs;

public interface IAppHub
{
    Task NewPlayerAdded();

    Task Taunted(string imageId, string? name);

    Task ReceiveValue(int challengeId, string playerId, int round, int score);
}
