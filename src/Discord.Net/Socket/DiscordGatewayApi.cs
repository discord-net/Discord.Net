using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Socket
{
    public class DiscordGatewayApi
    {
        private readonly DiscordConfig _config;
        private readonly string _token;

        public ISocket Socket { get; set; }

        public DiscordGatewayApi(DiscordConfig config, string token)
        {
            _config = config;
            _token = token;

            Socket = config.SocketFactory(OnAborted, OnPacket);
        }

        public async Task ConnectAsync(Uri? gatewayUri)
        {
            var baseUri = _config.GatewayUri ?? (gatewayUri ?? DiscordConfig.DefaultGatewayUri);
            await Socket.ConnectAsync(baseUri, CancellationToken.None).ConfigureAwait(false);
        }

        public void OnAborted(Exception error)
        {
            // todo: log
        }
        public async Task OnPacket(object packet)
        {
            await Task.CompletedTask;
        }
    }
}
