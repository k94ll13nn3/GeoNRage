namespace GeoNRage.App.Core;

[AutoConstructor]
public partial class MapStatusHandler : DelegatingHandler
{
    private readonly UserSettingsService _userSettingsService;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _ = request ?? throw new ArgumentNullException(nameof(request));

        if (request.Headers.Contains(Constants.MapStatusHeaderName))
        {
            request.Headers.Remove(Constants.MapStatusHeaderName);
            request.Headers.Add(Constants.MapStatusHeaderName, (await _userSettingsService.GetAsync()).AllMaps.ToString());
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
