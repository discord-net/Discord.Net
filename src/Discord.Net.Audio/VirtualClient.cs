using System.IO;
using System.Threading.Tasks;

namespace Discord.Audio
{
    internal class VirtualClient : IAudioClient
    {
        private readonly AudioClient _client;

        public Server Server { get; }

        public ConnectionState State => _client.VoiceSocket.Server == Server ? _client.VoiceSocket.State : ConnectionState.Disconnected;
        public Channel Channel => _client.VoiceSocket.Server == Server ? _client.VoiceSocket.Channel : null;
        public Stream OutputStream => _client.VoiceSocket.Server == Server ? _client.OutputStream : null;

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
