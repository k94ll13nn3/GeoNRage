namespace GeoNRage.Server.Tasks;

public interface IStartupTask
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
