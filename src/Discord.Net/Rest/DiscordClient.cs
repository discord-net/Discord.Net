using Discord.API.Rest;
using Discord.Logging;
using Discord.Net.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Rest
{
    //TODO: Docstrings
    //TODO: Log Internal/External REST Rate Limits, 502s
    //TODO: Log Logins/Logouts
    public sealed class DiscordClient : IDiscordClient, IDisposable
    {
        public event EventHandler<LogMessageEventArgs> Log;
        public event EventHandler LoggedIn, LoggedOut;

        private readonly Logger _discordLogger, _restLogger;
        private readonly SemaphoreSlim _connectionLock;
        private readonly RestClientProvider _restClientProvider;
        private readonly LogManager _log;
        private CancellationTokenSource _cancelTokenSource;
        private bool _isDisposed;
        private SelfUser _currentUser;

        public bool IsLoggedIn { get; private set; }
        public API.DiscordApiClient ApiClient { get; private set; }

        public TokenType AuthTokenType => ApiClient.AuthTokenType;
        public IRestClient RestClient => ApiClient.RestClient;
        public IRequestQueue RequestQueue => ApiClient.RequestQueue;

        public DiscordClient(DiscordConfig config = null)
        {
            if (config == null)
                config = new DiscordConfig();

            _restClientProvider = config.RestClientProvider;

            _log = new LogManager(config.LogLevel);
            _log.Message += (s, e) => Log.Raise(this, e);
            _discordLogger = _log.CreateLogger("Discord");
            _restLogger = _log.CreateLogger("Rest");

            _connectionLock = new SemaphoreSlim(1, 1);
            ApiClient = new API.DiscordApiClient(_restClientProvider);
            ApiClient.SentRequest += (s, e) => _log.Verbose("Rest", $"{e.Method} {e.Endpoint}: {e.Milliseconds} ms");
        }

        public async Task Login(string email, string password)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LoginInternal(email, password).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
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
        private async Task LoginInternal(string email, string password)
        {
            if (IsLoggedIn)
                await LogoutInternal().ConfigureAwait(false);
            try
            {
                _cancelTokenSource = new CancellationTokenSource();

                var args = new LoginParams { Email = email, Password = password };
                await ApiClient.Login(args, _cancelTokenSource.Token).ConfigureAwait(false);
                await CompleteLogin(false).ConfigureAwait(false);
            }
            catch { await LogoutInternal().ConfigureAwait(false); throw; }
        }
        private async Task LoginInternal(TokenType tokenType, string token, bool validateToken)
        {
            if (IsLoggedIn)
                await LogoutInternal().ConfigureAwait(false);
            try
            {
                _cancelTokenSource = new CancellationTokenSource();

                await ApiClient.Login(tokenType, token, _cancelTokenSource.Token).ConfigureAwait(false);
                await CompleteLogin(validateToken).ConfigureAwait(false);
            }
            catch { await LogoutInternal().ConfigureAwait(false); throw; }
        }
        private async Task CompleteLogin(bool validateToken)
        {
            if (validateToken)
            {
                try
                {
                    await ApiClient.ValidateToken().ConfigureAwait(false);
                }
                catch { await ApiClient.Logout().ConfigureAwait(false); }
            }
            
            IsLoggedIn = true;
            LoggedIn.Raise(this);
        }

        public async Task Logout()
        {
            _cancelTokenSource?.Cancel();
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LogoutInternal().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task LogoutInternal()
        {
            bool wasLoggedIn = IsLoggedIn;

            if (_cancelTokenSource != null)
            {
                try { _cancelTokenSource.Cancel(false); }
                catch { }
            }

            await ApiClient.Logout().ConfigureAwait(false);
            _currentUser = null;

            if (wasLoggedIn)
            {
                IsLoggedIn = false;
                LoggedOut.Raise(this);
            }
        }

        public async Task<IEnumerable<Connection>> GetConnections()
        {
            var models = await ApiClient.GetCurrentUserConnections().ConfigureAwait(false);
            return models.Select(x => new Connection(x));
        }

        public async Task<IChannel> GetChannel(ulong id)
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
                    return new DMChannel(this, model);
            }
            return null;
        }
        public async Task<IEnumerable<DMChannel>> GetDMChannels()
        {
            var models = await ApiClient.GetCurrentUserDMs().ConfigureAwait(false);
            return models.Select(x => new DMChannel(this, x));
        }

        public async Task<Invite> GetInvite(string inviteIdOrXkcd)
        {
            var model = await ApiClient.GetInvite(inviteIdOrXkcd).ConfigureAwait(false);
            if (model != null)
                return new Invite(this, model);
            return null;
        }

        public async Task<Guild> GetGuild(ulong id)
        {
            var model = await ApiClient.GetGuild(id).ConfigureAwait(false);
            if (model != null)
                return new Guild(this, model);
            return null;
        }
        public async Task<GuildEmbed> GetGuildEmbed(ulong id)
        {
            var model = await ApiClient.GetGuildEmbed(id).ConfigureAwait(false);
            if (model != null)
                return new GuildEmbed(model);
            return null;
        }
        public async Task<IEnumerable<UserGuild>> GetGuilds()
        {
            var models = await ApiClient.GetCurrentUserGuilds().ConfigureAwait(false);
            return models.Select(x => new UserGuild(this, x));

        }
        public async Task<Guild> CreateGuild(string name, IVoiceRegion region, Stream jpegIcon = null)
        {
            var args = new CreateGuildParams();
            var model = await ApiClient.CreateGuild(args).ConfigureAwait(false);
            return new Guild(this, model);
        }

        public async Task<User> GetUser(ulong id)
        {
            var model = await ApiClient.GetUser(id).ConfigureAwait(false);
            if (model != null)
                return new PublicUser(this, model);
            return null;
        }
        public async Task<User> GetUser(string username, ushort discriminator)
        {
            var model = await ApiClient.GetUser(username, discriminator).ConfigureAwait(false);
            if (model != null)
                return new PublicUser(this, model);
            return null;
        }
        public async Task<SelfUser> GetCurrentUser()
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
        public async Task<IEnumerable<User>> QueryUsers(string query, int limit)
        {
            var models = await ApiClient.QueryUsers(query, limit).ConfigureAwait(false);
            return models.Select(x => new PublicUser(this, x));
        }

        public async Task<IEnumerable<VoiceRegion>> GetVoiceRegions()
        {
            var models = await ApiClient.GetVoiceRegions().ConfigureAwait(false);
            return models.Select(x => new VoiceRegion(x));
        }
        public async Task<VoiceRegion> GetVoiceRegion(string id)
        {
            var models = await ApiClient.GetVoiceRegions().ConfigureAwait(false);
            return models.Select(x => new VoiceRegion(x)).Where(x => x.Id == id).FirstOrDefault();
        }

        void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                    _cancelTokenSource.Dispose();
                _isDisposed = true;
            }
        }
        public void Dispose() => Dispose(true);

        API.DiscordApiClient IDiscordClient.ApiClient => ApiClient;

        async Task<IChannel> IDiscordClient.GetChannel(ulong id)
            => await GetChannel(id).ConfigureAwait(false);
        async Task<IEnumerable<IDMChannel>> IDiscordClient.GetDMChannels()
            => await GetDMChannels().ConfigureAwait(false);
        async Task<IEnumerable<IConnection>> IDiscordClient.GetConnections()
            => await GetConnections().ConfigureAwait(false);
        async Task<IInvite> IDiscordClient.GetInvite(string inviteIdOrXkcd)
            => await GetInvite(inviteIdOrXkcd).ConfigureAwait(false);
        async Task<IGuild> IDiscordClient.GetGuild(ulong id)
            => await GetGuild(id).ConfigureAwait(false);
        async Task<IEnumerable<IUserGuild>> IDiscordClient.GetGuilds()
            => await GetGuilds().ConfigureAwait(false);
        async Task<IGuild> IDiscordClient.CreateGuild(string name, IVoiceRegion region, Stream jpegIcon)
            => await CreateGuild(name, region, jpegIcon).ConfigureAwait(false);
        async Task<IUser> IDiscordClient.GetUser(ulong id)
            => await GetUser(id).ConfigureAwait(false);
        async Task<IUser> IDiscordClient.GetUser(string username, ushort discriminator)
            => await GetUser(username, discriminator).ConfigureAwait(false);
        async Task<ISelfUser> IDiscordClient.GetCurrentUser()
            => await GetCurrentUser().ConfigureAwait(false);
        async Task<IEnumerable<IUser>> IDiscordClient.QueryUsers(string query, int limit)
            => await QueryUsers(query, limit).ConfigureAwait(false);
        async Task<IEnumerable<IVoiceRegion>> IDiscordClient.GetVoiceRegions()
            => await GetVoiceRegions().ConfigureAwait(false);
        async Task<IVoiceRegion> IDiscordClient.GetVoiceRegion(string id)
            => await GetVoiceRegion(id).ConfigureAwait(false);
    }
}
