using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GeoNRage.Data.Entities;

namespace GeoNRage.App.Clients
{
    public class GamesHttpClient
    {
        private readonly HttpClient _http;

        public GamesHttpClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<Game[]> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<Game[]>("/") ?? Array.Empty<Game>();
        }

        public async Task<Game?> GetAsync(int id)
        {
            return await _http.GetFromJsonAsync<Game>($"/{id}");
        }
    }
}
