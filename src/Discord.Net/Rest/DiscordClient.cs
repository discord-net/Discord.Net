using Discord.API.Rest;
using Discord.Logging;
using Discord.Net.Rest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Rest
{
    public sealed class DiscordClient : IDiscordClient, IDisposable
    {
        public event EventHandler<LogMessageEventArgs> Log;
        public event EventHandler LoggedIn, LoggedOut;

        private readonly SemaphoreSlim _connectionLock;
        private readonly RestClientProvider _restClientProvider;
        private readonly LogManager _log;
        private CancellationTokenSource _cancelTokenSource;
        private bool _isDisposed;
        private string _userAgent;
        
        public bool IsLoggedIn { get; private set; }
        internal API.DiscordRawClient BaseClient { get; private set; }
        internal SelfUser CurrentUser { get; private set; }

        public DiscordClient(DiscordConfig config = null)
        {
            if (config == null)
                config = new DiscordConfig();

            _restClientProvider = config.RestClientProvider;

            _connectionLock = new SemaphoreSlim(1, 1);
            _log = new LogManager(config.LogLevel);
            _userAgent = DiscordConfig.UserAgent;

            _log.Message += (s,e) => Log.Raise(this, e);
        }

        public async Task Login(TokenType tokenType, string token)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LoginInternal(tokenType, token).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task LoginInternal(TokenType tokenType, string token)
        {
            if (IsLoggedIn)
                LogoutInternal();

            try
            {
                var cancelTokenSource = new CancellationTokenSource();
                
                BaseClient = new API.DiscordRawClient(_restClientProvider, cancelTokenSource.Token, tokenType, token);
                BaseClient.SentRequest += (s, e) => _log.Verbose("Rest", $"{e.Method} {e.Endpoint}: {e.Milliseconds} ms");

                //MessageQueue = new MessageQueue(RestClient, _restLogger);
                //await MessageQueue.Start(_cancelTokenSource.Token).ConfigureAwait(false);
                
                var currentUser = await BaseClient.GetCurrentUser().ConfigureAwait(false);
                CurrentUser = new SelfUser(this, currentUser);
                
                _cancelTokenSource = cancelTokenSource;
                IsLoggedIn = true;
                LoggedIn.Raise(this);
            }
            catch { LogoutInternal(); throw; }
        }

        public async Task Logout()
        {
            _cancelTokenSource?.Cancel();
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                LogoutInternal();
            }
            finally { _connectionLock.Release(); }
        }
        private void LogoutInternal()
        {
            bool wasLoggedIn = IsLoggedIn;

            try { _cancelTokenSource.Cancel(false); } catch { }

            BaseClient = null;

            if (wasLoggedIn)
            {
                IsLoggedIn = false;
                LoggedOut.Raise(this);
            }
        }

        public async Task<IEnumerable<Connection>> GetConnections()
        {
            var models = await BaseClient.GetCurrentUserConnections().ConfigureAwait(false);
            return models.Select(x => new Connection(x));
        }

        public async Task<IChannel> GetChannel(ulong id)
        {
            var model = await BaseClient.GetChannel(id).ConfigureAwait(false);
            if (model != null)
            {
                if (model.GuildId != null)
                {
                    var guildModel = await BaseClient.GetGuild(model.GuildId.Value).ConfigureAwait(false);
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
            var models = await BaseClient.GetCurrentUserDMs().ConfigureAwait(false);
            return models.Select(x => new DMChannel(this, x));
        }

        public async Task<PublicInvite> GetInvite(string inviteIdOrXkcd)
        {
            var model = await BaseClient.GetInvite(inviteIdOrXkcd).ConfigureAwait(false);
            if (model != null)
                return new PublicInvite(this, model);
            return null;
        }

        public async Task<Guild> GetGuild(ulong id)
        {
            var model = await BaseClient.GetGuild(id).ConfigureAwait(false);
            if (model != null)
                return new Guild(this, model);
            return null;
        }
        public async Task<GuildEmbed> GetGuildEmbed(ulong id)
        {
            var model = await BaseClient.GetGuildEmbed(id).ConfigureAwait(false);
            if (model != null)
                return new GuildEmbed(this, model);
            return null;
        }
        public async Task<IEnumerable<UserGuild>> GetGuilds()
        {
            var models = await BaseClient.GetCurrentUserGuilds().ConfigureAwait(false);
            return models.Select(x => new UserGuild(this, x));

        }
        public async Task<Guild> CreateGuild(string name, IVoiceRegion region, Stream jpegIcon = null)
        {
            var args = new CreateGuildParams();
            var model = await BaseClient.CreateGuild(args).ConfigureAwait(false);
            return new Guild(this, model);
        }

        public async Task<User> GetUser(ulong id)
        {
            var model = await BaseClient.GetUser(id).ConfigureAwait(false);
            if (model != null)
                return new PublicUser(this, model);
            return null;
        }
        public async Task<IUser> GetUser(string username, ushort discriminator)
        {
            var model = await BaseClient.GetUser(username, discriminator).ConfigureAwait(false);
            if (model != null)
                return new PublicUser(this, model);
            return null;
        }
        public async Task<ISelfUser> GetCurrentUser()
        {
            var currentUser = CurrentUser;
            if (currentUser == null)
            {
                var model = await BaseClient.GetCurrentUser().ConfigureAwait(false);
                currentUser = new SelfUser(this, model);
                CurrentUser = currentUser;
            }
            return currentUser;
        }
        public async Task<IEnumerable<IUser>> QueryUsers(string query, int limit)
        {
            var models = await BaseClient.QueryUsers(query, limit).ConfigureAwait(false);
            return models.Select(x => new PublicUser(this, x));
        }

        public async Task<IEnumerable<VoiceRegion>> GetVoiceRegions()
        {
            var models = await BaseClient.GetVoiceRegions().ConfigureAwait(false);
            return models.Select(x => new VoiceRegion(x));
        }
        public async Task<VoiceRegion> GetVoiceRegion(string id)
        {
            var models = await BaseClient.GetVoiceRegions().ConfigureAwait(false);
            return models.Select(x => new VoiceRegion(x)).Where(x => x.Id == id).FirstOrDefault();
        }
        public async Task<VoiceRegion> GetOptimalVoiceRegion()
        {
            var models = await BaseClient.GetVoiceRegions().ConfigureAwait(false);
            return models.Select(x => new VoiceRegion(x)).Where(x => x.IsOptimal).FirstOrDefault();
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

        API.DiscordRawClient IDiscordClient.BaseClient => BaseClient;
        ISelfUser IDiscordClient.CurrentUser => CurrentUser;

        async Task<IChannel> IDiscordClient.GetChannel(ulong id)
            => await GetChannel(id).ConfigureAwait(false);
        async Task<IEnumerable<IDMChannel>> IDiscordClient.GetDMChannels()
            => await GetDMChannels().ConfigureAwait(false);
        async Task<IEnumerable<IConnection>> IDiscordClient.GetConnections()
            => await GetConnections().ConfigureAwait(false);
        async Task<IPublicInvite> IDiscordClient.GetInvite(string inviteIdOrXkcd)
            => await GetInvite(inviteIdOrXkcd).ConfigureAwait(false);
        async Task<IGuild> IDiscordClient.GetGuild(ulong id)
            => await GetGuild(id).ConfigureAwait(false);
        async Task<IEnumerable<IUserGuild>> IDiscordClient.GetGuilds()
            => await GetGuilds().ConfigureAwait(false);
        async Task<IGuild> IDiscordClient.CreateGuild(string name, IVoiceRegion region, Stream jpegIcon)
            => await CreateGuild(name, region, jpegIcon).ConfigureAwait(false);
        async Task<IUser> IDiscordClient.GetUser(ulong id)
            => await GetUser(id).ConfigureAwait(false);
        async Task<IEnumerable<IUser>> IDiscordClient.QueryUsers(string query, int limit)
            => await QueryUsers(query, limit).ConfigureAwait(false);
        async Task<IEnumerable<IVoiceRegion>> IDiscordClient.GetVoiceRegions()
            => await GetVoiceRegions().ConfigureAwait(false);
        async Task<IVoiceRegion> IDiscordClient.GetVoiceRegion(string id)
            => await GetVoiceRegion(id).ConfigureAwait(false);
        async Task<IVoiceRegion> IDiscordClient.GetOptimalVoiceRegion()
            => await GetOptimalVoiceRegion().ConfigureAwait(false);
    }
}
