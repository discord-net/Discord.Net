#pragma warning disable CS1591
using Discord.API.Gateway;
using Discord.API.Rest;
using Discord.Net.Queue;
using Discord.Net.Rest;
using Discord.Net.WebSockets;
using Discord.Serialization;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class DiscordSocketApiClient : DiscordRestApiClient
    {
        public event Func<GatewayOpCode, Task> SentGatewayMessage { add { _sentGatewayMessageEvent.Add(value); } remove { _sentGatewayMessageEvent.Remove(value); } }
        private readonly AsyncEvent<Func<GatewayOpCode, Task>> _sentGatewayMessageEvent = new AsyncEvent<Func<GatewayOpCode, Task>>();
        public event Func<GatewayOpCode, int?, string, ReadOnlyBuffer<byte>, Task> ReceivedGatewayEvent { add { _receivedGatewayEvent.Add(value); } remove { _receivedGatewayEvent.Remove(value); } }
        private readonly AsyncEvent<Func<GatewayOpCode, int?, string, ReadOnlyBuffer<byte>, Task>> _receivedGatewayEvent = new AsyncEvent<Func<GatewayOpCode, int?, string, ReadOnlyBuffer<byte>, Task>>();

        public event Func<Exception, Task> Disconnected { add { _disconnectedEvent.Add(value); } remove { _disconnectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();
        
        private readonly MemoryStream _decompressionStream;
        private CancellationTokenSource _connectCancelToken;
        private string _gatewayUrl;
        private bool _isExplicitUrl;
        
        internal IWebSocketClient WebSocketClient { get; }

        public ConnectionState ConnectionState { get; private set; }

        public DiscordSocketApiClient(RestClientProvider restClientProvider, WebSocketProvider webSocketProvider, string userAgent, ScopedSerializer serializer,
            string url = null, RetryMode defaultRetryMode = RetryMode.AlwaysRetry)
            : base(restClientProvider, userAgent, serializer, defaultRetryMode)
        {
            _gatewayUrl = url;
            if (url != null)
                _isExplicitUrl = true;

            _decompressionStream = new MemoryStream(10 * 1024); //10 KB

            WebSocketClient = webSocketProvider();
            //WebSocketClient.SetHeader("user-agent", DiscordConfig.UserAgent); (Causes issues in .NET Framework 4.6+)
            WebSocketClient.Message += async (data, isText) =>
            {
                if (!isText)
                {
                    using (var compressed = new MemoryStream(data.ToArray(), 2, data.Length - 2))
                    {
                        _decompressionStream.Position = 0;
                        using (var zlib = new DeflateStream(compressed, CompressionMode.Decompress))
                            zlib.CopyTo(_decompressionStream);
                        _decompressionStream.SetLength(_decompressionStream.Position);

                        _decompressionStream.Position = 0;
                        var msg = _serializer.ReadJson<SocketFrame>(_decompressionStream.ToReadOnlyBuffer());
                        if (msg != null)
                            await _receivedGatewayEvent.InvokeAsync((GatewayOpCode)msg.Operation, msg.Sequence, msg.Type, msg.Payload).ConfigureAwait(false);
                    }
                }
                else
                {
                    var msg = _serializer.ReadJson<SocketFrame>(data);
                    if (msg != null)
                        await _receivedGatewayEvent.InvokeAsync((GatewayOpCode)msg.Operation, msg.Sequence, msg.Type, msg.Payload).ConfigureAwait(false);
                }
            };
            WebSocketClient.Closed += async ex =>
            {
                await DisconnectAsync().ConfigureAwait(false);
                await _disconnectedEvent.InvokeAsync(ex).ConfigureAwait(false);
            };
        }
        internal override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _connectCancelToken?.Dispose();
                    (WebSocketClient as IDisposable)?.Dispose();
                }
                _isDisposed = true;
            }
        }

        public async Task ConnectAsync()
        {
            await _stateLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await ConnectInternalAsync().ConfigureAwait(false);
            }
            finally { _stateLock.Release(); }
        }
        internal override async Task ConnectInternalAsync()
        {
            if (LoginState != LoginState.LoggedIn)
                throw new InvalidOperationException("You must log in before connecting.");
            if (WebSocketClient == null)
                throw new NotSupportedException("This client is not configured with websocket support.");

            ConnectionState = ConnectionState.Connecting;
            try
            {
                _connectCancelToken = new CancellationTokenSource();
                if (WebSocketClient != null)
                    WebSocketClient.SetCancelToken(_connectCancelToken.Token);

                if (!_isExplicitUrl)
                {
                    var gatewayResponse = await GetGatewayAsync().ConfigureAwait(false);
                    _gatewayUrl = $"{gatewayResponse.Url}?v={DiscordConfig.APIVersion}&encoding={DiscordSocketConfig.GatewayEncoding}";
                }
                await WebSocketClient.ConnectAsync(_gatewayUrl).ConfigureAwait(false);

                ConnectionState = ConnectionState.Connected;
            }
            catch
            {
                if (!_isExplicitUrl)
                    _gatewayUrl = null; //Uncache in case the gateway url changed
                await DisconnectInternalAsync().ConfigureAwait(false);
                throw;
            }
        }

        public async Task DisconnectAsync()
        {
            await _stateLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
            }
            finally { _stateLock.Release(); }
        }
        public async Task DisconnectAsync(Exception ex)
        {
            await _stateLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
            }
            finally { _stateLock.Release(); }
        }
        internal override async Task DisconnectInternalAsync()
        {
            if (WebSocketClient == null)
                throw new NotSupportedException("This client is not configured with websocket support.");

            if (ConnectionState == ConnectionState.Disconnected) return;
            ConnectionState = ConnectionState.Disconnecting;

            try { _connectCancelToken?.Cancel(false); }
            catch { }

            await WebSocketClient.DisconnectAsync().ConfigureAwait(false);

            ConnectionState = ConnectionState.Disconnected;
        }

        //Core
        public Task SendGatewayAsync(GatewayOpCode opCode, object payload, RequestOptions options = null)
            => SendGatewayInternalAsync(opCode, payload, options);
        private async Task SendGatewayInternalAsync(GatewayOpCode opCode, object payload, RequestOptions options)
        {
            CheckState();

            if (_formatters.TryDequeue(out var data1))
                data1 = new ArrayFormatter(128, SymbolTable.InvariantUtf8);
            if (_formatters.TryDequeue(out var data2))
                data2 = new ArrayFormatter(128, SymbolTable.InvariantUtf8);
            try
            {
                payload = new SocketFrame { Operation = (int)opCode, Payload = SerializeJson(data1, payload) };
                await RequestQueue.SendAsync(new WebSocketRequest(WebSocketClient, null, SerializeJson(data2, payload), true, options)).ConfigureAwait(false);
                await _sentGatewayMessageEvent.InvokeAsync(opCode).ConfigureAwait(false);
            }
            finally
            {
                _formatters.Enqueue(data1);
                _formatters.Enqueue(data2);
            }            
        }

        //Gateway
        public async Task<GetGatewayResponse> GetGatewayAsync(RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            return await SendAsync<GetGatewayResponse>("GET", () => "gateway", new BucketIds(), options: options).ConfigureAwait(false);
        }
        public async Task<GetBotGatewayResponse> GetBotGatewayAsync(RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            return await SendAsync<GetBotGatewayResponse>("GET", () => "gateway/bot", new BucketIds(), options: options).ConfigureAwait(false);
        }
        public async Task SendIdentifyAsync(int largeThreshold = 100, bool useCompression = true, int shardID = 0, int totalShards = 1, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            var props = new Dictionary<string, string>
            {
                ["$device"] = "Discord.Net"
            };
            var msg = new IdentifyParams()
            {
                Token = AuthToken,
                Properties = props,
                LargeThreshold = largeThreshold,
                UseCompression = useCompression,
            };
            if (totalShards > 1)
                msg.ShardingParams = new int[] { shardID, totalShards };

            await SendGatewayAsync(GatewayOpCode.Identify, msg, options: options).ConfigureAwait(false);
        }
        public async Task SendResumeAsync(string sessionId, int lastSeq, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            var msg = new ResumeParams()
            {
                Token = AuthToken,
                SessionId = sessionId,
                Sequence = lastSeq
            };
            await SendGatewayAsync(GatewayOpCode.Resume, msg, options: options).ConfigureAwait(false);
        }
        public async Task SendHeartbeatAsync(int lastSeq, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            await SendGatewayAsync(GatewayOpCode.Heartbeat, lastSeq, options: options).ConfigureAwait(false);
        }
        public async Task SendStatusUpdateAsync(UserStatus status, bool isAFK, long? since, Game game, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            var args = new StatusUpdateParams
            {
                Status = status,
                IdleSince = since,
                IsAFK = isAFK,
                Game = game
            };
            await SendGatewayAsync(GatewayOpCode.StatusUpdate, args, options: options).ConfigureAwait(false);
        }
        public async Task SendRequestMembersAsync(IEnumerable<ulong> guildIds, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            await SendGatewayAsync(GatewayOpCode.RequestGuildMembers, new RequestMembersParams { GuildIds = guildIds, Query = "", Limit = 0 }, options: options).ConfigureAwait(false);
        }
        public async Task SendVoiceStateUpdateAsync(ulong guildId, ulong? channelId, bool selfDeaf, bool selfMute, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            var payload = new VoiceStateUpdateParams
            {
                GuildId = guildId,
                ChannelId = channelId,
                SelfDeaf = selfDeaf,
                SelfMute = selfMute
            };
            await SendGatewayAsync(GatewayOpCode.VoiceStateUpdate, payload, options: options).ConfigureAwait(false);
        }
        public async Task SendGuildSyncAsync(IEnumerable<ulong> guildIds, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            await SendGatewayAsync(GatewayOpCode.GuildSync, guildIds, options: options).ConfigureAwait(false);
        }
    }
}
