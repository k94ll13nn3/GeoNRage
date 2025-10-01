using Microsoft.Extensions.Logging;

namespace GeoNRage.Shared;

public static class Loggers
{
    public static readonly Action<ILogger, string, Exception> LogUnhandledException =
        LoggerMessage.Define<string>(LogLevel.Error, eventId:
        new EventId(id: 0, name: "UnhandledException"), formatString: "Unhandled exception: {Message}");

    public static readonly Action<ILogger, string, Exception?> LogErrorFromApp =
        LoggerMessage.Define<string>(LogLevel.Error, eventId:
        new EventId(id: 1, name: "UnhandledException"), formatString: "Received error from app: {Message}");

    public static readonly Func<ILogger, string?, string?, IDisposable?> Scope =
        LoggerMessage.DefineScope<string?, string?>("{Source} {Stacktrace}");
}
