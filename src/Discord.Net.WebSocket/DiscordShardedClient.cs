using Discord.API;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public partial class DiscordShardedClient : BaseDiscordClient, IDiscordClient
    {
        private readonly DiscordSocketConfig _baseConfig;
        private int[] _shardIds;
        private Dictionary<int, int> _shardIdsToIndex;
        private DiscordSocketClient[] _shards;
        private int _totalShards;
        private bool _automaticShards;
        
        /// <summary> Gets the estimated round-trip latency, in milliseconds, to the gateway server. </summary>
        public int Latency { get; private set; }
        internal UserStatus Status => _shards[0].Status;
        internal Game? Game => _shards[0].Game;

        internal new DiscordSocketApiClient ApiClient => base.ApiClient as DiscordSocketApiClient;
        public new SocketSelfUser CurrentUser { get { return base.CurrentUser as SocketSelfUser; } private set { base.CurrentUser = value; } }
        public IReadOnlyCollection<SocketGuild> Guilds => GetGuilds().ToReadOnlyCollection(() => GetGuildCount());
        public IReadOnlyCollection<ISocketPrivateChannel> PrivateChannels => GetPrivateChannels().ToReadOnlyCollection(() => GetPrivateChannelCount());
        public IReadOnlyCollection<DiscordSocketClient> Shards => _shards;
        public IReadOnlyCollection<RestVoiceRegion> VoiceRegions => _shards[0].VoiceRegions;

        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordShardedClient() : this(null, new DiscordSocketConfig()) { }
        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordShardedClient(DiscordSocketConfig config) : this(null, config, CreateApiClient(config)) { }
        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordShardedClient(int[] ids) : this(ids, new DiscordSocketConfig()) { }
        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordShardedClient(int[] ids, DiscordSocketConfig config) : this(ids, config, CreateApiClient(config)) { }
        private DiscordShardedClient(int[] ids, DiscordSocketConfig config, API.DiscordSocketApiClient client)
            : base(config, client)
        {
            if (config.ShardId != null)
                throw new ArgumentException($"{nameof(config.ShardId)} must not be set.");
            if (ids != null && config.TotalShards == null)
                throw new ArgumentException($"Custom ids are not supported when {nameof(config.TotalShards)} is not specified.");

            _shardIdsToIndex = new Dictionary<int, int>();
            config.DisplayInitialLog = false;
            _baseConfig = config;

            if (config.TotalShards == null)
                _automaticShards = true;
            else
            {
                _totalShards = config.TotalShards.Value;
                _shardIds = ids ?? Enumerable.Range(0, _totalShards).ToArray();
                _shards = new DiscordSocketClient[_shardIds.Length];
                for (int i = 0; i < _shardIds.Length; i++)
                {
                    _shardIdsToIndex.Add(_shardIds[i], i);
                    var newConfig = config.Clone();
                    newConfig.ShardId = _shardIds[i];
                    _shards[i] = new DiscordSocketClient(newConfig);
                    RegisterEvents(_shards[i]);
                }
            }
        }
        private static API.DiscordSocketApiClient CreateApiClient(DiscordSocketConfig config)
            => new API.DiscordSocketApiClient(config.RestClientProvider, DiscordRestConfig.UserAgent, config.WebSocketProvider);

        protected override async Task OnLoginAsync(TokenType tokenType, string token)
        {
            if (_automaticShards)
            {
                var response = await ApiClient.GetBotGatewayAsync().ConfigureAwait(false);
                _shardIds = Enumerable.Range(0, response.Shards).ToArray();
                _totalShards = _shardIds.Length;
                _shards = new DiscordSocketClient[_shardIds.Length];
                for (int i = 0; i < _shardIds.Length; i++)
                {
                    _shardIdsToIndex.Add(_shardIds[i], i);
                    var newConfig = _baseConfig.Clone();
                    newConfig.ShardId = _shardIds[i];
                    newConfig.TotalShards = _totalShards;
                    _shards[i] = new DiscordSocketClient(newConfig);
                    RegisterEvents(_shards[i]);
                }
            }

            //Assume threadsafe: already in a connection lock
            for (int i = 0; i < _shards.Length; i++)
                await _shards[i].LoginAsync(tokenType, token, false);
        }
        protected override async Task OnLogoutAsync()
        {
            //Assume threadsafe: already in a connection lock
            for (int i = 0; i < _shards.Length; i++)
                await _shards[i].LogoutAsync();

            CurrentUser = null;
            if (_automaticShards)
            {
                _shardIds = new int[0];
                _shardIdsToIndex.Clear();
                _totalShards = 0;
                _shards = null;
            }
        }

        /// <inheritdoc />
        public async Task ConnectAsync(bool waitForGuilds = true)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await ConnectInternalAsync(waitForGuilds).ConfigureAwait(false);
            }
            catch
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
                throw;
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ConnectInternalAsync(bool waitForGuilds)
        {
            for (int i = 0; i < _shards.Length; i++)
            {
                await _shards[i].ConnectAsync(waitForGuilds).ConfigureAwait(false);
                if (i == 0)
                    CurrentUser = _shards[i].CurrentUser;
            }
        }
        /// <inheritdoc />
        public async Task DisconnectAsync()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task DisconnectInternalAsync()
        {
            for (int i = 0; i < _shards.Length; i++)
                await _shards[i].DisconnectAsync();
        }

        public DiscordSocketClient GetShard(int id)
        {
            if (_shardIdsToIndex.TryGetValue(id, out id))
                return _shards[id];
            return null;
        }
        private int GetShardIdFor(ulong guildId)
            => (int)((guildId >> 22) % (uint)_totalShards);
        private int GetShardIdFor(IGuild guild)
            => GetShardIdFor(guild.Id);
        private DiscordSocketClient GetShardFor(ulong guildId)
            => GetShard(GetShardIdFor(guildId));
        private DiscordSocketClient GetShardFor(IGuild guild)
            => GetShardFor(guild.Id);

        /// <inheritdoc />
        public async Task<RestApplication> GetApplicationInfoAsync()
            => await _shards[0].GetApplicationInfoAsync().ConfigureAwait(false);

        /// <inheritdoc />
        public SocketGuild GetGuild(ulong id) => GetShardFor(id).GetGuild(id);
        /// <inheritdoc />
        public Task<RestGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null)
            => ClientHelper.CreateGuildAsync(this, name, region, jpegIcon);

        /// <inheritdoc />
        public SocketChannel GetChannel(ulong id)
        {
            for (int i = 0; i < _shards.Length; i++)
            {
                var channel = _shards[i].GetChannel(id);
                if (channel != null)
                    return channel;
            }
            return null;
        }
        private IEnumerable<ISocketPrivateChannel> GetPrivateChannels()
        {
            for (int i = 0; i < _shards.Length; i++)
            {
                foreach (var channel in _shards[i].PrivateChannels)
                    yield return channel;
            }
        }
        private int GetPrivateChannelCount()
        {
            int result = 0;
            for (int i = 0; i < _shards.Length; i++)
                result += _shards[i].PrivateChannels.Count;
            return result;
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<RestConnection>> GetConnectionsAsync()
            => ClientHelper.GetConnectionsAsync(this);

        private IEnumerable<SocketGuild> GetGuilds()
        {
            for (int i = 0; i < _shards.Length; i++)
            {
                foreach (var guild in _shards[i].Guilds)
                    yield return guild;
            }
        }
        private int GetGuildCount()
        {
            int result = 0;
            for (int i = 0; i < _shards.Length; i++)
                result += _shards[i].Guilds.Count;
            return result;
        }

        /// <inheritdoc />
        public Task<RestInvite> GetInviteAsync(string inviteId)
            => ClientHelper.GetInviteAsync(this, inviteId);

        /// <inheritdoc />
        public SocketUser GetUser(ulong id)
        {
            for (int i = 0; i < _shards.Length; i++)
            {
                var user = _shards[i].GetUser(id);
                if (user != null)
                    return user;
            }
            return null;
        }
        /// <inheritdoc />
        public SocketUser GetUser(string username, string discriminator)
        {
            for (int i = 0; i < _shards.Length; i++)
            {
                var user = _shards[i].GetUser(username, discriminator);
                if (user != null)
                    return user;
            }
            return null;
        }

        /// <inheritdoc />
        public RestVoiceRegion GetVoiceRegion(string id)
            => _shards[0].GetVoiceRegion(id);

        /// <summary> Downloads the users list for all large guilds. </summary>
        public async Task DownloadAllUsersAsync()
        {
            for (int i = 0; i < _shards.Length; i++)
                await _shards[i].DownloadAllUsersAsync().ConfigureAwait(false);
        }
        /// <summary> Downloads the users list for the provided guilds, if they don't have a complete list. </summary>
        public async Task DownloadUsersAsync(IEnumerable<SocketGuild> guilds)
        {
            for (int i = 0; i < _shards.Length; i++)
            {
                int id = _shardIds[i];
                var arr = guilds.Where(x => GetShardIdFor(x) == id).ToArray();
                if (arr.Length > 0)
                    await _shards[i].DownloadUsersAsync(arr);
            }
        }

        public async Task SetStatusAsync(UserStatus status)
        {
            for (int i = 0; i < _shards.Length; i++)
                await _shards[i].SetStatusAsync(status).ConfigureAwait(false);
        }
        public async Task SetGameAsync(string name, string streamUrl = null, StreamType streamType = StreamType.NotStreaming)
        {
            for (int i = 0; i < _shards.Length; i++)
                await _shards[i].SetGameAsync(name, streamUrl, streamType).ConfigureAwait(false);
        }

        private void RegisterEvents(DiscordSocketClient client)
        {
            client.Log += (msg) => _logEvent.InvokeAsync(msg);
            client.ChannelCreated += (channel) => _channelCreatedEvent.InvokeAsync(channel);
            client.ChannelDestroyed += (channel) => _channelDestroyedEvent.InvokeAsync(channel);
            client.ChannelUpdated += (oldChannel, newChannel) => _channelUpdatedEvent.InvokeAsync(oldChannel, newChannel);

            client.MessageReceived += (msg) => _messageReceivedEvent.InvokeAsync(msg);
            client.MessageDeleted += (id, msg) => _messageDeletedEvent.InvokeAsync(id, msg);
            client.MessageUpdated += (oldMsg, newMsg) => _messageUpdatedEvent.InvokeAsync(oldMsg, newMsg);
            client.ReactionAdded += (id, msg, reaction) => _reactionAddedEvent.InvokeAsync(id, msg, reaction);
            client.ReactionRemoved += (id, msg, reaction) => _reactionRemovedEvent.InvokeAsync(id, msg, reaction);
            client.ReactionsCleared += (id, msg) => _reactionsClearedEvent.InvokeAsync(id, msg);

            client.RoleCreated += (role) => _roleCreatedEvent.InvokeAsync(role);
            client.RoleDeleted += (role) => _roleDeletedEvent.InvokeAsync(role);
            client.RoleUpdated += (oldRole, newRole) => _roleUpdatedEvent.InvokeAsync(oldRole, newRole);

            client.JoinedGuild += (guild) => _joinedGuildEvent.InvokeAsync(guild);
            client.LeftGuild += (guild) => _leftGuildEvent.InvokeAsync(guild);
            client.GuildAvailable += (guild) => _guildAvailableEvent.InvokeAsync(guild);
            client.GuildUnavailable += (guild) => _guildUnavailableEvent.InvokeAsync(guild);
            client.GuildMembersDownloaded += (guild) => _guildMembersDownloadedEvent.InvokeAsync(guild);
            client.GuildUpdated += (oldGuild, newGuild) => _guildUpdatedEvent.InvokeAsync(oldGuild, newGuild);

            client.UserJoined += (user) => _userJoinedEvent.InvokeAsync(user);
            client.UserLeft += (user) => _userLeftEvent.InvokeAsync(user);
            client.UserBanned += (user, guild) => _userBannedEvent.InvokeAsync(user, guild);
            client.UserUnbanned += (user, guild) => _userUnbannedEvent.InvokeAsync(user, guild);
            client.UserUpdated += (oldUser, newUser) => _userUpdatedEvent.InvokeAsync(oldUser, newUser);
            client.UserPresenceUpdated += (guild, user, oldPresence, newPresence) => _userPresenceUpdatedEvent.InvokeAsync(guild, user, oldPresence, newPresence);
            client.UserVoiceStateUpdated += (user, oldVoiceState, newVoiceState) => _userVoiceStateUpdatedEvent.InvokeAsync(user, oldVoiceState, newVoiceState);
            client.CurrentUserUpdated += (oldUser, newUser) => _selfUpdatedEvent.InvokeAsync(oldUser, newUser);
            client.UserIsTyping += (oldUser, newUser) => _userIsTypingEvent.InvokeAsync(oldUser, newUser);
            client.RecipientAdded += (user) => _recipientAddedEvent.InvokeAsync(user);
            client.RecipientAdded += (user) => _recipientRemovedEvent.InvokeAsync(user);
        }

        //IDiscordClient
        Task IDiscordClient.ConnectAsync()
            => ConnectAsync();

        async Task<IApplication> IDiscordClient.GetApplicationInfoAsync()
            => await GetApplicationInfoAsync().ConfigureAwait(false);

        Task<IChannel> IDiscordClient.GetChannelAsync(ulong id, CacheMode mode)
            => Task.FromResult<IChannel>(GetChannel(id));
        Task<IReadOnlyCollection<IPrivateChannel>> IDiscordClient.GetPrivateChannelsAsync(CacheMode mode)
            => Task.FromResult<IReadOnlyCollection<IPrivateChannel>>(PrivateChannels);

        async Task<IReadOnlyCollection<IConnection>> IDiscordClient.GetConnectionsAsync()
            => await GetConnectionsAsync().ConfigureAwait(false);

        async Task<IInvite> IDiscordClient.GetInviteAsync(string inviteId)
            => await GetInviteAsync(inviteId).ConfigureAwait(false);

        Task<IGuild> IDiscordClient.GetGuildAsync(ulong id, CacheMode mode)
            => Task.FromResult<IGuild>(GetGuild(id));
        Task<IReadOnlyCollection<IGuild>> IDiscordClient.GetGuildsAsync(CacheMode mode)
            => Task.FromResult<IReadOnlyCollection<IGuild>>(Guilds);
        async Task<IGuild> IDiscordClient.CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon)
            => await CreateGuildAsync(name, region, jpegIcon).ConfigureAwait(false);

        Task<IUser> IDiscordClient.GetUserAsync(ulong id, CacheMode mode)
            => Task.FromResult<IUser>(GetUser(id));
        Task<IUser> IDiscordClient.GetUserAsync(string username, string discriminator)
            => Task.FromResult<IUser>(GetUser(username, discriminator));

        Task<IReadOnlyCollection<IVoiceRegion>> IDiscordClient.GetVoiceRegionsAsync()
            => Task.FromResult<IReadOnlyCollection<IVoiceRegion>>(VoiceRegions);
        Task<IVoiceRegion> IDiscordClient.GetVoiceRegionAsync(string id)
            => Task.FromResult<IVoiceRegion>(GetVoiceRegion(id));
    }
}
