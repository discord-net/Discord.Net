using System.Text.Json;
using System.Threading.Tasks;
using Refit;
using Discord.Rest.Models;
using System.Net.Http.Headers;
using System;
using System.Net.Http;

// This is essentially a reimplementation of Wumpus.Net.Rest
namespace Discord.Rest
{
    public class DiscordRestApi : IDiscordRestApi
    {
        private readonly IDiscordRestApi _api;
        private readonly HttpClient _http;

        public DiscordRestApi(DiscordConfig config, AuthenticationHeaderValue token)
        {
            _http = new HttpClient(new DiscordHttpClientHandler(token), true)
            {
                BaseAddress = config.RestUri ?? DiscordConfig.DefaultRestUri,
            };

            var jsonOptions = new JsonSerializerOptions();
            var refitSettings = new RefitSettings
            {
                ContentSerializer = new JsonContentSerializer(jsonOptions),
            };
            _api = RestService.For<IDiscordRestApi>(_http, refitSettings);
        }

        public Task<GatewayInfo> GetGatewayInfoAsync()
        {
            return _api.GetGatewayInfoAsync();
        }

        public void Dispose()
        {
            _http.Dispose();
        }
    }
}
