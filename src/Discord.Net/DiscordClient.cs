using Discord.API.Rest;
using Discord.Extensions;
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

namespace Discord
{
    public class DiscordClient : IDiscordClient
    {
        public event Func<LogMessage, Task> Log;
        public event Func<Task> LoggedIn, LoggedOut;

        internal readonly Logger _discordLogger, _restLogger;
        internal readonly SemaphoreSlim _connectionLock;
        internal readonly LogManager _log;
        internal readonly RequestQueue _requestQueue;
        internal bool _isDisposed;
        internal SelfUser _currentUser;

        public LoginState LoginState { get; private set; }
        public API.DiscordApiClient ApiClient { get; private set; }

        public DiscordClient()
            : this(new DiscordConfig()) { }
        public DiscordClient(DiscordConfig config)
        {
            _log = new LogManager(config.LogLevel);
            _log.Message += async msg => await Log.Raise(msg).ConfigureAwait(false);
            _discordLogger = _log.CreateLogger("Discord");
            _restLogger = _log.CreateLogger("Rest");

            _connectionLock = new SemaphoreSlim(1, 1);
            _requestQueue = new RequestQueue();

            ApiClient = new API.DiscordApiClient(config.RestClientProvider, (config as DiscordSocketConfig)?.WebSocketProvider, requestQueue: _requestQueue);
            ApiClient.SentRequest += async (method, endpoint, millis) => await _log.Verbose("Rest", $"{method} {endpoint}: {millis} ms").ConfigureAwait(false);
        }

        public async Task Login(TokenType tokenType, string token, bool validateToken = true)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LoginInternal(tokenType, token, validateToken).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task LoginInternal(TokenType tokenType, string token, bool validateToken)
        {
            if (LoginState != LoginState.LoggedOut)
                await LogoutInternal().ConfigureAwait(false);
            LoginState = LoginState.LoggingIn;

            try
            {
                await ApiClient.Login(tokenType, token).ConfigureAwait(false);

                if (validateToken)
                {
                    try
                    {
                        await ApiClient.ValidateToken().ConfigureAwait(false);
                    }
                    catch (HttpException ex)
                    {
                        throw new ArgumentException("Token validation failed", nameof(token), ex);
                    }
                }

                await OnLogin().ConfigureAwait(false);

                LoginState = LoginState.LoggedIn;
            }
            catch (Exception)
            {
                await LogoutInternal().ConfigureAwait(false);
                throw;
            }

            await LoggedIn.Raise().ConfigureAwait(false);
        }
        protected virtual Task OnLogin() => Task.CompletedTask;

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
            if (LoginState == LoginState.LoggedOut) return;
            LoginState = LoginState.LoggingOut;

            await ApiClient.Logout().ConfigureAwait(false);
            
            await OnLogout().ConfigureAwait(false);

            _currentUser = null;

            LoginState = LoginState.LoggedOut;

            await LoggedOut.Raise().ConfigureAwait(false);
        }
        protected virtual Task OnLogout() => Task.CompletedTask;

        public async Task<IReadOnlyCollection<IConnection>> GetConnections()
        {
            var models = await ApiClient.GetCurrentUserConnections().ConfigureAwait(false);
            return models.Select(x => new Connection(x)).ToImmutableArray();
        }

        public virtual async Task<IChannel> GetChannel(ulong id)
        {
            var model = await ApiClient.GetChannel(id).ConfigureAwait(false);
            if (model != null)
            {
                if (model.GuildId != null)
                {
                    var guildModel = await ApiClient.GetGuild(model.GuildId.Value).ConfigureAwait(false);
                    if (guildModel != null)
                    {
                        var guild = new Guild(this, guildModel);
                        return guild.ToChannel(model);
                    }
                }
                else
                    return new DMChannel(this, new User(this, model.Recipient), model);
            }
            return null;
        }
        public virtual async Task<IReadOnlyCollection<IDMChannel>> GetDMChannels()
        {
            var models = await ApiClient.GetCurrentUserDMs().ConfigureAwait(false);
            return models.Select(x => new DMChannel(this, new User(this, x.Recipient), x)).ToImmutableArray();
        }

        public virtual async Task<IInvite> GetInvite(string inviteIdOrXkcd)
        {
            var model = await ApiClient.GetInvite(inviteIdOrXkcd).ConfigureAwait(false);
            if (model != null)
                return new Invite(this, model);
            return null;
        }

        public virtual async Task<IGuild> GetGuild(ulong id)
        {
            var model = await ApiClient.GetGuild(id).ConfigureAwait(false);
            if (model != null)
                return new Guild(this, model);
            return null;
        }
        public virtual async Task<GuildEmbed?> GetGuildEmbed(ulong id)
        {
            var model = await ApiClient.GetGuildEmbed(id).ConfigureAwait(false);
            if (model != null)
                return new GuildEmbed(model);
            return null;
        }
        public virtual async Task<IReadOnlyCollection<IUserGuild>> GetGuilds()
        {
            var models = await ApiClient.GetCurrentUserGuilds().ConfigureAwait(false);
            return models.Select(x => new UserGuild(this, x)).ToImmutableArray();

        }
        public virtual async Task<IGuild> CreateGuild(string name, IVoiceRegion region, Stream jpegIcon = null)
        {
            var args = new CreateGuildParams();
            var model = await ApiClient.CreateGuild(args).ConfigureAwait(false);
            return new Guild(this, model);
        }

        public virtual async Task<IUser> GetUser(ulong id)
        {
            var model = await ApiClient.GetUser(id).ConfigureAwait(false);
            if (model != null)
                return new User(this, model);
            return null;
        }
        public virtual async Task<IUser> GetUser(string username, string discriminator)
        {
            var model = await ApiClient.GetUser(username, discriminator).ConfigureAwait(false);
            if (model != null)
                return new User(this, model);
            return null;
        }
        public virtual async Task<ISelfUser> GetCurrentUser()
        {
            var user = _currentUser;
            if (user == null)
            {
                var model = await ApiClient.GetCurrentUser().ConfigureAwait(false);
                user = new SelfUser(this, model);
                _currentUser = user;
            }
            return user;
        }
        public virtual async Task<IReadOnlyCollection<IUser>> QueryUsers(string query, int limit)
        {
            var models = await ApiClient.QueryUsers(query, limit).ConfigureAwait(false);
            return models.Select(x => new User(this, x)).ToImmutableArray();
        }

        public virtual async Task<IReadOnlyCollection<IVoiceRegion>> GetVoiceRegions()
        {
            var models = await ApiClient.GetVoiceRegions().ConfigureAwait(false);
            return models.Select(x => new VoiceRegion(x)).ToImmutableArray();
        }
        public virtual async Task<IVoiceRegion> GetVoiceRegion(string id)
        {
            var models = await ApiClient.GetVoiceRegions().ConfigureAwait(false);
            return models.Select(x => new VoiceRegion(x)).Where(x => x.Id == id).FirstOrDefault();
        }

        internal void Dispose(bool disposing)
        {
            if (!_isDisposed)
                _isDisposed = true;
        }
        public void Dispose() => Dispose(true);
        
        ConnectionState IDiscordClient.ConnectionState => ConnectionState.Disconnected;
        Task IDiscordClient.Connect() { throw new NotSupportedException(); }
        Task IDiscordClient.Disconnect() { throw new NotSupportedException(); }
    }
}
