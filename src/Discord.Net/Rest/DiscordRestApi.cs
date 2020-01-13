using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Refit;
using Discord.Models;
using Discord.Serialization;

// This is essentially a reimplementation of Wumpus.Net.Rest
namespace Discord.Rest
{
    public class DiscordRestApi : IDiscordRestApi
    {
        private readonly IDiscordRestApi _api;
        private readonly HttpClient _http;

        internal Logger Logger { get; private set; }

        public DiscordRestApi(DiscordConfig config, AuthenticationHeaderValue token)
        {
            Logger = new Logger("Rest", config.MinRestSeverity);

            _http = new HttpClient(new DiscordHttpClientHandler(token), true)
            {
                BaseAddress = config.RestUri ?? DiscordConfig.DefaultRestUri,
            };

            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new OptionalConverter());
            var refitSettings = new RefitSettings
            {
                ContentSerializer = new JsonContentSerializer(jsonOptions),
            };
            _api = RestService.For<IDiscordRestApi>(_http, refitSettings);
        }
        
        public Task<GatewayInfo> GetGatewayInfoAsync()
            => _api.GetGatewayInfoAsync();
        public Task<GatewayInfo> GetBotGatewayInfoAsync()
            => _api.GetBotGatewayInfoAsync();

        public void Dispose()
        {
            _http.Dispose();
        }
    }
}
