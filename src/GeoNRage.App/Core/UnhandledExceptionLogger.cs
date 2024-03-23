using GeoNRage.App.Apis;

namespace GeoNRage.App.Core;

[AutoConstructor]
public partial class UnhandledExceptionLogger : ILogger
{
    private readonly IServiceProvider _serviceProvider;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return new NoopDisposable();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (logLevel >= LogLevel.Error)
        {
            await _serviceProvider
                .GetRequiredService<ILogApi>()
                .PostLogAsync(new ErrorLog(exception?.Message ?? "An unexpected error occured", exception?.Source, exception?.StackTrace));
        }
    }

    private sealed class NoopDisposable : IDisposable
    {
        public void Dispose()
        {
            // Method intentionally left empty.
        }
    }
}
