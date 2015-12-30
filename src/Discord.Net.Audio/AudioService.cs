using Discord.Net.WebSockets;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Audio
{
	public class VoiceDisconnectedEventArgs : DisconnectedEventArgs
	{
		public readonly ulong ServerId;

		public VoiceDisconnectedEventArgs(ulong serverId, DisconnectedEventArgs e)
			: base(e.WasUnexpected, e.Exception)
		{
			ServerId = serverId;
		}
	}
	public class UserIsSpeakingEventArgs : UserEventArgs
	{
		public readonly bool IsSpeaking;

		public UserIsSpeakingEventArgs(User user, bool isSpeaking)
			: base(user)
		{
			IsSpeaking = isSpeaking;
		}
	}
	public class VoicePacketEventArgs : EventArgs
	{
		public readonly ulong UserId;
		public readonly ulong ChannelId;
		public readonly byte[] Buffer;
		public readonly int Offset;
		public readonly int Count;

		public VoicePacketEventArgs(ulong userId, ulong channelId, byte[] buffer, int offset, int count)
		{
			UserId = userId;
			ChannelId = channelId;
			Buffer = buffer;
			Offset = offset;
			Count = count;
		}
	}

	public class AudioService : IService
    {
        private DiscordAudioClient _defaultClient;
		private ConcurrentDictionary<ulong, DiscordAudioClient> _voiceClients;
		private ConcurrentDictionary<User, bool> _talkingUsers;
		//private int _nextClientId;

		internal DiscordClient Client => _client;
		private DiscordClient _client;

		public AudioServiceConfig Config => _config;
		private readonly AudioServiceConfig _config;

		public event EventHandler Connected;
		private void RaiseConnected()
		{
			if (Connected != null)
				Connected(this, EventArgs.Empty);
		}
		public event EventHandler<VoiceDisconnectedEventArgs> Disconnected;
		private void RaiseDisconnected(ulong serverId, DisconnectedEventArgs e)
		{
			if (Disconnected != null)
				Disconnected(this, new VoiceDisconnectedEventArgs(serverId, e));
		}
		public event EventHandler<VoicePacketEventArgs> OnPacket;
		internal void RaiseOnPacket(VoicePacketEventArgs e)
		{
			if (OnPacket != null)
				OnPacket(this, e);
		}
		public event EventHandler<UserIsSpeakingEventArgs> UserIsSpeakingUpdated;
		private void RaiseUserIsSpeakingUpdated(User user, bool isSpeaking)
		{
			if (UserIsSpeakingUpdated != null)
				UserIsSpeakingUpdated(this, new UserIsSpeakingEventArgs(user, isSpeaking));
		}

		public AudioService(AudioServiceConfig config)
		{
			_config = config;
			_config.Lock();
		}
		public void Install(DiscordClient client)
		{
			_client = client;
			if (Config.EnableMultiserver)
				_voiceClients = new ConcurrentDictionary<ulong, DiscordAudioClient>();
			else
			{
				var logger = Client.Log.CreateLogger("Voice");
				_defaultClient = new DiscordAudioClient(this, 0, logger, _client.GatewaySocket);
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
						RaiseUserIsSpeakingUpdated(member.Key, false);
				}
			};
		}

		public DiscordAudioClient GetClient(Server server)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));

			if (!Config.EnableMultiserver)
			{
				if (server.Id == _defaultClient.ServerId)
					return _defaultClient;
				else
					return null;
			}

			DiscordAudioClient client;
			if (_voiceClients.TryGetValue(server.Id, out client))
				return client;
			else
				return null;
		}
		private async Task<DiscordAudioClient> CreateClient(Server server)
		{
			if (!Config.EnableMultiserver)
			{
				await _defaultClient.SetServer(server.Id);
				return _defaultClient;
			}
            else
                throw new InvalidOperationException("Multiserver voice is not currently supported");

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

		public async Task<DiscordAudioClient> Join(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			//CheckReady(true);

			var client = await CreateClient(channel.Server).ConfigureAwait(false);
			await client.JoinChannel(channel).ConfigureAwait(false);
			return client;
		}
		
		public async Task Leave(Server server)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));
			//CheckReady(true);

			if (Config.EnableMultiserver)
			{
				//client.CheckReady();
				DiscordAudioClient client;
				if (_voiceClients.TryRemove(server.Id, out client))
					await client.Disconnect().ConfigureAwait(false);
			}
			else
				await _defaultClient.Disconnect().ConfigureAwait(false);
		}
	}
}
