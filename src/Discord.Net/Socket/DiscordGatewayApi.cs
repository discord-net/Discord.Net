using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Socket
{
    public class DiscordGatewayApi
    {
        static readonly Uri DefaultGatewayUri = new Uri("wss://gateway.discord.gg");

        ISocket Socket { get; set; }

        public DiscordGatewayApi(DiscordConfig config)
        {
            Socket = config.SocketFactory(OnAborted, OnPacket);
        }

        public async Task ConnectAsync(Uri? gatewayUri)
        {
            await Socket.ConnectAsync(gatewayUri ?? DefaultGatewayUri, CancellationToken.None).ConfigureAwait(false);
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
