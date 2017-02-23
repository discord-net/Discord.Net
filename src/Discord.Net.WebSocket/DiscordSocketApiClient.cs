#pragma warning disable CS1591
using Discord.API.Gateway;
using Discord.API.Rest;
using Discord.Net.Queue;
using Discord.Net.Rest;
using Discord.Net.WebSockets;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class DiscordSocketApiClient : DiscordRestApiClient
    {
        public event Func<GatewayOpCode, Task> SentGatewayMessage { add { _sentGatewayMessageEvent.Add(value); } remove { _sentGatewayMessageEvent.Remove(value); } }
        private readonly AsyncEvent<Func<GatewayOpCode, Task>> _sentGatewayMessageEvent = new AsyncEvent<Func<GatewayOpCode, Task>>();
        public event Func<GatewayOpCode, int?, string, object, Task> ReceivedGatewayEvent { add { _receivedGatewayEvent.Add(value); } remove { _receivedGatewayEvent.Remove(value); } }
        private readonly AsyncEvent<Func<GatewayOpCode, int?, string, object, Task>> _receivedGatewayEvent = new AsyncEvent<Func<GatewayOpCode, int?, string, object, Task>>();

        public event Func<Exception, Task> Disconnected { add { _disconnectedEvent.Add(value); } remove { _disconnectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();

        private CancellationTokenSource _connectCancelToken;
        private string _gatewayUrl;
        private bool _isExplicitUrl;
        
        internal IWebSocketClient WebSocketClient { get; }

        public ConnectionState ConnectionState { get; private set; }

        public DiscordSocketApiClient(RestClientProvider restClientProvider, WebSocketProvider webSocketProvider, string userAgent,
            string url = null, RetryMode defaultRetryMode = RetryMode.AlwaysRetry, JsonSerializer serializer = null)
            : base(restClientProvider, userAgent, defaultRetryMode, serializer)
        {
            _gatewayUrl = url;
            if (url != null)
                _isExplicitUrl = true;
            WebSocketClient = webSocketProvider();
            //WebSocketClient.SetHeader("user-agent", DiscordConfig.UserAgent); (Causes issues in .NET Framework 4.6+)
            WebSocketClient.BinaryMessage += async (data, index, count) =>
            {
                using (var compressed = new MemoryStream(data, index + 2, count - 2))
                using (var decompressed = new MemoryStream())
                {
                    using (var zlib = new DeflateStream(compressed, CompressionMode.Decompress))
                        zlib.CopyTo(decompressed);
                    decompressed.Position = 0;
                    using (var reader = new StreamReader(decompressed))
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        var msg = _serializer.Deserialize<SocketFrame>(jsonReader);
                        if (msg != null)
                            await _receivedGatewayEvent.InvokeAsync((GatewayOpCode)msg.Operation, msg.Sequence, msg.Type, msg.Payload).ConfigureAwait(false);
                    }
                }
            };
            WebSocketClient.TextMessage += async text =>
            {
                using (var reader = new StringReader(text))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var msg = _serializer.Deserialize<SocketFrame>(jsonReader);
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

            //TODO: Add ETF
            byte[] bytes = null;
            payload = new SocketFrame { Operation = (int)opCode, Payload = payload };
            if (payload != null)
                bytes = Encoding.UTF8.GetBytes(SerializeJson(payload));
            await RequestQueue.SendAsync(new WebSocketRequest(WebSocketClient, null, bytes, true, options)).ConfigureAwait(false);
            await _sentGatewayMessageEvent.InvokeAsync(opCode).ConfigureAwait(false);
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
