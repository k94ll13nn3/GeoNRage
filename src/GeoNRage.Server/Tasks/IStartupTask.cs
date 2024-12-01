namespace GeoNRage.Server.Tasks;

internal interface IStartupTask
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
