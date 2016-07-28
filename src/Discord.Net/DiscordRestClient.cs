using Discord.API.Rest;
using Discord.Logging;
using Discord.Net;
using Discord.Net.Queue;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Discord
{
    public class DiscordRestClient : IDiscordClient
    {
        private readonly object _eventLock = new object();

        public event Func<LogMessage, Task> Log { add { _logEvent.Add(value); } remove { _logEvent.Remove(value); } }
        private readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new AsyncEvent<Func<LogMessage, Task>>();

        public event Func<Task> LoggedIn { add { _loggedInEvent.Add(value); } remove { _loggedInEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _loggedInEvent = new AsyncEvent<Func<Task>>();
        public event Func<Task> LoggedOut { add { _loggedOutEvent.Add(value); } remove { _loggedOutEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _loggedOutEvent = new AsyncEvent<Func<Task>>();

        internal readonly ILogger _clientLogger, _restLogger, _queueLogger;
        internal readonly SemaphoreSlim _connectionLock;
        internal SelfUser _currentUser;
        private bool _isFirstLogSub;
        internal bool _isDisposed;

        public API.DiscordRestApiClient ApiClient { get; }
        internal LogManager LogManager { get; }
        public LoginState LoginState { get; private set; }

        /// <summary> Creates a new REST-only discord client. </summary>
        public DiscordRestClient() : this(new DiscordRestConfig()) { }
        public DiscordRestClient(DiscordRestConfig config) : this(config, CreateApiClient(config)) { }
        /// <summary> Creates a new REST-only discord client. </summary>
        internal DiscordRestClient(DiscordRestConfig config, API.DiscordRestApiClient client)
        {
            ApiClient = client;
            LogManager = new LogManager(config.LogLevel);
            LogManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);
            _clientLogger = LogManager.CreateLogger("Client");
            _restLogger = LogManager.CreateLogger("Rest");
            _queueLogger = LogManager.CreateLogger("Queue");
            _isFirstLogSub = true;

            _connectionLock = new SemaphoreSlim(1, 1);

            ApiClient.RequestQueue.RateLimitTriggered += async (id, bucket, millis) =>
            {
                await _queueLogger.WarningAsync($"Rate limit triggered (id = \"{id ?? "null"}\")").ConfigureAwait(false);
                if (bucket == null && id != null)
                    await _queueLogger.WarningAsync($"Unknown rate limit bucket \"{id ?? "null"}\"").ConfigureAwait(false);
            };
            ApiClient.SentRequest += async (method, endpoint, millis) => await _restLogger.VerboseAsync($"{method} {endpoint}: {millis} ms").ConfigureAwait(false);
        }
        private static API.DiscordRestApiClient CreateApiClient(DiscordRestConfig config)
            => new API.DiscordRestApiClient(config.RestClientProvider, requestQueue: new RequestQueue());

        /// <inheritdoc />
        public async Task LoginAsync(TokenType tokenType, string token, bool validateToken = true)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LoginInternalAsync(tokenType, token, validateToken).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task LoginInternalAsync(TokenType tokenType, string token, bool validateToken)
        {
            if (_isFirstLogSub)
            {
                _isFirstLogSub = false;
                await WriteInitialLog().ConfigureAwait(false);
            }

            if (LoginState != LoginState.LoggedOut)
                await LogoutInternalAsync().ConfigureAwait(false);
            LoginState = LoginState.LoggingIn;

            try
            {
                await ApiClient.LoginAsync(tokenType, token).ConfigureAwait(false);
                if (validateToken)
                    await ValidateTokenAsync(tokenType, token).ConfigureAwait(false);
                await OnLoginAsync(tokenType, token).ConfigureAwait(false);

                LoginState = LoginState.LoggedIn;
            }
            catch (Exception)
            {
                await LogoutInternalAsync().ConfigureAwait(false);
                throw;
            }

            await _loggedInEvent.InvokeAsync().ConfigureAwait(false);
        }
        protected virtual async Task ValidateTokenAsync(TokenType tokenType, string token)
        {
            try
            {
                var user = await GetCurrentUserAsync().ConfigureAwait(false);
                if (user == null) //Is using a cached DiscordClient
                    user = new SelfUser(this, await ApiClient.GetMyUserAsync().ConfigureAwait(false));

                if (user.IsBot && tokenType == TokenType.User)
                    throw new InvalidOperationException($"A bot token used provided with {nameof(TokenType)}.{nameof(TokenType.User)}");
                else if (!user.IsBot && tokenType == TokenType.Bot) //Discord currently sends a 401 in this case
                    throw new InvalidOperationException($"A user token used provided with {nameof(TokenType)}.{nameof(TokenType.Bot)}");
            }
            catch (HttpException ex)
            {
                throw new ArgumentException("Token validation failed", nameof(token), ex);
            }
        }
        protected virtual Task OnLoginAsync(TokenType tokenType, string token) => Task.CompletedTask;


        /// <inheritdoc />
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
            if (LoginState == LoginState.LoggedOut) return;
            LoginState = LoginState.LoggingOut;

            await ApiClient.LogoutAsync().ConfigureAwait(false);
            
            await OnLogoutAsync().ConfigureAwait(false);

            _currentUser = null;

            LoginState = LoginState.LoggedOut;

            await _loggedOutEvent.InvokeAsync().ConfigureAwait(false);
        }
        protected virtual Task OnLogoutAsync() => Task.CompletedTask;

        /// <inheritdoc />
        public async Task<IApplication> GetApplicationInfoAsync()
        {
            var model = await ApiClient.GetMyApplicationAsync().ConfigureAwait(false);
            return new Application(this, model);
        }
        
        /// <inheritdoc />
        public virtual async Task<IChannel> GetChannelAsync(ulong id)
        {
            var model = await ApiClient.GetChannelAsync(id).ConfigureAwait(false);
            if (model != null)
            {
                if (model.GuildId.IsSpecified)
                {
                    var guildModel = await ApiClient.GetGuildAsync(model.GuildId.Value).ConfigureAwait(false);
                    if (guildModel != null)
                    {
                        var guild = new Guild(this, guildModel);
                        return guild.ToChannel(model);
                    }
                }
                else if (model.Type == ChannelType.DM)
                    return new DMChannel(this, new User(model.Recipients.Value[0]), model);
                else if (model.Type == ChannelType.Group)
                {
                    var channel = new GroupChannel(this, model);
                    channel.UpdateUsers(model.Recipients.Value, UpdateSource.Creation);
                    return channel;
                }
                else
                    throw new InvalidOperationException($"Unexpected channel type: {model.Type}");
            }
            return null;
        }
        /// <inheritdoc />
        public virtual async Task<IReadOnlyCollection<IPrivateChannel>> GetPrivateChannelsAsync()
        {
            var models = await ApiClient.GetMyPrivateChannelsAsync().ConfigureAwait(false);
            return models.Select(x => new DMChannel(this, new User(x.Recipients.Value[0]), x)).ToImmutableArray();
        }
        
        /// <inheritdoc />
        public async Task<IReadOnlyCollection<IConnection>> GetConnectionsAsync()
        {
            var models = await ApiClient.GetMyConnectionsAsync().ConfigureAwait(false);
            return models.Select(x => new Connection(x)).ToImmutableArray();
        }

        /// <inheritdoc />
        public virtual async Task<IInvite> GetInviteAsync(string inviteIdOrXkcd)
        {
            var model = await ApiClient.GetInviteAsync(inviteIdOrXkcd).ConfigureAwait(false);
            if (model != null)
                return new Invite(this, model);
            return null;
        }

        /// <inheritdoc />
        public virtual async Task<IGuild> GetGuildAsync(ulong id)
        {
            var model = await ApiClient.GetGuildAsync(id).ConfigureAwait(false);
            if (model != null)
                return new Guild(this, model);
            return null;
        }
        /// <inheritdoc />
        public virtual async Task<GuildEmbed?> GetGuildEmbedAsync(ulong id)
        {
            var model = await ApiClient.GetGuildEmbedAsync(id).ConfigureAwait(false);
            if (model != null)
                return new GuildEmbed(model);
            return null;
        }
        /// <inheritdoc />
        public virtual async Task<IReadOnlyCollection<IUserGuild>> GetGuildSummariesAsync()
        {
            var models = await ApiClient.GetMyGuildsAsync().ConfigureAwait(false);
            return models.Select(x => new UserGuild(this, x)).ToImmutableArray();
        }
        /// <inheritdoc />
        public virtual async Task<IReadOnlyCollection<IGuild>> GetGuildsAsync()
        {
            var summaryModels = await ApiClient.GetMyGuildsAsync().ConfigureAwait(false);
            var guilds = ImmutableArray.CreateBuilder<IGuild>(summaryModels.Count);
            foreach (var summaryModel in summaryModels)
            {
                var guildModel = await ApiClient.GetGuildAsync(summaryModel.Id).ConfigureAwait(false);
                if (guildModel != null)
                    guilds.Add(new Guild(this, guildModel));
            }
            return guilds.ToImmutable();
        }
        /// <inheritdoc />
        public virtual async Task<IGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null)
        {
            var args = new CreateGuildParams();
            var model = await ApiClient.CreateGuildAsync(args).ConfigureAwait(false);
            return new Guild(this, model);
        }

        /// <inheritdoc />
        public virtual async Task<IUser> GetUserAsync(ulong id)
        {
            var model = await ApiClient.GetUserAsync(id).ConfigureAwait(false);
            if (model != null)
                return new User(model);
            return null;
        }
        /// <inheritdoc />
        public virtual async Task<IUser> GetUserAsync(string username, string discriminator)
        {
            var model = await ApiClient.GetUserAsync(username, discriminator).ConfigureAwait(false);
            if (model != null)
                return new User(model);
            return null;
        }
        /// <inheritdoc />
        public virtual async Task<ISelfUser> GetCurrentUserAsync()
        {
            var user = _currentUser;
            if (user == null)
            {
                var model = await ApiClient.GetMyUserAsync().ConfigureAwait(false);
                user = new SelfUser(this, model);
                _currentUser = user;
            }
            return user;
        }

        /// <inheritdoc />
        public virtual async Task<IReadOnlyCollection<IUser>> QueryUsersAsync(string query, int limit)
        {
            var models = await ApiClient.QueryUsersAsync(query, limit).ConfigureAwait(false);
            return models.Select(x => new User(x)).ToImmutableArray();
        }

        /// <inheritdoc />
        public virtual async Task<IReadOnlyCollection<IVoiceRegion>> GetVoiceRegionsAsync()
        {
            var models = await ApiClient.GetVoiceRegionsAsync().ConfigureAwait(false);
            return models.Select(x => new VoiceRegion(x)).ToImmutableArray();
        }
        /// <inheritdoc />
        public virtual async Task<IVoiceRegion> GetVoiceRegionAsync(string id)
        {
            var models = await ApiClient.GetVoiceRegionsAsync().ConfigureAwait(false);
            return models.Select(x => new VoiceRegion(x)).Where(x => x.Id == id).FirstOrDefault();
        }

        internal virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                ApiClient.Dispose();
                _isDisposed = true;
            }
        }
        /// <inheritdoc />
        public void Dispose() => Dispose(true);

        private async Task WriteInitialLog()
        {
            if (this is DiscordSocketClient)
                await _clientLogger.InfoAsync($"DiscordSocketClient v{DiscordConfig.Version} (API v{DiscordConfig.APIVersion}, {DiscordSocketConfig.GatewayEncoding})").ConfigureAwait(false);
            else if (this is DiscordRpcClient)
                await _clientLogger.InfoAsync($"DiscordRpcClient v{DiscordConfig.Version} (API v{DiscordConfig.APIVersion}, RPC API v{DiscordRpcConfig.RpcAPIVersion})").ConfigureAwait(false);
            else
                await _clientLogger.InfoAsync($"DiscordClient v{DiscordConfig.Version} (API v{DiscordConfig.APIVersion})").ConfigureAwait(false);
            await _clientLogger.VerboseAsync($"Runtime: {RuntimeInformation.FrameworkDescription.Trim()} ({ToArchString(RuntimeInformation.ProcessArchitecture)})").ConfigureAwait(false);
            await _clientLogger.VerboseAsync($"OS: {RuntimeInformation.OSDescription.Trim()} ({ToArchString(RuntimeInformation.OSArchitecture)})").ConfigureAwait(false);
            await _clientLogger.VerboseAsync($"Processors: {Environment.ProcessorCount}").ConfigureAwait(false);
        }

        private static string ToArchString(Architecture arch)
        {
            switch (arch)
            {
                case Architecture.X64: return "x64";
                case Architecture.X86: return "x86";
                default: return arch.ToString();
            }
        }

        ConnectionState IDiscordClient.ConnectionState => ConnectionState.Disconnected;
        ILogManager IDiscordClient.LogManager => LogManager;

        Task IDiscordClient.ConnectAsync() { throw new NotSupportedException(); }
        Task IDiscordClient.DisconnectAsync() { throw new NotSupportedException(); }
    }
}
