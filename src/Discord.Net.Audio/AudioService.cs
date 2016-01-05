using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Audio
{
	public class AudioService : IService
    {
        private AudioClient _defaultClient;
		private ConcurrentDictionary<ulong, IAudioClient> _voiceClients;
		private ConcurrentDictionary<User, bool> _talkingUsers;
		//private int _nextClientId;

		internal DiscordClient Client { get; private set; }
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

		public AudioService(AudioServiceConfig config)
		{
			Config = config;
		}
		void IService.Install(DiscordClient client)
		{
			Client = client;
            Config.Lock();

            if (Config.EnableMultiserver)
				_voiceClients = new ConcurrentDictionary<ulong, IAudioClient>();
			else
			{
				var logger = Client.Log.CreateLogger("Voice");
				_defaultClient = new SimpleAudioClient(this, 0, logger);
			}
			_talkingUsers = new ConcurrentDictionary<User, bool>();

			client.Disconnected += async (s, e) =>
			{
				if (Config.EnableMultiserver)
				{
					var tasks = _voiceClients
						.Select(x => x.Value.Disconnect())
						.ToArray();
					await Task.WhenAll(tasks).ConfigureAwait(false);
					_voiceClients.Clear();
				}
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

            if (!Config.EnableMultiserver)
            {
                if (server == _defaultClient.Server)
                    return (_defaultClient as SimpleAudioClient).CurrentClient;
                else
                    return null;
            }
            else
            {
                IAudioClient client;
                if (_voiceClients.TryGetValue(server.Id, out client))
                    return client;
                else
                    return null;
            }
		}
		private Task<IAudioClient> CreateClient(Server server)
		{
            throw new NotImplementedException();
			/*var client = _voiceClients.GetOrAdd(server.Id, _ =>
			{
				int id = unchecked(++_nextClientId);
				var logger = Client.Log.CreateLogger($"Voice #{id}");
				var voiceClient = new DiscordAudioClient(this, id, logger, Client.GatewaySocket);
				voiceClient.SetServerId(server.Id);

                voiceClient.VoiceSocket.OnPacket += (s, e) =>
				{
                    RaiseOnPacket(e);
				};
                voiceClient.VoiceSocket.IsSpeaking += (s, e) =>
				{
					var user = server.GetUser(e.UserId);
                    RaiseUserIsSpeakingUpdated(user, e.IsSpeaking);
				};

				return voiceClient;
			});
			//await client.Connect(gatewaySocket.Host, _client.Token).ConfigureAwait(false);
			return Task.FromResult(client);*/
		}

        //TODO: This isn't threadsafe
        internal void RemoveClient(Server server, IAudioClient client)
        {
            if (Config.EnableMultiserver && server != null)
                _voiceClients.TryRemove(server.Id, out client);
        }

		public async Task<IAudioClient> Join(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));

            IAudioClient client;
            if (!Config.EnableMultiserver)
                client = await (_defaultClient as SimpleAudioClient).Connect(channel).ConfigureAwait(false);
            else
            {
                client = await CreateClient(channel.Server).ConfigureAwait(false);
                await client.Join(channel).ConfigureAwait(false);
            }
			return client;
		}
		
		public async Task Leave(Server server)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));

            if (Config.EnableMultiserver)
            {
                IAudioClient client;
                if (_voiceClients.TryRemove(server.Id, out client))
                    await client.Disconnect().ConfigureAwait(false);
            }
            else
            {
                IAudioClient client = GetClient(server);
                if (client != null)
                    await (_defaultClient as SimpleAudioClient).Leave(client as SimpleAudioClient.VirtualClient).ConfigureAwait(false);
            }
		}
	}
}
