using Discord.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Rest
{
    public abstract class BaseDiscordClient : IDiscordClient
    {
        public event Func<LogMessage, Task> Log { add { _logEvent.Add(value); } remove { _logEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new AsyncEvent<Func<LogMessage, Task>>();

        public event Func<Task> LoggedIn { add { _loggedInEvent.Add(value); } remove { _loggedInEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _loggedInEvent = new AsyncEvent<Func<Task>>();
        public event Func<Task> LoggedOut { add { _loggedOutEvent.Add(value); } remove { _loggedOutEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _loggedOutEvent = new AsyncEvent<Func<Task>>();

        internal readonly Logger _restLogger, _queueLogger;
        internal readonly SemaphoreSlim _connectionLock;
        private bool _isFirstLogin;
        private bool _isDisposed;

        internal API.DiscordRestApiClient ApiClient { get; }
        internal LogManager LogManager { get; }
        public LoginState LoginState { get; private set; }
        public ISelfUser CurrentUser { get; protected set; }
        
        /// <summary> Creates a new REST-only discord client. </summary>
        internal BaseDiscordClient(DiscordRestConfig config, API.DiscordRestApiClient client)
        {
            ApiClient = client;
            LogManager = new LogManager(config.LogLevel);
            LogManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);

            _connectionLock = new SemaphoreSlim(1, 1);
            _restLogger = LogManager.CreateLogger("Rest");
            _queueLogger = LogManager.CreateLogger("Queue");
            _isFirstLogin = config.DisplayInitialLog;

            ApiClient.RequestQueue.RateLimitTriggered += async (id, info) =>
            {
                if (info == null)
                    await _queueLogger.WarningAsync($"Preemptive Rate limit triggered: {id ?? "null"}").ConfigureAwait(false);
                else
                    await _queueLogger.WarningAsync($"Rate limit triggered: {id ?? "null"}").ConfigureAwait(false);
            };
            ApiClient.SentRequest += async (method, endpoint, millis) => await _restLogger.VerboseAsync($"{method} {endpoint}: {millis} ms").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task LoginAsync(TokenType tokenType, string token, bool validateToken = true)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LoginInternalAsync(tokenType, token).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task LoginInternalAsync(TokenType tokenType, string token)
        {
            if (_isFirstLogin)
            {
                _isFirstLogin = false;
                await LogManager.WriteInitialLog().ConfigureAwait(false);
            }

            if (LoginState != LoginState.LoggedOut)
                await LogoutInternalAsync().ConfigureAwait(false);
            LoginState = LoginState.LoggingIn;

            try
            {
                await ApiClient.LoginAsync(tokenType, token).ConfigureAwait(false);
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
        protected virtual Task OnLoginAsync(TokenType tokenType, string token) { return Task.Delay(0); }

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
            CurrentUser = null;
            LoginState = LoginState.LoggedOut;

            await _loggedOutEvent.InvokeAsync().ConfigureAwait(false);
        }
        protected virtual Task OnLogoutAsync() { return Task.Delay(0); }

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

        //IDiscordClient
        ConnectionState IDiscordClient.ConnectionState => ConnectionState.Disconnected;
        ISelfUser IDiscordClient.CurrentUser => CurrentUser;

        Task<IApplication> IDiscordClient.GetApplicationInfoAsync() { throw new NotSupportedException(); }

        Task<IChannel> IDiscordClient.GetChannelAsync(ulong id, CacheMode mode)
            => Task.FromResult<IChannel>(null);
        Task<IReadOnlyCollection<IPrivateChannel>> IDiscordClient.GetPrivateChannelsAsync(CacheMode mode)
            => Task.FromResult<IReadOnlyCollection<IPrivateChannel>>(ImmutableArray.Create<IPrivateChannel>());

        Task<IReadOnlyCollection<IConnection>> IDiscordClient.GetConnectionsAsync()
            => Task.FromResult<IReadOnlyCollection<IConnection>>(ImmutableArray.Create<IConnection>());

        Task<IInvite> IDiscordClient.GetInviteAsync(string inviteId)
            => Task.FromResult<IInvite>(null);

        Task<IGuild> IDiscordClient.GetGuildAsync(ulong id, CacheMode mode)
            => Task.FromResult<IGuild>(null);
        Task<IReadOnlyCollection<IGuild>> IDiscordClient.GetGuildsAsync(CacheMode mode)
            => Task.FromResult<IReadOnlyCollection<IGuild>>(ImmutableArray.Create<IGuild>());
        Task<IGuild> IDiscordClient.CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon) { throw new NotSupportedException(); }

        Task<IUser> IDiscordClient.GetUserAsync(ulong id, CacheMode mode)
            => Task.FromResult<IUser>(null);
        Task<IUser> IDiscordClient.GetUserAsync(string username, string discriminator)
            => Task.FromResult<IUser>(null);

        Task<IReadOnlyCollection<IVoiceRegion>> IDiscordClient.GetVoiceRegionsAsync()
            => Task.FromResult<IReadOnlyCollection<IVoiceRegion>>(ImmutableArray.Create<IVoiceRegion>());
        Task<IVoiceRegion> IDiscordClient.GetVoiceRegionAsync(string id)
            => Task.FromResult<IVoiceRegion>(null);

        Task IDiscordClient.ConnectAsync() { throw new NotSupportedException(); }
        Task IDiscordClient.DisconnectAsync() { throw new NotSupportedException(); }

    }
}
