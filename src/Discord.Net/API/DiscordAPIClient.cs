using Discord.API.Gateway;
using Discord.API.Rest;
using Discord.Extensions;
using Discord.Net;
using Discord.Net.Converters;
using Discord.Net.Queue;
using Discord.Net.Rest;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public event Func<string, string, double, Task> SentRequest;
        public event Func<int, Task> SentGatewayMessage;
        public event Func<GatewayOpCode, string, JToken, Task> ReceivedGatewayEvent;

        private readonly RequestQueue _requestQueue;
        private readonly JsonSerializer _serializer;
        private readonly IRestClient _restClient;
        private readonly IWebSocketClient _gatewayClient;
        private readonly SemaphoreSlim _connectionLock;
        private CancellationTokenSource _loginCancelToken, _connectCancelToken;
        private string _authToken;
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
                _gatewayClient.SetHeader("user-agent", DiscordConfig.UserAgent);
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
                            await ReceivedGatewayEvent.Raise((GatewayOpCode)msg.Operation, msg.Type, msg.Payload as JToken).ConfigureAwait(false);
                        }
                    }
                };
                _gatewayClient.TextMessage += async text =>
                {
                    var msg = JsonConvert.DeserializeObject<WebSocketMessage>(text);
                    await ReceivedGatewayEvent.Raise((GatewayOpCode)msg.Operation, msg.Type, msg.Payload as JToken).ConfigureAwait(false);
                };
            }

            _serializer = serializer ?? new JsonSerializer { ContractResolver = new DiscordContractResolver() };
        }
        void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _loginCancelToken?.Dispose();
                    _connectCancelToken?.Dispose();
                }
                _isDisposed = true;
            }
        }
        public void Dispose() => Dispose(true);
        
        public async Task Login(TokenType tokenType, string token, RequestOptions options = null)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LoginInternal(tokenType, token, options).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task LoginInternal(TokenType tokenType, string token, RequestOptions options = null)
        {
            if (LoginState != LoginState.LoggedOut)
                await LogoutInternal().ConfigureAwait(false);
            LoginState = LoginState.LoggingIn;
            
            try
            {
                _loginCancelToken = new CancellationTokenSource();

                AuthTokenType = TokenType.User;
                _authToken = null;
                _restClient.SetHeader("authorization", null);
                await _requestQueue.SetCancelToken(_loginCancelToken.Token).ConfigureAwait(false);
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
                await LogoutInternal().ConfigureAwait(false);
                throw;
            }
        }

        public async Task Logout()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LogoutInternal().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task LogoutInternal()
        {
            //TODO: An exception here will lock the client into the unusable LoggingOut state. How should we handle? (Add same solution to both DiscordClients too)
            if (LoginState == LoginState.LoggedOut) return;
            LoginState = LoginState.LoggingOut;
            
            try { _loginCancelToken?.Cancel(false); }
            catch { }

            await DisconnectInternal().ConfigureAwait(false);
            await _requestQueue.Clear().ConfigureAwait(false);

            await _requestQueue.SetCancelToken(CancellationToken.None).ConfigureAwait(false);
            _restClient.SetCancelToken(CancellationToken.None);

            LoginState = LoginState.LoggedOut;
        }

        public async Task Connect()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await ConnectInternal().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ConnectInternal()
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

                var gatewayResponse = await GetGateway().ConfigureAwait(false);
                var url = $"{gatewayResponse.Url}?v={DiscordConfig.GatewayAPIVersion}&encoding={DiscordConfig.GatewayEncoding}";
                await _gatewayClient.Connect(url).ConfigureAwait(false);

                ConnectionState = ConnectionState.Connected;
            }
            catch (Exception)
            {
                await DisconnectInternal().ConfigureAwait(false);
                throw;
            }
        }

        public async Task Disconnect()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternal().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task DisconnectInternal()
        {
            if (_gatewayClient == null)
                throw new NotSupportedException("This client is not configured with websocket support.");

            if (ConnectionState == ConnectionState.Disconnected) return;
            ConnectionState = ConnectionState.Disconnecting;
            
            try { _connectCancelToken?.Cancel(false); }
            catch { }

            await _gatewayClient.Disconnect().ConfigureAwait(false);

            ConnectionState = ConnectionState.Disconnected;
        }

        //Core
        public Task Send(string method, string endpoint, 
            GlobalBucket bucket = GlobalBucket.GeneralRest, RequestOptions options = null)
            => SendInternal(method, endpoint, null, true, bucket, options);
        public Task Send(string method, string endpoint, object payload, 
            GlobalBucket bucket = GlobalBucket.GeneralRest, RequestOptions options = null)
            => SendInternal(method, endpoint, payload, true, bucket, options);
        public Task Send(string method, string endpoint, Stream file, IReadOnlyDictionary<string, string> multipartArgs, 
            GlobalBucket bucket = GlobalBucket.GeneralRest, RequestOptions options = null)
            => SendInternal(method, endpoint, multipartArgs, true, bucket, options);
        public async Task<TResponse> Send<TResponse>(string method, string endpoint,
            GlobalBucket bucket = GlobalBucket.GeneralRest, RequestOptions options = null)
            where TResponse : class
            => DeserializeJson<TResponse>(await SendInternal(method, endpoint, null, false, bucket, options).ConfigureAwait(false));
        public async Task<TResponse> Send<TResponse>(string method, string endpoint, object payload, GlobalBucket bucket = 
            GlobalBucket.GeneralRest, RequestOptions options = null)
            where TResponse : class
            => DeserializeJson<TResponse>(await SendInternal(method, endpoint, payload, false, bucket, options).ConfigureAwait(false));
        public async Task<TResponse> Send<TResponse>(string method, string endpoint, Stream file, IReadOnlyDictionary<string, string> multipartArgs, 
            GlobalBucket bucket = GlobalBucket.GeneralRest, RequestOptions options = null)
            where TResponse : class
            => DeserializeJson<TResponse>(await SendInternal(method, endpoint, multipartArgs, false, bucket, options).ConfigureAwait(false));

        public Task Send(string method, string endpoint, 
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            => SendInternal(method, endpoint, null, true, bucket, guildId, options);
        public Task Send(string method, string endpoint, object payload, 
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            => SendInternal(method, endpoint, payload, true, bucket, guildId, options);
        public Task Send(string method, string endpoint, Stream file, IReadOnlyDictionary<string, string> multipartArgs, 
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            => SendInternal(method, endpoint, multipartArgs, true, bucket, guildId, options);
        public async Task<TResponse> Send<TResponse>(string method, string endpoint, 
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            where TResponse : class
            => DeserializeJson<TResponse>(await SendInternal(method, endpoint, null, false, bucket, guildId, options).ConfigureAwait(false));
        public async Task<TResponse> Send<TResponse>(string method, string endpoint, object payload, 
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            where TResponse : class
            => DeserializeJson<TResponse>(await SendInternal(method, endpoint, payload, false, bucket, guildId, options).ConfigureAwait(false));
        public async Task<TResponse> Send<TResponse>(string method, string endpoint, Stream file, IReadOnlyDictionary<string, string> multipartArgs, 
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            where TResponse : class
            => DeserializeJson<TResponse>(await SendInternal(method, endpoint, multipartArgs, false, bucket, guildId, options).ConfigureAwait(false));
        
        private Task<Stream> SendInternal(string method, string endpoint, object payload, bool headerOnly, 
            GlobalBucket bucket, RequestOptions options)
            => SendInternal(method, endpoint, payload, headerOnly, BucketGroup.Global, (int)bucket, 0, options);
        private Task<Stream> SendInternal(string method, string endpoint, object payload, bool headerOnly, 
            GuildBucket bucket, ulong guildId, RequestOptions options)
            => SendInternal(method, endpoint, payload, headerOnly, BucketGroup.Guild, (int)bucket, guildId, options);
        private Task<Stream> SendInternal(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs, bool headerOnly, 
            GlobalBucket bucket, RequestOptions options)
            => SendInternal(method, endpoint, multipartArgs, headerOnly, BucketGroup.Global, (int)bucket, 0, options);
        private Task<Stream> SendInternal(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs, bool headerOnly, 
            GuildBucket bucket, ulong guildId, RequestOptions options)
            => SendInternal(method, endpoint, multipartArgs, headerOnly, BucketGroup.Guild, (int)bucket, guildId, options);

        private async Task<Stream> SendInternal(string method, string endpoint, object payload, bool headerOnly, 
            BucketGroup group, int bucketId, ulong guildId, RequestOptions options = null)
        {
            var stopwatch = Stopwatch.StartNew();
            string json = null;
            if (payload != null)
                json = SerializeJson(payload);
            var responseStream = await _requestQueue.Send(new RestRequest(_restClient, method, endpoint, json, headerOnly, options), group, bucketId, guildId).ConfigureAwait(false);
            stopwatch.Stop();

            double milliseconds = ToMilliseconds(stopwatch);
            await SentRequest.Raise(method, endpoint, milliseconds).ConfigureAwait(false);

            return responseStream;
        }
        private async Task<Stream> SendInternal(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs, bool headerOnly, 
            BucketGroup group, int bucketId, ulong guildId, RequestOptions options = null)
        {
            var stopwatch = Stopwatch.StartNew();
            var responseStream = await _requestQueue.Send(new RestRequest(_restClient, method, endpoint, multipartArgs, headerOnly, options), group, bucketId, guildId).ConfigureAwait(false);
            int bytes = headerOnly ? 0 : (int)responseStream.Length;
            stopwatch.Stop();

            double milliseconds = ToMilliseconds(stopwatch);
            await SentRequest.Raise(method, endpoint, milliseconds).ConfigureAwait(false);

            return responseStream;
        }

        public Task SendGateway(GatewayOpCode opCode, object payload, 
            GlobalBucket bucket = GlobalBucket.GeneralGateway, RequestOptions options = null)
            => SendGateway(opCode, payload, BucketGroup.Global, (int)bucket, 0, options);
        public Task SendGateway(GatewayOpCode opCode, object payload, 
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            => SendGateway(opCode, payload, BucketGroup.Guild, (int)bucket, guildId, options);
        private async Task SendGateway(GatewayOpCode opCode, object payload, 
            BucketGroup group, int bucketId, ulong guildId, RequestOptions options)
        {
            //TODO: Add ETF
            byte[] bytes = null;
            payload = new WebSocketMessage { Operation = (int)opCode, Payload = payload };
            if (payload != null)
                bytes = Encoding.UTF8.GetBytes(SerializeJson(payload));
            await _requestQueue.Send(new WebSocketRequest(_gatewayClient, bytes, true, options), group, bucketId, guildId).ConfigureAwait(false);
            await SentGatewayMessage.Raise((int)opCode).ConfigureAwait(false);
        }

        //Auth
        public async Task ValidateToken(RequestOptions options = null)
        {
            await Send("GET", "auth/login", options: options).ConfigureAwait(false);
        }

        //Gateway
        public async Task<GetGatewayResponse> GetGateway(RequestOptions options = null)
        {
            return await Send<GetGatewayResponse>("GET", "gateway", options: options).ConfigureAwait(false);
        }
        public async Task SendIdentify(int largeThreshold = 100, bool useCompression = true, RequestOptions options = null)
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
            await SendGateway(GatewayOpCode.Identify, msg, options: options).ConfigureAwait(false);
        }

        //Channels
        public async Task<Channel> GetChannel(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            try
            {
                return await Send<Channel>("GET", $"channels/{channelId}", options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<Channel> GetChannel(ulong guildId, ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            try
            {
                var model = await Send<Channel>("GET", $"channels/{channelId}", options: options).ConfigureAwait(false);
                if (model.GuildId != guildId)
                    return null;
                return model;
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<IReadOnlyCollection<Channel>> GetGuildChannels(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<IReadOnlyCollection<Channel>>("GET", $"guilds/{guildId}/channels", options: options).ConfigureAwait(false);
        }
        public async Task<Channel> CreateGuildChannel(ulong guildId, CreateGuildChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args.Bitrate, 0, nameof(args.Bitrate));
            Preconditions.NotNullOrWhitespace(args.Name, nameof(args.Name));

            return await Send<Channel>("POST", $"guilds/{guildId}/channels", args, options: options).ConfigureAwait(false);
        }
        public async Task<Channel> DeleteChannel(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            return await Send<Channel>("DELETE", $"channels/{channelId}", options: options).ConfigureAwait(false);
        }
        public async Task<Channel> ModifyGuildChannel(ulong channelId, ModifyGuildChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Position, 0, nameof(args.Position));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));

            return await Send<Channel>("PATCH", $"channels/{channelId}", args, options: options).ConfigureAwait(false);
        }
        public async Task<Channel> ModifyGuildChannel(ulong channelId, ModifyTextChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Position, 0, nameof(args.Position));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));

            return await Send<Channel>("PATCH", $"channels/{channelId}", args, options: options).ConfigureAwait(false);
        }
        public async Task<Channel> ModifyGuildChannel(ulong channelId, ModifyVoiceChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args.Bitrate, 0, nameof(args.Bitrate));
            Preconditions.AtLeast(args.UserLimit, 0, nameof(args.Bitrate));
            Preconditions.AtLeast(args.Position, 0, nameof(args.Position));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));

            return await Send<Channel>("PATCH", $"channels/{channelId}", args, options: options).ConfigureAwait(false);
        }
        public async Task ModifyGuildChannels(ulong guildId, IEnumerable<ModifyGuildChannelsParams> args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));

            var channels = args.ToArray();
            switch (channels.Length)
            {
                case 0:
                    return;
                case 1:
                    await ModifyGuildChannel(channels[0].Id, new ModifyGuildChannelParams { Position = channels[0].Position }).ConfigureAwait(false);
                    break;
                default:
                    await Send("PATCH", $"guilds/{guildId}/channels", channels, options: options).ConfigureAwait(false);
                    break;
            }
        }

        //Channel Permissions
        public async Task ModifyChannelPermissions(ulong channelId, ulong targetId, ModifyChannelPermissionsParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));

            await Send("PUT", $"channels/{channelId}/permissions/{targetId}", args, options: options).ConfigureAwait(false);
        }
        public async Task DeleteChannelPermission(ulong channelId, ulong targetId, RequestOptions options = null)
        {
            await Send("DELETE", $"channels/{channelId}/permissions/{targetId}", options: options).ConfigureAwait(false);
        }

        //Guilds
        public async Task<Guild> GetGuild(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            try
            {
                return await Send<Guild>("GET", $"guilds/{guildId}", options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<Guild> CreateGuild(CreateGuildParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNullOrWhitespace(args.Name, nameof(args.Name));
            Preconditions.NotNullOrWhitespace(args.Region, nameof(args.Region));

            return await Send<Guild>("POST", "guilds", args, options: options).ConfigureAwait(false);
        }
        public async Task<Guild> DeleteGuild(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<Guild>("DELETE", $"guilds/{guildId}", options: options).ConfigureAwait(false);
        }
        public async Task<Guild> LeaveGuild(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<Guild>("DELETE", $"users/@me/guilds/{guildId}", options: options).ConfigureAwait(false);
        }
        public async Task<Guild> ModifyGuild(ulong guildId, ModifyGuildParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(args.AFKChannelId, 0, nameof(args.AFKChannelId));
            Preconditions.AtLeast(args.AFKTimeout, 0, nameof(args.AFKTimeout));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));
            Preconditions.NotNull(args.Owner, nameof(args.Owner));
            Preconditions.NotNull(args.Region, nameof(args.Region));
            Preconditions.AtLeast(args.VerificationLevel, 0, nameof(args.VerificationLevel));

            return await Send<Guild>("PATCH", $"guilds/{guildId}", args, options: options).ConfigureAwait(false);
        }
        public async Task<GetGuildPruneCountResponse> BeginGuildPrune(ulong guildId, GuildPruneParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Days, 0, nameof(args.Days));

            return await Send<GetGuildPruneCountResponse>("POST", $"guilds/{guildId}/prune", args, options: options).ConfigureAwait(false);
        }
        public async Task<GetGuildPruneCountResponse> GetGuildPruneCount(ulong guildId, GuildPruneParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Days, 0, nameof(args.Days));

            return await Send<GetGuildPruneCountResponse>("GET", $"guilds/{guildId}/prune", args, options: options).ConfigureAwait(false);
        }

        //Guild Bans
        public async Task<IReadOnlyCollection<User>> GetGuildBans(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<IReadOnlyCollection<User>>("GET", $"guilds/{guildId}/bans", options: options).ConfigureAwait(false);
        }
        public async Task CreateGuildBan(ulong guildId, ulong userId, CreateGuildBanParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.PruneDays, 0, nameof(args.PruneDays));

            await Send("PUT", $"guilds/{guildId}/bans/{userId}", args, options: options).ConfigureAwait(false);
        }
        public async Task RemoveGuildBan(ulong guildId, ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));

            await Send("DELETE", $"guilds/{guildId}/bans/{userId}", options: options).ConfigureAwait(false);
        }

        //Guild Embeds
        public async Task<GuildEmbed> GetGuildEmbed(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            try
            {
                return await Send<GuildEmbed>("GET", $"guilds/{guildId}/embed", options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<GuildEmbed> ModifyGuildEmbed(ulong guildId, ModifyGuildEmbedParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<GuildEmbed>("PATCH", $"guilds/{guildId}/embed", args, options: options).ConfigureAwait(false);
        }

        //Guild Integrations
        public async Task<IReadOnlyCollection<Integration>> GetGuildIntegrations(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<IReadOnlyCollection<Integration>>("GET", $"guilds/{guildId}/integrations", options: options).ConfigureAwait(false);
        }
        public async Task<Integration> CreateGuildIntegration(ulong guildId, CreateGuildIntegrationParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(args.Id, 0, nameof(args.Id));

            return await Send<Integration>("POST", $"guilds/{guildId}/integrations", options: options).ConfigureAwait(false);
        }
        public async Task<Integration> DeleteGuildIntegration(ulong guildId, ulong integrationId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(integrationId, 0, nameof(integrationId));

            return await Send<Integration>("DELETE", $"guilds/{guildId}/integrations/{integrationId}", options: options).ConfigureAwait(false);
        }
        public async Task<Integration> ModifyGuildIntegration(ulong guildId, ulong integrationId, ModifyGuildIntegrationParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(integrationId, 0, nameof(integrationId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.ExpireBehavior, 0, nameof(args.ExpireBehavior));
            Preconditions.AtLeast(args.ExpireGracePeriod, 0, nameof(args.ExpireGracePeriod));

            return await Send<Integration>("PATCH", $"guilds/{guildId}/integrations/{integrationId}", args, options: options).ConfigureAwait(false);
        }
        public async Task<Integration> SyncGuildIntegration(ulong guildId, ulong integrationId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(integrationId, 0, nameof(integrationId));

            return await Send<Integration>("POST", $"guilds/{guildId}/integrations/{integrationId}/sync", options: options).ConfigureAwait(false);
        }

        //Guild Invites
        public async Task<Invite> GetInvite(string inviteIdOrXkcd, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(inviteIdOrXkcd, nameof(inviteIdOrXkcd));

            try
            {
                return await Send<Invite>("GET", $"invites/{inviteIdOrXkcd}", options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<IReadOnlyCollection<InviteMetadata>> GetGuildInvites(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<IReadOnlyCollection<InviteMetadata>>("GET", $"guilds/{guildId}/invites", options: options).ConfigureAwait(false);
        }
        public async Task<InviteMetadata[]> GetChannelInvites(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            return await Send<InviteMetadata[]>("GET", $"channels/{channelId}/invites", options: options).ConfigureAwait(false);
        }
        public async Task<InviteMetadata> CreateChannelInvite(ulong channelId, CreateChannelInviteParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.MaxAge, 0, nameof(args.MaxAge));
            Preconditions.AtLeast(args.MaxUses, 0, nameof(args.MaxUses));

            return await Send<InviteMetadata>("POST", $"channels/{channelId}/invites", args, options: options).ConfigureAwait(false);
        }
        public async Task<Invite> DeleteInvite(string inviteCode, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(inviteCode, nameof(inviteCode));

            return await Send<Invite>("DELETE", $"invites/{inviteCode}", options: options).ConfigureAwait(false);
        }
        public async Task AcceptInvite(string inviteCode, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(inviteCode, nameof(inviteCode));

            await Send("POST", $"invites/{inviteCode}", options: options).ConfigureAwait(false);
        }

        //Guild Members
        public async Task<GuildMember> GetGuildMember(ulong guildId, ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));

            try
            {
                return await Send<GuildMember>("GET", $"guilds/{guildId}/members/{userId}", options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<IReadOnlyCollection<GuildMember>> GetGuildMembers(ulong guildId, GetGuildMembersParams args, RequestOptions options = null)
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
                var models = await Send<GuildMember[]>("GET", endpoint, options: options).ConfigureAwait(false);

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
        public async Task RemoveGuildMember(ulong guildId, ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));

            await Send("DELETE", $"guilds/{guildId}/members/{userId}", options: options).ConfigureAwait(false);
        }
        public async Task ModifyGuildMember(ulong guildId, ulong userId, ModifyGuildMemberParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));
            Preconditions.NotNull(args, nameof(args));

            await Send("PATCH", $"guilds/{guildId}/members/{userId}", args, GuildBucket.ModifyMember, guildId, options: options).ConfigureAwait(false);
        }

        //Guild Roles
        public async Task<IReadOnlyCollection<Role>> GetGuildRoles(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<IReadOnlyCollection<Role>>("GET", $"guilds/{guildId}/roles", options: options).ConfigureAwait(false);
        }
        public async Task<Role> CreateGuildRole(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<Role>("POST", $"guilds/{guildId}/roles", options: options).ConfigureAwait(false);
        }
        public async Task DeleteGuildRole(ulong guildId, ulong roleId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(roleId, 0, nameof(roleId));

            await Send("DELETE", $"guilds/{guildId}/roles/{roleId}", options: options).ConfigureAwait(false);
        }
        public async Task<Role> ModifyGuildRole(ulong guildId, ulong roleId, ModifyGuildRoleParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(roleId, 0, nameof(roleId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Color, 0, nameof(args.Color));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));
            Preconditions.AtLeast(args.Position, 0, nameof(args.Position));

            return await Send<Role>("PATCH", $"guilds/{guildId}/roles/{roleId}", args, options: options).ConfigureAwait(false);
        }
        public async Task<IReadOnlyCollection<Role>> ModifyGuildRoles(ulong guildId, IEnumerable<ModifyGuildRolesParams> args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));

            var roles = args.ToArray();
            switch (roles.Length)
            {
                case 0:
                    return ImmutableArray.Create<Role>();
                case 1:
                    return ImmutableArray.Create(await ModifyGuildRole(guildId, roles[0].Id, roles[0]).ConfigureAwait(false));
                default:
                    return await Send<IReadOnlyCollection<Role>>("PATCH", $"guilds/{guildId}/roles", args, options: options).ConfigureAwait(false);
            }
        }

        //Messages
        public async Task<Message> GetChannelMessage(ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));

            //TODO: Improve when Discord adds support
            var msgs = await GetChannelMessages(channelId, new GetChannelMessagesParams { Limit = 1, RelativeDirection = Direction.Before, RelativeMessageId = messageId + 1 }).ConfigureAwait(false);
            return msgs.FirstOrDefault();

            /*try
            {
                return await Send<Message>("GET", $"channels/{channelId}/messages/{messageId}", options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }*/
        }
        public async Task<IReadOnlyCollection<Message>> GetChannelMessages(ulong channelId, GetChannelMessagesParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Limit, 0, nameof(args.Limit));

            int limit = args.Limit;
            ulong? relativeId = args.RelativeMessageId.IsSpecified ? args.RelativeMessageId.Value : (ulong?)null;
            string relativeDir = args.RelativeDirection == Direction.After ? "after" : "before";
            
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
                var models = await Send<Message[]>("GET", endpoint, options: options).ConfigureAwait(false);

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
        public Task<Message> CreateMessage(ulong guildId, ulong channelId, CreateMessageParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return CreateMessageInternal(guildId, channelId, args);
        }
        public Task<Message> CreateDMMessage(ulong channelId, CreateMessageParams args, RequestOptions options = null)
        {
            return CreateMessageInternal(0, channelId, args);
        }
        public async Task<Message> CreateMessageInternal(ulong guildId, ulong channelId, CreateMessageParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNullOrEmpty(args.Content, nameof(args.Content));
            if (args.Content.Length > DiscordConfig.MaxMessageSize)
                throw new ArgumentException($"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.", nameof(args.Content));

            if (guildId != 0)
                return await Send<Message>("POST", $"channels/{channelId}/messages", args, GuildBucket.SendEditMessage, guildId, options: options).ConfigureAwait(false);
            else
                return await Send<Message>("POST", $"channels/{channelId}/messages", args, GlobalBucket.DirectMessage, options: options).ConfigureAwait(false);
        }
        public Task<Message> UploadFile(ulong guildId, ulong channelId, Stream file, UploadFileParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return UploadFileInternal(guildId, channelId, file, args);
        }
        public Task<Message> UploadDMFile(ulong channelId, Stream file, UploadFileParams args, RequestOptions options = null)
        {
            return UploadFileInternal(0, channelId, file, args);
        }
        private async Task<Message> UploadFileInternal(ulong guildId, ulong channelId, Stream file, UploadFileParams args, RequestOptions options = null)
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
                return await Send<Message>("POST", $"channels/{channelId}/messages", file, args.ToDictionary(), GuildBucket.SendEditMessage, guildId, options: options).ConfigureAwait(false);
            else
                return await Send<Message>("POST", $"channels/{channelId}/messages", file, args.ToDictionary(), GlobalBucket.DirectMessage, options: options).ConfigureAwait(false);
        }
        public Task DeleteMessage(ulong guildId, ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return DeleteMessageInternal(guildId, channelId, messageId);
        }
        public Task DeleteDMMessage(ulong channelId, ulong messageId, RequestOptions options = null)
        {
            return DeleteMessageInternal(0, channelId, messageId);
        }
        private async Task DeleteMessageInternal(ulong guildId, ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));

            if (guildId != 0)
                await Send("DELETE", $"channels/{channelId}/messages/{messageId}", GuildBucket.DeleteMessage, guildId, options: options).ConfigureAwait(false);
            else
                await Send("DELETE", $"channels/{channelId}/messages/{messageId}", options: options).ConfigureAwait(false);
        }
        public Task DeleteMessages(ulong guildId, ulong channelId, DeleteMessagesParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return DeleteMessagesInternal(guildId, channelId, args);
        }
        public Task DeleteDMMessages(ulong channelId, DeleteMessagesParams args, RequestOptions options = null)
        {
            return DeleteMessagesInternal(0, channelId, args);
        }
        private async Task DeleteMessagesInternal(ulong guildId, ulong channelId, DeleteMessagesParams args, RequestOptions options = null)
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
                    await DeleteMessageInternal(guildId, channelId, messageIds[0]).ConfigureAwait(false);
                    break;
                default:
                    if (guildId != 0)
                        await Send("POST", $"channels/{channelId}/messages/bulk_delete", args, GuildBucket.DeleteMessages, guildId, options: options).ConfigureAwait(false);
                    else
                        await Send("POST", $"channels/{channelId}/messages/bulk_delete", args, options: options).ConfigureAwait(false);
                    break;
            }
        }
        public Task<Message> ModifyMessage(ulong guildId, ulong channelId, ulong messageId, ModifyMessageParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return ModifyMessageInternal(guildId, channelId, messageId, args);
        }
        public Task<Message> ModifyDMMessage(ulong channelId, ulong messageId, ModifyMessageParams args, RequestOptions options = null)
        {
            return ModifyMessageInternal(0, channelId, messageId, args);
        }
        private async Task<Message> ModifyMessageInternal(ulong guildId, ulong channelId, ulong messageId, ModifyMessageParams args, RequestOptions options = null)
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
                return await Send<Message>("PATCH", $"channels/{channelId}/messages/{messageId}", args, GuildBucket.SendEditMessage, guildId, options: options).ConfigureAwait(false);
            else
                return await Send<Message>("PATCH", $"channels/{channelId}/messages/{messageId}", args, options: options).ConfigureAwait(false);
        }
        public async Task AckMessage(ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));

            await Send("POST", $"channels/{channelId}/messages/{messageId}/ack", options: options).ConfigureAwait(false);
        }
        public async Task TriggerTypingIndicator(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            await Send("POST", $"channels/{channelId}/typing", options: options).ConfigureAwait(false);
        }

        //Users
        public async Task<User> GetUser(ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(userId, 0, nameof(userId));

            try
            {
                return await Send<User>("GET", $"users/{userId}", options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<User> GetUser(string username, string discriminator, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(username, nameof(username));
            Preconditions.NotNullOrEmpty(discriminator, nameof(discriminator));
            
            try
            {
                var models = await QueryUsers($"{username}#{discriminator}", 1, options: options).ConfigureAwait(false);
                return models.FirstOrDefault();
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<IReadOnlyCollection<User>> QueryUsers(string query, int limit, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(query, nameof(query));
            Preconditions.AtLeast(limit, 0, nameof(limit));

            return await Send<IReadOnlyCollection<User>>("GET", $"users?q={Uri.EscapeDataString(query)}&limit={limit}", options: options).ConfigureAwait(false);
        }

        //Current User/DMs
        public async Task<User> GetCurrentUser(RequestOptions options = null)
        {
            return await Send<User>("GET", "users/@me", options: options).ConfigureAwait(false);
        }
        public async Task<IReadOnlyCollection<Connection>> GetCurrentUserConnections(RequestOptions options = null)
        {
            return await Send<IReadOnlyCollection<Connection>>("GET", "users/@me/connections", options: options).ConfigureAwait(false);
        }
        public async Task<IReadOnlyCollection<Channel>> GetCurrentUserDMs(RequestOptions options = null)
        {
            return await Send<IReadOnlyCollection<Channel>>("GET", "users/@me/channels", options: options).ConfigureAwait(false);
        }
        public async Task<IReadOnlyCollection<UserGuild>> GetCurrentUserGuilds(RequestOptions options = null)
        {
            return await Send<IReadOnlyCollection<UserGuild>>("GET", "users/@me/guilds", options: options).ConfigureAwait(false);
        }
        public async Task<User> ModifyCurrentUser(ModifyCurrentUserParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNullOrEmpty(args.Username, nameof(args.Username));

            return await Send<User>("PATCH", "users/@me", args, options: options).ConfigureAwait(false);
        }
        public async Task ModifyCurrentUserNick(ulong guildId, ModifyCurrentUserNickParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEmpty(args.Nickname, nameof(args.Nickname));

            await Send("PATCH", $"guilds/{guildId}/members/@me/nick", args, options: options).ConfigureAwait(false);
        }
        public async Task<Channel> CreateDMChannel(CreateDMChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(args.RecipientId, 0, nameof(args.RecipientId));

            return await Send<Channel>("POST", $"users/@me/channels", args, options: options).ConfigureAwait(false);
        }

        //Voice Regions
        public async Task<IReadOnlyCollection<VoiceRegion>> GetVoiceRegions(RequestOptions options = null)
        {
            return await Send<IReadOnlyCollection<VoiceRegion>>("GET", "voice/regions", options: options).ConfigureAwait(false);
        }
        public async Task<IReadOnlyCollection<VoiceRegion>> GetGuildVoiceRegions(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<IReadOnlyCollection<VoiceRegion>>("GET", $"guilds/{guildId}/regions", options: options).ConfigureAwait(false);
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
