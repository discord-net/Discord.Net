using Discord.API.Rest;
using Discord.Logging;
using Discord.Net;
using Discord.Net.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    public class DiscordClient : IDisposable
    {
        public event EventHandler<LogMessageEventArgs> Log;
        public event EventHandler LoggedIn, LoggedOut;

        protected readonly RestClientProvider _restClientProvider;
        protected readonly string _token;
        protected readonly LogManager _logManager;
        protected readonly SemaphoreSlim _connectionLock;
        protected readonly Logger _restLogger;
        protected CancellationTokenSource _cancelToken;
        protected bool _isDisposed;

        public string UserAgent { get; }
        public IReadOnlyList<VoiceRegion> VoiceRegions { get; private set; }
        /// <summary> Gets the internal RestClient. </summary>
        public RestClient RestClient { get; protected set; }
        /// <summary> Gets the queue used for outgoing messages, if enabled. </summary>
        public MessageQueue MessageQueue { get; protected set; }
        public SelfUser CurrentUser { get; protected set; }
        public bool IsLoggedIn { get; private set; }

        internal CancellationToken CancelToken => _cancelToken.Token;

        public DiscordClient(string token, DiscordConfig config = null)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));

            if (config == null)
                config = new DiscordConfig();

            _token = token;
            _connectionLock = new SemaphoreSlim(1, 1);
            
            _restClientProvider = config.RestClientProvider;
            UserAgent = $"DiscordBot ({DiscordConfig.LibUrl}, v{DiscordConfig.LibVersion})";

            _logManager = new LogManager(config.LogLevel);
            _logManager.Message += (s, e) => Log(this, e);
            _restLogger = _logManager.CreateLogger("Rest");
        }

        public async Task Login()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LoginInternal().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        protected virtual async Task LoginInternal()
        {
            if (IsLoggedIn)
                await LogoutInternal().ConfigureAwait(false);

            try
            {
                _cancelToken = new CancellationTokenSource();

                RestClient = new RestClient(_restClientProvider(DiscordConfig.ClientAPIUrl, _cancelToken.Token));
                RestClient.SetHeader("accept", "*/*");
                RestClient.SetHeader("authorization", _token);
                RestClient.SetHeader("user-agent", UserAgent);
                RestClient.SentRequest += (s, e) => _restLogger.Verbose($"{e.Request.Method} {e.Request.Endpoint}: {e.Milliseconds} ms");

                MessageQueue = new MessageQueue(RestClient, _restLogger);
                await MessageQueue.Start(_cancelToken.Token).ConfigureAwait(false);

                var selfResponse = await RestClient.Send(new GetCurrentUserRequest()).ConfigureAwait(false);
                var regionsResponse = await RestClient.Send(new GetVoiceRegionsRequest()).ConfigureAwait(false);

                CurrentUser = CreateSelfUser(selfResponse);
                VoiceRegions = regionsResponse.Select(x => CreateVoiceRegion(x)).ToImmutableArray();

                IsLoggedIn = true;
                RaiseEvent(LoggedIn);
            }
            catch (Exception) { await LogoutInternal().ConfigureAwait(false); throw; }
        }

        public async Task Logout()
        {
            _cancelToken?.Cancel();
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LogoutInternal().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        protected virtual async Task LogoutInternal()
        {
            bool wasLoggedIn = IsLoggedIn;

            try { _cancelToken.Cancel(); } catch { }
            try { await MessageQueue.Stop().ConfigureAwait(false); } catch { }

            RestClient = null;
            MessageQueue = null;

            if (wasLoggedIn)
            {
                IsLoggedIn = false;
                RaiseEvent(LoggedOut);
            }
        }

        public virtual async Task<IEnumerable<DMChannel>> GetDMChannels()
        {
            var response = await RestClient.Send(new GetCurrentUserDMsRequest()).ConfigureAwait(false);
            var result = ImmutableArray.CreateBuilder<DMChannel>(response.Length);
            for (int i = 0; i < response.Length; i++)
                result[i] = CreateDMChannel(response[i]);
            return result.ToImmutable();
        }
        public virtual async Task<PublicInvite> GetInvite(string inviteIdOrXkcd)
        {
            if (inviteIdOrXkcd == null) throw new ArgumentNullException(nameof(inviteIdOrXkcd));

            //Remove trailing slash
            if (inviteIdOrXkcd.Length > 0 && inviteIdOrXkcd[inviteIdOrXkcd.Length - 1] == '/')
                inviteIdOrXkcd = inviteIdOrXkcd.Substring(0, inviteIdOrXkcd.Length - 1);
            //Remove leading URL
            int index = inviteIdOrXkcd.LastIndexOf('/');
            if (index >= 0)
                inviteIdOrXkcd = inviteIdOrXkcd.Substring(index + 1);

            try
            {
                var response = await RestClient.Send(new GetInviteRequest(inviteIdOrXkcd)).ConfigureAwait(false);
                return CreatePublicInvite(response);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }
        public virtual async Task<Guild> GetGuild(ulong id)
        {
            var response = await RestClient.Send(new GetGuildRequest(id)).ConfigureAwait(false);
            return CreateGuild(response);
        }
        public virtual async Task<IEnumerable<Guild>> GetGuilds()
        {
            var response = await RestClient.Send(new GetCurrentUserGuildsRequest()).ConfigureAwait(false);
            var result = ImmutableArray.CreateBuilder<Guild>(response.Length);
            for (int i = 0; i < response.Length; i++)
                result[i] = CreateGuild(response[i]);
            return result.ToImmutable();
        }
        public virtual async Task<IUser> GetUser(ulong id)
        {
            var response = await RestClient.Send(new GetUserRequest(id));
            var user = CreateGlobalUser(response);
            return user;
        }
        public virtual async Task<IUser> GetUser(string username, ushort discriminator)
        {
            var response = await RestClient.Send(new QueryUserRequest() { Query = $"{username}#{discriminator}", Limit = 1 });
            if (response.Length > 0)
            {
                var user = CreateGlobalUser(response[0]);
                return user;
            }
            return null;
        }
        public virtual VoiceRegion GetOptimalVoiceRegion()
        {
            var regions = VoiceRegions;
            for (int i = 0; i < regions.Count; i++)
            {
                if (regions[i].IsOptimal)
                    return regions[i];
            }
            return null;
        }
        public virtual VoiceRegion GetVoiceRegion(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var regions = VoiceRegions;
            for (int i = 0; i < regions.Count; i++)
            {
                if (regions[i].Id == id)
                    return regions[i];
            }
            return null;
        }

        public virtual async Task<DMChannel> GetOrCreateDMChannel(ulong userId)
        {
            var response = await RestClient.Send(new CreateDMChannelRequest
            {
                RecipientId = userId
            }).ConfigureAwait(false);

            return CreateDMChannel(response);
        }
        /// <summary> Creates a new guild with the provided name and region. This function requires your bot to be whitelisted by Discord. </summary>
        public virtual async Task<Guild> CreateGuild(string name, VoiceRegion region, Stream jpegIcon = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (region == null) throw new ArgumentNullException(nameof(region));

            var response = await RestClient.Send(new CreateGuildRequest
            {
                Name = name,
                Region = region.Id,
                Icon = jpegIcon
            }).ConfigureAwait(false);

            return CreateGuild(response);
        }

        internal virtual DMChannel CreateDMChannel(API.Channel model)
        {
            var channel = new DMChannel(model.Id, this, 0);
            channel.Update(model);
            return channel;
        }
        internal virtual TextChannel CreateTextChannel(Guild guild, API.Channel model)
        {
            var channel = new TextChannel(model.Id, guild, 0, false);
            channel.Update(model);
            return channel;
        }
        internal virtual VoiceChannel CreateVoiceChannel(Guild guild, API.Channel model)
        {
            var channel = new VoiceChannel(model.Id, guild, false);
            channel.Update(model);
            return channel;
        }
        internal virtual GuildInvite CreateGuildInvite(GuildChannel channel, API.InviteMetadata model)
        {
            var invite = new GuildInvite(model.Code, channel);
            invite.Update(model);
            return invite;
        }
        internal virtual PublicInvite CreatePublicInvite(API.Invite model)
        {
            var invite = new PublicInvite(model.Code, this);
            invite.Update(model);
            return invite;
        }
        internal virtual Guild CreateGuild(API.Guild model)
        {
            var guild = new Guild(model.Id, this);
            guild.Update(model);
            return guild;
        }
        internal virtual Message CreateMessage(IMessageChannel channel, IUser user, API.Message model)
        {
            var msg = new Message(model.Id, channel, user);
            msg.Update(model);
            return msg;
        }
        internal virtual Role CreateRole(Guild guild, API.Role model)
        {
            var role = new Role(model.Id, guild);
            role.Update(model);
            return role;
        }
        internal virtual DMUser CreateDMUser(DMChannel channel, API.User model)
        {
            var user = new DMUser(CreateGlobalUser(model), channel);
            user.Update(model);
            return user;
        }
        internal virtual GuildUser CreateGuildUser(Guild guild, API.GuildMember model)
        {
            var user = new GuildUser(CreateGlobalUser(model.User), guild);
            user.Update(model);
            return user;
        }
        internal virtual GuildUser CreateBannedUser(Guild guild, API.User model)
        {
            var user = new GuildUser(CreateGlobalUser(model), guild);
            //user.Update(model);
            return user;
        }
        internal virtual SelfUser CreateSelfUser(API.User model)
        {
            var user = new SelfUser(model.Id, this);
            user.Update(model);
            return user;
        }
        internal virtual GlobalUser CreateGlobalUser(API.User model)
        {
            var user = new GlobalUser(model.Id, this);
            user.Update(model);
            return user;
        }
        internal virtual VoiceRegion CreateVoiceRegion(API.Rest.GetVoiceRegionsResponse model)
        {
            var region = new VoiceRegion(model.Id, this);
            region.Update(model);
            return region;
        }

        internal virtual void RemoveUser(GlobalUser user) { }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    MessageQueue.Dispose();
                    RestClient.Dispose();
                    _connectionLock.Dispose();
                }
                _isDisposed = true;
            }
        }
        public void Dispose() => Dispose(true);

        protected void RaiseEvent(EventHandler eventHandler)
            => eventHandler?.Invoke(this, EventArgs.Empty);
        protected void RaiseEvent<T>(EventHandler<T> eventHandler, T eventArgs) where T : EventArgs
            => eventHandler?.Invoke(this, eventArgs);
        protected void RaiseEvent<T>(EventHandler<T> eventHandler, Func<T> eventArgs) where T : EventArgs
            => eventHandler?.Invoke(this, eventArgs());
    }
}
