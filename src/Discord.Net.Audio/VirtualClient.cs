using Discord.Net.Rest;
using Discord.Net.WebSockets;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    internal class VirtualClient : IAudioClient
    {
        private readonly AudioClient _client;

        public Server Server { get; }

        public int Id => 0;
        public string SessionId => _client.Server == Server ? _client.SessionId : null;

        public ConnectionState State => _client.Server == Server ? _client.State : ConnectionState.Disconnected;
        public Channel Channel => _client.Server == Server ? _client.Channel : null;
        public Stream OutputStream => _client.Server == Server ? _client.OutputStream : null;
        public CancellationToken CancelToken => _client.Server == Server ? _client.CancelToken : CancellationToken.None;

        public RestClient ClientAPI => _client.Server == Server ? _client.ClientAPI : null;
        public GatewaySocket GatewaySocket => _client.Server == Server ? _client.GatewaySocket : null;
        public VoiceSocket VoiceSocket => _client.Server == Server ? _client.VoiceSocket : null;

        public VirtualClient(AudioClient client, Server server)
        {
            _client = client;
            Server = server;
        }

        public Task Disconnect() => _client.Service.Leave(Server);
        public Task Join(Channel channel) => _client.Join(channel);

        public void Send(byte[] data, int offset, int count) => _client.Send(data, offset, count);
        public void Clear() => _client.Clear();
        public void Wait() => _client.Wait();
    }
}
