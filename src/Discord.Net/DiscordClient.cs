using Discord.Rest;
using Discord.Socket;

namespace Discord
{
    internal class DiscordClient : IDiscordClient
    {
        public DiscordRestApi Rest { get; }
        public DiscordGatewayApi Gateway { get; }

        private readonly DiscordConfig _config;

        public DiscordClient(DiscordConfig config, DiscordRestApi restApi, DiscordGatewayApi gatewayApi)
        {
            _config = config;
            Rest = restApi;
            Gateway = gatewayApi;
        }

        public void Dispose()
        {
            Rest.Dispose();
            Gateway.Dispose();
        }
    }
}
