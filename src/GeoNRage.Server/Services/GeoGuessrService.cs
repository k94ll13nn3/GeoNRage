using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using GeoNRage.Server.Models;
using Microsoft.Extensions.Options;

namespace GeoNRage.Server.Services;

[AutoConstructor]
public partial class GeoGuessrService
{
    private readonly IHttpClientFactory _clientFactory;

    [AutoConstructorInject("options?.Value", "options", typeof(IOptions<ApplicationOptions>))]
    private readonly ApplicationOptions _options;

    private readonly CookieContainer _cookieContainer;

    public async Task<(GeoGuessrChallenge challenge, IList<GeoGuessrChallengeResult> results)> ImportChallengeAsync(string geoGuessrId)
    {
        _ = geoGuessrId ?? throw new ArgumentNullException(nameof(geoGuessrId));
        HttpClient client = _clientFactory.CreateClient("geoguessr");

        if (_cookieContainer.Count == 0 || _cookieContainer.GetCookies(new Uri("https://www.geoguessr.com")).FirstOrDefault()?.Expired == true)
        {
            await client.PostAsJsonAsync("accounts/signin", new GeoGuessrLogin(_options.GeoGuessrEmail, _options.GeoGuessrPassword));
        }

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };

        GeoGuessrProfile? profile = await client.GetFromJsonAsync<GeoGuessrProfile>("profiles/me", options);
        if (profile is null)
        {
            throw new InvalidOperationException("Cannot import data.");
        }

        GeoGuessrChallenge? challenge = null;
        try
        {
            challenge = await client.GetFromJsonAsync<GeoGuessrChallenge>($"challenges/{geoGuessrId}", options);
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            throw new InvalidOperationException("Challenge not found.");
        }

        if (challenge is null)
        {
            throw new InvalidOperationException("Cannot import data.");
        }

        GeoGuessrChallengeResult[]? challengeResults;
        try
        {
            challengeResults = await client.GetFromJsonAsync<GeoGuessrChallengeResult[]>($"results/scores/{geoGuessrId}/0/26", options);
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.Unauthorized)
        {
            HttpResponseMessage response = await client.PostAsync(new Uri($"challenges/{geoGuessrId}", UriKind.Relative), null);
            string? gameId = (await response.Content.ReadFromJsonAsync<GeoGuessrChallengeGame>())?.Token;
            if (gameId is null)
            {
                throw new InvalidOperationException("Cannot import data.");
            }

            for (int i = 0; i < 5; i++)
            {
                response = await client.GetAsync(new Uri($"games/{gameId}", UriKind.Relative));
                if (response.IsSuccessStatusCode)
                {
                    response = await client.PostAsJsonAsync(new Uri($"games/{gameId}", UriKind.Relative), new GeoGuessrGameGuess(gameId, 0m, 0m, false));
                    if (!response.IsSuccessStatusCode)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            challengeResults = await client.GetFromJsonAsync<GeoGuessrChallengeResult[]>($"results/scores/{geoGuessrId}/0/26", options);
        }

        if (challengeResults is null)
        {
            throw new InvalidOperationException("Cannot import data.");
        }

        if (!challengeResults.Any(r => r.Game.Player.Id != profile.Id))
        {
            throw new InvalidOperationException("At least one player must play the challenge.");
        }

        return (challenge, results: challengeResults.Where(r => r.Game.Player.Id != profile.Id).ToList());
    }
}
