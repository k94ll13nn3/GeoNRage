using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Server.Tasks;

[AutoConstructor]
public partial class DatabaseMigrationTask : IStartupTask
{
    private readonly IServiceProvider _serviceProvider;

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        GeoNRageDbContext context = scope.ServiceProvider.GetRequiredService<GeoNRageDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }
}
