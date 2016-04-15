using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Discord.Audio
{
    public class AudioService : IService
    {
        private readonly AsyncLock _asyncLock;
        private ConcurrentDictionary<ulong, AudioClient> _voiceClients;
        private ConcurrentDictionary<User, bool> _talkingUsers;
        private int _nextClientId;

        public DiscordClient Client { get; private set; }
        public AudioServiceConfig Config { get; }

        public event EventHandler Connected = delegate { };
        public event EventHandler<VoiceDisconnectedEventArgs> Disconnected = delegate { };
        public event EventHandler<UserIsSpeakingEventArgs> UserIsSpeakingUpdated = delegate { };

        private void OnConnected()
            => Connected(this, EventArgs.Empty);
        private void OnDisconnected(ulong serverId, bool wasUnexpected, Exception ex)
            => Disconnected(this, new VoiceDisconnectedEventArgs(serverId, wasUnexpected, ex));
        private void OnUserIsSpeakingUpdated(User user, bool isSpeaking)
            => UserIsSpeakingUpdated(this, new UserIsSpeakingEventArgs(user, isSpeaking));

        public AudioService()
            : this(new AudioServiceConfigBuilder())
        {
        }
        public AudioService(AudioServiceConfigBuilder builder)
            : this(builder.Build())
        {
        }
        public AudioService(AudioServiceConfig config)
        {
            Config = config;
            _asyncLock = new AsyncLock();

        }
        void IService.Install(DiscordClient client)
        {
            Client = client;

            _voiceClients = new ConcurrentDictionary<ulong, AudioClient>();

            _talkingUsers = new ConcurrentDictionary<User, bool>();

            client.GatewaySocket.Disconnected += (s, e) =>
            {
                foreach (var member in _talkingUsers)
                {
                    bool ignored;
                    if (_talkingUsers.TryRemove(member.Key, out ignored))
                        OnUserIsSpeakingUpdated(member.Key, false);
                }
            };
        }

        public IAudioClient GetClient(Server server)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            AudioClient client;
            if (_voiceClients.TryGetValue(server.Id, out client))
                return client;
            else
                return null;
        }

        //Called from AudioClient.Cleanup
        internal async Task RemoveClient(Server server, AudioClient client)
        {
            using (await _asyncLock.LockAsync().ConfigureAwait(false))
            {
                if (_voiceClients.TryUpdate(server.Id, null, client))
                    _voiceClients.TryRemove(server.Id, out client);
            }
        }

        public async Task<IAudioClient> Join(Channel channel)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));

            var server = channel.Server;
            using (await _asyncLock.LockAsync().ConfigureAwait(false))
            {
                AudioClient client;
                if (!_voiceClients.TryGetValue(server.Id, out client))
                {
                    client = new AudioClient(Client, server, unchecked(++_nextClientId));
                    _voiceClients[server.Id] = client;

                    await client.Connect().ConfigureAwait(false);
                }

                await client.Join(channel).ConfigureAwait(false);
                return client;
            }
        }

        public Task Leave(Server server) => Leave(server, null);
        public Task Leave(Channel channel) => Leave(channel.Server, channel);
        private async Task Leave(Server server, Channel channel)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));

            AudioClient client;
            //Potential race condition if changing channels during this call, but that's acceptable
            if (channel == null || (_voiceClients.TryGetValue(server.Id, out client) && client.Channel == channel))
            {
                if (_voiceClients.TryRemove(server.Id, out client))
                    await client.Disconnect().ConfigureAwait(false);
            }
        }
    }
}
