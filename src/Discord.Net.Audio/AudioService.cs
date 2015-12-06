using Discord.Net.WebSockets;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Audio
{
	public class VoiceDisconnectedEventArgs : DisconnectedEventArgs
	{
		public readonly long ServerId;

		public VoiceDisconnectedEventArgs(long serverId, DisconnectedEventArgs e)
			: base(e.WasUnexpected, e.Error)
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
		public readonly long UserId;
		public readonly long ChannelId;
		public readonly byte[] Buffer;
		public readonly int Offset;
		public readonly int Count;

		public VoicePacketEventArgs(long userId, long channelId, byte[] buffer, int offset, int count)
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
		private ConcurrentDictionary<long, DiscordAudioClient> _voiceClients;
		private ConcurrentDictionary<User, bool> _talkingUsers;
		private int _nextClientId;

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
		private void RaiseDisconnected(long serverId, DisconnectedEventArgs e)
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
				_voiceClients = new ConcurrentDictionary<long, DiscordAudioClient>();
			else
			{
				var logger = Client.Log().CreateLogger("Voice");
				var voiceSocket = new VoiceWebSocket(Client.Config, _config, logger);
				_defaultClient = new DiscordAudioClient(this, 0, logger, _client.WebSocket, voiceSocket);
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

		public DiscordAudioClient GetVoiceClient(Server server)
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
		private Task<DiscordAudioClient> CreateVoiceClient(Server server)
		{
			if (!Config.EnableMultiserver)
			{
				_defaultClient.SetServerId(server.Id);
				return Task.FromResult(_defaultClient);
			}

			var client = _voiceClients.GetOrAdd(server.Id, _ =>
			{
				int id = unchecked(++_nextClientId);
				var logger = Client.Log().CreateLogger($"Voice #{id}");
				DataWebSocket dataSocket = null;
				var voiceSocket = new VoiceWebSocket(Client.Config, _config, logger);
				var voiceClient = new DiscordAudioClient(this, id, logger, dataSocket, voiceSocket);
				voiceClient.SetServerId(server.Id);

				voiceSocket.OnPacket += (s, e) =>
				{
					RaiseOnPacket(e);
				};
				voiceSocket.IsSpeaking += (s, e) =>
				{
					var user = Client.GetUser(server, e.UserId);
					RaiseUserIsSpeakingUpdated(user, e.IsSpeaking);
				};

				return voiceClient;
			});
			//await client.Connect(dataSocket.Host, _client.Token).ConfigureAwait(false);
			return Task.FromResult(client);
		}

		public async Task<DiscordAudioClient> JoinVoiceServer(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			//CheckReady(true);

			var client = await CreateVoiceClient(channel.Server).ConfigureAwait(false);
			await client.Join(channel).ConfigureAwait(false);
			return client;
		}

		public async Task LeaveVoiceServer(Server server)
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
