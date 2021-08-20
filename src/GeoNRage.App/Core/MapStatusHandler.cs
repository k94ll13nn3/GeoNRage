namespace GeoNRage.App.Core;

[AutoConstructor]
public partial class MapStatusHandler : DelegatingHandler
{
    public const string HeaderName = "show-all-maps";

    private readonly MapStatusService _mapStatusService;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _ = request ?? throw new ArgumentNullException(nameof(request));

        if (request.Headers.Contains(HeaderName))
        {
            request.Headers.Remove(HeaderName);
            request.Headers.Add(HeaderName, _mapStatusService.AllMaps.ToString());
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
