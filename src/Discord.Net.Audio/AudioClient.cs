using Discord.API.Client.GatewaySocket;
using Discord.API.Client.Rest;
using Discord.Logging;
using Discord.Net.Rest;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using Nito.AsyncEx;
using System;
using System.Diagnostics;
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

        private readonly DiscordConfig _config;
        private readonly AsyncLock _connectionLock;
        private readonly TaskManager _taskManager;
        private ConnectionState _gatewayState;

        internal Logger Logger { get; }

        /// <summary> Gets the unique identifier for this client. </summary>
        public int Id { get; }
        /// <summary> Gets the service managing this client. </summary>
        public AudioService Service { get; }
        /// <summary> Gets the configuration object used to make this client. </summary>
        public AudioServiceConfig Config { get; }
        /// <summary> Gets the internal RestClient for the Client API endpoint. </summary>
        public RestClient ClientAPI { get; }
        /// <summary> Gets the internal WebSocket for the Gateway event stream. </summary>
        public GatewaySocket GatewaySocket { get; }
        /// <summary> Gets the internal WebSocket for the Voice control stream. </summary>
        public VoiceSocket VoiceSocket { get; }
        /// <summary> Gets the JSON serializer used by this client. </summary>
        public JsonSerializer Serializer { get; }
        /// <summary>  </summary>
        public Stream OutputStream { get; }

        /// <summary> Gets a cancellation token that triggers when the client is manually disconnected. </summary>
        public CancellationToken CancelToken { get; private set; }
        /// <summary> Gets the session id for the current connection. </summary>
        public string SessionId { get; private set; }

        /// <summary> Gets the current state of this client. </summary>
        public ConnectionState State => VoiceSocket.State;
        /// <summary> Gets the server this client is bound to. </summary>
        public Server Server => VoiceSocket.Server;
        /// <summary> Gets the channel  </summary>
        public Channel Channel => VoiceSocket.Channel;

        public AudioClient(DiscordClient client, Server server, int id)
		{
            Id = id;
            _config = client.Config;
            Service = client.Audio();
            Config = Service.Config;
            Serializer = client.Serializer;
            _gatewayState = (int)ConnectionState.Disconnected;

            //Logging
            Logger = client.Log.CreateLogger($"AudioClient #{id}");

            //Async
            _taskManager = new TaskManager(Cleanup, false);
            _connectionLock = new AsyncLock();
            CancelToken = new CancellationToken(true);

            //Networking
            if (Config.EnableMultiserver)
            {
                ClientAPI = new RestClient(_config, DiscordConfig.ClientAPIUrl, client.Log.CreateLogger($"ClientAPI #{id}"));
                GatewaySocket = new GatewaySocket(_config, client.Serializer, client.Log.CreateLogger($"Gateway #{id}"));
                GatewaySocket.Connected += (s, e) =>
                {
                    if (_gatewayState == ConnectionState.Connecting)
                        EndGatewayConnect();
                };
            }
            else
                GatewaySocket = client.GatewaySocket;
            GatewaySocket.ReceivedDispatch += (s, e) => OnReceivedEvent(e);
            VoiceSocket = new VoiceSocket(_config, Config, client.Serializer, client.Log.CreateLogger($"Voice #{id}"));
            VoiceSocket.Server = server;
            OutputStream = new OutStream(this);
        }

        /// <summary> Connects to the Discord server with the provided token. </summary>
        public async Task Connect()
        {
            if (Config.EnableMultiserver)
                await BeginGatewayConnect().ConfigureAwait(false);
            else
            {
                var cancelSource = new CancellationTokenSource();
                CancelToken = cancelSource.Token;
                await _taskManager.Start(new Task[0], cancelSource).ConfigureAwait(false);
            }
        }
        private async Task BeginGatewayConnect()
        {
            try
            {
                using (await _connectionLock.LockAsync().ConfigureAwait(false))
                {
                    await Disconnect().ConfigureAwait(false);
                    _taskManager.ClearException();

                    ClientAPI.Token = Service.Client.ClientAPI.Token;

                    Stopwatch stopwatch = null;
                    if (_config.LogLevel >= LogSeverity.Verbose)
                        stopwatch = Stopwatch.StartNew();
                    _gatewayState = ConnectionState.Connecting;

                    var cancelSource = new CancellationTokenSource();
                    CancelToken = cancelSource.Token;
                    ClientAPI.CancelToken = CancelToken;
                    
                    await GatewaySocket.Connect(ClientAPI, CancelToken).ConfigureAwait(false);

                    await _taskManager.Start(new Task[0], cancelSource).ConfigureAwait(false);
                    GatewaySocket.WaitForConnection(CancelToken);

                    if (_config.LogLevel >= LogSeverity.Verbose)
                    {
                        stopwatch.Stop();
                        double seconds = Math.Round(stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerSecond, 2);
                        Logger.Verbose($"Connection took {seconds} sec");
                    }
                }
            }
            catch (Exception ex)
            {
                await _taskManager.SignalError(ex).ConfigureAwait(false);
                throw;
            }
        }
        private void EndGatewayConnect()
        {
            _gatewayState = ConnectionState.Connected;
        }
        
        /// <summary> Disconnects from the Discord server, canceling any pending requests. </summary>
        public async Task Disconnect()
        {
            await _taskManager.Stop(true).ConfigureAwait(false);
            if (Config.EnableMultiserver)
                ClientAPI.Token = null;
        }
        private async Task Cleanup()
        {
            var oldState = _gatewayState;
            _gatewayState = ConnectionState.Disconnecting;

            if (Config.EnableMultiserver)
            {
                if (oldState == ConnectionState.Connected)
                {
                    try { await ClientAPI.Send(new LogoutRequest()).ConfigureAwait(false); }
                    catch (OperationCanceledException) { }
                }

                await GatewaySocket.Disconnect().ConfigureAwait(false);
                ClientAPI.Token = null;
            }

            var server = VoiceSocket.Server;
            VoiceSocket.Server = null;
            VoiceSocket.Channel = null;
            if (Config.EnableMultiserver)
                await Service.RemoveClient(server, this).ConfigureAwait(false);
            SendVoiceUpdate(server.Id, null);

            await VoiceSocket.Disconnect().ConfigureAwait(false);
            if (Config.EnableMultiserver)
                await GatewaySocket.Disconnect().ConfigureAwait(false);

            _gatewayState = (int)ConnectionState.Disconnected;
        }

        public async Task Join(Channel channel)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (channel.Type != ChannelType.Voice)
                throw new ArgumentException("Channel must be a voice channel.", nameof(channel));
            if (channel == VoiceSocket.Channel) return;
            var server = channel.Server;
            if (server != VoiceSocket.Server)
                throw new ArgumentException("This is channel is not part of the current server.", nameof(channel));
            if (VoiceSocket.Server == null)
                throw new InvalidOperationException("This client has been closed.");

            SendVoiceUpdate(channel.Server.Id, channel.Id);
            using (await _connectionLock.LockAsync().ConfigureAwait(false))
                await Task.Run(() => VoiceSocket.WaitForConnection(CancelToken));
        }

        private async void OnReceivedEvent(WebSocketEventEventArgs e)
        {
            try
            {
                switch (e.Type)
                {
                    case "VOICE_STATE_UPDATE":
                        {
                            var data = e.Payload.ToObject<VoiceStateUpdateEvent>(Serializer);
                            if (data.GuildId == VoiceSocket.Server?.Id && data.UserId == Service.Client.CurrentUser?.Id)
                            {
                                if (data.ChannelId == null)
                                    await Disconnect().ConfigureAwait(false);
                                else
                                {
                                    var channel = Service.Client.GetChannel(data.ChannelId.Value);
                                    if (channel != null)
                                        VoiceSocket.Channel = channel;
                                    else
                                    {
                                        Logger.Warning("VOICE_STATE_UPDATE referenced an unknown channel, disconnecting.");
                                        await Disconnect().ConfigureAwait(false);
                                    }
                                }
                            }
                        }
                        break;
                    case "VOICE_SERVER_UPDATE":
                        {
                            var data = e.Payload.ToObject<VoiceServerUpdateEvent>(Serializer);
                            if (data.GuildId == VoiceSocket.Server?.Id)
                            {
                                var client = Service.Client;
                                var id = client.CurrentUser?.Id;
                                if (id != null)
                                {
                                    var host = "wss://" + e.Payload.Value<string>("endpoint").Split(':')[0];
                                    await VoiceSocket.Connect(host, data.Token, id.Value, GatewaySocket.SessionId, CancelToken).ConfigureAwait(false);
                                }
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
            if (count == 0) return;

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

        public void SendVoiceUpdate(ulong? serverId, ulong? channelId)
        {
            GatewaySocket.SendUpdateVoice(serverId, channelId,
                (Service.Config.Mode | AudioMode.Outgoing) == 0,
                (Service.Config.Mode | AudioMode.Incoming) == 0);
        }
	}
}
