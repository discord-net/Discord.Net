using System.Text.Json;
using System.Threading.Tasks;
using Refit;
using Discord.Rest.Models;

// This is essentially a reimplementation of Wumpus.Net.Rest
namespace Discord.Rest
{
    public class DiscordRestApi : IDiscordRestApi
    {
        private readonly IDiscordRestApi _api;

        public DiscordRestApi(DiscordConfig config)
        {
            var jsonOptions = new JsonSerializerOptions();
            var refitSettings = new RefitSettings
            {
                ContentSerializer = new JsonContentSerializer(jsonOptions),
            };
            _api = RestService.For<IDiscordRestApi>(config.RestApiUrl, refitSettings);
        }

        public Task<GatewayInfo> GetGatewayInfoAsync()
        {
            return _api.GetGatewayInfoAsync();
        }
    }
}
