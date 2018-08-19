using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.API;
using Discord.Rest;

namespace Discord.WebSocket
{
    public partial class DiscordShardedClient : BaseSocketClient, IDiscordClient
    {
        private readonly DiscordSocketConfig _baseConfig;
        private readonly SemaphoreSlim _connectionGroupLock;
        private readonly bool _automaticShards;
        private int[] _shardIds;
        private readonly Dictionary<int, int> _shardIdsToIndex;
        private DiscordSocketClient[] _shards;
        private int _totalShards;

        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordShardedClient() : this(null, new DiscordSocketConfig())
        {
        }

        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordShardedClient(DiscordSocketConfig config) : this(null, config, CreateApiClient(config))
        {
        }

        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordShardedClient(int[] ids) : this(ids, new DiscordSocketConfig())
        {
        }

        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordShardedClient(int[] ids, DiscordSocketConfig config) : this(ids, config, CreateApiClient(config))
        {
        }

        private DiscordShardedClient(int[] ids, DiscordSocketConfig config, DiscordSocketApiClient client)
            : base(config, client)
        {
            if (config.ShardId != null)
                throw new ArgumentException($"{nameof(config.ShardId)} must not be set.");
            if (ids != null && config.TotalShards == null)
                throw new ArgumentException(
                    $"Custom ids are not supported when {nameof(config.TotalShards)} is not specified.");

            _shardIdsToIndex = new Dictionary<int, int>();
            config.DisplayInitialLog = false;
            _baseConfig = config;
            _connectionGroupLock = new SemaphoreSlim(1, 1);

            if (config.TotalShards == null)
                _automaticShards = true;
            else
            {
                _totalShards = config.TotalShards.Value;
                _shardIds = ids ?? Enumerable.Range(0, _totalShards).ToArray();
                _shards = new DiscordSocketClient[_shardIds.Length];
                for (var i = 0; i < _shardIds.Length; i++)
                {
                    _shardIdsToIndex.Add(_shardIds[i], i);
                    var newConfig = config.Clone();
                    newConfig.ShardId = _shardIds[i];
                    _shards[i] = new DiscordSocketClient(newConfig, _connectionGroupLock, i != 0 ? _shards[0] : null);
                    RegisterEvents(_shards[i], i == 0);
                }
            }
        }

        /// <summary> Gets the estimated round-trip latency, in milliseconds, to the gateway server. </summary>
        public override int Latency
        {
            get => GetLatency();
            protected set { }
        }

        public override UserStatus Status
        {
            get => _shards[0].Status;
            protected set { }
        }

        public override IActivity Activity
        {
            get => _shards[0].Activity;
            protected set { }
        }

        internal new DiscordSocketApiClient ApiClient => base.ApiClient;

        public override IReadOnlyCollection<SocketGuild> Guilds =>
            GetGuilds().ToReadOnlyCollection(() => GetGuildCount());

        public override IReadOnlyCollection<ISocketPrivateChannel> PrivateChannels =>
            GetPrivateChannels().ToReadOnlyCollection(() => GetPrivateChannelCount());

        public IReadOnlyCollection<DiscordSocketClient> Shards => _shards;
        public override IReadOnlyCollection<RestVoiceRegion> VoiceRegions => _shards[0].VoiceRegions;

        /// <inheritdoc />
        public override async Task StartAsync()
            => await Task.WhenAll(_shards.Select(x => x.StartAsync())).ConfigureAwait(false);

        /// <inheritdoc />
        public override async Task StopAsync()
            => await Task.WhenAll(_shards.Select(x => x.StopAsync())).ConfigureAwait(false);

        //IDiscordClient
        async Task<IApplication> IDiscordClient.GetApplicationInfoAsync(RequestOptions options)
            => await GetApplicationInfoAsync().ConfigureAwait(false);

        Task<IChannel> IDiscordClient.GetChannelAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IChannel>(GetChannel(id));

        Task<IReadOnlyCollection<IPrivateChannel>> IDiscordClient.GetPrivateChannelsAsync(CacheMode mode,
            RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IPrivateChannel>>(PrivateChannels);

        async Task<IReadOnlyCollection<IConnection>> IDiscordClient.GetConnectionsAsync(RequestOptions options)
            => await GetConnectionsAsync().ConfigureAwait(false);

        async Task<IInvite> IDiscordClient.GetInviteAsync(string inviteId, RequestOptions options)
            => await GetInviteAsync(inviteId, options).ConfigureAwait(false);

        Task<IGuild> IDiscordClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuild>(GetGuild(id));

        Task<IReadOnlyCollection<IGuild>> IDiscordClient.GetGuildsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IGuild>>(Guilds);

        async Task<IGuild> IDiscordClient.CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon,
            RequestOptions options)
            => await CreateGuildAsync(name, region, jpegIcon).ConfigureAwait(false);

        Task<IUser> IDiscordClient.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(GetUser(id));

        Task<IUser> IDiscordClient.GetUserAsync(string username, string discriminator, RequestOptions options)
            => Task.FromResult<IUser>(GetUser(username, discriminator));

        Task<IReadOnlyCollection<IVoiceRegion>> IDiscordClient.GetVoiceRegionsAsync(RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IVoiceRegion>>(VoiceRegions);

        Task<IVoiceRegion> IDiscordClient.GetVoiceRegionAsync(string id, RequestOptions options)
            => Task.FromResult<IVoiceRegion>(GetVoiceRegion(id));

        private static DiscordSocketApiClient CreateApiClient(DiscordSocketConfig config)
            => new DiscordSocketApiClient(config.RestClientProvider, config.WebSocketProvider, DiscordConfig.UserAgent);

        internal override async Task OnLoginAsync(TokenType tokenType, string token)
        {
            if (_automaticShards)
            {
                var shardCount = await GetRecommendedShardCountAsync().ConfigureAwait(false);
                _shardIds = Enumerable.Range(0, shardCount).ToArray();
                _totalShards = _shardIds.Length;
                _shards = new DiscordSocketClient[_shardIds.Length];
                for (var i = 0; i < _shardIds.Length; i++)
                {
                    _shardIdsToIndex.Add(_shardIds[i], i);
                    var newConfig = _baseConfig.Clone();
                    newConfig.ShardId = _shardIds[i];
                    newConfig.TotalShards = _totalShards;
                    _shards[i] = new DiscordSocketClient(newConfig, _connectionGroupLock, i != 0 ? _shards[0] : null);
                    RegisterEvents(_shards[i], i == 0);
                }
            }

            //Assume threadsafe: already in a connection lock
            foreach (var t in _shards)
                await t.LoginAsync(tokenType, token, false);
        }

        internal override async Task OnLogoutAsync()
        {
            //Assume threadsafe: already in a connection lock
            if (_shards != null)
                foreach (var t in _shards)
                    await t.LogoutAsync();

            CurrentUser = null;
            if (_automaticShards)
            {
                _shardIds = new int[0];
                _shardIdsToIndex.Clear();
                _totalShards = 0;
                _shards = null;
            }
        }

        public DiscordSocketClient GetShard(int id)
        {
            return _shardIdsToIndex.TryGetValue(id, out id) ? _shards[id] : null;
        }

        private int GetShardIdFor(ulong guildId)
            => (int)((guildId >> 22) % (uint)_totalShards);

        public int GetShardIdFor(IGuild guild)
            => GetShardIdFor(guild?.Id ?? 0);

        private DiscordSocketClient GetShardFor(ulong guildId)
            => GetShard(GetShardIdFor(guildId));

        public DiscordSocketClient GetShardFor(IGuild guild)
            => GetShardFor(guild?.Id ?? 0);

        /// <inheritdoc />
        public override async Task<RestApplication> GetApplicationInfoAsync(RequestOptions options = null)
            => await _shards[0].GetApplicationInfoAsync(options).ConfigureAwait(false);

        /// <inheritdoc />
        public override SocketGuild GetGuild(ulong id)
            => GetShardFor(id).GetGuild(id);

        /// <inheritdoc />
        public override SocketChannel GetChannel(ulong id)
        {
            return _shards.Select(t => t.GetChannel(id)).FirstOrDefault(channel => channel != null);
        }

        private IEnumerable<ISocketPrivateChannel> GetPrivateChannels()
        {
            return _shards.SelectMany(t => t.PrivateChannels);
        }

        private int GetPrivateChannelCount()
        {
            return _shards.Sum(t => t.PrivateChannels.Count);
        }

        private IEnumerable<SocketGuild> GetGuilds()
        {
            return _shards.SelectMany(t => t.Guilds);
        }

        private int GetGuildCount()
        {
            return _shards.Sum(t => t.Guilds.Count);
        }

        /// <inheritdoc />
        public override SocketUser GetUser(ulong id)
        {
            return _shards.Select(t => t.GetUser(id)).FirstOrDefault(user => user != null);
        }

        /// <inheritdoc />
        public override SocketUser GetUser(string username, string discriminator)
        {
            return _shards.Select(t => t.GetUser(username, discriminator)).FirstOrDefault(user => user != null);
        }

        /// <inheritdoc />
        public override RestVoiceRegion GetVoiceRegion(string id)
            => _shards[0].GetVoiceRegion(id);

        /// <summary> Downloads the users list for the provided guilds, if they don't have a complete list. </summary>
        public override async Task DownloadUsersAsync(IEnumerable<IGuild> guilds)
        {
            for (var i = 0; i < _shards.Length; i++)
            {
                var id = _shardIds[i];
                var arr = guilds.Where(x => GetShardIdFor(x) == id).ToArray();
                if (arr.Length > 0)
                    await _shards[i].DownloadUsersAsync(arr).ConfigureAwait(false);
            }
        }

        private int GetLatency()
        {
            var total = _shards.Sum(t => t.Latency);
            return (int)Math.Round(total / (double)_shards.Length);
        }

        public override async Task SetStatusAsync(UserStatus status)
        {
            foreach (var t in _shards)
                await t.SetStatusAsync(status).ConfigureAwait(false);
        }

        public override async Task SetGameAsync(string name, string streamUrl = null,
            ActivityType type = ActivityType.Playing)
        {
            IActivity activity = null;
            if (!string.IsNullOrEmpty(streamUrl))
                activity = new StreamingGame(name, streamUrl);
            else if (!string.IsNullOrEmpty(name))
                activity = new Game(name, type);
            await SetActivityAsync(activity).ConfigureAwait(false);
        }

        public override async Task SetActivityAsync(IActivity activity)
        {
            foreach (var t in _shards)
                await t.SetActivityAsync(activity).ConfigureAwait(false);
        }

        private void RegisterEvents(DiscordSocketClient client, bool isPrimary)
        {
            client.Log += msg => _logEvent.InvokeAsync(msg);
            client.LoggedOut += () =>
            {
                var state = LoginState;
                if (state == LoginState.LoggedIn || state == LoginState.LoggingIn)
                {
                    //Should only happen if token is changed
                    var _ = LogoutAsync(); //Signal the logout, fire and forget
                }

                return Task.Delay(0);
            };
            if (isPrimary)
                client.Ready += () =>
                {
                    CurrentUser = client.CurrentUser;
                    return Task.Delay(0);
                };

            client.Connected += () => _shardConnectedEvent.InvokeAsync(client);
            client.Disconnected += exception => _shardDisconnectedEvent.InvokeAsync(exception, client);
            client.Ready += () => _shardReadyEvent.InvokeAsync(client);
            client.LatencyUpdated += (oldLatency, newLatency) =>
                _shardLatencyUpdatedEvent.InvokeAsync(oldLatency, newLatency, client);

            client.ChannelCreated += channel => _channelCreatedEvent.InvokeAsync(channel);
            client.ChannelDestroyed += channel => _channelDestroyedEvent.InvokeAsync(channel);
            client.ChannelUpdated +=
                (oldChannel, newChannel) => _channelUpdatedEvent.InvokeAsync(oldChannel, newChannel);

            client.MessageReceived += msg => _messageReceivedEvent.InvokeAsync(msg);
            client.MessageDeleted += (cache, channel) => _messageDeletedEvent.InvokeAsync(cache, channel);
            client.MessageUpdated += (oldMsg, newMsg, channel) =>
                _messageUpdatedEvent.InvokeAsync(oldMsg, newMsg, channel);
            client.ReactionAdded += (cache, channel, reaction) =>
                _reactionAddedEvent.InvokeAsync(cache, channel, reaction);
            client.ReactionRemoved += (cache, channel, reaction) =>
                _reactionRemovedEvent.InvokeAsync(cache, channel, reaction);
            client.ReactionsCleared += (cache, channel) => _reactionsClearedEvent.InvokeAsync(cache, channel);

            client.RoleCreated += role => _roleCreatedEvent.InvokeAsync(role);
            client.RoleDeleted += role => _roleDeletedEvent.InvokeAsync(role);
            client.RoleUpdated += (oldRole, newRole) => _roleUpdatedEvent.InvokeAsync(oldRole, newRole);

            client.JoinedGuild += guild => _joinedGuildEvent.InvokeAsync(guild);
            client.LeftGuild += guild => _leftGuildEvent.InvokeAsync(guild);
            client.GuildAvailable += guild => _guildAvailableEvent.InvokeAsync(guild);
            client.GuildUnavailable += guild => _guildUnavailableEvent.InvokeAsync(guild);
            client.GuildMembersDownloaded += guild => _guildMembersDownloadedEvent.InvokeAsync(guild);
            client.GuildUpdated += (oldGuild, newGuild) => _guildUpdatedEvent.InvokeAsync(oldGuild, newGuild);

            client.UserJoined += user => _userJoinedEvent.InvokeAsync(user);
            client.UserLeft += user => _userLeftEvent.InvokeAsync(user);
            client.UserBanned += (user, guild) => _userBannedEvent.InvokeAsync(user, guild);
            client.UserUnbanned += (user, guild) => _userUnbannedEvent.InvokeAsync(user, guild);
            client.UserUpdated += (oldUser, newUser) => _userUpdatedEvent.InvokeAsync(oldUser, newUser);
            client.GuildMemberUpdated += (oldUser, newUser) => _guildMemberUpdatedEvent.InvokeAsync(oldUser, newUser);
            client.UserVoiceStateUpdated += (user, oldVoiceState, newVoiceState) =>
                _userVoiceStateUpdatedEvent.InvokeAsync(user, oldVoiceState, newVoiceState);
            client.VoiceServerUpdated += server => _voiceServerUpdatedEvent.InvokeAsync(server);
            client.CurrentUserUpdated += (oldUser, newUser) => _selfUpdatedEvent.InvokeAsync(oldUser, newUser);
            client.UserIsTyping += (oldUser, newUser) => _userIsTypingEvent.InvokeAsync(oldUser, newUser);
            client.RecipientAdded += user => _recipientAddedEvent.InvokeAsync(user);
            client.RecipientRemoved += user => _recipientRemovedEvent.InvokeAsync(user);
        }
    }
}
