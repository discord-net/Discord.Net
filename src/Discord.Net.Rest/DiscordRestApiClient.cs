using Discord.API.Rest;
using Discord.Net;
using Discord.Net.Converters;
using Discord.Net.Queue;
using Discord.Net.Rest;

using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class DiscordRestApiClient : IDisposable, IAsyncDisposable
    {
        #region DiscordRestApiClient
        private static readonly ConcurrentDictionary<string, Func<BucketIds, BucketId>> _bucketIdGenerators = new ConcurrentDictionary<string, Func<BucketIds, BucketId>>();

        public event Func<string, string, double, Task> SentRequest { add { _sentRequestEvent.Add(value); } remove { _sentRequestEvent.Remove(value); } }
        private readonly AsyncEvent<Func<string, string, double, Task>> _sentRequestEvent = new AsyncEvent<Func<string, string, double, Task>>();

        protected readonly JsonSerializer _serializer;
        protected readonly SemaphoreSlim _stateLock;
        private readonly RestClientProvider _restClientProvider;

        protected bool _isDisposed;
        private CancellationTokenSource _loginCancelToken;

        public RetryMode DefaultRetryMode { get; }
        public string UserAgent { get; }
        internal RequestQueue RequestQueue { get; }

        public LoginState LoginState { get; private set; }
        public TokenType AuthTokenType { get; private set; }
        internal string AuthToken { get; private set; }
        internal IRestClient RestClient { get; private set; }
        internal ulong? CurrentUserId { get; set; }
        internal ulong? CurrentApplicationId { get; set; }
        internal bool UseSystemClock { get; set; }
        internal Func<IRateLimitInfo, Task> DefaultRatelimitCallback { get; set; }
        internal JsonSerializer Serializer => _serializer;

        /// <exception cref="ArgumentException">Unknown OAuth token type.</exception>
        public DiscordRestApiClient(RestClientProvider restClientProvider, string userAgent, RetryMode defaultRetryMode = RetryMode.AlwaysRetry,
            JsonSerializer serializer = null, bool useSystemClock = true, Func<IRateLimitInfo, Task> defaultRatelimitCallback = null)
        {
            _restClientProvider = restClientProvider;
            UserAgent = userAgent;
            DefaultRetryMode = defaultRetryMode;
            _serializer = serializer ?? new JsonSerializer { ContractResolver = new DiscordContractResolver() };
            UseSystemClock = useSystemClock;
            DefaultRatelimitCallback = defaultRatelimitCallback;

            RequestQueue = new RequestQueue();
            _stateLock = new SemaphoreSlim(1, 1);

            SetBaseUrl(DiscordConfig.APIUrl);
        }

        /// <exception cref="ArgumentException">Unknown OAuth token type.</exception>
        internal void SetBaseUrl(string baseUrl)
        {
            RestClient?.Dispose();
            RestClient = _restClientProvider(baseUrl);
            RestClient.SetHeader("accept", "*/*");
            RestClient.SetHeader("user-agent", UserAgent);
            RestClient.SetHeader("authorization", GetPrefixedToken(AuthTokenType, AuthToken));
        }
        /// <exception cref="ArgumentException">Unknown OAuth token type.</exception>
        internal static string GetPrefixedToken(TokenType tokenType, string token)
        {
            return tokenType switch
            {
                TokenType.Bot => $"Bot {token}",
                TokenType.Bearer => $"Bearer {token}",
                _ => throw new ArgumentException(message: "Unknown OAuth token type.", paramName: nameof(tokenType)),
            };
        }
        internal virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _loginCancelToken?.Dispose();
                    RestClient?.Dispose();
                    RequestQueue?.Dispose();
                    _stateLock?.Dispose();
                }
                _isDisposed = true;
            }
        }

        internal virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _loginCancelToken?.Dispose();
                    RestClient?.Dispose();

                    if (!(RequestQueue is null))
                        await RequestQueue.DisposeAsync().ConfigureAwait(false);

                    _stateLock?.Dispose();
                }
                _isDisposed = true;
            }
        }

        public void Dispose() => Dispose(true);

        public ValueTask DisposeAsync() => DisposeAsync(true);

        public async Task LoginAsync(TokenType tokenType, string token, RequestOptions options = null)
        {
            await _stateLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LoginInternalAsync(tokenType, token, options).ConfigureAwait(false);
            }
            finally { _stateLock.Release(); }
        }
        private async Task LoginInternalAsync(TokenType tokenType, string token, RequestOptions options = null)
        {
            if (LoginState != LoginState.LoggedOut)
                await LogoutInternalAsync().ConfigureAwait(false);
            LoginState = LoginState.LoggingIn;

            try
            {
                _loginCancelToken?.Dispose();
                _loginCancelToken = new CancellationTokenSource();

                AuthToken = null;
                await RequestQueue.SetCancelTokenAsync(_loginCancelToken.Token).ConfigureAwait(false);
                RestClient.SetCancelToken(_loginCancelToken.Token);

                AuthTokenType = tokenType;
                AuthToken = token?.TrimEnd();
                if (tokenType != TokenType.Webhook)
                    RestClient.SetHeader("authorization", GetPrefixedToken(AuthTokenType, AuthToken));

                LoginState = LoginState.LoggedIn;
            }
            catch
            {
                await LogoutInternalAsync().ConfigureAwait(false);
                throw;
            }
        }

        public async Task LogoutAsync()
        {
            await _stateLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LogoutInternalAsync().ConfigureAwait(false);
            }
            finally { _stateLock.Release(); }
        }
        private async Task LogoutInternalAsync()
        {
            //An exception here will lock the client into the unusable LoggingOut state, but that's probably fine since our client is in an undefined state too.
            if (LoginState == LoginState.LoggedOut)
                return;
            LoginState = LoginState.LoggingOut;

            try
            { _loginCancelToken?.Cancel(false); }
            catch { }

            await DisconnectInternalAsync(null).ConfigureAwait(false);
            await RequestQueue.ClearAsync().ConfigureAwait(false);

            await RequestQueue.SetCancelTokenAsync(CancellationToken.None).ConfigureAwait(false);
            RestClient.SetCancelToken(CancellationToken.None);

            CurrentUserId = null;
            LoginState = LoginState.LoggedOut;
        }

        internal virtual Task ConnectInternalAsync() => Task.Delay(0);
        internal virtual Task DisconnectInternalAsync(Exception ex = null) => Task.Delay(0);
        #endregion

        #region Core
        internal Task SendAsync(string method, Expression<Func<string>> endpointExpr, BucketIds ids,
             ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null, [CallerMemberName] string funcName = null)
            => SendAsync(method, GetEndpoint(endpointExpr), GetBucketId(method, ids, endpointExpr, funcName), clientBucket, options);
        public Task SendAsync(string method, string endpoint,
            BucketId bucketId = null, ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null)
        {
            options ??= new RequestOptions();
            options.HeaderOnly = true;
            options.BucketId = bucketId;

            var request = new RestRequest(RestClient, method, endpoint, options);
            return SendInternalAsync(method, endpoint, request);
        }

        internal Task SendJsonAsync(string method, Expression<Func<string>> endpointExpr, object payload, BucketIds ids,
             ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null, [CallerMemberName] string funcName = null)
            => SendJsonAsync(method, GetEndpoint(endpointExpr), payload, GetBucketId(method, ids, endpointExpr, funcName), clientBucket, options);
        public Task SendJsonAsync(string method, string endpoint, object payload,
            BucketId bucketId = null, ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null)
        {
            options ??= new RequestOptions();
            options.HeaderOnly = true;
            options.BucketId = bucketId;

            string json = payload != null ? SerializeJson(payload) : null;
            var request = new JsonRestRequest(RestClient, method, endpoint, json, options);
            return SendInternalAsync(method, endpoint, request);
        }

        internal Task SendMultipartAsync(string method, Expression<Func<string>> endpointExpr, IReadOnlyDictionary<string, object> multipartArgs, BucketIds ids,
             ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null, [CallerMemberName] string funcName = null)
            => SendMultipartAsync(method, GetEndpoint(endpointExpr), multipartArgs, GetBucketId(method, ids, endpointExpr, funcName), clientBucket, options);
        public Task SendMultipartAsync(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs,
            BucketId bucketId = null, ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null)
        {
            options ??= new RequestOptions();
            options.HeaderOnly = true;
            options.BucketId = bucketId;

            var request = new MultipartRestRequest(RestClient, method, endpoint, multipartArgs, options);
            return SendInternalAsync(method, endpoint, request);
        }

        internal Task<TResponse> SendAsync<TResponse>(string method, Expression<Func<string>> endpointExpr, BucketIds ids,
             ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null, [CallerMemberName] string funcName = null) where TResponse : class
            => SendAsync<TResponse>(method, GetEndpoint(endpointExpr), GetBucketId(method, ids, endpointExpr, funcName), clientBucket, options);
        public async Task<TResponse> SendAsync<TResponse>(string method, string endpoint,
            BucketId bucketId = null, ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null) where TResponse : class
        {
            options ??= new RequestOptions();
            options.BucketId = bucketId;

            var request = new RestRequest(RestClient, method, endpoint, options);
            return DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, request).ConfigureAwait(false));
        }

        internal Task<TResponse> SendJsonAsync<TResponse>(string method, Expression<Func<string>> endpointExpr, object payload, BucketIds ids,
             ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null, [CallerMemberName] string funcName = null) where TResponse : class
            => SendJsonAsync<TResponse>(method, GetEndpoint(endpointExpr), payload, GetBucketId(method, ids, endpointExpr, funcName), clientBucket, options);
        public async Task<TResponse> SendJsonAsync<TResponse>(string method, string endpoint, object payload,
            BucketId bucketId = null, ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null) where TResponse : class
        {
            options ??= new RequestOptions();
            options.BucketId = bucketId;

            string json = payload != null ? SerializeJson(payload) : null;

            var request = new JsonRestRequest(RestClient, method, endpoint, json, options);
            return DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, request).ConfigureAwait(false));
        }

        internal Task<TResponse> SendMultipartAsync<TResponse>(string method, Expression<Func<string>> endpointExpr, IReadOnlyDictionary<string, object> multipartArgs, BucketIds ids,
             ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null, [CallerMemberName] string funcName = null)
            => SendMultipartAsync<TResponse>(method, GetEndpoint(endpointExpr), multipartArgs, GetBucketId(method, ids, endpointExpr, funcName), clientBucket, options);
        public async Task<TResponse> SendMultipartAsync<TResponse>(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs,
            BucketId bucketId = null, ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null)
        {
            options ??= new RequestOptions();
            options.BucketId = bucketId;

            var request = new MultipartRestRequest(RestClient, method, endpoint, multipartArgs, options);
            return DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, request).ConfigureAwait(false));
        }

        private async Task<Stream> SendInternalAsync(string method, string endpoint, RestRequest request)
        {
            if (!request.Options.IgnoreState)
                CheckState();

            request.Options.RetryMode ??= DefaultRetryMode;
            request.Options.UseSystemClock ??= UseSystemClock;
            request.Options.RatelimitCallback ??= DefaultRatelimitCallback;

            var stopwatch = Stopwatch.StartNew();
            var responseStream = await RequestQueue.SendAsync(request).ConfigureAwait(false);
            stopwatch.Stop();

            double milliseconds = ToMilliseconds(stopwatch);
            await _sentRequestEvent.InvokeAsync(method, endpoint, milliseconds).ConfigureAwait(false);

            return responseStream;
        }
        #endregion

        #region Auth
        public Task ValidateTokenAsync(RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            return SendAsync("GET", () => "auth/login", new BucketIds(), options: options);
        }
        #endregion

        #region Gateway
        public Task<GetGatewayResponse> GetGatewayAsync(RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            return SendAsync<GetGatewayResponse>("GET", () => "gateway", new BucketIds(), options: options);
        }
        public Task<GetBotGatewayResponse> GetBotGatewayAsync(RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            return SendAsync<GetBotGatewayResponse>("GET", () => "gateway/bot", new BucketIds(), options: options);
        }
        #endregion

        #region Channels
        public async Task<Channel> GetChannelAsync(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            options = RequestOptions.CreateOrClone(options);

            try
            {
                var ids = new BucketIds(channelId: channelId);
                return await SendAsync<Channel>("GET", () => $"channels/{channelId}", ids, options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<Channel> GetChannelAsync(ulong guildId, ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            options = RequestOptions.CreateOrClone(options);

            try
            {
                var ids = new BucketIds(channelId: channelId);
                var model = await SendAsync<Channel>("GET", () => $"channels/{channelId}", ids, options: options).ConfigureAwait(false);
                if (!model.GuildId.IsSpecified || model.GuildId.Value != guildId)
                    return null;

                return model;
            }
            catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) { return null; }
        }
        public Task<IReadOnlyCollection<Channel>> GetGuildChannelsAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync<IReadOnlyCollection<Channel>>("GET", () => $"guilds/{guildId}/channels", ids, options: options);
        }
        public Task<Channel> CreateGuildChannelAsync(ulong guildId, CreateGuildChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args.Bitrate, 0, nameof(args.Bitrate));
            Preconditions.NotNullOrWhitespace(args.Name, nameof(args.Name));
            Preconditions.AtMost(args.Name.Length, 100, nameof(args.Name));
            if (args.Topic is { IsSpecified: true, Value: not null })
                Preconditions.AtMost(args.Topic.Value.Length, 1024, nameof(args.Name));

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendJsonAsync<Channel>("POST", () => $"guilds/{guildId}/channels", args, ids, options: options);
        }

        public Task<Channel> DeleteChannelAsync(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendAsync<Channel>("DELETE", () => $"channels/{channelId}", ids, options: options);
        }
        /// <exception cref="ArgumentException">
        /// <paramref name="channelId"/> must not be equal to zero.
        /// -and-
        /// <paramref name="args.Position"/> must be greater than zero.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="args"/> must not be <see langword="null"/>.
        /// -and-
        /// <paramref name="args.Name"/> must not be <see langword="null"/> or empty.
        /// </exception>
        public Task<Channel> ModifyGuildChannelAsync(ulong channelId, Rest.ModifyGuildChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Position, 0, nameof(args.Position));
            Preconditions.NotNullOrWhitespace(args.Name, nameof(args.Name));

            if (args.Name.IsSpecified)
                Preconditions.AtMost(args.Name.Value.Length, 100, nameof(args.Name));

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendJsonAsync<Channel>("PATCH", () => $"channels/{channelId}", args, ids, options: options);
        }

        public async Task<Channel> ModifyGuildChannelAsync(ulong channelId, Rest.ModifyTextChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Position, 0, nameof(args.Position));
            Preconditions.NotNullOrWhitespace(args.Name, nameof(args.Name));

            if (args.Name.IsSpecified)
                Preconditions.AtMost(args.Name.Value.Length, 100, nameof(args.Name));
            if (args.Topic.IsSpecified)
                Preconditions.AtMost(args.Topic.Value.Length, 1024, nameof(args.Name));

            Preconditions.AtLeast(args.SlowModeInterval, 0, nameof(args.SlowModeInterval));
            Preconditions.AtMost(args.SlowModeInterval, 21600, nameof(args.SlowModeInterval));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return await SendJsonAsync<Channel>("PATCH", () => $"channels/{channelId}", args, ids, options: options).ConfigureAwait(false);
        }

        public Task<Channel> ModifyGuildChannelAsync(ulong channelId, Rest.ModifyVoiceChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Bitrate, 8000, nameof(args.Bitrate));
            Preconditions.AtLeast(args.UserLimit, 0, nameof(args.UserLimit));
            Preconditions.AtLeast(args.Position, 0, nameof(args.Position));
            Preconditions.NotNullOrWhitespace(args.Name, nameof(args.Name));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendJsonAsync<Channel>("PATCH", () => $"channels/{channelId}", args, ids, options: options);
        }

        public Task ModifyGuildChannelsAsync(ulong guildId, IEnumerable<Rest.ModifyGuildChannelsParams> args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            options = RequestOptions.CreateOrClone(options);

            var channels = args.ToArray();
            switch (channels.Length)
            {
                case 0:
                    return Task.CompletedTask;
                case 1:
                    return ModifyGuildChannelAsync(channels[0].Id, new Rest.ModifyGuildChannelParams { Position = channels[0].Position });
                default:
                    var ids = new BucketIds(guildId: guildId);
                    return SendJsonAsync("PATCH", () => $"guilds/{guildId}/channels", channels, ids, options: options);
            }
        }

        public async Task ModifyVoiceChannelStatusAsync(ulong channelId, string status, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            var payload = new ModifyVoiceStatusParams { Status = status };
            var ids = new BucketIds();

            await SendJsonAsync("PUT", () => $"channels/{channelId}/voice-status", payload, ids, options: options);
        }

        #endregion

        #region Threads
        public Task<Channel> CreatePostAsync(ulong channelId, CreatePostParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            var bucket = new BucketIds(channelId: channelId);

            return SendJsonAsync<Channel>("POST", () => $"channels/{channelId}/threads", args, bucket, options: options);
        }

        public Task<Channel> CreatePostAsync(ulong channelId, CreateMultipartPostAsync args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            var bucket = new BucketIds(channelId: channelId);

            return SendMultipartAsync<Channel>("POST", () => $"channels/{channelId}/threads", args.ToDictionary(), bucket, options: options);
        }

        public Task<Channel> ModifyThreadAsync(ulong channelId, ModifyThreadParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            var bucket = new BucketIds(channelId: channelId);

            return SendJsonAsync<Channel>("PATCH", () => $"channels/{channelId}", args, bucket, options: options);
        }

        public Task<Channel> StartThreadAsync(ulong channelId, ulong messageId, StartThreadParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(0, channelId);

            return SendJsonAsync<Channel>("POST", () => $"channels/{channelId}/messages/{messageId}/threads", args, bucket, options: options);
        }

        public Task<Channel> StartThreadAsync(ulong channelId, StartThreadParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(channelId: channelId);

            return SendJsonAsync<Channel>("POST", () => $"channels/{channelId}/threads", args, bucket, options: options);
        }

        public Task JoinThreadAsync(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(channelId: channelId);

            return SendAsync("PUT", () => $"channels/{channelId}/thread-members/@me", bucket, options: options);
        }

        public Task AddThreadMemberAsync(ulong channelId, ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(userId, 0, nameof(channelId));

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(channelId: channelId);

            return SendAsync("PUT", () => $"channels/{channelId}/thread-members/{userId}", bucket, options: options);
        }

        public Task LeaveThreadAsync(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(channelId: channelId);

            return SendAsync("DELETE", () => $"channels/{channelId}/thread-members/@me", bucket, options: options);
        }

        public Task RemoveThreadMemberAsync(ulong channelId, ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(userId, 0, nameof(channelId));

            options = RequestOptions.CreateOrClone(options);
            var bucket = new BucketIds(channelId: channelId);

            return SendAsync("DELETE", () => $"channels/{channelId}/thread-members/{userId}", bucket, options: options);
        }

        public async Task<ThreadMember[]> ListThreadMembersAsync(ulong channelId, ulong? after = null, int? limit = null, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            var query = "?with_member=true";

            if (limit.HasValue)
                query += $"&limit={limit}";
            if (after.HasValue)
                query += $"&after={after}";

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(channelId: channelId);

            return await SendAsync<ThreadMember[]>("GET", () => $"channels/{channelId}/thread-members{query}", bucket, options: options).ConfigureAwait(false);
        }

        public Task<ThreadMember> GetThreadMemberAsync(ulong channelId, ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(userId, 0, nameof(userId));

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(channelId: channelId);
            var query = "?with_member=true";

            return SendAsync<ThreadMember>("GET", () => $"channels/{channelId}/thread-members/{userId}{query}", bucket, options: options);
        }

        public Task<ChannelThreads> GetActiveThreadsAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(guildId: guildId);

            return SendAsync<ChannelThreads>("GET", () => $"guilds/{guildId}/threads/active", bucket, options: options);
        }

        public Task<ChannelThreads> GetPublicArchivedThreadsAsync(ulong channelId, DateTimeOffset? before = null, int? limit = null, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(channelId: channelId);

            string query = "";

            if (limit.HasValue)
            {
                string beforeEncoded = WebUtility.UrlEncode(before.GetValueOrDefault(DateTimeOffset.UtcNow).ToString("O"));
                query = $"?before={beforeEncoded}&limit={limit.Value}";
            }
            else if (before.HasValue)
            {
                string beforeEncoded = WebUtility.UrlEncode(before.Value.ToString("O"));
                query = $"?before={beforeEncoded}";
            }

            return SendAsync<ChannelThreads>("GET", () => $"channels/{channelId}/threads/archived/public{query}", bucket, options: options);
        }

        public Task<ChannelThreads> GetPrivateArchivedThreadsAsync(ulong channelId, DateTimeOffset? before = null, int? limit = null,
            RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(channelId: channelId);

            string query = "";

            if (limit.HasValue)
            {
                string beforeEncoded = WebUtility.UrlEncode(before.GetValueOrDefault(DateTimeOffset.UtcNow).ToString("O"));
                query = $"?before={beforeEncoded}&limit={limit.Value}";
            }
            else if (before.HasValue)
            {
                string beforeEncoded = WebUtility.UrlEncode(before.Value.ToString("O"));
                query = $"?before={beforeEncoded}";
            }

            return SendAsync<ChannelThreads>("GET", () => $"channels/{channelId}/threads/archived/private{query}", bucket, options: options);
        }

        public Task<ChannelThreads> GetJoinedPrivateArchivedThreadsAsync(ulong channelId, DateTimeOffset? before = null, int? limit = null,
            RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(channelId: channelId);

            string query = "";

            if (limit.HasValue)
            {
                query = $"?before={SnowflakeUtils.ToSnowflake(before.GetValueOrDefault(DateTimeOffset.UtcNow))}&limit={limit.Value}";
            }
            else if (before.HasValue)
            {
                query = $"?before={before.Value.ToString("O")}";
            }

            return SendAsync<ChannelThreads>("GET", () => $"channels/{channelId}/users/@me/threads/archived/private{query}", bucket, options: options);
        }
        #endregion

        #region Stage
        public Task<StageInstance> CreateStageInstanceAsync(CreateStageInstanceParams args, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds();

            return SendJsonAsync<StageInstance>("POST", () => $"stage-instances", args, bucket, options: options);
        }

        public Task<StageInstance> ModifyStageInstanceAsync(ulong channelId, ModifyStageInstanceParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(channelId: channelId);

            return SendJsonAsync<StageInstance>("PATCH", () => $"stage-instances/{channelId}", args, bucket, options: options);
        }

        public async Task DeleteStageInstanceAsync(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(channelId: channelId);

            try
            {
                await SendAsync("DELETE", () => $"stage-instances/{channelId}", bucket, options: options).ConfigureAwait(false);
            }
            catch (HttpException httpEx) when (httpEx.HttpCode == HttpStatusCode.NotFound) { }
        }

        public async Task<StageInstance> GetStageInstanceAsync(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(channelId: channelId);

            try
            {
                return await SendAsync<StageInstance>("POST", () => $"stage-instances/{channelId}", bucket, options: options).ConfigureAwait(false);
            }
            catch (HttpException httpEx) when (httpEx.HttpCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public Task ModifyMyVoiceState(ulong guildId, ModifyVoiceStateParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds();

            return SendJsonAsync("PATCH", () => $"guilds/{guildId}/voice-states/@me", args, bucket, options: options);
        }

        public Task ModifyUserVoiceState(ulong guildId, ulong userId, ModifyVoiceStateParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds();

            return SendJsonAsync("PATCH", () => $"guilds/{guildId}/voice-states/{userId}", args, bucket, options: options);
        }
        #endregion

        #region Roles
        public Task AddRoleAsync(ulong guildId, ulong userId, ulong roleId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));
            Preconditions.NotEqual(roleId, 0, nameof(roleId));
            Preconditions.NotEqual(roleId, guildId, nameof(roleId), "The Everyone role cannot be added to a user.");
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync("PUT", () => $"guilds/{guildId}/members/{userId}/roles/{roleId}", ids, options: options);
        }
        public Task RemoveRoleAsync(ulong guildId, ulong userId, ulong roleId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));
            Preconditions.NotEqual(roleId, 0, nameof(roleId));
            Preconditions.NotEqual(roleId, guildId, nameof(roleId), "The Everyone role cannot be removed from a user.");
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync("DELETE", () => $"guilds/{guildId}/members/{userId}/roles/{roleId}", ids, options: options);
        }
        #endregion

        #region Channel Messages
        public async Task<Message> GetChannelMessageAsync(ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));
            options = RequestOptions.CreateOrClone(options);

            try
            {
                var ids = new BucketIds(channelId: channelId);
                return await SendAsync<Message>("GET", () => $"channels/{channelId}/messages/{messageId}", ids, options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) { return null; }
        }
        public Task<IReadOnlyCollection<Message>> GetChannelMessagesAsync(ulong channelId, GetChannelMessagesParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Limit, 0, nameof(args.Limit));
            Preconditions.AtMost(args.Limit, DiscordConfig.MaxMessagesPerBatch, nameof(args.Limit));
            options = RequestOptions.CreateOrClone(options);

            int limit = args.Limit.GetValueOrDefault(DiscordConfig.MaxMessagesPerBatch);
            ulong? relativeId = args.RelativeMessageId.IsSpecified ? args.RelativeMessageId.Value : (ulong?)null;
            var relativeDir = args.RelativeDirection.GetValueOrDefault(Direction.Before) switch
            {
                Direction.After => "after",
                Direction.Around => "around",
                _ => "before",
            };
            var ids = new BucketIds(channelId: channelId);
            Expression<Func<string>> endpoint;
            if (relativeId != null)
                endpoint = () => $"channels/{channelId}/messages?limit={limit}&{relativeDir}={relativeId}";
            else
                endpoint = () => $"channels/{channelId}/messages?limit={limit}";

            return SendAsync<IReadOnlyCollection<Message>>("GET", endpoint, ids, options: options);
        }

        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        public Task<Message> CreateMessageAsync(ulong channelId, CreateMessageParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            if (args.Content.IsSpecified && args.Content.Value is not null)
                Preconditions.AtMost(args.Content.Value.Length, DiscordConfig.MaxMessageSize, nameof(args.Content), $"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.");

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendJsonAsync<Message>("POST", () => $"channels/{channelId}/messages", args, ids, clientBucket: ClientBucketType.SendEdit, options: options);
        }

        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="InvalidOperationException">This operation may only be called with a <see cref="TokenType.Webhook"/> token.</exception>
        public Task<Message> CreateWebhookMessageAsync(ulong webhookId, CreateWebhookMessageParams args, RequestOptions options = null, ulong? threadId = null)
        {
            if (AuthTokenType != TokenType.Webhook)
                throw new InvalidOperationException($"This operation may only be called with a {nameof(TokenType.Webhook)} token.");

            if (args.Embeds.IsSpecified)
                Preconditions.AtMost(args.Embeds.Value.Length, DiscordConfig.MaxEmbedsPerMessage, nameof(args.Embeds), "A max of 10 Embeds are allowed.");
            if (args.Content.IsSpecified && args.Content.Value is not null)
                Preconditions.AtMost(args.Content.Value.Length, DiscordConfig.MaxMessageSize, nameof(args.Content), $"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.");

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(webhookId: webhookId);
            return SendJsonAsync<Message>("POST", () => $"webhooks/{webhookId}/{AuthToken}?{WebhookQuery(true, threadId)}", args, ids, clientBucket: ClientBucketType.SendEdit, options: options);
        }

        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="InvalidOperationException">This operation may only be called with a <see cref="TokenType.Webhook"/> token.</exception>
        public async Task ModifyWebhookMessageAsync(ulong webhookId, ulong messageId, ModifyWebhookMessageParams args, RequestOptions options = null, ulong? threadId = null)
        {
            if (AuthTokenType != TokenType.Webhook)
                throw new InvalidOperationException($"This operation may only be called with a {nameof(TokenType.Webhook)} token.");

            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(webhookId, 0, nameof(webhookId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));

            if (args.Embeds.IsSpecified)
                Preconditions.AtMost(args.Embeds.Value.Length, DiscordConfig.MaxEmbedsPerMessage, nameof(args.Embeds), $"A max of {DiscordConfig.MaxEmbedsPerMessage} Embeds are allowed.");
            if (args.Content.IsSpecified && args.Content.Value is not null)
                Preconditions.AtMost(args.Content.Value.Length, DiscordConfig.MaxMessageSize, nameof(args.Content), $"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.");

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(webhookId: webhookId);
            await SendJsonAsync<Message>("PATCH", () => $"webhooks/{webhookId}/{AuthToken}/messages/{messageId}?{WebhookQuery(false, threadId)}", args, ids, clientBucket: ClientBucketType.SendEdit, options: options).ConfigureAwait(false);
        }

        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="InvalidOperationException">This operation may only be called with a <see cref="TokenType.Webhook"/> token.</exception>
        public Task ModifyWebhookMessageAsync(ulong webhookId, ulong messageId, UploadWebhookFileParams args, RequestOptions options = null, ulong? threadId = null)
        {
            if (AuthTokenType != TokenType.Webhook)
                throw new InvalidOperationException($"This operation may only be called with a {nameof(TokenType.Webhook)} token.");

            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(webhookId, 0, nameof(webhookId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));

            if (args.Embeds.IsSpecified)
                Preconditions.AtMost(args.Embeds.Value.Length, DiscordConfig.MaxEmbedsPerMessage, nameof(args.Embeds), $"A max of {DiscordConfig.MaxEmbedsPerMessage} Embeds are allowed.");
            if (args.Content.IsSpecified && args.Content.Value is not null)
                Preconditions.AtMost(args.Content.Value.Length, DiscordConfig.MaxMessageSize, nameof(args.Content), $"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.");

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(webhookId: webhookId);
            return SendMultipartAsync<Message>("PATCH", () => $"webhooks/{webhookId}/{AuthToken}/messages/{messageId}?{WebhookQuery(false, threadId)}", args.ToDictionary(), ids, clientBucket: ClientBucketType.SendEdit, options: options);
        }

        /// <exception cref="InvalidOperationException">This operation may only be called with a <see cref="TokenType.Webhook"/> token.</exception>
        public Task DeleteWebhookMessageAsync(ulong webhookId, ulong messageId, RequestOptions options = null, ulong? threadId = null)
        {
            if (AuthTokenType != TokenType.Webhook)
                throw new InvalidOperationException($"This operation may only be called with a {nameof(TokenType.Webhook)} token.");

            Preconditions.NotEqual(webhookId, 0, nameof(webhookId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(webhookId: webhookId);
            return SendAsync("DELETE", () => $"webhooks/{webhookId}/{AuthToken}/messages/{messageId}?{WebhookQuery(false, threadId)}", ids, options: options);
        }

        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        public Task<Message> UploadFileAsync(ulong channelId, UploadFileParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            options = RequestOptions.CreateOrClone(options);

            if (args.Embeds.IsSpecified)
                Preconditions.AtMost(args.Embeds.Value.Length, DiscordConfig.MaxEmbedsPerMessage, nameof(args.Embeds), $"A max of {DiscordConfig.MaxEmbedsPerMessage} Embeds are allowed.");
            if (args.Content.IsSpecified && args.Content.Value is not null)
                Preconditions.AtMost(args.Content.Value.Length, DiscordConfig.MaxMessageSize, nameof(args.Content), $"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.");

            var ids = new BucketIds(channelId: channelId);
            return SendMultipartAsync<Message>("POST", () => $"channels/{channelId}/messages", args.ToDictionary(), ids, clientBucket: ClientBucketType.SendEdit, options: options);
        }

        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="InvalidOperationException">This operation may only be called with a <see cref="TokenType.Webhook"/> token.</exception>
        public Task<Message> UploadWebhookFileAsync(ulong webhookId, UploadWebhookFileParams args, RequestOptions options = null, ulong? threadId = null)
        {
            if (AuthTokenType != TokenType.Webhook)
                throw new InvalidOperationException($"This operation may only be called with a {nameof(TokenType.Webhook)} token.");

            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(webhookId, 0, nameof(webhookId));
            options = RequestOptions.CreateOrClone(options);

            if (args.Embeds.IsSpecified)
                Preconditions.AtMost(args.Embeds.Value.Length, DiscordConfig.MaxEmbedsPerMessage, nameof(args.Embeds), $"A max of {DiscordConfig.MaxEmbedsPerMessage} Embeds are allowed.");
            if (args.Content.IsSpecified && args.Content.Value is not null)
                Preconditions.AtMost(args.Content.Value.Length, DiscordConfig.MaxMessageSize, nameof(args.Content), $"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.");

            var ids = new BucketIds(webhookId: webhookId);
            return SendMultipartAsync<Message>("POST", () => $"webhooks/{webhookId}/{AuthToken}?{WebhookQuery(true, threadId)}", args.ToDictionary(), ids, clientBucket: ClientBucketType.SendEdit, options: options);
        }

        public Task DeleteMessageAsync(ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendAsync("DELETE", () => $"channels/{channelId}/messages/{messageId}", ids, options: options);
        }

        public Task DeleteMessagesAsync(ulong channelId, DeleteMessagesParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNull(args.MessageIds, nameof(args.MessageIds));
            Preconditions.AtMost(args.MessageIds.Length, 100, nameof(args.MessageIds.Length));
            Preconditions.YoungerThanTwoWeeks(args.MessageIds, nameof(args.MessageIds));
            options = RequestOptions.CreateOrClone(options);

            switch (args.MessageIds.Length)
            {
                case 0:
                    return Task.CompletedTask;
                case 1:
                    return DeleteMessageAsync(channelId, args.MessageIds[0]);
                default:
                    var ids = new BucketIds(channelId: channelId);
                    return SendJsonAsync("POST", () => $"channels/{channelId}/messages/bulk-delete", args, ids, options: options);
            }
        }
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        public Task<Message> ModifyMessageAsync(ulong channelId, ulong messageId, Rest.ModifyMessageParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));
            Preconditions.NotNull(args, nameof(args));

            if (args.Embeds.IsSpecified)
                Preconditions.AtMost(args.Embeds.Value.Length, DiscordConfig.MaxEmbedsPerMessage, nameof(args.Embeds), "A max of 10 Embeds are allowed.");
            if (args.Content.IsSpecified && args.Content.Value is not null)
                Preconditions.AtMost(args.Content.Value.Length, DiscordConfig.MaxMessageSize, nameof(args.Content), $"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.");

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendJsonAsync<Message>("PATCH", () => $"channels/{channelId}/messages/{messageId}", args, ids, clientBucket: ClientBucketType.SendEdit, options: options);
        }

        public Task<Message> ModifyMessageAsync(ulong channelId, ulong messageId, Rest.UploadFileParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));
            Preconditions.NotNull(args, nameof(args));

            if (args.Embeds.IsSpecified)
                Preconditions.AtMost(args.Embeds.Value.Length, DiscordConfig.MaxEmbedsPerMessage, nameof(args.Embeds), "A max of 10 Embeds are allowed.");
            if (args.Content.IsSpecified && args.Content.Value is not null)
                Preconditions.AtMost(args.Content.Value.Length, DiscordConfig.MaxMessageSize, nameof(args.Content), $"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.");

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendMultipartAsync<Message>("PATCH", () => $"channels/{channelId}/messages/{messageId}", args.ToDictionary(), ids, clientBucket: ClientBucketType.SendEdit, options: options);
        }
        #endregion

        #region Stickers, Reactions, Crosspost, and Acks
        public Task<Sticker> GetStickerAsync(ulong id, RequestOptions options = null)
        {
            Preconditions.NotEqual(id, 0, nameof(id));

            options = RequestOptions.CreateOrClone(options);

            return NullifyNotFound(SendAsync<Sticker>("GET", () => $"stickers/{id}", new BucketIds(), options: options));
        }

        public Task<Sticker> GetGuildStickerAsync(ulong guildId, ulong id, RequestOptions options = null)
        {
            Preconditions.NotEqual(id, 0, nameof(id));
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            options = RequestOptions.CreateOrClone(options);

            return NullifyNotFound(SendAsync<Sticker>("GET", () => $"guilds/{guildId}/stickers/{id}", new BucketIds(guildId), options: options));
        }

        public Task<Sticker[]> ListGuildStickersAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            options = RequestOptions.CreateOrClone(options);

            return SendAsync<Sticker[]>("GET", () => $"guilds/{guildId}/stickers", new BucketIds(guildId), options: options);
        }

        public Task<NitroStickerPacks> ListNitroStickerPacksAsync(RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            return SendAsync<NitroStickerPacks>("GET", () => $"sticker-packs", new BucketIds(), options: options);
        }

        public Task<Sticker> CreateGuildStickerAsync(CreateStickerParams args, ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            options = RequestOptions.CreateOrClone(options);

            return SendMultipartAsync<Sticker>("POST", () => $"guilds/{guildId}/stickers", args.ToDictionary(), new BucketIds(guildId), options: options);
        }

        public Task<Sticker> ModifyStickerAsync(ModifyStickerParams args, ulong guildId, ulong stickerId, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(stickerId, 0, nameof(stickerId));

            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<Sticker>("PATCH", () => $"guilds/{guildId}/stickers/{stickerId}", args, new BucketIds(guildId), options: options);
        }

        public Task DeleteStickerAsync(ulong guildId, ulong stickerId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(stickerId, 0, nameof(stickerId));

            options = RequestOptions.CreateOrClone(options);

            return SendAsync("DELETE", () => $"guilds/{guildId}/stickers/{stickerId}", new BucketIds(guildId), options: options);
        }

        public Task AddReactionAsync(ulong channelId, ulong messageId, string emoji, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));
            Preconditions.NotNullOrWhitespace(emoji, nameof(emoji));

            options = RequestOptions.CreateOrClone(options);
            options.IsReactionBucket = true;

            var ids = new BucketIds(channelId: channelId);

            // @me is non-const to fool the ratelimiter, otherwise it will put add/remove in separate buckets
            var me = "@me";
            return SendAsync("PUT", () => $"channels/{channelId}/messages/{messageId}/reactions/{emoji}/{me}", ids, options: options);
        }

        public Task RemoveReactionAsync(ulong channelId, ulong messageId, ulong userId, string emoji, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));
            Preconditions.NotNullOrWhitespace(emoji, nameof(emoji));

            options = RequestOptions.CreateOrClone(options);
            options.IsReactionBucket = true;

            var ids = new BucketIds(channelId: channelId);

            var user = CurrentUserId.HasValue ? (userId == CurrentUserId.Value ? "@me" : userId.ToString()) : userId.ToString();
            return SendAsync("DELETE", () => $"channels/{channelId}/messages/{messageId}/reactions/{emoji}/{user}", ids, options: options);
        }

        public Task RemoveAllReactionsAsync(ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);

            return SendAsync("DELETE", () => $"channels/{channelId}/messages/{messageId}/reactions", ids, options: options);
        }

        public Task RemoveAllReactionsForEmoteAsync(ulong channelId, ulong messageId, string emoji, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));
            Preconditions.NotNullOrWhitespace(emoji, nameof(emoji));

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);

            return SendAsync("DELETE", () => $"channels/{channelId}/messages/{messageId}/reactions/{emoji}", ids, options: options);
        }

        public Task<IReadOnlyCollection<User>> GetReactionUsersAsync(ulong channelId, ulong messageId, string emoji, GetReactionUsersParams args, ReactionType reactionType, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));
            Preconditions.NotNullOrWhitespace(emoji, nameof(emoji));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args.Limit, 0, nameof(args.Limit));
            Preconditions.AtMost(args.Limit, DiscordConfig.MaxUserReactionsPerBatch, nameof(args.Limit));
            Preconditions.GreaterThan(args.AfterUserId, 0, nameof(args.AfterUserId));
            options = RequestOptions.CreateOrClone(options);

            int limit = args.Limit.GetValueOrDefault(DiscordConfig.MaxUserReactionsPerBatch);
            ulong afterUserId = args.AfterUserId.GetValueOrDefault(0);

            var ids = new BucketIds(channelId: channelId);
            Expression<Func<string>> endpoint = () => $"channels/{channelId}/messages/{messageId}/reactions/{emoji}?limit={limit}&after={afterUserId}&type={(int)reactionType}";
            return SendAsync<IReadOnlyCollection<User>>("GET", endpoint, ids, options: options);
        }

        public Task AckMessageAsync(ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendAsync("POST", () => $"channels/{channelId}/messages/{messageId}/ack", ids, options: options);
        }

        public Task TriggerTypingIndicatorAsync(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendAsync("POST", () => $"channels/{channelId}/typing", ids, options: options);
        }

        public Task CrosspostAsync(ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendAsync("POST", () => $"channels/{channelId}/messages/{messageId}/crosspost", ids, options: options);
        }

        public Task<FollowedChannel> FollowChannelAsync(ulong newsChannelId, ulong followingChannelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(newsChannelId, 0, nameof(newsChannelId));
            Preconditions.NotEqual(followingChannelId, 0, nameof(followingChannelId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: newsChannelId);
            return SendJsonAsync<FollowedChannel>("POST", () => $"channels/{newsChannelId}/followers", new { webhook_channel_id = followingChannelId }, ids, options: options);
        }
        #endregion

        #region Channel Permissions
        public Task ModifyChannelPermissionsAsync(ulong channelId, ulong targetId, ModifyChannelPermissionsParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(targetId, 0, nameof(targetId));
            Preconditions.NotNull(args, nameof(args));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendJsonAsync("PUT", () => $"channels/{channelId}/permissions/{targetId}", args, ids, options: options);
        }

        public Task DeleteChannelPermissionAsync(ulong channelId, ulong targetId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(targetId, 0, nameof(targetId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendAsync("DELETE", () => $"channels/{channelId}/permissions/{targetId}", ids, options: options);
        }
        #endregion

        #region Channel Pins
        public Task AddPinAsync(ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.GreaterThan(channelId, 0, nameof(channelId));
            Preconditions.GreaterThan(messageId, 0, nameof(messageId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendAsync("PUT", () => $"channels/{channelId}/pins/{messageId}", ids, options: options);
        }

        public Task RemovePinAsync(ulong channelId, ulong messageId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendAsync("DELETE", () => $"channels/{channelId}/pins/{messageId}", ids, options: options);
        }

        public Task<IReadOnlyCollection<Message>> GetPinsAsync(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendAsync<IReadOnlyCollection<Message>>("GET", () => $"channels/{channelId}/pins", ids, options: options);
        }
        #endregion

        #region Channel Recipients
        public Task AddGroupRecipientAsync(ulong channelId, ulong userId, RequestOptions options = null)
        {
            Preconditions.GreaterThan(channelId, 0, nameof(channelId));
            Preconditions.GreaterThan(userId, 0, nameof(userId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendAsync("PUT", () => $"channels/{channelId}/recipients/{userId}", ids, options: options);
        }

        public Task RemoveGroupRecipientAsync(ulong channelId, ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(userId, 0, nameof(userId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendAsync("DELETE", () => $"channels/{channelId}/recipients/{userId}", ids, options: options);
        }
        #endregion

        #region Interactions
        public Task<ApplicationCommand[]> GetGlobalApplicationCommandsAsync(bool withLocalizations = false, string locale = null, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            if (locale is not null)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(locale, @"^\w{2}(?:-\w{2})?$"))
                    throw new ArgumentException($"{locale} is not a valid locale.", nameof(locale));

                options.RequestHeaders["X-Discord-Locale"] = new[] { locale };
            }

            //with_localizations=false doesn't return localized names and descriptions
            var query = withLocalizations ? "?with_localizations=true" : string.Empty;
            return SendAsync<ApplicationCommand[]>("GET", () => $"applications/{CurrentApplicationId}/commands{query}", new BucketIds(), options: options);
        }

        public async Task<ApplicationCommand> GetGlobalApplicationCommandAsync(ulong id, RequestOptions options = null)
        {
            Preconditions.NotEqual(id, 0, nameof(id));

            options = RequestOptions.CreateOrClone(options);

            try
            {
                return await SendAsync<ApplicationCommand>("GET", () => $"applications/{CurrentApplicationId}/commands/{id}", new BucketIds(), options: options).ConfigureAwait(false);
            }
            catch (HttpException x) when (x.HttpCode == HttpStatusCode.NotFound) { return null; }
        }

        public Task<ApplicationCommand> CreateGlobalApplicationCommandAsync(CreateApplicationCommandParams command, RequestOptions options = null)
        {
            Preconditions.NotNull(command, nameof(command));
            Preconditions.AtMost(command.Name.Length, 32, nameof(command.Name));
            Preconditions.AtLeast(command.Name.Length, 1, nameof(command.Name));

            if (command.Type == ApplicationCommandType.Slash)
            {
                Preconditions.NotNullOrEmpty(command.Description, nameof(command.Description));
                Preconditions.AtMost(command.Description.Length, 100, nameof(command.Description));
                Preconditions.AtLeast(command.Description.Length, 1, nameof(command.Description));
            }

            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<ApplicationCommand>("POST", () => $"applications/{CurrentApplicationId}/commands", command, new BucketIds(), options: options);
        }

        public Task<ApplicationCommand> ModifyGlobalApplicationCommandAsync(ModifyApplicationCommandParams command, ulong commandId, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<ApplicationCommand>("PATCH", () => $"applications/{CurrentApplicationId}/commands/{commandId}", command, new BucketIds(), options: options);
        }

        public Task<ApplicationCommand> ModifyGlobalApplicationUserCommandAsync(ModifyApplicationCommandParams command, ulong commandId, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<ApplicationCommand>("PATCH", () => $"applications/{CurrentApplicationId}/commands/{commandId}", command, new BucketIds(), options: options);
        }

        public Task<ApplicationCommand> ModifyGlobalApplicationMessageCommandAsync(ModifyApplicationCommandParams command, ulong commandId, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<ApplicationCommand>("PATCH", () => $"applications/{CurrentApplicationId}/commands/{commandId}", command, new BucketIds(), options: options);
        }

        public Task DeleteGlobalApplicationCommandAsync(ulong commandId, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            return SendAsync("DELETE", () => $"applications/{CurrentApplicationId}/commands/{commandId}", new BucketIds(), options: options);
        }

        public Task<ApplicationCommand[]> BulkOverwriteGlobalApplicationCommandsAsync(CreateApplicationCommandParams[] commands, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<ApplicationCommand[]>("PUT", () => $"applications/{CurrentApplicationId}/commands", commands, new BucketIds(), options: options);
        }

        public Task<ApplicationCommand[]> GetGuildApplicationCommandsAsync(ulong guildId, bool withLocalizations = false, string locale = null, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(guildId: guildId);

            if (locale is not null)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(locale, @"^\w{2}(?:-\w{2})?$"))
                    throw new ArgumentException($"{locale} is not a valid locale.", nameof(locale));

                options.RequestHeaders["X-Discord-Locale"] = new[] { locale };
            }

            //with_localizations=false doesn't return localized names and descriptions
            var query = withLocalizations ? "?with_localizations=true" : string.Empty;
            return SendAsync<ApplicationCommand[]>("GET", () => $"applications/{CurrentApplicationId}/guilds/{guildId}/commands{query}", bucket, options: options);
        }

        public async Task<ApplicationCommand> GetGuildApplicationCommandAsync(ulong guildId, ulong commandId, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(guildId: guildId);

            try
            {
                return await SendAsync<ApplicationCommand>("GET", () => $"applications/{CurrentApplicationId}/guilds/{guildId}/commands/{commandId}", bucket, options: options);
            }
            catch (HttpException x) when (x.HttpCode == HttpStatusCode.NotFound) { return null; }
        }

        public Task<ApplicationCommand> CreateGuildApplicationCommandAsync(CreateApplicationCommandParams command, ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotNull(command, nameof(command));
            Preconditions.AtMost(command.Name.Length, 32, nameof(command.Name));
            Preconditions.AtLeast(command.Name.Length, 1, nameof(command.Name));

            if (command.Type == ApplicationCommandType.Slash)
            {
                Preconditions.NotNullOrEmpty(command.Description, nameof(command.Description));
                Preconditions.AtMost(command.Description.Length, 100, nameof(command.Description));
                Preconditions.AtLeast(command.Description.Length, 1, nameof(command.Description));
            }

            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(guildId: guildId);

            return SendJsonAsync<ApplicationCommand>("POST", () => $"applications/{CurrentApplicationId}/guilds/{guildId}/commands", command, bucket, options: options);
        }

        public Task<ApplicationCommand> ModifyGuildApplicationCommandAsync(ModifyApplicationCommandParams command, ulong guildId, ulong commandId, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(guildId: guildId);

            return SendJsonAsync<ApplicationCommand>("PATCH", () => $"applications/{CurrentApplicationId}/guilds/{guildId}/commands/{commandId}", command, bucket, options: options);
        }

        public Task DeleteGuildApplicationCommandAsync(ulong guildId, ulong commandId, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(guildId: guildId);

            return SendAsync<ApplicationCommand>("DELETE", () => $"applications/{CurrentApplicationId}/guilds/{guildId}/commands/{commandId}", bucket, options: options);
        }

        public Task<ApplicationCommand[]> BulkOverwriteGuildApplicationCommandsAsync(ulong guildId, CreateApplicationCommandParams[] commands, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            var bucket = new BucketIds(guildId: guildId);

            return SendJsonAsync<ApplicationCommand[]>("PUT", () => $"applications/{CurrentApplicationId}/guilds/{guildId}/commands", commands, bucket, options: options);
        }
        #endregion

        #region Interaction Responses
        public Task CreateInteractionResponseAsync(InteractionResponse response, ulong interactionId, string interactionToken, RequestOptions options = null)
        {
            if (response.Data.IsSpecified && response.Data.Value.Content.IsSpecified)
                Preconditions.AtMost(response.Data.Value.Content.Value?.Length ?? 0, 2000, nameof(response.Data.Value.Content));

            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync("POST", () => $"interactions/{interactionId}/{interactionToken}/callback", response, new BucketIds(), options: options);
        }

        public Task CreateInteractionResponseAsync(UploadInteractionFileParams response, ulong interactionId, string interactionToken, RequestOptions options = null)
        {
            if ((!response.Embeds.IsSpecified || response.Embeds.Value == null || response.Embeds.Value.Length == 0) && !response.Files.Any())
                Preconditions.NotNullOrEmpty(response.Content, nameof(response.Content));

            if (response.Content.IsSpecified && response.Content.Value.Length > DiscordConfig.MaxMessageSize)
                throw new ArgumentException(message: $"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.", paramName: nameof(response.Content));

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds();
            return SendMultipartAsync("POST", () => $"interactions/{interactionId}/{interactionToken}/callback", response.ToDictionary(), ids, clientBucket: ClientBucketType.SendEdit, options: options);
        }

        public Task<Message> GetInteractionResponseAsync(string interactionToken, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(interactionToken, nameof(interactionToken));

            options = RequestOptions.CreateOrClone(options);

            return NullifyNotFound(SendAsync<Message>("GET", () => $"webhooks/{CurrentApplicationId}/{interactionToken}/messages/@original", new BucketIds(), options: options));
        }

        public Task<Message> ModifyInteractionResponseAsync(ModifyInteractionResponseParams args, string interactionToken, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<Message>("PATCH", () => $"webhooks/{CurrentApplicationId}/{interactionToken}/messages/@original", args, new BucketIds(), options: options);
        }

        public Task<Message> ModifyInteractionResponseAsync(UploadWebhookFileParams args, string interactionToken, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            return SendMultipartAsync<Message>("PATCH", () => $"webhooks/{CurrentApplicationId}/{interactionToken}/messages/@original", args.ToDictionary(), new BucketIds(), options: options);
        }

        public Task DeleteInteractionResponseAsync(string interactionToken, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            return SendAsync("DELETE", () => $"webhooks/{CurrentApplicationId}/{interactionToken}/messages/@original", new BucketIds(), options: options);
        }

        public Task<Message> CreateInteractionFollowupMessageAsync(CreateWebhookMessageParams args, string token, RequestOptions options = null)
        {
            if ((!args.Embeds.IsSpecified || args.Embeds.Value == null || args.Embeds.Value.Length == 0)
                && (!args.Content.IsSpecified || args.Content.Value is null || string.IsNullOrWhiteSpace(args.Content.Value))
                && (!args.Components.IsSpecified || args.Components.Value is null || args.Components.Value.Length == 0)
                && (!args.File.IsSpecified))
            {
                throw new ArgumentException("At least one of 'Content', 'Embeds', 'File' or 'Components' must be specified.", nameof(args));
            }

            if (args.Content.IsSpecified && args.Content.Value is not null && args.Content.Value.Length > DiscordConfig.MaxMessageSize)
                throw new ArgumentException(message: $"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.", paramName: nameof(args.Content));

            if (args.Content.IsSpecified && args.Content.Value?.Length > DiscordConfig.MaxMessageSize)
                throw new ArgumentException(message: $"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.", paramName: nameof(args.Content));

            options = RequestOptions.CreateOrClone(options);

            if (!args.File.IsSpecified)
                return SendJsonAsync<Message>("POST", () => $"webhooks/{CurrentApplicationId}/{token}?wait=true", args, new BucketIds(), options: options);
            else
                return SendMultipartAsync<Message>("POST", () => $"webhooks/{CurrentApplicationId}/{token}?wait=true", args.ToDictionary(), new BucketIds(), options: options);
        }

        public Task<Message> CreateInteractionFollowupMessageAsync(UploadWebhookFileParams args, string token, RequestOptions options = null)
        {
            if ((!args.Embeds.IsSpecified || args.Embeds.Value == null || args.Embeds.Value.Length == 0)
                && (!args.Content.IsSpecified || args.Content.Value is null || string.IsNullOrWhiteSpace(args.Content.Value))
                && (!args.MessageComponents.IsSpecified || args.MessageComponents.Value is null || args.MessageComponents.Value.Length == 0)
                && (args.Files.Length == 0))
            {
                throw new ArgumentException("At least one of 'Content', 'Embeds', 'Files' or 'Components' must be specified.", nameof(args));
            }
            if (args.Content.IsSpecified && args.Content.Value?.Length > DiscordConfig.MaxMessageSize)
                throw new ArgumentException(message: $"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.", paramName: nameof(args.Content));

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds();
            return SendMultipartAsync<Message>("POST", () => $"webhooks/{CurrentApplicationId}/{token}?wait=true", args.ToDictionary(), ids, clientBucket: ClientBucketType.SendEdit, options: options);
        }

        public Task<Message> ModifyInteractionFollowupMessageAsync(ModifyInteractionResponseParams args, ulong id, string token, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(id, 0, nameof(id));

            if (args.Content.IsSpecified && args.Content.Value?.Length > DiscordConfig.MaxMessageSize)
                throw new ArgumentException(message: $"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.", paramName: nameof(args.Content));

            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<Message>("PATCH", () => $"webhooks/{CurrentApplicationId}/{token}/messages/{id}", args, new BucketIds(), options: options);
        }

        public Task DeleteInteractionFollowupMessageAsync(ulong id, string token, RequestOptions options = null)
        {
            Preconditions.NotEqual(id, 0, nameof(id));

            options = RequestOptions.CreateOrClone(options);

            return SendAsync("DELETE", () => $"webhooks/{CurrentApplicationId}/{token}/messages/{id}", new BucketIds(), options: options);
        }
        #endregion

        #region Application Command permissions
        public Task<GuildApplicationCommandPermission[]> GetGuildApplicationCommandPermissionsAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            options = RequestOptions.CreateOrClone(options);

            return SendAsync<GuildApplicationCommandPermission[]>("GET", () => $"applications/{CurrentApplicationId}/guilds/{guildId}/commands/permissions", new BucketIds(), options: options);
        }

        public Task<GuildApplicationCommandPermission> GetGuildApplicationCommandPermissionAsync(ulong guildId, ulong commandId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(commandId, 0, nameof(commandId));

            options = RequestOptions.CreateOrClone(options);

            return SendAsync<GuildApplicationCommandPermission>("GET", () => $"applications/{CurrentApplicationId}/guilds/{guildId}/commands/{commandId}/permissions", new BucketIds(), options: options);
        }

        public Task<GuildApplicationCommandPermission> ModifyApplicationCommandPermissionsAsync(ModifyGuildApplicationCommandPermissionsParams permissions, ulong guildId, ulong commandId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(commandId, 0, nameof(commandId));

            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<GuildApplicationCommandPermission>("PUT", () => $"applications/{CurrentApplicationId}/guilds/{guildId}/commands/{commandId}/permissions", permissions, new BucketIds(), options: options);
        }

        public async Task<IReadOnlyCollection<GuildApplicationCommandPermission>> BatchModifyApplicationCommandPermissionsAsync(ModifyGuildApplicationCommandPermissions[] permissions, ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(permissions, nameof(permissions));

            options = RequestOptions.CreateOrClone(options);

            return await SendJsonAsync<GuildApplicationCommandPermission[]>("PUT", () => $"applications/{CurrentApplicationId}/guilds/{guildId}/commands/permissions", permissions, new BucketIds(), options: options).ConfigureAwait(false);
        }
        #endregion

        #region Guilds
        public async Task<Guild> GetGuildAsync(ulong guildId, bool withCounts, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            try
            {
                var ids = new BucketIds(guildId: guildId);
                return await SendAsync<Guild>("GET", () => $"guilds/{guildId}?with_counts={(withCounts ? "true" : "false")}", ids, options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) { return null; }
        }

        public Task<Guild> CreateGuildAsync(CreateGuildParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNullOrWhitespace(args.Name, nameof(args.Name));
            Preconditions.NotNullOrWhitespace(args.RegionId, nameof(args.RegionId));
            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<Guild>("POST", () => "guilds", args, new BucketIds(), options: options);
        }

        public Task<Guild> DeleteGuildAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync<Guild>("DELETE", () => $"guilds/{guildId}", ids, options: options);
        }

        public Task<Guild> LeaveGuildAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync<Guild>("DELETE", () => $"users/@me/guilds/{guildId}", ids, options: options);
        }

        public Task<Guild> ModifyGuildAsync(ulong guildId, Rest.ModifyGuildParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(args.AfkChannelId, 0, nameof(args.AfkChannelId));
            Preconditions.AtLeast(args.AfkTimeout, 0, nameof(args.AfkTimeout));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));
            Preconditions.GreaterThan(args.OwnerId, 0, nameof(args.OwnerId));
            Preconditions.NotNull(args.RegionId, nameof(args.RegionId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendJsonAsync<Guild>("PATCH", () => $"guilds/{guildId}", args, ids, options: options);
        }

        public Task<GetGuildPruneCountResponse> BeginGuildPruneAsync(ulong guildId, GuildPruneParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Days, 1, nameof(args.Days));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendJsonAsync<GetGuildPruneCountResponse>("POST", () => $"guilds/{guildId}/prune", args, ids, options: options);
        }

        public Task<GetGuildPruneCountResponse> GetGuildPruneCountAsync(ulong guildId, GuildPruneParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Days, 1, nameof(args.Days));
            string endpointRoleIds = args.IncludeRoleIds?.Length > 0 ? $"&include_roles={string.Join(",", args.IncludeRoleIds)}" : "";
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync<GetGuildPruneCountResponse>("GET", () => $"guilds/{guildId}/prune?days={args.Days}{endpointRoleIds}", ids, options: options);
        }

        public async Task<GuildIncidentsData> ModifyGuildIncidentActionsAsync(ulong guildId, ModifyGuildIncidentsDataParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            var ids = new BucketIds(guildId: guildId);

            return await SendJsonAsync<GuildIncidentsData>("PUT", () => $"guilds/{guildId}/incident-actions", args, ids, options: options).ConfigureAwait(false);
        }

        #endregion

        #region Guild Bans
        public Task<IReadOnlyCollection<Ban>> GetGuildBansAsync(ulong guildId, GetGuildBansParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Limit, 0, nameof(args.Limit));
            Preconditions.AtMost(args.Limit, DiscordConfig.MaxBansPerBatch, nameof(args.Limit));
            options = RequestOptions.CreateOrClone(options);

            int limit = args.Limit.GetValueOrDefault(DiscordConfig.MaxBansPerBatch);
            ulong? relativeId = args.RelativeUserId.IsSpecified ? args.RelativeUserId.Value : (ulong?)null;
            var relativeDir = args.RelativeDirection.GetValueOrDefault(Direction.Before) switch
            {
                Direction.After => "after",
                Direction.Around => "around",
                _ => "before",
            };
            var ids = new BucketIds(guildId: guildId);
            Expression<Func<string>> endpoint;
            if (relativeId != null)
                endpoint = () => $"guilds/{guildId}/bans?limit={limit}&{relativeDir}={relativeId}";
            else
                endpoint = () => $"guilds/{guildId}/bans?limit={limit}";
            return SendAsync<IReadOnlyCollection<Ban>>("GET", endpoint, ids, options: options);
        }

        public async Task<Ban> GetGuildBanAsync(ulong guildId, ulong userId, RequestOptions options)
        {
            Preconditions.NotEqual(userId, 0, nameof(userId));
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            try
            {
                var ids = new BucketIds(guildId: guildId);
                return await SendAsync<Ban>("GET", () => $"guilds/{guildId}/bans/{userId}", ids, options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) { return null; }
        }

        /// <exception cref="ArgumentException">
        /// <paramref name="guildId"/> and <paramref name="userId"/> must not be equal to zero.
        /// -and-
        /// <paramref name="deleteMessageSeconds"/> must be between 0 and 604800.
        /// </exception>
        public Task CreateGuildBanAsync(ulong guildId, ulong userId, uint deleteMessageSeconds, string reason, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));

            Preconditions.AtMost(deleteMessageSeconds, 604800, nameof(deleteMessageSeconds), "Prune length must be within [0, 604800]");

            var data = new CreateGuildBanParams { DeleteMessageSeconds = deleteMessageSeconds };

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            if (!string.IsNullOrWhiteSpace(reason))
                options.AuditLogReason = reason;
            return SendJsonAsync("PUT", () => $"guilds/{guildId}/bans/{userId}", data, ids, options: options);
        }

        /// <exception cref="ArgumentException"><paramref name="guildId"/> and <paramref name="userId"/> must not be equal to zero.</exception>
        public Task RemoveGuildBanAsync(ulong guildId, ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync("DELETE", () => $"guilds/{guildId}/bans/{userId}", ids, options: options);
        }

        public Task<BulkBanResult> BulkBanAsync(ulong guildId, ulong[] userIds, int? deleteMessagesSeconds = null, RequestOptions options = null)
        {
            Preconditions.NotEqual(userIds.Length, 0, nameof(userIds));
            Preconditions.AtMost(userIds.Length, 200, nameof(userIds));
            Preconditions.AtMost(deleteMessagesSeconds ?? 0, 604800, nameof(deleteMessagesSeconds));

            options = RequestOptions.CreateOrClone(options);

            var data = new BulkBanParams
            {
                DeleteMessageSeconds = deleteMessagesSeconds ?? Optional<int>.Unspecified,
                UserIds = userIds
            };

            return SendJsonAsync<BulkBanResult>("POST", () => $"guilds/{guildId}/bulk-ban", data, new BucketIds(guildId), options: options);
        }
        #endregion

        #region Guild Widget
        /// <exception cref="ArgumentException"><paramref name="guildId"/> must not be equal to zero.</exception>
        public async Task<GuildWidget> GetGuildWidgetAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            try
            {
                var ids = new BucketIds(guildId: guildId);
                return await SendAsync<GuildWidget>("GET", () => $"guilds/{guildId}/widget", ids, options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) { return null; }
        }

        /// <exception cref="ArgumentException"><paramref name="guildId"/> must not be equal to zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> must not be <see langword="null"/>.</exception>
        public Task<GuildWidget> ModifyGuildWidgetAsync(ulong guildId, Rest.ModifyGuildWidgetParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendJsonAsync<GuildWidget>("PATCH", () => $"guilds/{guildId}/widget", args, ids, options: options);
        }
        #endregion

        #region Guild Integrations
        /// <exception cref="ArgumentException"><paramref name="guildId"/> must not be equal to zero.</exception>
        public Task<IReadOnlyCollection<Integration>> GetIntegrationsAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync<IReadOnlyCollection<Integration>>("GET", () => $"guilds/{guildId}/integrations", ids, options: options);
        }

        public Task DeleteIntegrationAsync(ulong guildId, ulong integrationId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(integrationId, 0, nameof(integrationId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync("DELETE", () => $"guilds/{guildId}/integrations/{integrationId}", ids, options: options);
        }
        #endregion

        #region Guild Invites
        /// <exception cref="ArgumentException"><paramref name="inviteId"/> cannot be blank.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="inviteId"/> must not be <see langword="null"/>.</exception>
        public async Task<InviteMetadata> GetInviteAsync(string inviteId, RequestOptions options = null, ulong? scheduledEventId = null)
        {
            Preconditions.NotNullOrEmpty(inviteId, nameof(inviteId));
            options = RequestOptions.CreateOrClone(options);

            //Remove trailing slash
            if (inviteId[inviteId.Length - 1] == '/')
                inviteId = inviteId.Substring(0, inviteId.Length - 1);
            //Remove leading URL
            int index = inviteId.LastIndexOf('/');
            if (index >= 0)
                inviteId = inviteId.Substring(index + 1);

            var scheduledEventQuery = scheduledEventId is not null
                ? $"&guild_scheduled_event_id={scheduledEventId}"
                : string.Empty;

            try
            {
                return await SendAsync<InviteMetadata>("GET", () => $"invites/{inviteId}?with_counts=true&with_expiration=true{scheduledEventQuery}", new BucketIds(), options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) { return null; }
        }

        /// <exception cref="ArgumentException"><paramref name="guildId"/> may not be equal to zero.</exception>
        public Task<InviteVanity> GetVanityInviteAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync<InviteVanity>("GET", () => $"guilds/{guildId}/vanity-url", ids, options: options);
        }

        /// <exception cref="ArgumentException"><paramref name="guildId"/> may not be equal to zero.</exception>
        public Task<IReadOnlyCollection<InviteMetadata>> GetGuildInvitesAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync<IReadOnlyCollection<InviteMetadata>>("GET", () => $"guilds/{guildId}/invites", ids, options: options);
        }

        /// <exception cref="ArgumentException"><paramref name="channelId"/> may not be equal to zero.</exception>
        public Task<IReadOnlyCollection<InviteMetadata>> GetChannelInvitesAsync(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendAsync<IReadOnlyCollection<InviteMetadata>>("GET", () => $"channels/{channelId}/invites", ids, options: options);
        }

        /// <exception cref="ArgumentException">
        /// <paramref name="channelId"/> may not be equal to zero.
        /// -and-
        /// <paramref name="args.MaxAge"/> and <paramref name="args.MaxUses"/> must be greater than zero.
        /// -and-
        /// <paramref name="args.MaxAge"/> must be lesser than 86400.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> must not be <see langword="null"/>.</exception>
        public Task<InviteMetadata> CreateChannelInviteAsync(ulong channelId, CreateChannelInviteParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.MaxAge, 0, nameof(args.MaxAge));
            Preconditions.AtLeast(args.MaxUses, 0, nameof(args.MaxUses));
            Preconditions.AtMost(args.MaxAge, 86400, nameof(args.MaxAge),
                "The maximum age of an invite must be less than or equal to a day (86400 seconds).");
            if (args.TargetType.IsSpecified)
            {
                Preconditions.NotEqual((int)args.TargetType.Value, (int)TargetUserType.Undefined, nameof(args.TargetType));
                if (args.TargetType.Value == TargetUserType.Stream)
                    Preconditions.GreaterThan(args.TargetUserId, 0, nameof(args.TargetUserId));
                if (args.TargetType.Value == TargetUserType.EmbeddedApplication)
                    Preconditions.GreaterThan(args.TargetApplicationId, 0, nameof(args.TargetApplicationId));
            }
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendJsonAsync<InviteMetadata>("POST", () => $"channels/{channelId}/invites", args, ids, options: options);
        }

        public Task<Invite> DeleteInviteAsync(string inviteId, RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(inviteId, nameof(inviteId));
            options = RequestOptions.CreateOrClone(options);

            return SendAsync<Invite>("DELETE", () => $"invites/{inviteId}", new BucketIds(), options: options);
        }
        #endregion

        #region Guild Members
        public Task<GuildMember> AddGuildMemberAsync(ulong guildId, ulong userId, AddGuildMemberParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNullOrWhitespace(args.AccessToken, nameof(args.AccessToken));

            if (args.RoleIds.IsSpecified)
            {
                foreach (var roleId in args.RoleIds.Value)
                    Preconditions.NotEqual(roleId, 0, nameof(roleId));
            }

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendJsonAsync<GuildMember>("PUT", () => $"guilds/{guildId}/members/{userId}", args, ids, options: options);
        }

        public async Task<GuildMember> GetGuildMemberAsync(ulong guildId, ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));
            options = RequestOptions.CreateOrClone(options);

            try
            {
                var ids = new BucketIds(guildId: guildId);
                return await SendAsync<GuildMember>("GET", () => $"guilds/{guildId}/members/{userId}", ids, options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) { return null; }
        }

        public Task<IReadOnlyCollection<GuildMember>> GetGuildMembersAsync(ulong guildId, GetGuildMembersParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args.Limit, 0, nameof(args.Limit));
            Preconditions.AtMost(args.Limit, DiscordConfig.MaxUsersPerBatch, nameof(args.Limit));
            Preconditions.GreaterThan(args.AfterUserId, 0, nameof(args.AfterUserId));
            options = RequestOptions.CreateOrClone(options);

            int limit = args.Limit.GetValueOrDefault(int.MaxValue);
            ulong afterUserId = args.AfterUserId.GetValueOrDefault(0);

            var ids = new BucketIds(guildId: guildId);
            Expression<Func<string>> endpoint = () => $"guilds/{guildId}/members?limit={limit}&after={afterUserId}";
            return SendAsync<IReadOnlyCollection<GuildMember>>("GET", endpoint, ids, options: options);
        }

        public Task RemoveGuildMemberAsync(ulong guildId, ulong userId, string reason, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            if (!string.IsNullOrWhiteSpace(reason))
                options.AuditLogReason = reason;
            return SendAsync("DELETE", () => $"guilds/{guildId}/members/{userId}", ids, options: options);
        }

        public async Task ModifyGuildMemberAsync(ulong guildId, ulong userId, Rest.ModifyGuildMemberParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));
            Preconditions.NotNull(args, nameof(args));
            options = RequestOptions.CreateOrClone(options);

            bool isCurrentUser = userId == CurrentUserId;

            if (isCurrentUser && args.Nickname.IsSpecified)
            {
                var nickArgs = new Rest.ModifyCurrentUserNickParams(args.Nickname.Value ?? "");
                await ModifyMyNickAsync(guildId, nickArgs).ConfigureAwait(false);
                args.Nickname = Optional.Create<string>(); //Remove
            }
            if (!isCurrentUser || args.Deaf.IsSpecified || args.Mute.IsSpecified || args.RoleIds.IsSpecified)
            {
                var ids = new BucketIds(guildId: guildId);
                await SendJsonAsync("PATCH", () => $"guilds/{guildId}/members/{userId}", args, ids, options: options).ConfigureAwait(false);
            }
        }

        public Task<IReadOnlyCollection<GuildMember>> SearchGuildMembersAsync(ulong guildId, SearchGuildMembersParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args.Limit, 0, nameof(args.Limit));
            Preconditions.AtMost(args.Limit, DiscordConfig.MaxUsersPerBatch, nameof(args.Limit));
            Preconditions.NotNullOrEmpty(args.Query, nameof(args.Query));
            options = RequestOptions.CreateOrClone(options);

            int limit = args.Limit.GetValueOrDefault(DiscordConfig.MaxUsersPerBatch);
            string query = args.Query;

            var ids = new BucketIds(guildId: guildId);
            Expression<Func<string>> endpoint = () => $"guilds/{guildId}/members/search?limit={limit}&query={query}";
            return SendAsync<IReadOnlyCollection<GuildMember>>("GET", endpoint, ids, options: options);
        }
        #endregion

        #region Guild Roles
        public Task<IReadOnlyCollection<Role>> GetGuildRolesAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync<IReadOnlyCollection<Role>>("GET", () => $"guilds/{guildId}/roles", ids, options: options);
        }

        public async Task<Role> CreateGuildRoleAsync(ulong guildId, Rest.ModifyGuildRoleParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return await SendJsonAsync<Role>("POST", () => $"guilds/{guildId}/roles", args, ids, options: options).ConfigureAwait(false);
        }

        public Task DeleteGuildRoleAsync(ulong guildId, ulong roleId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(roleId, 0, nameof(roleId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync("DELETE", () => $"guilds/{guildId}/roles/{roleId}", ids, options: options);
        }

        public Task<Role> ModifyGuildRoleAsync(ulong guildId, ulong roleId, Rest.ModifyGuildRoleParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(roleId, 0, nameof(roleId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Color, 0, nameof(args.Color));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendJsonAsync<Role>("PATCH", () => $"guilds/{guildId}/roles/{roleId}", args, ids, options: options);
        }

        public Task<IReadOnlyCollection<Role>> ModifyGuildRolesAsync(ulong guildId, IEnumerable<Rest.ModifyGuildRolesParams> args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendJsonAsync<IReadOnlyCollection<Role>>("PATCH", () => $"guilds/{guildId}/roles", args, ids, options: options);
        }
        #endregion

        #region Guild emoji
        public Task<IReadOnlyCollection<Emoji>> GetGuildEmotesAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync<IReadOnlyCollection<Emoji>>("GET", () => $"guilds/{guildId}/emojis", ids, options: options);
        }

        public Task<Emoji> GetGuildEmoteAsync(ulong guildId, ulong emoteId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(emoteId, 0, nameof(emoteId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync<Emoji>("GET", () => $"guilds/{guildId}/emojis/{emoteId}", ids, options: options);
        }

        public Task<Emoji> CreateGuildEmoteAsync(ulong guildId, Rest.CreateGuildEmoteParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNullOrWhitespace(args.Name, nameof(args.Name));
            Preconditions.NotNull(args.Image.Stream, nameof(args.Image));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendJsonAsync<Emoji>("POST", () => $"guilds/{guildId}/emojis", args, ids, options: options);
        }

        public Task<Emoji> ModifyGuildEmoteAsync(ulong guildId, ulong emoteId, ModifyGuildEmoteParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(emoteId, 0, nameof(emoteId));
            Preconditions.NotNull(args, nameof(args));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendJsonAsync<Emoji>("PATCH", () => $"guilds/{guildId}/emojis/{emoteId}", args, ids, options: options);
        }

        public Task DeleteGuildEmoteAsync(ulong guildId, ulong emoteId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(emoteId, 0, nameof(emoteId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync("DELETE", () => $"guilds/{guildId}/emojis/{emoteId}", ids, options: options);
        }
        #endregion

        #region Guild Events

        public Task<GuildScheduledEvent[]> ListGuildScheduledEventsAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync<GuildScheduledEvent[]>("GET", () => $"guilds/{guildId}/scheduled-events?with_user_count=true", ids, options: options);
        }

        public Task<GuildScheduledEvent> GetGuildScheduledEventAsync(ulong eventId, ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(eventId, 0, nameof(eventId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return NullifyNotFound<GuildScheduledEvent>(SendAsync<GuildScheduledEvent>("GET", () => $"guilds/{guildId}/scheduled-events/{eventId}?with_user_count=true", ids, options: options));
        }

        public Task<GuildScheduledEvent> CreateGuildScheduledEventAsync(CreateGuildScheduledEventParams args, ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendJsonAsync<GuildScheduledEvent>("POST", () => $"guilds/{guildId}/scheduled-events", args, ids, options: options);
        }

        public Task<GuildScheduledEvent> ModifyGuildScheduledEventAsync(ModifyGuildScheduledEventParams args, ulong eventId, ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(eventId, 0, nameof(eventId));
            Preconditions.NotNull(args, nameof(args));

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendJsonAsync<GuildScheduledEvent>("PATCH", () => $"guilds/{guildId}/scheduled-events/{eventId}", args, ids, options: options);
        }

        public Task DeleteGuildScheduledEventAsync(ulong eventId, ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(eventId, 0, nameof(eventId));

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync("DELETE", () => $"guilds/{guildId}/scheduled-events/{eventId}", ids, options: options);
        }

        public Task<GuildScheduledEventUser[]> GetGuildScheduledEventUsersAsync(ulong eventId, ulong guildId, int limit = 100, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(eventId, 0, nameof(eventId));

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync<GuildScheduledEventUser[]>("GET", () => $"guilds/{guildId}/scheduled-events/{eventId}/users?limit={limit}&with_member=true", ids, options: options);
        }

        public Task<GuildScheduledEventUser[]> GetGuildScheduledEventUsersAsync(ulong eventId, ulong guildId, GetEventUsersParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(eventId, 0, nameof(eventId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Limit, 0, nameof(args.Limit));
            Preconditions.AtMost(args.Limit, DiscordConfig.MaxMessagesPerBatch, nameof(args.Limit));
            options = RequestOptions.CreateOrClone(options);

            int limit = args.Limit.GetValueOrDefault(DiscordConfig.MaxGuildEventUsersPerBatch);
            ulong? relativeId = args.RelativeUserId.IsSpecified ? args.RelativeUserId.Value : (ulong?)null;
            var relativeDir = args.RelativeDirection.GetValueOrDefault(Direction.Before) switch
            {
                Direction.After => "after",
                Direction.Around => "around",
                _ => "before",
            };
            var ids = new BucketIds(guildId: guildId);
            Expression<Func<string>> endpoint;
            if (relativeId != null)
                endpoint = () => $"guilds/{guildId}/scheduled-events/{eventId}/users?with_member=true&limit={limit}&{relativeDir}={relativeId}";
            else
                endpoint = () => $"guilds/{guildId}/scheduled-events/{eventId}/users?with_member=true&limit={limit}";

            return SendAsync<GuildScheduledEventUser[]>("GET", endpoint, ids, options: options);
        }

        #endregion

        #region Guild AutoMod

        public Task<AutoModerationRule[]> GetGuildAutoModRulesAsync(ulong guildId, RequestOptions options)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            options = RequestOptions.CreateOrClone(options);

            return SendAsync<AutoModerationRule[]>("GET", () => $"guilds/{guildId}/auto-moderation/rules", new BucketIds(guildId: guildId), options: options);
        }

        public Task<AutoModerationRule> GetGuildAutoModRuleAsync(ulong guildId, ulong ruleId, RequestOptions options)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(ruleId, 0, nameof(ruleId));

            options = RequestOptions.CreateOrClone(options);

            return SendAsync<AutoModerationRule>("GET", () => $"guilds/{guildId}/auto-moderation/rules/{ruleId}", new BucketIds(guildId), options: options);
        }

        public Task<AutoModerationRule> CreateGuildAutoModRuleAsync(ulong guildId, CreateAutoModRuleParams args, RequestOptions options)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<AutoModerationRule>("POST", () => $"guilds/{guildId}/auto-moderation/rules", args, new BucketIds(guildId: guildId), options: options);
        }

        public Task<AutoModerationRule> ModifyGuildAutoModRuleAsync(ulong guildId, ulong ruleId, ModifyAutoModRuleParams args, RequestOptions options)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(ruleId, 0, nameof(ruleId));

            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<AutoModerationRule>("PATCH", () => $"guilds/{guildId}/auto-moderation/rules/{ruleId}", args, new BucketIds(guildId: guildId), options: options);
        }

        public Task DeleteGuildAutoModRuleAsync(ulong guildId, ulong ruleId, RequestOptions options)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(ruleId, 0, nameof(ruleId));

            options = RequestOptions.CreateOrClone(options);

            return SendAsync("DELETE", () => $"guilds/{guildId}/auto-moderation/rules/{ruleId}", new BucketIds(guildId: guildId), options: options);
        }

        #endregion

        #region Guild Welcome Screen

        public async Task<WelcomeScreen> GetGuildWelcomeScreenAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            try
            {
                var ids = new BucketIds(guildId: guildId);
                return await SendAsync<WelcomeScreen>("GET", () => $"guilds/{guildId}/welcome-screen", ids, options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) { return null; }
        }

        public Task<WelcomeScreen> ModifyGuildWelcomeScreenAsync(ModifyGuildWelcomeScreenParams args, ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));

            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);

            return SendJsonAsync<WelcomeScreen>("PATCH", () => $"guilds/{guildId}/welcome-screen", args, ids, options: options);
        }

        #endregion

        #region Guild Onboarding

        public Task<GuildOnboarding> GetGuildOnboardingAsync(ulong guildId, RequestOptions options)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            options = RequestOptions.CreateOrClone(options);

            return SendAsync<GuildOnboarding>("GET", () => $"guilds/{guildId}/onboarding", new BucketIds(guildId: guildId), options: options);
        }

        public Task<GuildOnboarding> ModifyGuildOnboardingAsync(ulong guildId, ModifyGuildOnboardingParams args, RequestOptions options)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<GuildOnboarding>("PUT", () => $"guilds/{guildId}/onboarding", args, new BucketIds(guildId: guildId), options: options);
        }

        #endregion

        #region Users
        public async Task<User> GetUserAsync(ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(userId, 0, nameof(userId));
            options = RequestOptions.CreateOrClone(options);

            try
            {
                return await SendAsync<User>("GET", () => $"users/{userId}", new BucketIds(), options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) { return null; }
        }
        #endregion

        #region Current User/DMs
        public Task<User> GetMyUserAsync(RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            return SendAsync<User>("GET", () => "users/@me", new BucketIds(), options: options);
        }

        public Task<IReadOnlyCollection<Connection>> GetMyConnectionsAsync(RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            return SendAsync<IReadOnlyCollection<Connection>>("GET", () => "users/@me/connections", new BucketIds(), options: options);
        }

        public Task<IReadOnlyCollection<Channel>> GetMyPrivateChannelsAsync(RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            return SendAsync<IReadOnlyCollection<Channel>>("GET", () => "users/@me/channels", new BucketIds(), options: options);
        }

        public Task<IReadOnlyCollection<UserGuild>> GetMyGuildsAsync(GetGuildSummariesParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args.Limit, 0, nameof(args.Limit));
            Preconditions.AtMost(args.Limit, DiscordConfig.MaxGuildsPerBatch, nameof(args.Limit));
            Preconditions.GreaterThan(args.AfterGuildId, 0, nameof(args.AfterGuildId));
            options = RequestOptions.CreateOrClone(options);

            int limit = args.Limit.GetValueOrDefault(int.MaxValue);
            ulong afterGuildId = args.AfterGuildId.GetValueOrDefault(0);

            return SendAsync<IReadOnlyCollection<UserGuild>>("GET", () => $"users/@me/guilds?limit={limit}&after={afterGuildId}&with_counts=true", new BucketIds(), options: options);
        }

        public Task<Application> GetMyApplicationAsync(RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            return SendAsync<Application>("GET", () => "oauth2/applications/@me", new BucketIds(), options: options);
        }

        public Task<Application> GetCurrentBotApplicationAsync(RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            return SendAsync<Application>("GET", () => "applications/@me", new BucketIds(), options: options);
        }

        public Task<Application> ModifyCurrentBotApplicationAsync(ModifyCurrentApplicationBotParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<Application>("PATCH", () => "applications/@me", args, new BucketIds(), options: options);
        }

        public Task<User> ModifySelfAsync(Rest.ModifyCurrentUserParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNullOrEmpty(args.Username, nameof(args.Username));
            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<User>("PATCH", () => "users/@me", args, new BucketIds(), options: options);
        }

        public Task ModifyMyNickAsync(ulong guildId, Rest.ModifyCurrentUserNickParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNull(args.Nickname, nameof(args.Nickname));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendJsonAsync("PATCH", () => $"guilds/{guildId}/members/@me/nick", args, ids, options: options);
        }

        public Task<Channel> CreateDMChannelAsync(CreateDMChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args.RecipientId, 0, nameof(args.RecipientId));
            options = RequestOptions.CreateOrClone(options);

            return SendJsonAsync<Channel>("POST", () => "users/@me/channels", args, new BucketIds(), options: options);
        }

        public Task<GuildMember> GetCurrentUserGuildMember(ulong guildId, RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds();
            return SendAsync<GuildMember>("GET", () => $"users/@me/guilds/{guildId}/member", ids, options: options);
        }
        #endregion

        #region Voice Regions
        public Task<IReadOnlyCollection<VoiceRegion>> GetVoiceRegionsAsync(RequestOptions options = null)
        {
            options = RequestOptions.CreateOrClone(options);
            return SendAsync<IReadOnlyCollection<VoiceRegion>>("GET", () => "voice/regions", new BucketIds(), options: options);
        }

        public Task<IReadOnlyCollection<VoiceRegion>> GetGuildVoiceRegionsAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync<IReadOnlyCollection<VoiceRegion>>("GET", () => $"guilds/{guildId}/regions", ids, options: options);
        }
        #endregion

        #region Audit logs
        public Task<AuditLog> GetAuditLogsAsync(ulong guildId, GetAuditLogsParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            options = RequestOptions.CreateOrClone(options);

            int limit = args.Limit.GetValueOrDefault(int.MaxValue);

            var ids = new BucketIds(guildId: guildId);
            Expression<Func<string>> endpoint;

            var queryArgs = new StringBuilder();
            if (args.BeforeEntryId.IsSpecified)
            {
                queryArgs.Append("&before=")
                    .Append(args.BeforeEntryId);
            }
            if (args.UserId.IsSpecified)
            {
                queryArgs.Append("&user_id=")
                    .Append(args.UserId.Value);
            }
            if (args.ActionType.IsSpecified)
            {
                queryArgs.Append("&action_type=")
                    .Append(args.ActionType.Value);
            }
            if (args.AfterEntryId.IsSpecified)
            {
                queryArgs.Append("&after=")
                    .Append(args.AfterEntryId);
            }

            // Still use string interpolation for the query w/o params, as this is necessary for CreateBucketId
            endpoint = () => $"guilds/{guildId}/audit-logs?limit={limit}{queryArgs.ToString()}";
            return SendAsync<AuditLog>("GET", endpoint, ids, options: options);
        }
        #endregion

        #region Webhooks
        public Task<Webhook> CreateWebhookAsync(ulong channelId, CreateWebhookParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNull(args.Name, nameof(args.Name));
            options = RequestOptions.CreateOrClone(options);
            var ids = new BucketIds(channelId: channelId);

            return SendJsonAsync<Webhook>("POST", () => $"channels/{channelId}/webhooks", args, ids, options: options);
        }

        public async Task<Webhook> GetWebhookAsync(ulong webhookId, RequestOptions options = null)
        {
            Preconditions.NotEqual(webhookId, 0, nameof(webhookId));
            options = RequestOptions.CreateOrClone(options);

            try
            {
                if (AuthTokenType == TokenType.Webhook)
                    return await SendAsync<Webhook>("GET", () => $"webhooks/{webhookId}/{AuthToken}", new BucketIds(), options: options).ConfigureAwait(false);
                else
                    return await SendAsync<Webhook>("GET", () => $"webhooks/{webhookId}", new BucketIds(), options: options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) { return null; }
        }

        public Task<Webhook> ModifyWebhookAsync(ulong webhookId, ModifyWebhookParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(webhookId, 0, nameof(webhookId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));
            options = RequestOptions.CreateOrClone(options);

            if (AuthTokenType == TokenType.Webhook)
                return SendJsonAsync<Webhook>("PATCH", () => $"webhooks/{webhookId}/{AuthToken}", args, new BucketIds(), options: options);
            else
                return SendJsonAsync<Webhook>("PATCH", () => $"webhooks/{webhookId}", args, new BucketIds(), options: options);
        }

        public Task DeleteWebhookAsync(ulong webhookId, RequestOptions options = null)
        {
            Preconditions.NotEqual(webhookId, 0, nameof(webhookId));
            options = RequestOptions.CreateOrClone(options);

            if (AuthTokenType == TokenType.Webhook)
                return SendAsync("DELETE", () => $"webhooks/{webhookId}/{AuthToken}", new BucketIds(), options: options);
            else
                return SendAsync("DELETE", () => $"webhooks/{webhookId}", new BucketIds(), options: options);
        }

        public Task<IReadOnlyCollection<Webhook>> GetGuildWebhooksAsync(ulong guildId, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(guildId: guildId);
            return SendAsync<IReadOnlyCollection<Webhook>>("GET", () => $"guilds/{guildId}/webhooks", ids, options: options);
        }

        public Task<IReadOnlyCollection<Webhook>> GetChannelWebhooksAsync(ulong channelId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            options = RequestOptions.CreateOrClone(options);

            var ids = new BucketIds(channelId: channelId);
            return SendAsync<IReadOnlyCollection<Webhook>>("GET", () => $"channels/{channelId}/webhooks", ids, options: options);
        }
        #endregion

        #region Helpers
        /// <exception cref="InvalidOperationException">Client is not logged in.</exception>
        protected void CheckState()
        {
            if (LoginState != LoginState.LoggedIn)
                throw new InvalidOperationException("Client is not logged in.");
        }
        protected static double ToMilliseconds(Stopwatch stopwatch) => Math.Round((double)stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0, 2);
        protected string SerializeJson(object value)
        {
            var sb = new StringBuilder(256);
            using (TextWriter text = new StringWriter(sb, CultureInfo.InvariantCulture))
            using (JsonWriter writer = new JsonTextWriter(text))
                _serializer.Serialize(writer, value);
            return sb.ToString();
        }
        protected T DeserializeJson<T>(Stream jsonStream)
        {
            using (TextReader text = new StreamReader(jsonStream))
            using (JsonReader reader = new JsonTextReader(text))
                return _serializer.Deserialize<T>(reader);
        }

        protected async Task<T> NullifyNotFound<T>(Task<T> sendTask) where T : class
        {
            try
            {
                var result = await sendTask.ConfigureAwait(false);

                if (sendTask.Exception != null)
                {
                    if (sendTask.Exception.InnerException is HttpException x)
                    {
                        if (x.HttpCode == HttpStatusCode.NotFound)
                        {
                            return null;
                        }
                    }

                    throw sendTask.Exception;
                }
                else
                    return result;
            }
            catch (HttpException x) when (x.HttpCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }
        internal class BucketIds
        {
            public ulong GuildId { get; internal set; }
            public ulong ChannelId { get; internal set; }
            public ulong WebhookId { get; internal set; }
            public string HttpMethod { get; internal set; }

            internal BucketIds(ulong guildId = 0, ulong channelId = 0, ulong webhookId = 0)
            {
                GuildId = guildId;
                ChannelId = channelId;
                WebhookId = webhookId;
            }

            internal object[] ToArray()
                => new object[] { HttpMethod, GuildId, ChannelId, WebhookId };

            internal Dictionary<string, string> ToMajorParametersDictionary()
            {
                var dict = new Dictionary<string, string>();
                if (GuildId != 0)
                    dict["GuildId"] = GuildId.ToString();
                if (ChannelId != 0)
                    dict["ChannelId"] = ChannelId.ToString();
                if (WebhookId != 0)
                    dict["WebhookId"] = WebhookId.ToString();
                return dict;
            }

            internal static int? GetIndex(string name)
            {
                return name switch
                {
                    "httpMethod" => 0,
                    "guildId" => 1,
                    "channelId" => 2,
                    "webhookId" => 3,
                    _ => null,
                };
            }
        }

        private static string GetEndpoint(Expression<Func<string>> endpointExpr)
        {
            return endpointExpr.Compile()();
        }
        private static BucketId GetBucketId(string httpMethod, BucketIds ids, Expression<Func<string>> endpointExpr, string callingMethod)
        {
            ids.HttpMethod ??= httpMethod;
            return _bucketIdGenerators.GetOrAdd(callingMethod, x => CreateBucketId(endpointExpr))(ids);
        }

        private static Func<BucketIds, BucketId> CreateBucketId(Expression<Func<string>> endpoint)
        {
            try
            {
                //Is this a constant string?
                if (endpoint.Body.NodeType == ExpressionType.Constant)
                    return x => BucketId.Create(x.HttpMethod, (endpoint.Body as ConstantExpression).Value.ToString(), x.ToMajorParametersDictionary());

                var builder = new StringBuilder();
                var methodCall = endpoint.Body as MethodCallExpression;
                var methodArgs = methodCall.Arguments.ToArray();
                string format = (methodArgs[0] as ConstantExpression).Value as string;

                //Unpack the array, if one exists (happens with 4+ parameters)
                if (methodArgs.Length > 1 && methodArgs[1].NodeType == ExpressionType.NewArrayInit)
                {
                    var arrayExpr = methodArgs[1] as NewArrayExpression;
                    var elements = arrayExpr.Expressions.ToArray();
                    Array.Resize(ref methodArgs, elements.Length + 1);
                    Array.Copy(elements, 0, methodArgs, 1, elements.Length);
                }

                int endIndex = format.IndexOf('?'); //Don't include params
                if (endIndex == -1)
                    endIndex = format.Length;

                int lastIndex = 0;
                while (true)
                {
                    int leftIndex = format.IndexOf("{", lastIndex);
                    if (leftIndex == -1 || leftIndex > endIndex)
                    {
                        builder.Append(format, lastIndex, endIndex - lastIndex);
                        break;
                    }
                    builder.Append(format, lastIndex, leftIndex - lastIndex);
                    int rightIndex = format.IndexOf("}", leftIndex);

                    int argId = int.Parse(format.Substring(leftIndex + 1, rightIndex - leftIndex - 1), NumberStyles.None, CultureInfo.InvariantCulture);
                    string fieldName = GetFieldName(methodArgs[argId + 1]);

                    var mappedId = BucketIds.GetIndex(fieldName);

                    if (!mappedId.HasValue && rightIndex != endIndex && format.Length > rightIndex + 1 && format[rightIndex + 1] == '/') //Ignore the next slash
                        rightIndex++;

                    if (mappedId.HasValue)
                        builder.Append($"{{{mappedId.Value}}}");

                    lastIndex = rightIndex + 1;
                }
                if (builder[builder.Length - 1] == '/')
                    builder.Remove(builder.Length - 1, 1);

                format = builder.ToString();

                return x => BucketId.Create(x.HttpMethod, string.Format(format, x.ToArray()), x.ToMajorParametersDictionary());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to generate the bucket id for this operation.", ex);
            }
        }

        private static string GetFieldName(Expression expr)
        {
            if (expr.NodeType == ExpressionType.Convert)
                expr = (expr as UnaryExpression).Operand;

            if (expr.NodeType != ExpressionType.MemberAccess)
                throw new InvalidOperationException("Unsupported expression");

            return (expr as MemberExpression).Member.Name;
        }

        private static string WebhookQuery(bool wait = false, ulong? threadId = null)
        {
            List<string> querys = new List<string>() { };
            if (wait)
                querys.Add("wait=true");
            if (threadId.HasValue)
                querys.Add($"thread_id={threadId}");

            return $"{string.Join("&", querys)}";
        }

        #endregion

        #region Application Role Connections Metadata

        public Task<RoleConnectionMetadata[]> GetApplicationRoleConnectionMetadataRecordsAsync(RequestOptions options = null)
            => SendAsync<RoleConnectionMetadata[]>("GET", () => $"applications/{CurrentApplicationId}/role-connections/metadata", new BucketIds(), options: options);

        public Task<RoleConnectionMetadata[]> UpdateApplicationRoleConnectionMetadataRecordsAsync(RoleConnectionMetadata[] roleConnections, RequestOptions options = null)
            => SendJsonAsync<RoleConnectionMetadata[]>("PUT", () => $"applications/{CurrentApplicationId}/role-connections/metadata", roleConnections, new BucketIds(), options: options);

        public Task<RoleConnection> GetUserApplicationRoleConnectionAsync(ulong applicationId, RequestOptions options = null)
            => SendAsync<RoleConnection>("GET", () => $"users/@me/applications/{applicationId}/role-connection", new BucketIds(), options: options);

        public Task<RoleConnection> ModifyUserApplicationRoleConnectionAsync(ulong applicationId, RoleConnection connection, RequestOptions options = null)
            => SendJsonAsync<RoleConnection>("PUT", () => $"users/@me/applications/{applicationId}/role-connection", connection, new BucketIds(), options: options);

        #endregion

        #region App Monetization

        public Task<Entitlement> CreateEntitlementAsync(CreateEntitlementParams args, RequestOptions options = null)
            => SendJsonAsync<Entitlement>("POST", () => $"applications/{CurrentApplicationId}/entitlements", args, new BucketIds(), options: options);

        public Task DeleteEntitlementAsync(ulong entitlementId, RequestOptions options = null)
            => SendAsync("DELETE", () => $"applications/{CurrentApplicationId}/entitlements/{entitlementId}", new BucketIds(), options: options);

        public Task<Entitlement[]> ListEntitlementAsync(ListEntitlementsParams args, RequestOptions options = null)
        {
            var query = $"?limit={args.Limit.GetValueOrDefault(100)}";

            if (args.UserId.IsSpecified)
            {
                query += $"&user_id={args.UserId.Value}";
            }

            if (args.SkuIds.IsSpecified)
            {
                query += $"&sku_ids={WebUtility.UrlEncode(string.Join(",", args.SkuIds.Value))}";
            }

            if (args.BeforeId.IsSpecified)
            {
                query += $"&before={args.BeforeId.Value}";
            }

            if (args.AfterId.IsSpecified)
            {
                query += $"&after={args.AfterId.Value}";
            }

            if (args.GuildId.IsSpecified)
            {
                query += $"&guild_id={args.GuildId.Value}";
            }

            if (args.ExcludeEnded.IsSpecified)
            {
                query += $"&exclude_ended={args.ExcludeEnded.Value}";
            }

            return SendAsync<Entitlement[]>("GET", () => $"applications/{CurrentApplicationId}/entitlements{query}", new BucketIds(), options: options);
        }

        public Task<SKU[]> ListSKUsAsync(RequestOptions options = null)
            => SendAsync<SKU[]>("GET", () => $"applications/{CurrentApplicationId}/skus", new BucketIds(), options: options);

        public Task ConsumeEntitlementAsync(ulong entitlementId, RequestOptions options = null)
            => SendAsync("POST", () => $"applications/{CurrentApplicationId}/entitlements/{entitlementId}/consume", new BucketIds(), options: options);

        #endregion

        #region Polls

        public Task<PollAnswerVoters> GetPollAnswerVotersAsync(ulong channelId, ulong messageId, uint answerId, int limit = 100, ulong? afterId = null, RequestOptions options = null)
        {
            var urlParams = $"?limit={limit}{(afterId is not null ? $"&after={afterId}" : string.Empty)}";
            return SendAsync<PollAnswerVoters>("GET", () => $"channels/{channelId}/polls/{messageId}/answers/{answerId}{urlParams}", new BucketIds(channelId: channelId), options: options);
        }

        public Task<Message> ExpirePollAsync(ulong channelId, ulong messageId, RequestOptions options = null)
            => SendAsync<Message>("POST", () => $"channels/{channelId}/polls/{messageId}/expire", new BucketIds(channelId: channelId), options: options);

        #endregion
    }
}
