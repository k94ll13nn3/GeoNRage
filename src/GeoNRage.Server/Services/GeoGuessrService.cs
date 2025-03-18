using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using GeoNRage.Server.Models;
using Microsoft.Extensions.Options;

namespace GeoNRage.Server.Services;

[AutoConstructor]
internal sealed partial class GeoGuessrService
{
    private readonly IHttpClientFactory _clientFactory;

    [AutoConstructorInject("options?.Value", "options", typeof(IOptions<ApplicationOptions>))]
    private readonly ApplicationOptions _options;

    private readonly CookieContainer _cookieContainer;

    public async Task<(GeoGuessrChallenge challenge, IList<GeoGuessrChallengeResult> results)> ImportChallengeAsync(string geoGuessrId)
    {
        _ = geoGuessrId ?? throw new ArgumentNullException(nameof(geoGuessrId));
        HttpClient client = _clientFactory.CreateClient("geoguessr");

        EnsureCookieIsSet();

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
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new InvalidOperationException("Cannot import data.");
        }

        if (challenge is null)
        {
            throw new InvalidOperationException("Cannot import data.");
        }

        IList<GeoGuessrChallengeResult>? challengeResults;
        try
        {
            challengeResults = (await client.GetFromJsonAsync<GeoGuessrChallengeHighscore>($"results/highscores/{geoGuessrId}?limit=26", options))?.Items;
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
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

            challengeResults = (await client.GetFromJsonAsync<GeoGuessrChallengeHighscore>($"results/highscores/{geoGuessrId}?limit=26", options))?.Items;
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

    public async Task<GeoGuessrPlayerStatistics?> GetPlayerStatisticsAsync(string playerId)
    {
        EnsureCookieIsSet();

        HttpClient clientV4 = _clientFactory.CreateClient("geoguessrV4");
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };

        return await clientV4.GetFromJsonAsync<GeoGuessrPlayerStatistics>($"stats/users/{playerId}", options);
    }

    private void EnsureCookieIsSet()
    {
        Uri geoguessrUri = new("https://www.geoguessr.com");
        if (_cookieContainer.Count == 0 || _cookieContainer.GetCookies(geoguessrUri).FirstOrDefault()?.Expired == true)
        {
            Cookie secureCookie = new("_ncfa", _options.GeoGuessrCookie)
            {
                Domain = geoguessrUri.Host,
                Path = "/",
                Secure = true,
                HttpOnly = true
            };
            _cookieContainer.Add(geoguessrUri, secureCookie);
        }
    }
}
