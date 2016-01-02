using Discord.Logging;
using Nito.AsyncEx;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
	internal class SimpleAudioClient : AudioClient
    {
        internal class VirtualClient : IAudioClient
        {
            private readonly SimpleAudioClient _client;

            ConnectionState IAudioClient.State => _client.VoiceSocket.State;
            Server IAudioClient.Server => _client.VoiceSocket.Server;
            Channel IAudioClient.Channel => _client.VoiceSocket.Channel;

            public VirtualClient(SimpleAudioClient client)
            {
                _client = client;
            }

            Task IAudioClient.Disconnect() => _client.Leave(this);
            Task IAudioClient.Join(Channel channel) => _client.Join(channel);

            void IAudioClient.Send(byte[] data, int count) => _client.Send(data, count);
            void IAudioClient.Clear() => _client.Clear();
            void IAudioClient.Wait() => _client.Wait();
        }

        private readonly AsyncLock _connectionLock;

        internal VirtualClient CurrentClient { get; private set; }

        public SimpleAudioClient(AudioService service, int id, Logger logger)
            : base(service, id, null, service.Client.GatewaySocket, logger)
        {
            _connectionLock = new AsyncLock();
        }

        //Only disconnects if is current a member of this server
        public async Task Leave(VirtualClient client)
        {
            using (await _connectionLock.LockAsync())
            {
                if (CurrentClient == client)
                {
                    CurrentClient = null;
                    await Disconnect();
                }
            }
        }

        internal async Task<IAudioClient> Connect(Channel channel)
        {
            using (await _connectionLock.LockAsync())
            {
                bool changeServer = channel.Server != VoiceSocket.Server;
                if (changeServer || CurrentClient == null)
                {
                    await Disconnect().ConfigureAwait(false);
                    CurrentClient = new VirtualClient(this);
                    VoiceSocket.Server = channel.Server;
                }
                await Join(channel);
                return CurrentClient;
            }
        }
    }
}
