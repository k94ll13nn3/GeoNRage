namespace GeoNRage.App.Core;

[AutoConstructor]
public sealed partial class UnhandledExceptionLoggerProvider : ILoggerProvider
{
    private readonly IServiceProvider _serviceProvider;

    public ILogger CreateLogger(string categoryName)
    {
        return new UnhandledExceptionLogger(_serviceProvider);
    }

    public void Dispose()
    {
        // Method intentionally left empty.
    }
}
