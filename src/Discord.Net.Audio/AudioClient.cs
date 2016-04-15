using Discord.API.Client.GatewaySocket;
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

        public int Id { get; }
        public AudioService Service { get; }
        public AudioServiceConfig Config { get; }
        public RestClient ClientAPI { get; }
        public GatewaySocket GatewaySocket { get; }
        public VoiceSocket VoiceSocket { get; }
        public JsonSerializer Serializer { get; }
        public Stream OutputStream { get; }

        public CancellationToken CancelToken { get; private set; }
        public string SessionId => GatewaySocket.SessionId;

        public ConnectionState State => VoiceSocket.State;
        public Server Server => VoiceSocket.Server;
        public Channel Channel => VoiceSocket.Channel;

        public AudioClient(DiscordClient client, Server server, int id)
        {
            Id = id;
            Service = client.GetService<AudioService>();
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
            _config = client.Config;
            GatewaySocket = client.GatewaySocket;
            GatewaySocket.ReceivedDispatch += (s, e) => OnReceivedEvent(e);
            VoiceSocket = new VoiceSocket(_config, Config, client.Serializer, client.Log.CreateLogger($"Voice #{id}"));
            VoiceSocket.Server = server;
            OutputStream = new OutStream(this);
        }

        public async Task Connect()
        {
            var cancelSource = new CancellationTokenSource();
            CancelToken = cancelSource.Token;
            await _taskManager.Start(new Task[0], cancelSource).ConfigureAwait(false);
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

        public async Task Disconnect()
        {
            await _taskManager.Stop(true).ConfigureAwait(false);
        }
        private async Task Cleanup()
        {
            var oldState = _gatewayState;
            _gatewayState = ConnectionState.Disconnecting;

            var server = VoiceSocket.Server;
            VoiceSocket.Server = null;
            VoiceSocket.Channel = null;
            await Service.RemoveClient(server, this).ConfigureAwait(false);
            SendVoiceUpdate(server.Id, null);

            await VoiceSocket.Disconnect().ConfigureAwait(false);

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
                throw new ArgumentException("This channel is not part of the current server.", nameof(channel));
            if (VoiceSocket.Server == null)
                throw new InvalidOperationException("This client has been closed.");

            SendVoiceUpdate(channel.Server.Id, channel.Id);
            using (await _connectionLock.LockAsync().ConfigureAwait(false))
                await Task.Run(() => VoiceSocket.WaitForConnection(CancelToken)).ConfigureAwait(false);
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

        public void Send(byte[] data, int offset, int count)
        {
            if (data == null) throw new ArgumentException(nameof(data));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (VoiceSocket.Server == null) return; //Has been closed
            if (count == 0) return;

            VoiceSocket.SendPCMFrames(data, offset, count);
        }

        public void Clear()
        {
            if (VoiceSocket.Server == null) return; //Has been closed
            VoiceSocket.ClearPCMFrames();
        }
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
