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
    public class DiscordRestApiClient : IDisposable
    {
        public event Func<string, string, double, Task> SentRequest { add { _sentRequestEvent.Add(value); } remove { _sentRequestEvent.Remove(value); } }
        private readonly AsyncEvent<Func<string, string, double, Task>> _sentRequestEvent = new AsyncEvent<Func<string, string, double, Task>>();
        
        protected readonly JsonSerializer _serializer;
        protected readonly SemaphoreSlim _stateLock;
        private readonly RestClientProvider _restClientProvider;

        protected string _authToken;
        protected bool _isDisposed;
        private CancellationTokenSource _loginCancelToken;
        private IRestClient _restClient;

        public LoginState LoginState { get; private set; }
        public TokenType AuthTokenType { get; private set; }
        internal RequestQueue RequestQueue { get; private set; }

        public DiscordRestApiClient(RestClientProvider restClientProvider, JsonSerializer serializer = null, RequestQueue requestQueue = null)
        {
            _restClientProvider = restClientProvider;
            _serializer = serializer ?? new JsonSerializer { ContractResolver = new DiscordContractResolver() };
            RequestQueue = requestQueue;

            _stateLock = new SemaphoreSlim(1, 1);

            SetBaseUrl(DiscordConfig.ClientAPIUrl);
        }
        internal void SetBaseUrl(string baseUrl)
        {
            _restClient = _restClientProvider(baseUrl);
            _restClient.SetHeader("accept", "*/*");
            _restClient.SetHeader("user-agent", DiscordRestConfig.UserAgent);
            _restClient.SetHeader("authorization", GetPrefixedToken(AuthTokenType, _authToken));
        }
        internal static string GetPrefixedToken(TokenType tokenType, string token)
        {
            switch (tokenType)
            {
                case TokenType.Bot:
                    return $"Bot {token}";
                case TokenType.Bearer:
                    return $"Bearer {token}";
                case TokenType.User:
                    return token;
                default:
                    throw new ArgumentException("Unknown OAuth token type", nameof(tokenType));
            }
        }
        internal virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _loginCancelToken?.Dispose();
                    (_restClient as IDisposable)?.Dispose();
                }
                _isDisposed = true;
            }
        }
        public void Dispose() => Dispose(true);

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
                _loginCancelToken = new CancellationTokenSource();

                AuthTokenType = TokenType.User;
                _authToken = null;
                await RequestQueue.SetCancelTokenAsync(_loginCancelToken.Token).ConfigureAwait(false);
                _restClient.SetCancelToken(_loginCancelToken.Token);

                AuthTokenType = tokenType;
                _authToken = token;
                _restClient.SetHeader("authorization", GetPrefixedToken(AuthTokenType, _authToken));

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
            if (LoginState == LoginState.LoggedOut) return;
            LoginState = LoginState.LoggingOut;

            try { _loginCancelToken?.Cancel(false); }
            catch { }

            await DisconnectInternalAsync().ConfigureAwait(false);
            await RequestQueue.ClearAsync().ConfigureAwait(false);

            await RequestQueue.SetCancelTokenAsync(CancellationToken.None).ConfigureAwait(false);
            _restClient.SetCancelToken(CancellationToken.None);

            LoginState = LoginState.LoggedOut;
        }

        internal virtual Task ConnectInternalAsync() => Task.CompletedTask;
        internal virtual Task DisconnectInternalAsync() => Task.CompletedTask;

        //REST
        public Task SendAsync(string method, string endpoint,
            GlobalBucket bucket = GlobalBucket.GeneralRest, RequestOptions options = null)
            => SendInternalAsync(method, endpoint, null, true, BucketGroup.Global, (int)bucket, 0, options);
        public Task SendAsync(string method, string endpoint, object payload,
            GlobalBucket bucket = GlobalBucket.GeneralRest, RequestOptions options = null)
            => SendInternalAsync(method, endpoint, payload, true, BucketGroup.Global, (int)bucket, 0, options);
        public async Task<TResponse> SendAsync<TResponse>(string method, string endpoint,
            GlobalBucket bucket = GlobalBucket.GeneralRest, RequestOptions options = null) where TResponse : class
            => DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, null, false, BucketGroup.Global, (int)bucket, 0, options).ConfigureAwait(false));
        public async Task<TResponse> SendAsync<TResponse>(string method, string endpoint, object payload, GlobalBucket bucket =
            GlobalBucket.GeneralRest, RequestOptions options = null) where TResponse : class
            => DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, payload, false, BucketGroup.Global, (int)bucket, 0, options).ConfigureAwait(false));

        public Task SendAsync(string method, string endpoint,
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            => SendInternalAsync(method, endpoint, null, true, BucketGroup.Guild, (int)bucket, guildId, options);
        public Task SendAsync(string method, string endpoint, object payload,
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            => SendInternalAsync(method, endpoint, payload, true, BucketGroup.Guild, (int)bucket, guildId, options);
        public async Task<TResponse> SendAsync<TResponse>(string method, string endpoint,
            GuildBucket bucket, ulong guildId, RequestOptions options = null) where TResponse : class
            => DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, null, false, BucketGroup.Guild, (int)bucket, guildId, options).ConfigureAwait(false));
        public async Task<TResponse> SendAsync<TResponse>(string method, string endpoint, object payload,
            GuildBucket bucket, ulong guildId, RequestOptions options = null) where TResponse : class
            => DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, payload, false, BucketGroup.Guild, (int)bucket, guildId, options).ConfigureAwait(false));

        //REST - Multipart
        public Task SendMultipartAsync(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs,
            GlobalBucket bucket = GlobalBucket.GeneralRest, RequestOptions options = null)
            => SendMultipartInternalAsync(method, endpoint, multipartArgs, true, BucketGroup.Global, (int)bucket, 0, options);
        public async Task<TResponse> SendMultipartAsync<TResponse>(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs,
            GlobalBucket bucket = GlobalBucket.GeneralRest, RequestOptions options = null) where TResponse : class
            => DeserializeJson<TResponse>(await SendMultipartInternalAsync(method, endpoint, multipartArgs, false, BucketGroup.Global, (int)bucket, 0, options).ConfigureAwait(false));

        public Task SendMultipartAsync(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs,
            GuildBucket bucket, ulong guildId, RequestOptions options = null)
            => SendMultipartInternalAsync(method, endpoint, multipartArgs, true, BucketGroup.Guild, (int)bucket, guildId, options);
        public async Task<TResponse> SendMultipartAsync<TResponse>(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs,
            GuildBucket bucket, ulong guildId, RequestOptions options = null) where TResponse : class
            => DeserializeJson<TResponse>(await SendMultipartInternalAsync(method, endpoint, multipartArgs, false, BucketGroup.Guild, (int)bucket, guildId, options).ConfigureAwait(false));

        //Core
        private async Task<Stream> SendInternalAsync(string method, string endpoint, object payload, bool headerOnly,
            BucketGroup group, int bucketId, ulong guildId, RequestOptions options = null)
        {
            var stopwatch = Stopwatch.StartNew();
            string json = null;
            if (payload != null)
                json = SerializeJson(payload);
            var responseStream = await RequestQueue.SendAsync(new RestRequest(_restClient, method, endpoint, json, headerOnly, options), group, bucketId, guildId).ConfigureAwait(false);
            stopwatch.Stop();

            double milliseconds = ToMilliseconds(stopwatch);
            await _sentRequestEvent.InvokeAsync(method, endpoint, milliseconds).ConfigureAwait(false);

            return responseStream;
        }
        private async Task<Stream> SendMultipartInternalAsync(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs, bool headerOnly,
            BucketGroup group, int bucketId, ulong guildId, RequestOptions options = null)
        {
            var stopwatch = Stopwatch.StartNew();
            var responseStream = await RequestQueue.SendAsync(new RestRequest(_restClient, method, endpoint, multipartArgs, headerOnly, options), group, bucketId, guildId).ConfigureAwait(false);
            int bytes = headerOnly ? 0 : (int)responseStream.Length;
            stopwatch.Stop();

            double milliseconds = ToMilliseconds(stopwatch);
            await _sentRequestEvent.InvokeAsync(method, endpoint, milliseconds).ConfigureAwait(false);

            return responseStream;
        }

        //Auth
        public async Task ValidateTokenAsync(RequestOptions options = null)
        {
            await SendAsync("GET", "auth/login", options: options).ConfigureAwait(false);
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
            Preconditions.GreaterThan(args._bitrate, 0, nameof(args.Bitrate));
            Preconditions.NotNullOrWhitespace(args._name, nameof(args.Name));

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
            Preconditions.AtLeast(args._position, 0, nameof(args.Position));
            Preconditions.NotNullOrEmpty(args._name, nameof(args.Name));

            return await SendAsync<Channel>("PATCH", $"channels/{channelId}", args, options: options).ConfigureAwait(false);
        }
        public async Task<Channel> ModifyGuildChannelAsync(ulong channelId, ModifyTextChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args._position, 0, nameof(args.Position));
            Preconditions.NotNullOrEmpty(args._name, nameof(args.Name));

            return await SendAsync<Channel>("PATCH", $"channels/{channelId}", args, options: options).ConfigureAwait(false);
        }
        public async Task<Channel> ModifyGuildChannelAsync(ulong channelId, ModifyVoiceChannelParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args._bitrate, 0, nameof(args.Bitrate));
            Preconditions.AtLeast(args._userLimit, 0, nameof(args.Bitrate));
            Preconditions.AtLeast(args._position, 0, nameof(args.Position));
            Preconditions.NotNullOrEmpty(args._name, nameof(args.Name));

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

        //Channel Recipients
        public async Task AddGroupRecipientAsync(ulong channelId, ulong userId, RequestOptions options = null)
        {
            Preconditions.GreaterThan(channelId, 0, nameof(channelId));
            Preconditions.GreaterThan(userId, 0, nameof(userId));

            await SendAsync("PUT", $"channels/{channelId}/recipients/{userId}", options: options).ConfigureAwait(false);

        }
        public async Task RemoveGroupRecipientAsync(ulong channelId, ulong userId, RequestOptions options = null)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(userId, 0, nameof(userId));

            await SendAsync("DELETE", $"channels/{channelId}/recipients/{userId}", options: options).ConfigureAwait(false);
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
            Preconditions.NotEqual(args._afkChannelId, 0, nameof(args.AFKChannelId));
            Preconditions.AtLeast(args._afkTimeout, 0, nameof(args.AFKTimeout));
            Preconditions.NotNullOrEmpty(args._name, nameof(args.Name));
            Preconditions.GreaterThan(args._ownerId, 0, nameof(args.OwnerId));
            Preconditions.NotNull(args._region, nameof(args.Region));

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
            Preconditions.AtLeast(args._deleteMessageDays, 0, nameof(args.DeleteMessageDays));

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
            Preconditions.AtLeast(args._expireBehavior, 0, nameof(args.ExpireBehavior));
            Preconditions.AtLeast(args._expireGracePeriod, 0, nameof(args.ExpireGracePeriod));

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
            Preconditions.AtLeast(args._maxAge, 0, nameof(args.MaxAge));
            Preconditions.AtLeast(args._maxUses, 0, nameof(args.MaxUses));

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
            Preconditions.GreaterThan(args._limit, 0, nameof(args.Limit));
            Preconditions.GreaterThan(args._afterUserId, 0, nameof(args.AfterUserId));

            int limit = args._limit.GetValueOrDefault(int.MaxValue);
            ulong afterUserId = args._afterUserId.GetValueOrDefault(0);

            List<GuildMember[]> result;
            if (args._limit.IsSpecified)
                result = new List<GuildMember[]>((limit + DiscordRestConfig.MaxUsersPerBatch - 1) / DiscordRestConfig.MaxUsersPerBatch);
            else
                result = new List<GuildMember[]>();

            while (true)
            {
                int runLimit = (limit >= DiscordRestConfig.MaxUsersPerBatch) ? DiscordRestConfig.MaxUsersPerBatch : limit;
                string endpoint = $"guilds/{guildId}/members?limit={runLimit}&after={afterUserId}";
                var models = await SendAsync<GuildMember[]>("GET", endpoint, options: options).ConfigureAwait(false);

                //Was this an empty batch?
                if (models.Length == 0) break;

                result.Add(models);

                limit -= DiscordRestConfig.MaxUsersPerBatch;
                afterUserId = models[models.Length - 1].User.Id;

                //Was this an incomplete (the last) batch?
                if (models.Length != DiscordRestConfig.MaxUsersPerBatch) break;
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
            Preconditions.AtLeast(args._color, 0, nameof(args.Color));
            Preconditions.NotNullOrEmpty(args._name, nameof(args.Name));
            Preconditions.AtLeast(args._position, 0, nameof(args.Position));

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
            ulong? relativeId = args._relativeMessageId.IsSpecified ? args._relativeMessageId.Value : (ulong?)null;
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

            int runs = (limit + DiscordRestConfig.MaxMessagesPerBatch - 1) / DiscordRestConfig.MaxMessagesPerBatch;
            int lastRunCount = limit - (runs - 1) * DiscordRestConfig.MaxMessagesPerBatch;
            var result = new API.Message[runs][];

            int i = 0;
            for (; i < runs; i++)
            {
                int runCount = i == (runs - 1) ? lastRunCount : DiscordRestConfig.MaxMessagesPerBatch;
                string endpoint;
                if (relativeId != null)
                    endpoint = $"channels/{channelId}/messages?limit={runCount}&{relativeDir}={relativeId}";
                else
                    endpoint = $"channels/{channelId}/messages?limit={runCount}";
                var models = await SendAsync<Message[]>("GET", endpoint, options: options).ConfigureAwait(false);

                //Was this an empty batch?
                if (models.Length == 0) break;

                //We can't assume these messages to be sorted by id (fails in rare cases), lets search for the highest/lowest id ourselves
                switch (args.RelativeDirection)
                {
                    case Direction.Before:
                    case Direction.Around:
                    default:
                        result[i] = models;
                        relativeId = ulong.MaxValue;
                        //Lowest id *should* be the last one
                        for (int j = models.Length - 1; j >= 0; j--)
                        {
                            if (models[j].Id < relativeId.Value)
                                relativeId = models[j].Id;
                        }
                        break;
                    case Direction.After:
                        result[runs - i - 1] = models;
                        relativeId = ulong.MinValue;
                        //Highest id *should* be the first one
                        for (int j = 0; j < models.Length; j++)
                        {
                            if (models[j].Id > relativeId.Value)
                                relativeId = models[j].Id;
                        }
                        break;
                }

                //Was this an incomplete (the last) batch?
                if (models.Length != DiscordRestConfig.MaxMessagesPerBatch) { i++; break; }
            }

            if (i > 1)
            {
                switch (args.RelativeDirection)
                {
                    case Direction.Before:
                    case Direction.Around:
                    default:
                        return result.Take(i).SelectMany(x => x).ToImmutableArray();
                    case Direction.After:
                        return result.Skip(runs - i).Take(i).SelectMany(x => x).ToImmutableArray();
                }
            }
            else if (i == 1)
            {
                switch (args.RelativeDirection)
                {
                    case Direction.Before:
                    case Direction.Around:
                    default:
                        return result[0];
                    case Direction.After:
                        return result[runs - 1];
                }
            }
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
            Preconditions.NotNullOrEmpty(args._content, nameof(args.Content));
            if (args._content.Length > DiscordRestConfig.MaxMessageSize)
                throw new ArgumentException($"Message content is too long, length must be less or equal to {DiscordRestConfig.MaxMessageSize}.", nameof(args.Content));

            if (guildId != 0)
                return await SendAsync<Message>("POST", $"channels/{channelId}/messages", args, GuildBucket.SendEditMessage, guildId, options: options).ConfigureAwait(false);
            else
                return await SendAsync<Message>("POST", $"channels/{channelId}/messages", args, GlobalBucket.DirectMessage, options: options).ConfigureAwait(false);
        }
        public Task<Message> UploadFileAsync(ulong guildId, ulong channelId, UploadFileParams args, RequestOptions options = null)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return UploadFileInternalAsync(guildId, channelId, args);
        }
        public Task<Message> UploadDMFileAsync(ulong channelId, UploadFileParams args, RequestOptions options = null)
        {
            return UploadFileInternalAsync(0, channelId, args);
        }
        private async Task<Message> UploadFileInternalAsync(ulong guildId, ulong channelId, UploadFileParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            if (args._content.GetValueOrDefault(null) == null)
                args._content = "";
            else if (args._content.IsSpecified)
            {
                if (args._content.Value == null)
                    args._content = "";
                if (args._content.Value?.Length > DiscordRestConfig.MaxMessageSize)
                    throw new ArgumentOutOfRangeException($"Message content is too long, length must be less or equal to {DiscordRestConfig.MaxMessageSize}.", nameof(args.Content));
            }

            if (guildId != 0)
                return await SendMultipartAsync<Message>("POST", $"channels/{channelId}/messages", args.ToDictionary(), GuildBucket.SendEditMessage, guildId, options: options).ConfigureAwait(false);
            else
                return await SendMultipartAsync<Message>("POST", $"channels/{channelId}/messages", args.ToDictionary(), GlobalBucket.DirectMessage, options: options).ConfigureAwait(false);
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

            var messageIds = args._messages;
            Preconditions.NotNull(args._messages, nameof(args.MessageIds));
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
            if (args._content.IsSpecified)
            {
                Preconditions.NotNullOrEmpty(args._content, nameof(args.Content));
                if (args._content.Value.Length > DiscordRestConfig.MaxMessageSize)
                    throw new ArgumentOutOfRangeException($"Message content is too long, length must be less or equal to {DiscordRestConfig.MaxMessageSize}.", nameof(args.Content));
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
        public async Task<User> GetMyUserAsync(RequestOptions options = null)
        {
            return await SendAsync<User>("GET", "users/@me", options: options).ConfigureAwait(false);
        }
        public async Task<IReadOnlyCollection<Connection>> GetMyConnectionsAsync(RequestOptions options = null)
        {
            return await SendAsync<IReadOnlyCollection<Connection>>("GET", "users/@me/connections", options: options).ConfigureAwait(false);
        }
        public async Task<IReadOnlyCollection<Channel>> GetMyPrivateChannelsAsync(RequestOptions options = null)
        {
            return await SendAsync<IReadOnlyCollection<Channel>>("GET", "users/@me/channels", options: options).ConfigureAwait(false);
        }
        public async Task<IReadOnlyCollection<UserGuild>> GetMyGuildsAsync(RequestOptions options = null)
        {
            return await SendAsync<IReadOnlyCollection<UserGuild>>("GET", "users/@me/guilds", options: options).ConfigureAwait(false);
        }
        public async Task<Application> GetMyApplicationAsync(RequestOptions options = null)
        {
            return await SendAsync<Application>("GET", "oauth2/applications/@me", options: options).ConfigureAwait(false);
        }
        public async Task<User> ModifySelfAsync(ModifyCurrentUserParams args, RequestOptions options = null)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNullOrEmpty(args._username, nameof(args.Username));

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
            Preconditions.GreaterThan(args._recipientId, 0, nameof(args.Recipient));

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
    }
}
