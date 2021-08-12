using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Server.Tasks;

[AutoConstructor]
public partial class DatabaseMigrationTask : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        GeoNRageDbContext context = scope.ServiceProvider.GetRequiredService<GeoNRageDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
