using Discord.Net.WebSockets;
using System;
using System.Threading.Tasks;

namespace Discord.Audio
{
	public partial class DiscordAudioClient
	{
		private readonly int _id;
		public int Id => _id;

		private readonly AudioService _service;
		private readonly GatewayWebSocket _gatewaySocket;
		private readonly VoiceWebSocket _voiceSocket;
		private readonly Logger _logger;

		public long? ServerId => _voiceSocket.ServerId;
		public long? ChannelId => _voiceSocket.ChannelId;

		public DiscordAudioClient(AudioService service, int id, Logger logger, GatewayWebSocket gatewaySocket, VoiceWebSocket voiceSocket)
		{
			_service = service;
			_id = id;
			_logger = logger;
			_gatewaySocket = gatewaySocket;
			_voiceSocket = voiceSocket;

			/*_voiceSocket.Connected += (s, e) => RaiseVoiceConnected();
			_voiceSocket.Disconnected += async (s, e) =>
			{
				_voiceSocket.CurrentServerId;
				if (voiceServerId != null)
					_gatewaySocket.SendLeaveVoice(voiceServerId.Value);
				await _voiceSocket.Disconnect().ConfigureAwait(false);
				RaiseVoiceDisconnected(socket.CurrentServerId.Value, e);
				if (e.WasUnexpected)
					await socket.Reconnect().ConfigureAwait(false);
			};*/

			/*_voiceSocket.IsSpeaking += (s, e) =>
			{
				if (_voiceSocket.State == WebSocketState.Connected)
				{
					var user = _users[e.UserId, socket.CurrentServerId];
					bool value = e.IsSpeaking;
					if (user.IsSpeaking != value)
					{
						user.IsSpeaking = value;
						var channel = _channels[_voiceSocket.CurrentChannelId];
						RaiseUserIsSpeaking(user, channel, value);
						if (Config.TrackActivity)
							user.UpdateActivity();
					}
				}
			};*/

			/*this.Connected += (s, e) =>
			{
				_voiceSocket.ParentCancelToken = _cancelToken;
			};*/

			_gatewaySocket.ReceivedDispatch += async (s, e) =>
			{
				try
				{
					switch (e.Type)
					{
						case "VOICE_SERVER_UPDATE":
							{
								long serverId = IdConvert.ToLong(e.Payload.Value<string>("guild_id"));

								if (serverId == ServerId)
								{
									var client = _service.Client;
									string token = e.Payload.Value<string>("token");
									_voiceSocket.Host = "wss://" + e.Payload.Value<string>("endpoint").Split(':')[0];
									await _voiceSocket.Login(client.CurrentUser.Id, _gatewaySocket.SessionId, token, client.CancelToken).ConfigureAwait(false);
								}
							}
							break;
					}
				}
				catch (Exception ex)
				{
					_gatewaySocket.Logger.Log(LogSeverity.Error, $"Error handling {e.Type} event", ex);
				}
			};
		}
		
		public Task Disconnect()
		{
			return _voiceSocket.Disconnect();
		}

		internal void SetServerId(long serverId)
		{
			_voiceSocket.ServerId = serverId;
		}
		public async Task Join(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
			long? serverId = channel.Server?.Id;
			if (serverId != ServerId)
				throw new InvalidOperationException("Cannot join a channel on a different server than this voice client.");
			//CheckReady(checkVoice: true);

			await _voiceSocket.Disconnect().ConfigureAwait(false);
			_voiceSocket.ChannelId = channel.Id;
			_gatewaySocket.SendJoinVoice(channel.Server.Id, channel.Id);
			await _voiceSocket.WaitForConnection(_service.Config.ConnectionTimeout).ConfigureAwait(false);
		}

		/// <summary> Sends a PCM frame to the voice server. Will block until space frees up in the outgoing buffer. </summary>
		/// <param name="data">PCM frame to send. This must be a single or collection of uncompressed 48Kz monochannel 20ms PCM frames. </param>
		/// <param name="count">Number of bytes in this frame. </param>
		public void Send(byte[] data, int count)
		{
			if (data == null) throw new ArgumentException(nameof(data));
			if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
			//CheckReady(checkVoice: true);

			if (count != 0)
				_voiceSocket.SendPCMFrames(data, count);
		}

		/// <summary> Clears the PCM buffer. </summary>
		public void Clear()
		{
			//CheckReady(checkVoice: true);

			_voiceSocket.ClearPCMFrames();
		}

		/// <summary> Returns a task that completes once the voice output buffer is empty. </summary>
		public Task Wait()
		{
			//CheckReady(checkVoice: true);

			_voiceSocket.WaitForQueue();
			return TaskHelper.CompletedTask;
		}
	}
}
