#pragma warning disable CS1591
using Discord.API.Rpc;
using Discord.Net.Queue;
using Discord.Net.Rest;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API
{
    public class DiscordRpcApiClient : DiscordRestApiClient, IDisposable
    {
        private abstract class RpcRequest
        {
            public abstract Task SetResultAsync(JToken data, JsonSerializer serializer);
            public abstract Task SetExceptionAsync(JToken data, JsonSerializer serializer);
        }
        private class RpcRequest<T> : RpcRequest
        {
            public TaskCompletionSource<T> Promise { get; set; }

            public RpcRequest(RequestOptions options)
            {
                Promise = new TaskCompletionSource<T>();
                Task.Run(async () =>
                {
                    await Task.Delay(options?.Timeout ?? 15000).ConfigureAwait(false);
                    Promise.TrySetCanceled(); //Doesn't need to be async, we're already in a separate task
                });
            }
            public override Task SetResultAsync(JToken data, JsonSerializer serializer)
            {
                return Promise.TrySetResultAsync(data.ToObject<T>(serializer));
            }
            public override Task SetExceptionAsync(JToken data, JsonSerializer serializer)
            {
                var error = data.ToObject<ErrorEvent>(serializer);
                return Promise.TrySetExceptionAsync(new RpcException(error.Code, error.Message));
            }
        }

        private object _eventLock = new object();
        
        public event Func<string, Task> SentRpcMessage { add { _sentRpcMessageEvent.Add(value); } remove { _sentRpcMessageEvent.Remove(value); } }
        private readonly AsyncEvent<Func<string, Task>> _sentRpcMessageEvent = new AsyncEvent<Func<string, Task>>();

        public event Func<string, Optional<string>, Optional<object>, Task> ReceivedRpcEvent { add { _receivedRpcEvent.Add(value); } remove { _receivedRpcEvent.Remove(value); } }
        private readonly AsyncEvent<Func<string, Optional<string>, Optional<object>, Task>> _receivedRpcEvent = new AsyncEvent<Func<string, Optional<string>, Optional<object>, Task>>();
        public event Func<Exception, Task> Disconnected { add { _disconnectedEvent.Add(value); } remove { _disconnectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();

        private readonly ConcurrentDictionary<Guid, RpcRequest> _requests;
        private readonly RequestQueue _requestQueue;
        private readonly IWebSocketClient _webSocketClient;
        private readonly SemaphoreSlim _connectionLock;
        private readonly string _clientId;
        private CancellationTokenSource _stateCancelToken;
        private string _origin;

        public ConnectionState ConnectionState { get; private set; }

        public DiscordRpcApiClient(string clientId, string origin, RestClientProvider restClientProvider, WebSocketProvider webSocketProvider, JsonSerializer serializer = null, RequestQueue requestQueue = null)
            : base(restClientProvider, serializer, requestQueue)
        {
            _connectionLock = new SemaphoreSlim(1, 1);
            _clientId = clientId;
            _origin = origin;

            _requestQueue = requestQueue ?? new RequestQueue();
            _requests = new ConcurrentDictionary<Guid, RpcRequest>();
            
            _webSocketClient = webSocketProvider();
            //_webSocketClient.SetHeader("user-agent", DiscordConfig.UserAgent); (Causes issues in .Net 4.6+)
            _webSocketClient.SetHeader("origin", _origin);
            _webSocketClient.BinaryMessage += async (data, index, count) =>
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
                        var msg = _serializer.Deserialize<API.Rpc.RpcMessage>(jsonReader);
                        await _receivedRpcEvent.InvokeAsync(msg.Cmd, msg.Event, msg.Data).ConfigureAwait(false);
                        if (msg.Nonce.IsSpecified && msg.Nonce.Value.HasValue)
                            ProcessMessage(msg);
                    }
                }
            };
            _webSocketClient.TextMessage += async text =>
            {
                using (var reader = new StringReader(text))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var msg = _serializer.Deserialize<API.Rpc.RpcMessage>(jsonReader);
                    await _receivedRpcEvent.InvokeAsync(msg.Cmd, msg.Event, msg.Data).ConfigureAwait(false);
                    if (msg.Nonce.IsSpecified && msg.Nonce.Value.HasValue)
                        ProcessMessage(msg);
                }
            };
            _webSocketClient.Closed += async ex =>
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
                    _stateCancelToken?.Dispose();
                    (_webSocketClient as IDisposable)?.Dispose();
                }
                _isDisposed = true;
            }
        }

        public async Task ConnectAsync()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await ConnectInternalAsync().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        internal override async Task ConnectInternalAsync()
        {
            /*if (LoginState != LoginState.LoggedIn)
                throw new InvalidOperationException("Client is not logged in.");*/

            ConnectionState = ConnectionState.Connecting;
            try
            {
                _stateCancelToken = new CancellationTokenSource();
                if (_webSocketClient != null)
                    _webSocketClient.SetCancelToken(_stateCancelToken.Token);

                bool success = false;
                int port;
                string uuid = Guid.NewGuid().ToString();

                for ( port = DiscordRpcConfig.PortRangeStart; port <= DiscordRpcConfig.PortRangeEnd; port++)
                {
                    try
                    {
                        string url = $"wss://{uuid}.discordapp.io:{port}/?v={DiscordRpcConfig.RpcAPIVersion}&client_id={_clientId}";
                        await _webSocketClient.ConnectAsync(url).ConfigureAwait(false);
                        success = true;
                        break;
                    }
                    catch (Exception)
                    {
                    }
                }

                if (!success)
                    throw new Exception("Unable to connect to the RPC server.");

                SetBaseUrl($"https://{uuid}.discordapp.io:{port}/");
                ConnectionState = ConnectionState.Connected;
            }
            catch (Exception)
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
                throw;
            }
        }

        public async Task DisconnectAsync()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        internal override async Task DisconnectInternalAsync()
        {
            if (_webSocketClient == null)
                throw new NotSupportedException("This client is not configured with websocket support.");

            if (ConnectionState == ConnectionState.Disconnected) return;
            ConnectionState = ConnectionState.Disconnecting;
            
            try { _stateCancelToken?.Cancel(false); }
            catch { }

            await _webSocketClient.DisconnectAsync().ConfigureAwait(false);

            ConnectionState = ConnectionState.Disconnected;
        }

        //Core
        public Task<TResponse> SendRpcAsync<TResponse>(string cmd, object payload, GlobalBucket bucket = GlobalBucket.GeneralRpc, 
            Optional<string> evt = default(Optional<string>), bool ignoreState = false, RequestOptions options = null)
            where TResponse : class
            => SendRpcAsyncInternal<TResponse>(cmd, payload, BucketGroup.Global, (int)bucket, 0, evt, ignoreState, options);
        public Task<TResponse> SendRpcAsync<TResponse>(string cmd, object payload, GuildBucket bucket, ulong guildId, 
            Optional<string> evt = default(Optional<string>), bool ignoreState = false, RequestOptions options = null)
            where TResponse : class
            => SendRpcAsyncInternal<TResponse>(cmd, payload, BucketGroup.Guild, (int)bucket, guildId, evt, ignoreState, options);
        private async Task<TResponse> SendRpcAsyncInternal<TResponse>(string cmd, object payload, BucketGroup group, int bucketId, ulong guildId, 
            Optional<string> evt, bool ignoreState, RequestOptions options)
            where TResponse : class
        {
            if (!ignoreState)
                CheckState();

            byte[] bytes = null;
            var guid = Guid.NewGuid();
            payload = new API.Rpc.RpcMessage { Cmd = cmd, Event = evt, Args = payload, Nonce = guid };
            if (payload != null)
            {
                var json = SerializeJson(payload);
                bytes = Encoding.UTF8.GetBytes(json);
            }

            var requestTracker = new RpcRequest<TResponse>(options);
            _requests[guid] = requestTracker;

            await _requestQueue.SendAsync(new WebSocketRequest(_webSocketClient, bytes, true, options), group, bucketId, guildId).ConfigureAwait(false);
            await _sentRpcMessageEvent.InvokeAsync(cmd).ConfigureAwait(false);
            return await requestTracker.Promise.Task.ConfigureAwait(false);
        }

        //Rpc
        public async Task<AuthenticateResponse> SendAuthenticateAsync(RequestOptions options = null)
        {
            var msg = new AuthenticateParams
            {
                AccessToken = _authToken
            };
            return await SendRpcAsync<AuthenticateResponse>("AUTHENTICATE", msg, ignoreState: true, options: options).ConfigureAwait(false);
        }
        public async Task<AuthorizeResponse> SendAuthorizeAsync(string[] scopes, string rpcToken = null, RequestOptions options = null)
        {
            var msg = new AuthorizeParams
            {
                ClientId = _clientId,
                Scopes = scopes,
                RpcToken = rpcToken != null ? rpcToken : Optional.Create<string>()
            };
            if (options == null)
                options = new RequestOptions();
            if (options.Timeout == null)
                options.Timeout = 60000; //This requires manual input on the user's end, lets give them more time
            return await SendRpcAsync<AuthorizeResponse>("AUTHORIZE", msg, ignoreState: true, options: options).ConfigureAwait(false);
        }

        public async Task<GetGuildsResponse> SendGetGuildsAsync(RequestOptions options = null)
        {
            return await SendRpcAsync<GetGuildsResponse>("GET_GUILDS", null, options: options).ConfigureAwait(false);
        }
        public async Task<RpcGuild> SendGetGuildAsync(ulong guildId, RequestOptions options = null)
        {
            var msg = new GetGuildParams
            {
                GuildId = guildId
            };
            return await SendRpcAsync<RpcGuild>("GET_GUILD", msg, options: options).ConfigureAwait(false);
        }
        public async Task<GetChannelsResponse> SendGetChannelsAsync(ulong guildId, RequestOptions options = null)
        {
            var msg = new GetChannelsParams
            {
                GuildId = guildId
            };
            return await SendRpcAsync<GetChannelsResponse>("GET_CHANNELS", msg, options: options).ConfigureAwait(false);
        }
        public async Task<RpcChannel> SendGetChannelAsync(ulong channelId, RequestOptions options = null)
        {
            var msg = new GetChannelParams
            {
                ChannelId = channelId
            };
            return await SendRpcAsync<RpcChannel>("GET_CHANNEL", msg, options: options).ConfigureAwait(false);
        }

        public async Task<SetLocalVolumeResponse> SendSetLocalVolumeAsync(int volume, RequestOptions options = null)
        {
            var msg = new SetLocalVolumeParams
            {
                Volume = volume
            };
            return await SendRpcAsync<SetLocalVolumeResponse>("SET_LOCAL_VOLUME", msg, options: options).ConfigureAwait(false);
        }
        public async Task<RpcChannel> SendSelectVoiceChannelAsync(ulong channelId, RequestOptions options = null)
        {
            var msg = new SelectVoiceChannelParams
            {
                ChannelId = channelId
            };
            return await SendRpcAsync<RpcChannel>("SELECT_VOICE_CHANNEL", msg, options: options).ConfigureAwait(false);
        }

        public async Task<SubscriptionResponse> SendChannelSubscribeAsync(string evt, ulong channelId, RequestOptions options = null)
        {
            var msg = new ChannelSubscriptionParams
            {
                ChannelId = channelId
            };
            return await SendRpcAsync<SubscriptionResponse>("SUBSCRIBE", msg, evt: evt, options: options).ConfigureAwait(false);
        }
        public async Task<SubscriptionResponse> SendChannelUnsubscribeAsync(string evt, ulong channelId, RequestOptions options = null)
        {
            var msg = new ChannelSubscriptionParams
            {
                ChannelId = channelId
            };
            return await SendRpcAsync<SubscriptionResponse>("UNSUBSCRIBE", msg, evt: evt, options: options).ConfigureAwait(false);
        }

        public async Task<SubscriptionResponse> SendGuildSubscribeAsync(string evt, ulong guildId, RequestOptions options = null)
        {
            var msg = new GuildSubscriptionParams
            {
                GuildId = guildId
            };
            return await SendRpcAsync<SubscriptionResponse>("SUBSCRIBE", msg, evt: evt, options: options).ConfigureAwait(false);
        }
        public async Task<SubscriptionResponse> SendGuildUnsubscribeAsync(string evt, ulong guildId, RequestOptions options = null)
        {
            var msg = new GuildSubscriptionParams
            {
                GuildId = guildId
            };
            return await SendRpcAsync<SubscriptionResponse>("UNSUBSCRIBE", msg, evt: evt, options: options).ConfigureAwait(false);
        }

        private bool ProcessMessage(API.Rpc.RpcMessage msg)
        {
            RpcRequest requestTracker;
            if (_requests.TryGetValue(msg.Nonce.Value.Value, out requestTracker))
            {
                if (msg.Event.GetValueOrDefault("") == "ERROR")
                {
                    var _ = requestTracker.SetExceptionAsync(msg.Data.GetValueOrDefault() as JToken, _serializer);
                }
                else
                {
                    var _ = requestTracker.SetResultAsync(msg.Data.GetValueOrDefault() as JToken, _serializer);
                }
                return true;
            }
            else
                return false;
        }
    }
}
