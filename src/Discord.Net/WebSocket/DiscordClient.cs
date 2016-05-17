using Discord.API.Rest;
using Discord.Logging;
using Discord.Net.Rest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    //TODO: Docstrings
    //TODO: Log Logins/Logouts
    public sealed class DiscordClient : IDiscordClient, IDisposable
    {
        public event EventHandler<LogMessageEventArgs> Log;
        public event EventHandler LoggedIn, LoggedOut;
        public event EventHandler Connected, Disconnected;
        public event EventHandler<VoiceChannelEventArgs> VoiceConnected, VoiceDisconnected;

        public event EventHandler<ChannelEventArgs> ChannelCreated, ChannelDestroyed;
        public event EventHandler<ChannelUpdatedEventArgs> ChannelUpdated;
        public event EventHandler<MessageEventArgs> MessageReceived, MessageDeleted;
        public event EventHandler<MessageUpdatedEventArgs> MessageUpdated;
        public event EventHandler<RoleEventArgs> RoleCreated, RoleDeleted;
        public event EventHandler<RoleUpdatedEventArgs> RoleUpdated;
        public event EventHandler<GuildEventArgs> JoinedGuild, LeftGuild;
        public event EventHandler<GuildEventArgs> GuildAvailable, GuildUnavailable;
        public event EventHandler<GuildUpdatedEventArgs> GuildUpdated;
        public event EventHandler<CurrentUserUpdatedEventArgs> CurrentUserUpdated;
        public event EventHandler<UserEventArgs> UserJoined, UserLeft;
        public event EventHandler<UserEventArgs> UserBanned, UserUnbanned;
        public event EventHandler<UserUpdatedEventArgs> UserUpdated;
        public event EventHandler<TypingEventArgs> UserIsTyping;

        private readonly Logger _discordLogger, _gatewayLogger;
        private readonly SemaphoreSlim _connectionLock;
        private readonly RestClientProvider _restClientProvider;
        private readonly LogManager _log;
        private readonly int _connectionTimeout, _reconnectDelay, _failedReconnectDelay;
        private readonly bool _enablePreUpdateEvents;
        private readonly int _largeThreshold;
        private readonly int _totalShards;
        private IReadOnlyDictionary<string, VoiceRegion> _voiceRegions;
        private CancellationTokenSource _cancelTokenSource;
        private bool _isDisposed;
        private SelfUser _currentUser;
        private ConcurrentDictionary<ulong, Guild> _guilds;
        private ConcurrentDictionary<ulong, IChannel> _channels;
        private ConcurrentDictionary<ulong, DMChannel> _dmChannels; //Key = RecipientId
        private ConcurrentDictionary<ulong, User> _users;

        public int ShardId { get; }
        public bool IsLoggedIn { get; private set; }
        public API.DiscordApiClient ApiClient { get; private set; }
        public SelfUser CurrentUser { get; private set; }
        //public GatewaySocket GatewaySocket { get; private set; }
        internal int MessageCacheSize { get; private set; }
        internal bool UsePermissionCache { get; private set; }

        public TokenType AuthTokenType => ApiClient.AuthTokenType;
        public IRestClient RestClient => ApiClient.RestClient;
        public IRequestQueue RequestQueue => ApiClient.RequestQueue;
        public IEnumerable<Guild> Guilds => _guilds.Values;
        public IEnumerable<IChannel> Channels => _channels.Values;
        public IEnumerable<DMChannel> DMChannels => _dmChannels.Values;
        public IEnumerable<VoiceRegion> VoiceRegions => _voiceRegions.Values;

        //public bool IsConnected => GatewaySocket.State == ConnectionState.Connected;

        public DiscordClient(DiscordSocketConfig config = null)
        {
            if (config == null)
                config = new DiscordSocketConfig();

            _restClientProvider = config.RestClientProvider;
            ShardId = config.ShardId;
            _totalShards = config.TotalShards;

            _connectionTimeout = config.ConnectionTimeout;
            _reconnectDelay = config.ReconnectDelay;
            _failedReconnectDelay = config.FailedReconnectDelay;

            MessageCacheSize = config.MessageCacheSize;
            UsePermissionCache = config.UsePermissionsCache;
            _enablePreUpdateEvents = config.EnablePreUpdateEvents;
            _largeThreshold = config.LargeThreshold;

            _log = new LogManager(config.LogLevel);
            _log.Message += (s, e) => Log.Raise(this, e);
            _discordLogger = _log.CreateLogger("Discord");
            _gatewayLogger = _log.CreateLogger("Gateway");

            _connectionLock = new SemaphoreSlim(1, 1);
            ApiClient = new API.DiscordApiClient(_restClientProvider);
            ApiClient.SentRequest += (s, e) => _log.Verbose("Rest", $"{e.Method} {e.Endpoint}: {e.Milliseconds} ms");

            _channels = new ConcurrentDictionary<ulong, IChannel>(1, 100);
            _dmChannels = new ConcurrentDictionary<ulong, DMChannel>(1, 100);
            _guilds = new ConcurrentDictionary<ulong, Guild>(1, 25);
            _users = new ConcurrentDictionary<ulong, User>(1, 250);
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
                    var voiceRegions = await ApiClient.GetVoiceRegions().ConfigureAwait(false);
                    _voiceRegions = voiceRegions.Select(x => new VoiceRegion(x)).ToImmutableDictionary(x => x.Id);

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
            _channels.Clear();
            _dmChannels.Clear();
            _guilds.Clear();
            _users.Clear();
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

        public IChannel GetChannel(ulong id)
        {
            IChannel channel;
            if (_channels.TryGetValue(id, out channel))
                return channel;
            return null;
        }

        public async Task<Invite> GetInvite(string inviteIdOrXkcd)
        {
            var model = await ApiClient.GetInvite(inviteIdOrXkcd).ConfigureAwait(false);
            if (model != null)
                return new Invite(this, model);
            return null;
        }

        public Guild GetGuild(ulong id)
        {
            Guild guild;
            if (_guilds.TryGetValue(id, out guild))
                return guild;
            return null;
        }
        public async Task<Guild> CreateGuild(string name, IVoiceRegion region, Stream jpegIcon = null)
        {
            var args = new CreateGuildParams();
            var model = await ApiClient.CreateGuild(args).ConfigureAwait(false);
            return new Guild(this, model);
        }

        public User GetUser(ulong id)
        {
            User user;
            if (_users.TryGetValue(id, out user))
                return user;
            return null;
        }
        public User GetUser(string username, ushort discriminator)
        {
            return _users.Where(x => x.Value.Discriminator == discriminator && x.Value.Username == username).Select(x => x.Value).FirstOrDefault();
        }
        public async Task<IEnumerable<User>> QueryUsers(string query, int limit)
        {
            var models = await ApiClient.QueryUsers(query, limit).ConfigureAwait(false);
            return models.Select(x => new User(this, x));
        }

        public VoiceRegion GetVoiceRegion(string id)
        {
            VoiceRegion region;
            if (_voiceRegions.TryGetValue(id, out region))
                return region;
            return null;
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

        Task<IChannel> IDiscordClient.GetChannel(ulong id)
            => Task.FromResult(GetChannel(id));
        Task<IEnumerable<IDMChannel>> IDiscordClient.GetDMChannels()
            => Task.FromResult<IEnumerable<IDMChannel>>(DMChannels);
        async Task<IEnumerable<IConnection>> IDiscordClient.GetConnections()
            => await GetConnections().ConfigureAwait(false);
        async Task<IInvite> IDiscordClient.GetInvite(string inviteIdOrXkcd)
            => await GetInvite(inviteIdOrXkcd).ConfigureAwait(false);
        Task<IGuild> IDiscordClient.GetGuild(ulong id)
            => Task.FromResult<IGuild>(GetGuild(id));
        Task<IEnumerable<IUserGuild>> IDiscordClient.GetGuilds()
            => Task.FromResult<IEnumerable<IUserGuild>>(Guilds);
        async Task<IGuild> IDiscordClient.CreateGuild(string name, IVoiceRegion region, Stream jpegIcon)
            => await CreateGuild(name, region, jpegIcon).ConfigureAwait(false);
        Task<IUser> IDiscordClient.GetUser(ulong id)
            => Task.FromResult<IUser>(GetUser(id));
        Task<IUser> IDiscordClient.GetUser(string username, ushort discriminator)
            => Task.FromResult<IUser>(GetUser(username, discriminator));
        Task<ISelfUser> IDiscordClient.GetCurrentUser()
            => Task.FromResult<ISelfUser>(CurrentUser);
        async Task<IEnumerable<IUser>> IDiscordClient.QueryUsers(string query, int limit)
            => await QueryUsers(query, limit).ConfigureAwait(false);
        Task<IEnumerable<IVoiceRegion>> IDiscordClient.GetVoiceRegions()
            => Task.FromResult<IEnumerable<IVoiceRegion>>(VoiceRegions);
        Task<IVoiceRegion> IDiscordClient.GetVoiceRegion(string id)
            => Task.FromResult<IVoiceRegion>(GetVoiceRegion(id));
    }
}
