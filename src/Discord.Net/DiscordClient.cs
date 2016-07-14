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
    public class DiscordClient : IDiscordClient
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
        internal readonly RequestQueue _requestQueue;
        internal bool _isDisposed;
        internal SelfUser _currentUser;
        private bool _isFirstLogSub;

        public API.DiscordApiClient ApiClient { get; }
        internal LogManager LogManager { get; }
        public LoginState LoginState { get; private set; }

        /// <summary> Creates a new REST-only discord client. </summary>
        public DiscordClient() : this(new DiscordConfig()) { }
        /// <summary> Creates a new REST-only discord client. </summary>
        public DiscordClient(DiscordConfig config)
        {
            LogManager = new LogManager(config.LogLevel);
            LogManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);
            _clientLogger = LogManager.CreateLogger("Client");
            _restLogger = LogManager.CreateLogger("Rest");
            _queueLogger = LogManager.CreateLogger("Queue");
            _isFirstLogSub = true;

            _connectionLock = new SemaphoreSlim(1, 1);

            _requestQueue = new RequestQueue();
            _requestQueue.RateLimitTriggered += async (id, bucket, millis) =>
            {
                await _queueLogger.WarningAsync($"Rate limit triggered (id = \"{id ?? "null"}\")").ConfigureAwait(false);
                if (bucket == null && id != null)
                    await _queueLogger.WarningAsync($"Unknown rate limit bucket \"{id ?? "null"}\"").ConfigureAwait(false);
            };

            var restProvider = config.RestClientProvider;
            var webSocketProvider = (this is DiscordSocketClient) ? (config as DiscordSocketConfig)?.WebSocketProvider : null; //TODO: Clean this check
            ApiClient = new API.DiscordApiClient(restProvider, webSocketProvider, requestQueue: _requestQueue);
            ApiClient.SentRequest += async (method, endpoint, millis) => await _restLogger.VerboseAsync($"{method} {endpoint}: {millis} ms").ConfigureAwait(false);
        }

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
                {
                    try
                    {
                        await ApiClient.ValidateTokenAsync().ConfigureAwait(false);
                    }
                    catch (HttpException ex)
                    {
                        throw new ArgumentException("Token validation failed", nameof(token), ex);
                    }
                }

                await OnLoginAsync().ConfigureAwait(false);

                LoginState = LoginState.LoggedIn;
            }
            catch (Exception)
            {
                await LogoutInternalAsync().ConfigureAwait(false);
                throw;
            }

            await _loggedInEvent.InvokeAsync().ConfigureAwait(false);
        }
        protected virtual Task OnLoginAsync() => Task.CompletedTask;

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
        public async Task<IReadOnlyCollection<IConnection>> GetConnectionsAsync()
        {
            var models = await ApiClient.GetMyConnectionsAsync().ConfigureAwait(false);
            return models.Select(x => new Connection(x)).ToImmutableArray();
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
                else
                    return new DMChannel(this, new User(model.Recipient.Value), model);
            }
            return null;
        }
        /// <inheritdoc />
        public virtual async Task<IReadOnlyCollection<IDMChannel>> GetDMChannelsAsync()
        {
            var models = await ApiClient.GetMyDMsAsync().ConfigureAwait(false);
            return models.Select(x => new DMChannel(this, new User(x.Recipient.Value), x)).ToImmutableArray();
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
                var model = await ApiClient.GetSelfAsync().ConfigureAwait(false);
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
                _isDisposed = true;
            ApiClient.Dispose();
        }
        /// <inheritdoc />
        public void Dispose() => Dispose(true);

        protected async Task WriteInitialLog()
        {
            if (this is DiscordSocketClient)
                await _clientLogger.InfoAsync($"DiscordSocketClient v{DiscordConfig.Version} (Gateway v{DiscordConfig.GatewayAPIVersion}, {DiscordConfig.GatewayEncoding})").ConfigureAwait(false);
            else
                await _clientLogger.InfoAsync($"DiscordClient v{DiscordConfig.Version}").ConfigureAwait(false);
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
