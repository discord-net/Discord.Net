using Discord.Rest;
using Discord.Socket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Discord
{
    internal class DiscordClient : IDiscordClient
    {
        public DiscordRestApi Rest => _restApi;
        public DiscordGatewayApi Gateway => _gatewayApi;

        private readonly DiscordConfig _config;
        private readonly DiscordRestApi _restApi;
        private readonly DiscordGatewayApi _gatewayApi;

        public DiscordClient(DiscordConfig config, DiscordRestApi restApi, DiscordGatewayApi gatewayApi)
        {
            _config = config;
            _restApi = restApi;
            _gatewayApi = gatewayApi;
        }
    }
}
