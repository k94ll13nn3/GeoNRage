using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GeoNRage.Server.Models;
using Microsoft.Extensions.Options;

namespace GeoNRage.Server.Services
{
    [AutoConstructor]
    public partial class GeoGuessrService
    {
        private readonly IHttpClientFactory _clientFactory;

        [AutoConstructorInject("options?.Value", "options", typeof(IOptions<ApplicationOptions>))]
        private readonly ApplicationOptions _options;

        private readonly CookieContainer _cookieContainer;

        public async Task<IList<GeoGuessrChallenge>> ImportChallengeAsync(string geoGuessrId)
        {
            _ = geoGuessrId ?? throw new ArgumentNullException(nameof(geoGuessrId));
            HttpClient client = _clientFactory.CreateClient("geoguessr");

            if (_cookieContainer.Count == 0 || _cookieContainer.GetCookies(new Uri("https://www.geoguessr.com")).FirstOrDefault()?.Expired == true)
            {
                await client.PostAsJsonAsync("accounts/signin", new GeoGuessrLogin { Email = _options.GeoGuessrEmail, Password = _options.GeoGuessrPassword });
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

            try
            {
                GeoGuessrChallenge[]? challenges = await client.GetFromJsonAsync<GeoGuessrChallenge[]>($"results/scores/{geoGuessrId}/0/26", options);

                if (challenges is null)
                {
                    throw new InvalidOperationException("Cannot import data.");
                }

                if (!challenges.Any(r => r.Game.Player.Id != profile.Id))
                {
                    throw new InvalidOperationException("At least one player must play the challenge.");
                }

                return challenges.Where(r => r.Game.Player.Id != profile.Id).ToList();
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpResponseMessage response = await client.PostAsync(new Uri($"challenges/{geoGuessrId}", UriKind.Relative), null!);
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
                        response = await client.PostAsJsonAsync(new Uri($"games/{gameId}", UriKind.Relative), new GeoGuessrGameGuess { Token = gameId, Lat = 0m, Lng = 0m, TimedOut = false });
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

                GeoGuessrChallenge[]? challenges = await client.GetFromJsonAsync<GeoGuessrChallenge[]>($"results/scores/{geoGuessrId}/0/26", options);

                if (challenges is null)
                {
                    throw new InvalidOperationException("Cannot import data.");
                }

                if (!challenges.Any(r => r.Game.Player.Id != profile.Id))
                {
                    throw new InvalidOperationException("At least one player must play the challenge.");
                }

                return challenges.Where(r => r.Game.Player.Id != profile.Id).ToList();
            }
        }
    }
}
