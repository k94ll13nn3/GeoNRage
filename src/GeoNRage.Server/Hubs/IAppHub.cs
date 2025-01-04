namespace GeoNRage.Server.Hubs;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Needs to be public for SignalR")]
public interface IAppHub
{
    Task NewPlayerAdded();

    Task Taunted(string imageId, string? name);

    Task ReceiveValue(int challengeId, string playerId, int round, int score);
}
