using GeoNRage.Server.Tasks;

namespace GeoNRage.Server;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStartupTask<T>(this IServiceCollection services)
        where T : class, IStartupTask
    {
        return services.AddTransient<IStartupTask, T>();
    }
}
