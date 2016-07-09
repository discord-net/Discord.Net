using Discord.API.Gateway;
using Discord.API.Rest;
using Discord.Net;
using Discord.Net.Converters;
using Discord.Net.Queue;
using Discord.Net.Rest;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API
{
    public class DiscordApiClient : IDisposable
    {
        private object _eventLock = new object();

        public event Func<string, string, double, Task> SentRequest { add { _sentRequestEvent.Add(value); } remove { _sentRequestEvent.Remove(value); } }
        private readonly AsyncEvent<Func<string, string, double, Task>> _sentRequestEvent = new AsyncEvent<Func<string, string, double, Task>>();
        public event Func<GatewayOpCode, Task> SentGatewayMessage { add { _sentGatewayMessageEvent.Add(value); } remove { _sentGatewayMessageEvent.Remove(value); } }
        private readonly AsyncEvent<Func<GatewayOpCode, Task>> _sentGatewayMessageEvent = new AsyncEvent<Func<GatewayOpCode, Task>>();

        public event Func<GatewayOpCode, int?, string, object, Task> ReceivedGatewayEvent { add { _receivedGatewayEvent.Add(value); } remove { _receivedGatewayEvent.Remove(value); } }
        private readonly AsyncEvent<Func<GatewayOpCode, int?, string, object, Task>> _receivedGatewayEvent = new AsyncEvent<Func<GatewayOpCode, int?, string, object, Task>>();
        public event Func<Exception, Task> Disconnected { add { _disconnectedEvent.Add(value); } remove { _disconnectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();

        private readonly RequestQueue _requestQueue;
        private readonly JsonSerializer _serializer;
        private readonly IRestClient _restClient;
        private readonly IWebSocketClient _gatewayClient;
        private readonly SemaphoreSlim _connectionLock;
        private CancellationTokenSource _loginCancelToken, _connectCancelToken;
        private string _authToken;
        private string _gatewayUrl;
        private bool _isDisposed;

        public LoginState LoginState { get; private set; }
        public ConnectionState ConnectionState { get; private set; }
        public TokenType AuthTokenType { get; private set; }

        public DiscordApiClient(RestClientProvider restClientProvider, WebSocketProvider webSocketProvider = null, JsonSerializer serializer = null, RequestQueue requestQueue = null)
        {
            _connectionLock = new SemaphoreSlim(1, 1);

            _requestQueue = requestQueue ?? new RequestQueue();

            _restClient = restClientProvider(DiscordConfig.ClientAPIUrl);
            _restClient.SetHeader("accept", "*/*");
            _restClient.SetHeader("user-agent", DiscordConfig.UserAgent);
            if (webSocketProvider != null)
            {
                _gatewayClient = webSocketProvider();
                //_gatewayClient.SetHeader("user-agent", DiscordConfig.UserAgent); (Causes issues in .Net 4.6+)
                _gatewayClient.BinaryMessage += async (data, index, count) =>
                {
                    using (var compressed = new MemoryStream(data, index + 2, count - 2))
                    using (var decompressed = new MemoryStream())
                    {
                        using (var zlib = new DeflateStream(compressed, CompressionMode.Decompress))
                            zlib.CopyTo(decompressed);
                        decompressed.Position = 0;
                        using (var reader = new StreamReader(decompressed))
                        {
                            var msg = JsonConvert.DeserializeObject<WebSocketMessage>(reader.ReadToEnd());
                            await _receivedGatewayEvent.InvokeAsync((GatewayOpCode)msg.Operation, msg.Sequence, msg.Type, msg.Payload).ConfigureAwait(false);
                        }
                    }
                };
                _gatewayClient.TextMessage += async text =>
                {
                    var msg = JsonConvert.DeserializeObject<WebSocketMessage>(text);
                    await _receivedGatewayEvent.InvokeAsync((GatewayOpCode)msg.Operation, msg.Sequence, msg.Type, msg.Payload).ConfigureAwait(false);
                };
                _gatewayClient.Closed += async ex =>
                {
                    await DisconnectAsync().ConfigureAwait(false);
                    await _disconnectedEvent.InvokeAsync(ex).ConfigureAwait(false);
                };
            }

            _serializer = serializer ?? new JsonSerializer { ContractResolver = new DiscordContractResolver() };
        }
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _loginCancelToken?.Dispose();
                    _connectCancelToken?.Dispose();
                    (_restClient as IDisposable)?.Dispose();
                    (_gatewayClient as IDisposable)?.Dispose();
                }
                _isDisposed = true;
            }
        }
        public void Dispose() => Dispose(true);
        
        public async Task LoginAsync(TokenType tokenType, string token, RequestOptions options = null)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LoginInternalAsync(tokenType, token, options).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task LoginInternalAsync(TokenType tokenType, string token, RequestOptions options = null)
        {
            if (LoginState != LoginState.LoggedOut)
                await LogoutInternalAsync().ConfigureAwait(false);
            LoginState = LoginState.LoggingIn;
            
            try
            {
                _loginCancelToken = new CancellationTokenSource();

                AuthTokenType = TokenType.User;
                _authToken = null;
                _restClient.SetHeader("authorization", null);
                await _requestQueue.SetCancelTokenAsync(_loginCancelToken.Token).ConfigureAwait(false);
                _restClient.SetCancelToken(_loginCancelToken.Token);

                AuthTokenType = tokenType;
                _authToken = token;
                switch (tokenType)
                {
                    case TokenType.Bot:
                        token = $"Bot {token}";
                        break;
                    case TokenType.Bearer:
                        token = $"Bearer {token}";
                        break;
                    case TokenType.User:
                        break;
                    default:
                        throw new ArgumentException("Unknown oauth token type", nameof(tokenType));
                }
                _restClient.SetHeader("authorization", token);

                LoginState = LoginState.LoggedIn;
            }
            catch (Exception)
            {
                await LogoutInternalAsync().ConfigureAwait(false);
                throw;
            }
        }

        public async Task LogoutAsync()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LogoutInternalAsync().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task LogoutInternalAsync()
        {
            //An exception here will lock the client into the unusable LoggingOut state, but that's probably fine since our client is in an undefined state too.
            if (LoginState == LoginState.LoggedOut) return;
            LoginState = LoginState.LoggingOut;
            
            try { _loginCancelToken?.Cancel(false); }
            catch { }

            await DisconnectInternalAsync().ConfigureAwait(false);
            await _requestQueue.ClearAsync().ConfigureAwait(false);

            await _requestQueue.SetCancelTokenAsync(CancellationToken.None).ConfigureAwait(false);
            _restClient.SetCancelToken(CancellationToken.None);

            LoginState = LoginState.LoggedOut;
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
        private async Task ConnectInternalAsync()
        {
            if (LoginState != LoginState.LoggedIn)
                throw new InvalidOperationException("You must log in before connecting.");
            if (_gatewayClient == null)
                throw new NotSupportedException("This client is not configured with websocket support.");

            ConnectionState = ConnectionState.Connecting;
            try
            {
                _connectCancelToken = new CancellationTokenSource();
                if (_gatewayClient != null)
                    _gatewayClient.SetCancelToken(_connectCancelToken.Token);

                if (_gatewayUrl == null)
                {
                    var gatewayResponse = await GetGatewayAsync().ConfigureAwait(false);
                    _gatewayUrl = $"{gatewayResponse.Url}?v={DiscordConfig.GatewayAPIVersion}&encoding={DiscordConfig.GatewayEncoding}";
                }
                await _gatewayClient.ConnectAsync(_gatewayUrl).ConfigureAwait(false);

                ConnectionState = ConnectionState.Connected;
            }
            catch (Exception)
            {
                _gatewayUrl = null; //Uncache  in case the gateway url changed
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
        private async Task DisconnectInternalAsync()
        {
            if (_gatewayClient == null)
                throw new NotSupportedException("This client is not configured with websocket support.");

            if (ConnectionState == ConnectionState.Disconnected) return;
            ConnectionState = ConnectionState.Disconnecting;
            
            try { _connectCancelToken?.Cancel(false); }
            catch { }

            await _gatewayClient.DisconnectAsync().ConfigureAwait(false);

            ConnectionState = ConnectionState.Disconnected;
        }

        //Core
        public Task SendAsync(string method, string endpoint, 
            GlobalBucket bucket = GlobalBucket.GeneralRest, RequestOptions options = null)
            => SendInternalAsync(method, endpoint, null, true, bucket, options);
        public Task SendAsync(string method, string endpoint, object payload, 
            GlobalBucket bucket = GlobalBucket.GeneralRest, RequestOptions options = null)
            => SendInternalAsync(method, endpoint, payload, true, bucket, options);
        public Task SendAsync(string method, string endpoint, Stream file, IReadOnlyDictionary<string, string> multipartArgs, 
            GlobalBucket bucket = GlobalBucket.GeneralRest, RequestOptions options = null)
            => SendInternalAsync(method, endpoint, multipartArgs, true, bucket, options);
        public async Task<TResponse> SendAsync<TResponse>(string method, string endpoint,
            GlobalBucket bucket = GlobalBucket.GeneralRest, RequestOptions options = null)
            where TResponse : class
            => DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, null, false, bucket, options).ConfigureAwait(false));
        public async Task<TResponse> SendAsync<TResponse>(string method, string endpoint, object payload, GlobalBucket bucket = 
            GlobalBucket.GeneralRest, RequestOptions options = null)
            where TResponse : class
            => DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, payload, false, bucket, options).ConfigureAwait(false));
        public async Task<TResponse> SendAsync<TResponse>(string method, string endpoint, Stream file, IReadOnlyDictionary<string, string> multipartArgs, 
            GlobalBucket bucket = GlobalBucket.GeneralRest, RequestOptions options = null)
            where TResponse : class
            => DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, multipartArgs, false, bucket, options).ConfigureAwait(false));

        public Task SendAsync(string method, string endpoint, 
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            => SendInternalAsync(method, endpoint, null, true, bucket, guildId, options);
        public Task SendAsync(string method, string endpoint, object payload, 
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            => SendInternalAsync(method, endpoint, payload, true, bucket, guildId, options);
        public Task SendAsync(string method, string endpoint, Stream file, IReadOnlyDictionary<string, string> multipartArgs, 
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            => SendInternalAsync(method, endpoint, multipartArgs, true, bucket, guildId, options);
        public async Task<TResponse> SendAsync<TResponse>(string method, string endpoint, 
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            where TResponse : class
            => DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, null, false, bucket, guildId, options).ConfigureAwait(false));
        public async Task<TResponse> SendAsync<TResponse>(string method, string endpoint, object payload, 
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            where TResponse : class
            => DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, payload, false, bucket, guildId, options).ConfigureAwait(false));
        public async Task<TResponse> SendAsync<TResponse>(string method, string endpoint, Stream file, IReadOnlyDictionary<string, string> multipartArgs, 
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            where TResponse : class
            => DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, multipartArgs, false, bucket, guildId, options).ConfigureAwait(false));
        
        private Task<Stream> SendInternalAsync(string method, string endpoint, object payload, bool headerOnly, 
            GlobalBucket bucket, RequestOptions options)
            => SendInternalAsync(method, endpoint, payload, headerOnly, BucketGroup.Global, (int)bucket, 0, options);
        private Task<Stream> SendInternalAsync(string method, string endpoint, object payload, bool headerOnly, 
            GuildBucket bucket, ulong guildId, RequestOptions options)
            => SendInternalAsync(method, endpoint, payload, headerOnly, BucketGroup.Guild, (int)bucket, guildId, options);
        private Task<Stream> SendInternalAsync(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs, bool headerOnly, 
            GlobalBucket bucket, RequestOptions options)
            => SendInternalAsync(method, endpoint, multipartArgs, headerOnly, BucketGroup.Global, (int)bucket, 0, options);
        private Task<Stream> SendInternalAsync(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs, bool headerOnly, 
            GuildBucket bucket, ulong guildId, RequestOptions options)
            => SendInternalAsync(method, endpoint, multipartArgs, headerOnly, BucketGroup.Guild, (int)bucket, guildId, options);

        private async Task<Stream> SendInternalAsync(string method, string endpoint, object payload, bool headerOnly, 
            BucketGroup group, int bucketId, ulong guildId, RequestOptions options = null)
        {
            var stopwatch = Stopwatch.StartNew();
            string json = null;
            if (payload != null)
                json = SerializeJson(payload);
            var responseStream = await _requestQueue.SendAsync(new RestRequest(_restClient, method, endpoint, json, headerOnly, options), group, bucketId, guildId).ConfigureAwait(false);
            stopwatch.Stop();

            double milliseconds = ToMilliseconds(stopwatch);
            await _sentRequestEvent.InvokeAsync(method, endpoint, milliseconds).ConfigureAwait(false);

            return responseStream;
        }
        private async Task<Stream> SendInternalAsync(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs, bool headerOnly, 
            BucketGroup group, int bucketId, ulong guildId, RequestOptions options = null)
        {
            var stopwatch = Stopwatch.StartNew();
            var responseStream = await _requestQueue.SendAsync(new RestRequest(_restClient, method, endpoint, multipartArgs, headerOnly, options), group, bucketId, guildId).ConfigureAwait(false);
            int bytes = headerOnly ? 0 : (int)responseStream.Length;
            stopwatch.Stop();

            double milliseconds = ToMilliseconds(stopwatch);
            await _sentRequestEvent.InvokeAsync(method, endpoint, milliseconds).ConfigureAwait(false);

            return responseStream;
        }

        public Task SendGatewayAsync(GatewayOpCode opCode, object payload, 
            GlobalBucket bucket = GlobalBucket.GeneralGateway, RequestOptions options = null)
            => SendGatewayAsync(opCode, payload, BucketGroup.Global, (int)bucket, 0, options);
        public Task SendGatewayAsync(GatewayOpCode opCode, object payload, 
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            => SendGatewayAsync(opCode, payload, BucketGroup.Guild, (int)bucket, guildId, options);
        private async Task SendGatewayAsync(GatewayOpCode opCode, object payload, 
            BucketGroup group, int bucketId, ulong guildId, RequestOptions options)
        {
            //TODO: Add ETF
            byte[] bytes = null;
            payload = new WebSocketMessage { Operation = (int)opCode, Payload = payload };
            if (payload != null)
                bytes = Encoding.UTF8.GetBytes(SerializeJson(payload));
            await _requestQueue.SendAsync(new WebSocketRequest(_gatewayClient, bytes, true, options), group, bucketId, guildId).ConfigureAwait(false);
            await _sentGatewayMessageEvent.InvokeAsync(opCode).ConfigureAwait(false);
        }

        //Auth
        public async Task ValidateTokenAsync(RequestOptions options = null)
        {
            await SendAsync("GET", "auth/login", options: options).ConfigureAwait(false);
        }

        //Gateway
        public async Task<GetGatewayResponse> GetGatewayAsync(RequestOptions options = null)
        {
            return await SendAsync<GetGatewayResponse>("GET", "gateway", options: options).ConfigureAwait(false);
        }
        public async Task SendIdentifyAsync(int largeThreshold = 100, bool useCompression = true, RequestOptions options = null)
        {
            var props = new Dictionary<string, string>
            {
                ["$device"] = "Discord.Net"
            };
            var msg = new IdentifyParams()
            {
                Token = _authToken,
                Properties = props,
                LargeThreshold = largeThreshold,
                UseCompression = useCompression
            };
            await SendGatewayAsync(GatewayOpCode.Identify, msg, options: options).ConfigureAwait(false);
        }
        public async Task SendResumeAsync(string sessionId, int lastSeq, RequestOptions options = null)
        {
            var msg = new ResumeParams()
            {
                SessionId = sessionId,
                Sequence = lastSeq
            };
            await SendGatewayAsync(GatewayOpCode.Resume, msg, options: options).ConfigureAwait(false);
        }
        public async Task SendHeartbeatAsync(int lastSeq, RequestOptions options = null)
        {
            await SendGatewayAsync(GatewayOpCode.Heartbeat, lastSeq, options: options).ConfigureAwait(false);
        }
        public async Task SendStatusUpdateAsync(long? idleSince, Game game, RequestOptions options = null)
        {
            var args = new StatusUpdateParams
            {
                IdleSince = idleSince,
                Game = game
            };
            await SendGatewayAsync(GatewayOpCode.StatusUpdate, args, options: options).ConfigureAwait(false);
        }
        public async Task SendRequestMembersAsync(IEnumerable<ulong> guildIds, RequestOptions options = null)
        {
            await SendGatewayAsync(GatewayOpCode.RequestGuildMembers, new RequestMembersParams { GuildIds = guildIds, Query = "", Limit = 0 }, options: options).ConfigureAwait(false);
        }
        public async Task SendVoiceStateUpdateAsync(ulong guildId, ulong? channelId, bool selfDeaf, bool selfMute, RequestOptions options = null)
        {
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
            await SendGatewayAsync(GatewayOpCode.GuildSync, guildIds, options: options).ConfigureAwait(false);
        }

        //Channels
        public async Task<Channel> GetChannelAsync(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            try
            {
                return await SendAsync<Channel>("GET", $"channels/{channelId}", options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<Channel> GetChannelAsync(ulong guildId, ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            try
            {
                var model = await SendAsync<Channel>("GET", $"channels/{channelId}", options: options).ConfigureAwait(false);
                if (!model.GuildId.IsSpecified || model.GuildId.Value != guildId)
                    return null;
                return model;
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<IReadOnlyCollection<Channel>> GetGuildChannelsAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await SendAsync<IReadOnlyCollection<Channel>>("GET", $"guilds/{guildId}/channels", options: options).ConfigureAwait(false);
        }
        public async Task<Channel> CreateGuildChannelAsync(ulong guildId, CreateGuildChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args.Bitrate, 0, nameof(args.Bitrate));
            Preconditions.NotNullOrWhitespace(args.Name, nameof(args.Name));

            return await SendAsync<Channel>("POST", $"guilds/{guildId}/channels", args, options: options).ConfigureAwait(false);
        }
        public async Task<Channel> DeleteChannelAsync(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            return await SendAsync<Channel>("DELETE", $"channels/{channelId}", options: options).ConfigureAwait(false);
        }
        public async Task<Channel> ModifyGuildChannelAsync(ulong channelId, ModifyGuildChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Position, 0, nameof(args.Position));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));

            return await SendAsync<Channel>("PATCH", $"channels/{channelId}", args, options: options).ConfigureAwait(false);
        }
        public async Task<Channel> ModifyGuildChannelAsync(ulong channelId, ModifyTextChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Position, 0, nameof(args.Position));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));

            return await SendAsync<Channel>("PATCH", $"channels/{channelId}", args, options: options).ConfigureAwait(false);
        }
        public async Task<Channel> ModifyGuildChannelAsync(ulong channelId, ModifyVoiceChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args.Bitrate, 0, nameof(args.Bitrate));
            Preconditions.AtLeast(args.UserLimit, 0, nameof(args.Bitrate));
            Preconditions.AtLeast(args.Position, 0, nameof(args.Position));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));

            return await SendAsync<Channel>("PATCH", $"channels/{channelId}", args, options: options).ConfigureAwait(false);
        }
        public async Task ModifyGuildChannelsAsync(ulong guildId, IEnumerable<ModifyGuildChannelsParams> args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));

            var channels = args.ToArray();
            switch (channels.Length)
            {
                case 0:
                    return;
                case 1:
                    await ModifyGuildChannelAsync(channels[0].Id, new ModifyGuildChannelParams { Position = channels[0].Position }).ConfigureAwait(false);
                    break;
                default:
                    await SendAsync("PATCH", $"guilds/{guildId}/channels", channels, options: options).ConfigureAwait(false);
                    break;
            }
        }

        //Channel Permissions
        public async Task ModifyChannelPermissionsAsync(ulong channelId, ulong targetId, ModifyChannelPermissionsParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(targetId, 0, nameof(targetId));
            Preconditions.NotNull(args, nameof(args));

            await SendAsync("PUT", $"channels/{channelId}/permissions/{targetId}", args, options: options).ConfigureAwait(false);
        }
        public async Task DeleteChannelPermissionAsync(ulong channelId, ulong targetId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(targetId, 0, nameof(targetId));

            await SendAsync("DELETE", $"channels/{channelId}/permissions/{targetId}", options: options).ConfigureAwait(false);
        }

        //Channel Pins
        public async Task AddPinAsync(ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.GreaterThan(channelId, 0, nameof(channelId));
            Preconditions.GreaterThan(messageId, 0, nameof(messageId));

            await SendAsync("PUT", $"channels/{channelId}/pins/{messageId}", options: options).ConfigureAwait(false);

        }
        public async Task RemovePinAsync(ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));

            await SendAsync("DELETE", $"channels/{channelId}/pins/{messageId}", options: options).ConfigureAwait(false);
        }

        //Guilds
        public async Task<Guild> GetGuildAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            try
            {
                return await SendAsync<Guild>("GET", $"guilds/{guildId}", options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<Guild> CreateGuildAsync(CreateGuildParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNullOrWhitespace(args.Name, nameof(args.Name));
            Preconditions.NotNullOrWhitespace(args.Region, nameof(args.Region));

            return await SendAsync<Guild>("POST", "guilds", args, options: options).ConfigureAwait(false);
        }
        public async Task<Guild> DeleteGuildAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await SendAsync<Guild>("DELETE", $"guilds/{guildId}", options: options).ConfigureAwait(false);
        }
        public async Task<Guild> LeaveGuildAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await SendAsync<Guild>("DELETE", $"users/@me/guilds/{guildId}", options: options).ConfigureAwait(false);
        }
        public async Task<Guild> ModifyGuildAsync(ulong guildId, ModifyGuildParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(args.AFKChannelId, 0, nameof(args.AFKChannelId));
            Preconditions.AtLeast(args.AFKTimeout, 0, nameof(args.AFKTimeout));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));
            Preconditions.GreaterThan(args.OwnerId, 0, nameof(args.OwnerId));
            Preconditions.NotNull(args.Region, nameof(args.Region));

            return await SendAsync<Guild>("PATCH", $"guilds/{guildId}", args, options: options).ConfigureAwait(false);
        }
        public async Task<GetGuildPruneCountResponse> BeginGuildPruneAsync(ulong guildId, GuildPruneParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Days, 0, nameof(args.Days));

            return await SendAsync<GetGuildPruneCountResponse>("POST", $"guilds/{guildId}/prune", args, options: options).ConfigureAwait(false);
        }
        public async Task<GetGuildPruneCountResponse> GetGuildPruneCountAsync(ulong guildId, GuildPruneParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Days, 0, nameof(args.Days));

            return await SendAsync<GetGuildPruneCountResponse>("GET", $"guilds/{guildId}/prune", args, options: options).ConfigureAwait(false);
        }

        //Guild Bans
        public async Task<IReadOnlyCollection<User>> GetGuildBansAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await SendAsync<IReadOnlyCollection<User>>("GET", $"guilds/{guildId}/bans", options: options).ConfigureAwait(false);
        }
        public async Task CreateGuildBanAsync(ulong guildId, ulong userId, CreateGuildBanParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.PruneDays, 0, nameof(args.PruneDays));

            await SendAsync("PUT", $"guilds/{guildId}/bans/{userId}", args, options: options).ConfigureAwait(false);
        }
        public async Task RemoveGuildBanAsync(ulong guildId, ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));

            await SendAsync("DELETE", $"guilds/{guildId}/bans/{userId}", options: options).ConfigureAwait(false);
        }

        //Guild Embeds
        public async Task<GuildEmbed> GetGuildEmbedAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            try
            {
                return await SendAsync<GuildEmbed>("GET", $"guilds/{guildId}/embed", options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<GuildEmbed> ModifyGuildEmbedAsync(ulong guildId, ModifyGuildEmbedParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await SendAsync<GuildEmbed>("PATCH", $"guilds/{guildId}/embed", args, options: options).ConfigureAwait(false);
        }

        //Guild Integrations
        public async Task<IReadOnlyCollection<Integration>> GetGuildIntegrationsAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await SendAsync<IReadOnlyCollection<Integration>>("GET", $"guilds/{guildId}/integrations", options: options).ConfigureAwait(false);
        }
        public async Task<Integration> CreateGuildIntegrationAsync(ulong guildId, CreateGuildIntegrationParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(args.Id, 0, nameof(args.Id));

            return await SendAsync<Integration>("POST", $"guilds/{guildId}/integrations", options: options).ConfigureAwait(false);
        }
        public async Task<Integration> DeleteGuildIntegrationAsync(ulong guildId, ulong integrationId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(integrationId, 0, nameof(integrationId));

            return await SendAsync<Integration>("DELETE", $"guilds/{guildId}/integrations/{integrationId}", options: options).ConfigureAwait(false);
        }
        public async Task<Integration> ModifyGuildIntegrationAsync(ulong guildId, ulong integrationId, ModifyGuildIntegrationParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(integrationId, 0, nameof(integrationId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.ExpireBehavior, 0, nameof(args.ExpireBehavior));
            Preconditions.AtLeast(args.ExpireGracePeriod, 0, nameof(args.ExpireGracePeriod));

            return await SendAsync<Integration>("PATCH", $"guilds/{guildId}/integrations/{integrationId}", args, options: options).ConfigureAwait(false);
        }
        public async Task<Integration> SyncGuildIntegrationAsync(ulong guildId, ulong integrationId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(integrationId, 0, nameof(integrationId));

            return await SendAsync<Integration>("POST", $"guilds/{guildId}/integrations/{integrationId}/sync", options: options).ConfigureAwait(false);
        }

        //Guild Invites
        public async Task<Invite> GetInviteAsync(string inviteIdOrXkcd, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(inviteIdOrXkcd, nameof(inviteIdOrXkcd));

            //Remove trailing slash
            if (inviteIdOrXkcd[inviteIdOrXkcd.Length - 1] == '/')
                inviteIdOrXkcd = inviteIdOrXkcd.Substring(0, inviteIdOrXkcd.Length - 1);
            //Remove leading URL
            int index = inviteIdOrXkcd.LastIndexOf('/');
            if (index >= 0)
                inviteIdOrXkcd = inviteIdOrXkcd.Substring(index + 1);

            try
            {
                return await SendAsync<Invite>("GET", $"invites/{inviteIdOrXkcd}", options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<IReadOnlyCollection<InviteMetadata>> GetGuildInvitesAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await SendAsync<IReadOnlyCollection<InviteMetadata>>("GET", $"guilds/{guildId}/invites", options: options).ConfigureAwait(false);
        }
        public async Task<InviteMetadata[]> GetChannelInvitesAsync(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            return await SendAsync<InviteMetadata[]>("GET", $"channels/{channelId}/invites", options: options).ConfigureAwait(false);
        }
        public async Task<InviteMetadata> CreateChannelInviteAsync(ulong channelId, CreateChannelInviteParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.MaxAge, 0, nameof(args.MaxAge));
            Preconditions.AtLeast(args.MaxUses, 0, nameof(args.MaxUses));

            return await SendAsync<InviteMetadata>("POST", $"channels/{channelId}/invites", args, options: options).ConfigureAwait(false);
        }
        public async Task<Invite> DeleteInviteAsync(string inviteCode, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(inviteCode, nameof(inviteCode));

            return await SendAsync<Invite>("DELETE", $"invites/{inviteCode}", options: options).ConfigureAwait(false);
        }
        public async Task AcceptInviteAsync(string inviteCode, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(inviteCode, nameof(inviteCode));

            await SendAsync("POST", $"invites/{inviteCode}", options: options).ConfigureAwait(false);
        }

        //Guild Members
        public async Task<GuildMember> GetGuildMemberAsync(ulong guildId, ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));

            try
            {
                return await SendAsync<GuildMember>("GET", $"guilds/{guildId}/members/{userId}", options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<IReadOnlyCollection<GuildMember>> GetGuildMembersAsync(ulong guildId, GetGuildMembersParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args.Limit, 0, nameof(args.Limit));
            Preconditions.AtLeast(args.Offset, 0, nameof(args.Offset));

            int limit = args.Limit.GetValueOrDefault(int.MaxValue);
            int offset = args.Offset.GetValueOrDefault(0);

            List<GuildMember[]> result;
            if (args.Limit.IsSpecified)
                result = new List<GuildMember[]>((limit + DiscordConfig.MaxUsersPerBatch - 1) / DiscordConfig.MaxUsersPerBatch);
            else
                result = new List<GuildMember[]>();

            while (true)
            {
                int runLimit = (limit >= DiscordConfig.MaxUsersPerBatch) ? DiscordConfig.MaxUsersPerBatch : limit;
                string endpoint = $"guilds/{guildId}/members?limit={runLimit}&offset={offset}";
                var models = await SendAsync<GuildMember[]>("GET", endpoint, options: options).ConfigureAwait(false);

                //Was this an empty batch?
                if (models.Length == 0) break;

                result.Add(models);

                limit -= DiscordConfig.MaxUsersPerBatch;
                offset += models.Length;

                //Was this an incomplete (the last) batch?
                if (models.Length != DiscordConfig.MaxUsersPerBatch) break;
            }

            if (result.Count > 1)
                return result.SelectMany(x => x).ToImmutableArray();
            else if (result.Count == 1)
                return result[0];
            else
                return ImmutableArray.Create<GuildMember>();
        }
        public async Task RemoveGuildMemberAsync(ulong guildId, ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));

            await SendAsync("DELETE", $"guilds/{guildId}/members/{userId}", options: options).ConfigureAwait(false);
        }
        public async Task ModifyGuildMemberAsync(ulong guildId, ulong userId, ModifyGuildMemberParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));
            Preconditions.NotNull(args, nameof(args));

            await SendAsync("PATCH", $"guilds/{guildId}/members/{userId}", args, GuildBucket.ModifyMember, guildId, options: options).ConfigureAwait(false);
        }

        //Guild Roles
        public async Task<IReadOnlyCollection<Role>> GetGuildRolesAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await SendAsync<IReadOnlyCollection<Role>>("GET", $"guilds/{guildId}/roles", options: options).ConfigureAwait(false);
        }
        public async Task<Role> CreateGuildRoleAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await SendAsync<Role>("POST", $"guilds/{guildId}/roles", options: options).ConfigureAwait(false);
        }
        public async Task DeleteGuildRoleAsync(ulong guildId, ulong roleId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(roleId, 0, nameof(roleId));

            await SendAsync("DELETE", $"guilds/{guildId}/roles/{roleId}", options: options).ConfigureAwait(false);
        }
        public async Task<Role> ModifyGuildRoleAsync(ulong guildId, ulong roleId, ModifyGuildRoleParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(roleId, 0, nameof(roleId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Color, 0, nameof(args.Color));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));
            Preconditions.AtLeast(args.Position, 0, nameof(args.Position));

            return await SendAsync<Role>("PATCH", $"guilds/{guildId}/roles/{roleId}", args, options: options).ConfigureAwait(false);
        }
        public async Task<IReadOnlyCollection<Role>> ModifyGuildRolesAsync(ulong guildId, IEnumerable<ModifyGuildRolesParams> args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));

            var roles = args.ToArray();
            switch (roles.Length)
            {
                case 0:
                    return ImmutableArray.Create<Role>();
                case 1:
                    return ImmutableArray.Create(await ModifyGuildRoleAsync(guildId, roles[0].Id, roles[0]).ConfigureAwait(false));
                default:
                    return await SendAsync<IReadOnlyCollection<Role>>("PATCH", $"guilds/{guildId}/roles", args, options: options).ConfigureAwait(false);
            }
        }

        //Messages
        public async Task<Message> GetChannelMessageAsync(ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));

            try
            {
                return await SendAsync<Message>("GET", $"channels/{channelId}/messages/{messageId}", options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<IReadOnlyCollection<Message>> GetChannelMessagesAsync(ulong channelId, GetChannelMessagesParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Limit, 0, nameof(args.Limit));

            int limit = args.Limit;
            ulong? relativeId = args.RelativeMessageId.IsSpecified ? args.RelativeMessageId.Value : (ulong?)null;
            string relativeDir;

            switch (args.RelativeDirection)
            {
                case Direction.Before:
                default:
                    relativeDir = "before";
                    break;
                case Direction.After:
                    relativeDir = "after";
                    break;
                case Direction.Around:
                    relativeDir = "around";
                    break;
            }
            
            int runs = (limit + DiscordConfig.MaxMessagesPerBatch - 1) / DiscordConfig.MaxMessagesPerBatch;
            int lastRunCount = limit - (runs - 1) * DiscordConfig.MaxMessagesPerBatch;
            var result = new API.Message[runs][];

            int i = 0;
            for (; i < runs; i++)
            {
                int runCount = i == (runs - 1) ? lastRunCount : DiscordConfig.MaxMessagesPerBatch;
                string endpoint;
                if (relativeId != null)
                    endpoint = $"channels/{channelId}/messages?limit={runCount}&{relativeDir}={relativeId}";
                else
                    endpoint = $"channels/{channelId}/messages?limit={runCount}";
                var models = await SendAsync<Message[]>("GET", endpoint, options: options).ConfigureAwait(false);

                //Was this an empty batch?
                if (models.Length == 0) break;

                result[i] = models;                
                relativeId = args.RelativeDirection == Direction.Before ? models[0].Id : models[models.Length - 1].Id;

                //Was this an incomplete (the last) batch?
                if (models.Length != DiscordConfig.MaxMessagesPerBatch) { i++; break; }
            }

            if (i > 1)
            {
                if (args.RelativeDirection == Direction.Before)
                    return result.Take(i).SelectMany(x => x).ToImmutableArray();
                else
                    return result.Take(i).Reverse().SelectMany(x => x).ToImmutableArray();
            }
            else if (i == 1)
                return result[0];
            else
                return ImmutableArray.Create<Message>();
        }
        public Task<Message> CreateMessageAsync(ulong guildId, ulong channelId, CreateMessageParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return CreateMessageInternalAsync(guildId, channelId, args);
        }
        public Task<Message> CreateDMMessageAsync(ulong channelId, CreateMessageParams args, RequestOptions options = null)
        {
            return CreateMessageInternalAsync(0, channelId, args);
        }
        public async Task<Message> CreateMessageInternalAsync(ulong guildId, ulong channelId, CreateMessageParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNullOrEmpty(args.Content, nameof(args.Content));
            if (args.Content.Length > DiscordConfig.MaxMessageSize)
                throw new ArgumentException($"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.", nameof(args.Content));

            if (guildId != 0)
                return await SendAsync<Message>("POST", $"channels/{channelId}/messages", args, GuildBucket.SendEditMessage, guildId, options: options).ConfigureAwait(false);
            else
                return await SendAsync<Message>("POST", $"channels/{channelId}/messages", args, GlobalBucket.DirectMessage, options: options).ConfigureAwait(false);
        }
        public Task<Message> UploadFileAsync(ulong guildId, ulong channelId, Stream file, UploadFileParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return UploadFileInternalAsync(guildId, channelId, file, args);
        }
        public Task<Message> UploadDMFileAsync(ulong channelId, Stream file, UploadFileParams args, RequestOptions options = null)
        {
            return UploadFileInternalAsync(0, channelId, file, args);
        }
        private async Task<Message> UploadFileInternalAsync(ulong guildId, ulong channelId, Stream file, UploadFileParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            if (args.Content.IsSpecified)
            {
                Preconditions.NotNullOrEmpty(args.Content, nameof(args.Content));
                if (args.Content.Value.Length > DiscordConfig.MaxMessageSize)
                    throw new ArgumentOutOfRangeException($"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.", nameof(args.Content));
            }

            if (guildId != 0)
                return await SendAsync<Message>("POST", $"channels/{channelId}/messages", file, args.ToDictionary(), GuildBucket.SendEditMessage, guildId, options: options).ConfigureAwait(false);
            else
                return await SendAsync<Message>("POST", $"channels/{channelId}/messages", file, args.ToDictionary(), GlobalBucket.DirectMessage, options: options).ConfigureAwait(false);
        }
        public Task DeleteMessageAsync(ulong guildId, ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return DeleteMessageInternalAsync(guildId, channelId, messageId);
        }
        public Task DeleteDMMessageAsync(ulong channelId, ulong messageId, RequestOptions options = null)
        {
            return DeleteMessageInternalAsync(0, channelId, messageId);
        }
        private async Task DeleteMessageInternalAsync(ulong guildId, ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));

            if (guildId != 0)
                await SendAsync("DELETE", $"channels/{channelId}/messages/{messageId}", GuildBucket.DeleteMessage, guildId, options: options).ConfigureAwait(false);
            else
                await SendAsync("DELETE", $"channels/{channelId}/messages/{messageId}", options: options).ConfigureAwait(false);
        }
        public Task DeleteMessagesAsync(ulong guildId, ulong channelId, DeleteMessagesParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return DeleteMessagesInternalAsync(guildId, channelId, args);
        }
        public Task DeleteDMMessagesAsync(ulong channelId, DeleteMessagesParams args, RequestOptions options = null)
        {
            return DeleteMessagesInternalAsync(0, channelId, args);
        }
        private async Task DeleteMessagesInternalAsync(ulong guildId, ulong channelId, DeleteMessagesParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));

            var messageIds = args.MessageIds?.ToArray();
            Preconditions.NotNull(args.MessageIds, nameof(args.MessageIds));
            Preconditions.AtMost(messageIds.Length, 100, nameof(messageIds.Length));

            switch (messageIds.Length)
            {
                case 0:
                    return;
                case 1:
                    await DeleteMessageInternalAsync(guildId, channelId, messageIds[0]).ConfigureAwait(false);
                    break;
                default:
                    if (guildId != 0)
                        await SendAsync("POST", $"channels/{channelId}/messages/bulk_delete", args, GuildBucket.DeleteMessages, guildId, options: options).ConfigureAwait(false);
                    else
                        await SendAsync("POST", $"channels/{channelId}/messages/bulk_delete", args, options: options).ConfigureAwait(false);
                    break;
            }
        }
        public Task<Message> ModifyMessageAsync(ulong guildId, ulong channelId, ulong messageId, ModifyMessageParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return ModifyMessageInternalAsync(guildId, channelId, messageId, args);
        }
        public Task<Message> ModifyDMMessageAsync(ulong channelId, ulong messageId, ModifyMessageParams args, RequestOptions options = null)
        {
            return ModifyMessageInternalAsync(0, channelId, messageId, args);
        }
        private async Task<Message> ModifyMessageInternalAsync(ulong guildId, ulong channelId, ulong messageId, ModifyMessageParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));
            Preconditions.NotNull(args, nameof(args));
            if (args.Content.IsSpecified)
            {
                Preconditions.NotNullOrEmpty(args.Content, nameof(args.Content));
                if (args.Content.Value.Length > DiscordConfig.MaxMessageSize)
                    throw new ArgumentOutOfRangeException($"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.", nameof(args.Content));
            }

            if (guildId != 0)
                return await SendAsync<Message>("PATCH", $"channels/{channelId}/messages/{messageId}", args, GuildBucket.SendEditMessage, guildId, options: options).ConfigureAwait(false);
            else
                return await SendAsync<Message>("PATCH", $"channels/{channelId}/messages/{messageId}", args, options: options).ConfigureAwait(false);
        }
        public async Task AckMessageAsync(ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));

            await SendAsync("POST", $"channels/{channelId}/messages/{messageId}/ack", options: options).ConfigureAwait(false);
        }
        public async Task TriggerTypingIndicatorAsync(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            await SendAsync("POST", $"channels/{channelId}/typing", options: options).ConfigureAwait(false);
        }

        //Users
        public async Task<User> GetUserAsync(ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(userId, 0, nameof(userId));

            try
            {
                return await SendAsync<User>("GET", $"users/{userId}", options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<User> GetUserAsync(string username, string discriminator, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(username, nameof(username));
            Preconditions.NotNullOrEmpty(discriminator, nameof(discriminator));
            
            try
            {
                var models = await QueryUsersAsync($"{username}#{discriminator}", 1, options: options).ConfigureAwait(false);
                return models.FirstOrDefault();
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<IReadOnlyCollection<User>> QueryUsersAsync(string query, int limit, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(query, nameof(query));
            Preconditions.AtLeast(limit, 0, nameof(limit));

            return await SendAsync<IReadOnlyCollection<User>>("GET", $"users?q={Uri.EscapeDataString(query)}&limit={limit}", options: options).ConfigureAwait(false);
        }

        //Current User/DMs
        public async Task<User> GetSelfAsync(RequestOptions options = null)
        {
            return await SendAsync<User>("GET", "users/@me", options: options).ConfigureAwait(false);
        }
        public async Task<IReadOnlyCollection<Connection>> GetMyConnectionsAsync(RequestOptions options = null)
        {
            return await SendAsync<IReadOnlyCollection<Connection>>("GET", "users/@me/connections", options: options).ConfigureAwait(false);
        }
        public async Task<IReadOnlyCollection<Channel>> GetMyDMsAsync(RequestOptions options = null)
        {
            return await SendAsync<IReadOnlyCollection<Channel>>("GET", "users/@me/channels", options: options).ConfigureAwait(false);
        }
        public async Task<IReadOnlyCollection<UserGuild>> GetMyGuildsAsync(RequestOptions options = null)
        {
            return await SendAsync<IReadOnlyCollection<UserGuild>>("GET", "users/@me/guilds", options: options).ConfigureAwait(false);
        }
        public async Task<User> ModifySelfAsync(ModifyCurrentUserParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNullOrEmpty(args.Username, nameof(args.Username));

            return await SendAsync<User>("PATCH", "users/@me", args, options: options).ConfigureAwait(false);
        }
        public async Task ModifyMyNickAsync(ulong guildId, ModifyCurrentUserNickParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEmpty(args.Nickname, nameof(args.Nickname));

            await SendAsync("PATCH", $"guilds/{guildId}/members/@me/nick", args, options: options).ConfigureAwait(false);
        }
        public async Task<Channel> CreateDMChannelAsync(CreateDMChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args.RecipientId, 0, nameof(args.Recipient));

            return await SendAsync<Channel>("POST", $"users/@me/channels", args, options: options).ConfigureAwait(false);
        }

        //Voice Regions
        public async Task<IReadOnlyCollection<VoiceRegion>> GetVoiceRegionsAsync(RequestOptions options = null)
        {
            return await SendAsync<IReadOnlyCollection<VoiceRegion>>("GET", "voice/regions", options: options).ConfigureAwait(false);
        }
        public async Task<IReadOnlyCollection<VoiceRegion>> GetGuildVoiceRegionsAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await SendAsync<IReadOnlyCollection<VoiceRegion>>("GET", $"guilds/{guildId}/regions", options: options).ConfigureAwait(false);
        }

        //Helpers
        private static double ToMilliseconds(Stopwatch stopwatch) => Math.Round((double)stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0, 2);
        private string SerializeJson(object value)
        {
            var sb = new StringBuilder(256);
            using (TextWriter text = new StringWriter(sb, CultureInfo.InvariantCulture))
            using (JsonWriter writer = new JsonTextWriter(text))
                _serializer.Serialize(writer, value);
            return sb.ToString();
        }
        private T DeserializeJson<T>(Stream jsonStream)
        {
            using (TextReader text = new StreamReader(jsonStream))
            using (JsonReader reader = new JsonTextReader(text))
                return _serializer.Deserialize<T>(reader);
        }
    }
}
