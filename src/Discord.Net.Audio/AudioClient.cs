using Discord.API.Client.GatewaySocket;
using Discord.Logging;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using Nito.AsyncEx;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
	internal class AudioClient : IAudioClient
    {
        private class OutStream : Stream
        {
            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => true;

            private readonly AudioClient _client;

            internal OutStream(AudioClient client)
            {
                _client = client;
            }

            public override long Length { get { throw new InvalidOperationException(); } }
            public override long Position
            {
                get { throw new InvalidOperationException(); }
                set { throw new InvalidOperationException(); }
            }
            public override void Flush() { throw new InvalidOperationException(); }
            public override long Seek(long offset, SeekOrigin origin) { throw new InvalidOperationException(); }
            public override void SetLength(long value) { throw new InvalidOperationException(); }
            public override int Read(byte[] buffer, int offset, int count) { throw new InvalidOperationException(); }
            public override void Write(byte[] buffer, int offset, int count)
            {
                _client.Send(buffer, offset, count);
            }
        }

        private readonly AsyncLock _connectionLock;
        private readonly JsonSerializer _serializer;
        private CancellationTokenSource _cancelTokenSource;

        internal AudioService Service { get; }
        internal Logger Logger { get; }
        public int Id { get; }
        public GatewaySocket GatewaySocket { get; }
        public VoiceWebSocket VoiceSocket { get; }
        public Stream OutputStream { get; }

        public ConnectionState State => VoiceSocket.State;
        public Server Server => VoiceSocket.Server;
        public Channel Channel => VoiceSocket.Channel;

        public AudioClient(AudioService service, int clientId, Server server, GatewaySocket gatewaySocket, Logger logger)
		{
			Service = service;
			Id = clientId;
            GatewaySocket = gatewaySocket;
            Logger = logger;
            OutputStream = new OutStream(this);

            _connectionLock = new AsyncLock();   
                     
            _serializer = new JsonSerializer();
            _serializer.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            _serializer.Error += (s, e) =>
            {
                e.ErrorContext.Handled = true;
                Logger.Error("Serialization Failed", e.ErrorContext.Error);
            };

            GatewaySocket.ReceivedDispatch += OnReceivedDispatch;

            VoiceSocket = new VoiceWebSocket(service.Client, this, _serializer, logger);
            VoiceSocket.Server = server;

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

        public async Task Join(Channel channel)
		{
			if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (channel.Type != ChannelType.Voice)
                throw new ArgumentException("Channel must be a voice channel.", nameof(channel));            
            if (channel.Server != VoiceSocket.Server)
                throw new ArgumentException("This is channel is not part of the current server.", nameof(channel));
            if (channel == VoiceSocket.Channel) return;
            if (VoiceSocket.Server == null)
                throw new InvalidOperationException("This client has been closed.");

            using (await _connectionLock.LockAsync())
            {
                _cancelTokenSource = new CancellationTokenSource();
                var cancelToken = _cancelTokenSource.Token;
                VoiceSocket.ParentCancelToken = cancelToken;
                VoiceSocket.Channel = channel;

                await Task.Run(() =>
                {
                    SendVoiceUpdate();
                    VoiceSocket.WaitForConnection(cancelToken);
                });
            }
        }
        
        public async Task Disconnect()
        {
            using (await _connectionLock.LockAsync())
            {
                Service.RemoveClient(VoiceSocket.Server, this);
                VoiceSocket.Channel = null;
                SendVoiceUpdate();
                await VoiceSocket.Disconnect();
            }
        }

        private async void OnReceivedDispatch(object sender, WebSocketEventEventArgs e)
        {
            try
            {
                switch (e.Type)
                {
                    case "VOICE_STATE_UPDATE":
                        {
                            var data = e.Payload.ToObject<VoiceStateUpdateEvent>(_serializer);
                            if (data.GuildId == VoiceSocket.Server?.Id && data.UserId == Service.Client.CurrentUser?.Id)
                            {
                                if (data.ChannelId == null)
                                    await Disconnect();
                                else
                                {
                                    var channel = Service.Client.GetChannel(data.ChannelId.Value);
                                    if (channel != null)
                                        VoiceSocket.Channel = channel;
                                    else
                                    {
                                        Logger.Warning("VOICE_STATE_UPDATE referenced an unknown channel, disconnecting.");
                                        await Disconnect();
                                    }
                                }
                            }
                        }
                        break;
                    case "VOICE_SERVER_UPDATE":
                        {
                            var data = e.Payload.ToObject<VoiceServerUpdateEvent>(_serializer);
                            if (data.GuildId == VoiceSocket.Server?.Id)
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
        public void Send(byte[] data, int offset, int count)
		{
			if (data == null) throw new ArgumentException(nameof(data));
			if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (VoiceSocket.Server == null) return; //Has been closed

            if (count != 0)
				VoiceSocket.SendPCMFrames(data, offset, count);
		}

        /// <summary> Clears the PCM buffer. </summary>
        public void Clear()
        {
            if (VoiceSocket.Server == null) return; //Has been closed
            VoiceSocket.ClearPCMFrames();
        }

		/// <summary> Returns a task that completes once the voice output buffer is empty. </summary>
		public void Wait()
        {
            if (VoiceSocket.Server == null) return; //Has been closed
            VoiceSocket.WaitForQueue();
		}

        private void SendVoiceUpdate()
        {
            var serverId = VoiceSocket.Server?.Id;
            if (serverId != null)
            {
                GatewaySocket.SendUpdateVoice(serverId, VoiceSocket.Channel?.Id,
                    (Service.Config.Mode | AudioMode.Outgoing) == 0,
                    (Service.Config.Mode | AudioMode.Incoming) == 0);
            }
        }
	}
}
