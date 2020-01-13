using System;
using Discord.Rest;
using Discord.Socket;

namespace Discord
{
    internal class DiscordClient : IDiscordClient
    {
        public DiscordRestApi Rest { get; }
        public DiscordGatewayApi Gateway { get; }

        private readonly DiscordConfig _config;
        private readonly Logger _logger;

        public DiscordClient(DiscordConfig config, DiscordRestApi restApi, DiscordGatewayApi gatewayApi)
        {
            _config = config;
            _logger = new Logger("Client", config.MinClientSeverity);

            Rest = restApi;
            Gateway = gatewayApi;

            Log += _ => { }; // initialize log method
            Rest.Logger.Message += m => Log(m);
            Gateway.Logger.Message += m => Log(m);
            _logger.Message += m => Log(m);
        }

        public event Action<LogMessage> Log;

        public void Dispose()
        {
            Rest.Dispose();
            Gateway.Dispose();
        }
    }
}
