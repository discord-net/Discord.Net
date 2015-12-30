using Discord.API.Client.GatewaySocket;
using Discord.Logging;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
	public partial class DiscordAudioClient
    {
        private readonly Semaphore _connectionLock;
        private readonly JsonSerializer _serializer;
        private CancellationTokenSource _cancelTokenSource;

        internal AudioService Service { get; }
        internal Logger Logger { get; }
        public int Id { get; }
        public GatewaySocket GatewaySocket { get; }
        public VoiceWebSocket VoiceSocket { get; }

        public ulong? ServerId => VoiceSocket.ServerId;
        public ulong? ChannelId => VoiceSocket.ChannelId;
        public ConnectionState State => VoiceSocket.State;

        public DiscordAudioClient(AudioService service, int id, Logger logger, GatewaySocket gatewaySocket)
		{
			Service = service;
			Id = id;
			Logger = logger;
            GatewaySocket = gatewaySocket;

            _connectionLock = new Semaphore(1, 1);

            _serializer = new JsonSerializer();
            _serializer.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            _serializer.Error += (s, e) =>
            {
                e.ErrorContext.Handled = true;
                Logger.Error("Serialization Failed", e.ErrorContext.Error);
            };

			VoiceSocket = new VoiceWebSocket(service.Client, this, _serializer, logger);

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

        }
		

		internal async Task SetServer(ulong serverId)
		{
            if (serverId != VoiceSocket.ServerId)
            {
                await Disconnect().ConfigureAwait(false);
                VoiceSocket.ServerId = serverId;
                VoiceSocket.ChannelId = null;
                SendVoiceUpdate();
            }
		}
		public Task JoinChannel(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
            var serverId = channel.Server?.Id;
            var channelId = channel.Id;
            if (serverId != ServerId)
				throw new InvalidOperationException("Cannot join a channel on a different server than this voice client.");
            if (channelId == VoiceSocket.ChannelId)
                return TaskHelper.CompletedTask;
            //CheckReady(checkVoice: true);
            
            return Task.Run(async () =>
            {
                _connectionLock.WaitOne();
                GatewaySocket.ReceivedDispatch += OnReceivedDispatch;
                try
                {
                    if (State != ConnectionState.Disconnected)
                        await Disconnect().ConfigureAwait(false);

                    _cancelTokenSource = new CancellationTokenSource();
                    var cancelToken = _cancelTokenSource.Token;
                    VoiceSocket.ParentCancelToken = cancelToken;

                    VoiceSocket.ChannelId = channelId;
                    SendVoiceUpdate();

                    VoiceSocket.WaitForConnection(cancelToken);
                }
                finally
                {
                    GatewaySocket.ReceivedDispatch -= OnReceivedDispatch;
                    _connectionLock.Release();
                }
            });
        }
        public Task Disconnect()
            => VoiceSocket.Disconnect();

        private async void OnReceivedDispatch(object sender, WebSocketEventEventArgs e)
        {
            try
            {
                switch (e.Type)
                {
                    case "VOICE_SERVER_UPDATE":
                        {
                            var data = e.Payload.ToObject<VoiceServerUpdateEvent>(_serializer);
                            var serverId = data.GuildId;

                            if (serverId == ServerId)
                            {
                                var client = Service.Client;
                                VoiceSocket.Token = data.Token;
                                VoiceSocket.Host = "wss://" + e.Payload.Value<string>("endpoint").Split(':')[0];
                                await VoiceSocket.Connect().ConfigureAwait(false);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error handling {e.Type} event", ex);
            }
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
				VoiceSocket.SendPCMFrames(data, count);
		}

		/// <summary> Clears the PCM buffer. </summary>
		public void Clear()
		{
			//CheckReady(checkVoice: true);

			VoiceSocket.ClearPCMFrames();
		}

		/// <summary> Returns a task that completes once the voice output buffer is empty. </summary>
		public void Wait()
		{
			//CheckReady(checkVoice: true);

			VoiceSocket.WaitForQueue();
		}

        private void SendVoiceUpdate()
        {
            var serverId = VoiceSocket.ServerId;
            if (serverId != null)
            {
                GatewaySocket.SendUpdateVoice(serverId, VoiceSocket.ChannelId,
                    (Service.Config.Mode | AudioMode.Outgoing) == 0,
                    (Service.Config.Mode | AudioMode.Incoming) == 0);
            }
        }
	}
}
