using Discord.Gateway.Cache;
using Discord.Gateway.State;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    public class DiscordGatewayClient : IGatewayClient
    {
        internal StateController State { get; }

        internal DiscordGatewayConfig Config { get; }

        internal DiscordRestClient Rest { get; }

        private readonly ICacheProvider _cacheProvider;
        private readonly IGatewayConnection _connection;

        public DiscordGatewayClient(DiscordGatewayConfig config)
        {
            Config = config;
            _cacheProvider = config.CacheProvider;
            _connection = config.GatewayConnection;

            Rest = new DiscordRestClient(config);
            State = new(this, in _cacheProvider);
        }

        private async Task ConnectAndAuthenticateAsync()
        {
            var gatewayInformation = await Rest.ApiClient.get

            await _connection.ConnectAsync(this);


        }
    }
}
