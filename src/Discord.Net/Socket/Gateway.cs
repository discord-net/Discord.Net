using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Socket
{
    public class Gateway
    {
        static readonly Uri DefaultGatewayUri = new Uri("wss://gateway.discord.gg");

        ISocket Socket { get; set; }

        public Gateway(SocketFactory socketFactory)
        {
            Socket = socketFactory(OnAborted, OnPacket);
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
