namespace GeoNRage.Server;

internal sealed class ApplicationOptions
{
    public string GeoGuessrCookie { get; set; } = string.Empty;

    public string GoogleApiKey { get; set; } = string.Empty;

    public string SuperAdminPassword { get; set; } = string.Empty;

    public string SuperAdminUserName { get; set; } = string.Empty;

    public string DiscordBotToken { get; set; } = string.Empty;

    public string DiscordDevServerId { get; set; } = string.Empty;

    public Uri GeoNRageUrl { get; set; } = null!;
}
