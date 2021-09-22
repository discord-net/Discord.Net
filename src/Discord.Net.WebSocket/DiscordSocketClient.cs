using Discord.API;
using Discord.API.Gateway;
using Discord.Logging;
using Discord.Net.Converters;
using Discord.Net.Udp;
using Discord.Net.WebSockets;
using Discord.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameModel = Discord.API.Game;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based Discord client.
    /// </summary>
    public partial class DiscordSocketClient : BaseSocketClient, IDiscordClient
    {
        #region DiscordSocketClient
        private readonly ConcurrentQueue<ulong> _largeGuilds;
        internal readonly JsonSerializer _serializer;
        private readonly DiscordShardedClient _shardedClient;
        private readonly DiscordSocketClient _parentClient;
        private readonly ConcurrentQueue<long> _heartbeatTimes;
        private readonly ConnectionManager _connection;
        private readonly Logger _gatewayLogger;
        private readonly SemaphoreSlim _stateLock;

        private string _sessionId;
        private int _lastSeq;
        private ImmutableDictionary<string, RestVoiceRegion> _voiceRegions;
        private Task _heartbeatTask, _guildDownloadTask;
        private int _unavailableGuildCount;
        private long _lastGuildAvailableTime, _lastMessageTime;
        private int _nextAudioId;
        private DateTimeOffset? _statusSince;
        private RestApplication _applicationInfo;
        private bool _isDisposed;
        private GatewayIntents _gatewayIntents;
        private ImmutableArray<StickerPack<SocketSticker>> _defaultStickers;

        /// <summary>
        ///     Provides access to a REST-only client with a shared state from this client.
        /// </summary>
        public override DiscordSocketRestClient Rest { get; }
        /// <summary> Gets the shard of of this client. </summary>
        public int ShardId { get; }
        /// <summary> Gets the current connection state of this client. </summary>
        public ConnectionState ConnectionState => _connection.State;
        /// <inheritdoc />
        public override int Latency { get; protected set; }
        /// <inheritdoc />
        public override UserStatus Status { get => _status ?? UserStatus.Online; protected set => _status = value; }
        private UserStatus? _status;
        /// <inheritdoc />
        public override IActivity Activity { get => _activity.GetValueOrDefault(); protected set => _activity = Optional.Create(value); }
        private Optional<IActivity> _activity;
        #endregion

        //From DiscordSocketConfig
        internal int TotalShards { get; private set; }
        internal int MessageCacheSize { get; private set; }
        internal int LargeThreshold { get; private set; }
        internal ClientState State { get; private set; }
        internal UdpSocketProvider UdpSocketProvider { get; private set; }
        internal WebSocketProvider WebSocketProvider { get; private set; }
        internal bool AlwaysDownloadUsers { get; private set; }
        internal int? HandlerTimeout { get; private set; }
        internal new DiscordSocketApiClient ApiClient => base.ApiClient as DiscordSocketApiClient;
        /// <inheritdoc />
        public override IReadOnlyCollection<SocketGuild> Guilds => State.Guilds;
        /// <inheritdoc/>
        public override IReadOnlyCollection<StickerPack<SocketSticker>> DefaultStickerPacks
        {
            get
            {
                if (this._shardedClient != null)
                    return this._shardedClient.DefaultStickerPacks;
                else
                    return _defaultStickers.ToReadOnlyCollection();
            }
        }
        /// <inheritdoc />
        public override IReadOnlyCollection<ISocketPrivateChannel> PrivateChannels => State.PrivateChannels;
        /// <summary>
        ///     Gets a collection of direct message channels opened in this session.
        /// </summary>
        /// <remarks>
        ///     This method returns a collection of currently opened direct message channels.
        ///     <note type="warning">
        ///         This method will not return previously opened DM channels outside of the current session! If you
        ///         have just started the client, this may return an empty collection.
        ///     </note>
        /// </remarks>
        /// <returns>
        ///     A collection of DM channels that have been opened in this session.
        /// </returns>
        public IReadOnlyCollection<SocketDMChannel> DMChannels
            => State.PrivateChannels.OfType<SocketDMChannel>().ToImmutableArray();
        /// <summary>
        ///     Gets a collection of group channels opened in this session.
        /// </summary>
        /// <remarks>
        ///     This method returns a collection of currently opened group channels.
        ///     <note type="warning">
        ///         This method will not return previously opened group channels outside of the current session! If you
        ///         have just started the client, this may return an empty collection.
        ///     </note>
        /// </remarks>
        /// <returns>
        ///     A collection of group channels that have been opened in this session.
        /// </returns>
        public IReadOnlyCollection<SocketGroupChannel> GroupChannels
            => State.PrivateChannels.OfType<SocketGroupChannel>().ToImmutableArray();

        /// <summary>
        ///     Initializes a new REST/WebSocket-based Discord client.
        /// </summary>
        public DiscordSocketClient() : this(new DiscordSocketConfig()) { }
        /// <summary>
        ///     Initializes a new REST/WebSocket-based Discord client with the provided configuration.
        /// </summary>
        /// <param name="config">The configuration to be used with the client.</param>
#pragma warning disable IDISP004
        public DiscordSocketClient(DiscordSocketConfig config) : this(config, CreateApiClient(config), null, null) { }
        internal DiscordSocketClient(DiscordSocketConfig config, DiscordShardedClient shardedClient, DiscordSocketClient parentClient) : this(config, CreateApiClient(config), shardedClient, parentClient) { }
#pragma warning restore IDISP004
        private DiscordSocketClient(DiscordSocketConfig config, API.DiscordSocketApiClient client, DiscordShardedClient shardedClient, DiscordSocketClient parentClient)
            : base(config, client)
        {
            ShardId = config.ShardId ?? 0;
            TotalShards = config.TotalShards ?? 1;
            MessageCacheSize = config.MessageCacheSize;
            LargeThreshold = config.LargeThreshold;
            UdpSocketProvider = config.UdpSocketProvider;
            WebSocketProvider = config.WebSocketProvider;
            AlwaysDownloadUsers = config.AlwaysDownloadUsers;
            HandlerTimeout = config.HandlerTimeout;
            State = new ClientState(0, 0);
            Rest = new DiscordSocketRestClient(config, ApiClient);
            _heartbeatTimes = new ConcurrentQueue<long>();
            _gatewayIntents = config.GatewayIntents;
            _defaultStickers = ImmutableArray.Create<StickerPack<SocketSticker>>();

            _stateLock = new SemaphoreSlim(1, 1);
            _gatewayLogger = LogManager.CreateLogger(ShardId == 0 && TotalShards == 1 ? "Gateway" : $"Shard #{ShardId}");
            _connection = new ConnectionManager(_stateLock, _gatewayLogger, config.ConnectionTimeout,
                OnConnectingAsync, OnDisconnectingAsync, x => ApiClient.Disconnected += x);
            _connection.Connected += () => TimedInvokeAsync(_connectedEvent, nameof(Connected));
            _connection.Disconnected += (ex, recon) => TimedInvokeAsync(_disconnectedEvent, nameof(Disconnected), ex);

            _nextAudioId = 1;
            _shardedClient = shardedClient;
            _parentClient = parentClient;

            _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };
            _serializer.Error += (s, e) =>
            {
                _gatewayLogger.WarningAsync("Serializer Error", e.ErrorContext.Error).GetAwaiter().GetResult();
                e.ErrorContext.Handled = true;
            };

            ApiClient.SentGatewayMessage += async opCode => await _gatewayLogger.DebugAsync($"Sent {opCode}").ConfigureAwait(false);
            ApiClient.ReceivedGatewayEvent += ProcessMessageAsync;

            LeftGuild += async g => await _gatewayLogger.InfoAsync($"Left {g.Name}").ConfigureAwait(false);
            JoinedGuild += async g => await _gatewayLogger.InfoAsync($"Joined {g.Name}").ConfigureAwait(false);
            GuildAvailable += async g => await _gatewayLogger.VerboseAsync($"Connected to {g.Name}").ConfigureAwait(false);
            GuildUnavailable += async g => await _gatewayLogger.VerboseAsync($"Disconnected from {g.Name}").ConfigureAwait(false);
            LatencyUpdated += async (old, val) => await _gatewayLogger.DebugAsync($"Latency = {val} ms").ConfigureAwait(false);

            GuildAvailable += g =>
            {
                if (_guildDownloadTask?.IsCompleted == true && ConnectionState == ConnectionState.Connected && AlwaysDownloadUsers && !g.HasAllMembers)
                {
                    var _ = g.DownloadUsersAsync();
                }
                return Task.Delay(0);
            };

            _largeGuilds = new ConcurrentQueue<ulong>();
        }
        private static API.DiscordSocketApiClient CreateApiClient(DiscordSocketConfig config)
            => new API.DiscordSocketApiClient(config.RestClientProvider, config.WebSocketProvider, DiscordRestConfig.UserAgent, config.GatewayHost);
        /// <inheritdoc />
        internal override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    StopAsync().GetAwaiter().GetResult();
                    ApiClient?.Dispose();
                    _stateLock?.Dispose();
                }
                _isDisposed = true;
            }

            base.Dispose(disposing);
        }

        internal override async Task OnLoginAsync(TokenType tokenType, string token)
        {
            if(this._shardedClient == null && this._defaultStickers.Length == 0)
            {
                var models = await ApiClient.ListNitroStickerPacksAsync().ConfigureAwait(false);

                var builder = ImmutableArray.CreateBuilder<StickerPack<SocketSticker>>();

                foreach (var model in models.StickerPacks)
                {
                    var stickers = model.Stickers.Select(x => SocketSticker.Create(this, x));

                    var pack = new StickerPack<SocketSticker>(
                        model.Name,
                        model.Id,
                        model.SkuId,
                        model.CoverStickerId.ToNullable(),
                        model.Description,
                        model.BannerAssetId,
                        stickers
                    );

                    builder.Add(pack);
                }

                _defaultStickers = builder.ToImmutable();
            }
        }

        /// <inheritdoc />
        internal override async Task OnLogoutAsync()
        {
            await StopAsync().ConfigureAwait(false);
            _applicationInfo = null;
            _voiceRegions = null;
            await Rest.OnLogoutAsync();
        }

        /// <inheritdoc />
        public override async Task StartAsync()
            => await _connection.StartAsync().ConfigureAwait(false);
        /// <inheritdoc />
        public override async Task StopAsync()
            => await _connection.StopAsync().ConfigureAwait(false);

        private async Task OnConnectingAsync()
        {
            bool locked = false;
            if (_shardedClient != null && _sessionId == null)
            {
                await _shardedClient.AcquireIdentifyLockAsync(ShardId, _connection.CancelToken).ConfigureAwait(false);
                locked = true;
            }
            try
            {
                await _gatewayLogger.DebugAsync("Connecting ApiClient").ConfigureAwait(false);
                await ApiClient.ConnectAsync().ConfigureAwait(false);

                if (_sessionId != null)
                {
                    await _gatewayLogger.DebugAsync("Resuming").ConfigureAwait(false);
                    await ApiClient.SendResumeAsync(_sessionId, _lastSeq).ConfigureAwait(false);
                }
                else
                {
                    await _gatewayLogger.DebugAsync("Identifying").ConfigureAwait(false);
                    await ApiClient.SendIdentifyAsync(shardID: ShardId, totalShards: TotalShards, gatewayIntents: _gatewayIntents, presence: BuildCurrentStatus()).ConfigureAwait(false);
                }
            }
            finally
            {
                if (locked)
                    _shardedClient.ReleaseIdentifyLock();
            }

            //Wait for READY
            await _connection.WaitAsync().ConfigureAwait(false);
        }
        private async Task OnDisconnectingAsync(Exception ex)
        {

            await _gatewayLogger.DebugAsync("Disconnecting ApiClient").ConfigureAwait(false);
            await ApiClient.DisconnectAsync(ex).ConfigureAwait(false);

            //Wait for tasks to complete
            await _gatewayLogger.DebugAsync("Waiting for heartbeater").ConfigureAwait(false);
            var heartbeatTask = _heartbeatTask;
            if (heartbeatTask != null)
                await heartbeatTask.ConfigureAwait(false);
            _heartbeatTask = null;

            while (_heartbeatTimes.TryDequeue(out _)) { }
            _lastMessageTime = 0;

            await _gatewayLogger.DebugAsync("Waiting for guild downloader").ConfigureAwait(false);
            var guildDownloadTask = _guildDownloadTask;
            if (guildDownloadTask != null)
                await guildDownloadTask.ConfigureAwait(false);
            _guildDownloadTask = null;

            //Clear large guild queue
            await _gatewayLogger.DebugAsync("Clearing large guild queue").ConfigureAwait(false);
            while (_largeGuilds.TryDequeue(out _)) { }

            //Raise virtual GUILD_UNAVAILABLEs
            await _gatewayLogger.DebugAsync("Raising virtual GuildUnavailables").ConfigureAwait(false);
            foreach (var guild in State.Guilds)
            {
                if (guild.IsAvailable)
                    await GuildUnavailableAsync(guild).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public override async Task<RestApplication> GetApplicationInfoAsync(RequestOptions options = null)
            => _applicationInfo ?? (_applicationInfo = await ClientHelper.GetApplicationInfoAsync(this, options ?? RequestOptions.Default).ConfigureAwait(false));

        /// <inheritdoc />
        public override SocketGuild GetGuild(ulong id)
            => State.GetGuild(id);

        /// <inheritdoc />
        public override SocketChannel GetChannel(ulong id)
            => State.GetChannel(id);
        /// <summary>
        ///     Gets a generic channel from the cache or does a rest request if unavailable.
        /// </summary>
        /// <example>
        ///     <code language="cs" title="Example method">
        ///     var channel = await _client.GetChannelAsync(381889909113225237);
        ///     if (channel != null &amp;&amp; channel is IMessageChannel msgChannel)
        ///     {
        ///         await msgChannel.SendMessageAsync($"{msgChannel} is created at {msgChannel.CreatedAt}");
        ///     }
        ///     </code>
        /// </example>
        /// <param name="id">The snowflake identifier of the channel (e.g. `381889909113225237`).</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the channel associated
        ///     with the snowflake identifier; <c>null</c> when the channel cannot be found.
        /// </returns>
        public async ValueTask<IChannel> GetChannelAsync(ulong id, RequestOptions options = null)
            => GetChannel(id) ?? (IChannel)await ClientHelper.GetChannelAsync(this, id, options).ConfigureAwait(false);
        /// <summary>
        ///     Gets a user from the cache or does a rest request if unavailable.
        /// </summary>
        /// <example>
        ///     <code language="cs" title="Example method">
        ///     var user = await _client.GetUserAsync(168693960628371456);
        ///     if (user != null)
        ///         Console.WriteLine($"{user} is created at {user.CreatedAt}.";
        ///     </code>
        /// </example>
        /// <param name="id">The snowflake identifier of the user (e.g. `168693960628371456`).</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the user associated with
        ///     the snowflake identifier; <c>null</c> if the user is not found.
        /// </returns>
        public async ValueTask<IUser> GetUserAsync(ulong id, RequestOptions options = null)
            => await ClientHelper.GetUserAsync(this, id, options).ConfigureAwait(false);
        /// <summary>
        ///     Clears all cached channels from the client.
        /// </summary>
        public void PurgeChannelCache() => State.PurgeAllChannels();
        /// <summary>
        ///     Clears cached DM channels from the client.
        /// </summary>
        public void PurgeDMChannelCache() => RemoveDMChannels();

        /// <inheritdoc />
        public override SocketUser GetUser(ulong id)
            => State.GetUser(id);
        /// <inheritdoc />
        public override SocketUser GetUser(string username, string discriminator)
            => State.Users.FirstOrDefault(x => x.Discriminator == discriminator && x.Username == username);

        /// <summary>
        ///     Gets a global application command.
        /// </summary>
        /// <param name="id">The id of the command.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A ValueTask that represents the asynchronous get operation. The task result contains the application command if found, otherwise
        ///     <see langword="null"/>.
        /// </returns>
        public async ValueTask<SocketApplicationCommand> GetGlobalApplicationCommandAsync(ulong id, RequestOptions options = null)
        {
            var command = State.GetCommand(id);

            if (command != null)
                return command;

            var model = await ApiClient.GetGlobalApplicationCommandAsync(id, options);

            if (model == null)
                return null;

            command = SocketApplicationCommand.Create(this, model);

            State.AddCommand(command);

            return command;
        }
        /// <summary>
        ///     Gets a collection of all global commands.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of global
        ///     application commands.
        /// </returns>
        public async Task<IReadOnlyCollection<SocketApplicationCommand>> GetGlobalApplicationCommandsAsync(RequestOptions options = null)
        {
            var commands = (await ApiClient.GetGlobalApplicationCommandsAsync(options)).Select(x => SocketApplicationCommand.Create(this, x));

            foreach(var command in commands)
            {
                State.AddCommand(command);
            }

            return commands.ToImmutableArray();
        }

        public async Task<SocketApplicationCommand> CreateGlobalApplicationCommandAsync(ApplicationCommandProperties properties, RequestOptions options = null)
        {
            var model = await InteractionHelper.CreateGlobalCommand(this, properties, options).ConfigureAwait(false);

            var entity = State.GetOrAddCommand(model.Id, (id) => SocketApplicationCommand.Create(this, model));

            //Update it incase it was cached
            entity.Update(model);

            return entity;
        }
        public async Task<IReadOnlyCollection<SocketApplicationCommand>> BulkOverwriteGlobalApplicationCommandsAsync(
            ApplicationCommandProperties[] properties, RequestOptions options = null)
        {
            var models = await InteractionHelper.BulkOverwriteGlobalCommands(this, properties, options);

            var entities = models.Select(x => SocketApplicationCommand.Create(this, x));

            //Purge our previous commands
            State.PurgeCommands(x => x.IsGlobalCommand);

            foreach(var entity in entities)
            {
                State.AddCommand(entity);
            }

            return entities.ToImmutableArray();
        }

        /// <summary>
        ///     Clears cached users from the client.
        /// </summary>
        public void PurgeUserCache() => State.PurgeUsers();
        internal SocketGlobalUser GetOrCreateUser(ClientState state, Discord.API.User model)
        {
            return state.GetOrAddUser(model.Id, x => SocketGlobalUser.Create(this, state, model));
        }
        internal SocketUser GetOrCreateTemporaryUser(ClientState state, Discord.API.User model)
        {
            return state.GetUser(model.Id) ?? (SocketUser)SocketUnknownUser.Create(this, state, model);
        }
        internal SocketGlobalUser GetOrCreateSelfUser(ClientState state, Discord.API.User model)
        {
            return state.GetOrAddUser(model.Id, x =>
            {
                var user = SocketGlobalUser.Create(this, state, model);
                user.GlobalUser.AddRef();
                user.Presence = new SocketPresence(UserStatus.Online, null, null);
                return user;
            });
        }
        internal void RemoveUser(ulong id)
            => State.RemoveUser(id);

        /// <inheritdoc/>
        public override async Task<SocketSticker> GetStickerAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            var sticker = _defaultStickers.FirstOrDefault(x => x.Stickers.Any(y => y.Id == id))?.Stickers.FirstOrDefault(x => x.Id == id);

            if (sticker != null)
                return sticker;

            foreach(var guild in Guilds)
            {
                sticker = await guild.GetStickerAsync(id, CacheMode.CacheOnly).ConfigureAwait(false);

                if (sticker != null)
                    return sticker;
            }

            if (mode == CacheMode.CacheOnly)
                return null;

            var model = await ApiClient.GetStickerAsync(id, options).ConfigureAwait(false);

            if(model == null)
                return null;

            
            if (model.GuildId.IsSpecified)
            {
                var guild = State.GetGuild(model.GuildId.Value);

                //Since the sticker can be from another guild, check if we are in the guild or its in the cache
                if (guild != null)
                    sticker = guild.AddOrUpdateSticker(model);
                else
                    sticker = SocketSticker.Create(this, model);
                return sticker;
            }
            else
            {
                return SocketSticker.Create(this, model);
            }
        }

        /// <summary>
        ///     Gets a sticker.
        /// </summary>
        /// <param name="id">The unique identifier of the sticker.</param>
        /// <returns>A sticker if found, otherwise <see langword="null"/>.</returns>
        public SocketSticker GetSticker(ulong id)
            => GetStickerAsync(id, CacheMode.CacheOnly).GetAwaiter().GetResult();

        /// <inheritdoc />
        public override async ValueTask<IReadOnlyCollection<RestVoiceRegion>> GetVoiceRegionsAsync(RequestOptions options = null)
        {
            if (_parentClient == null)
            {
                if (_voiceRegions == null)
                {
                    options = RequestOptions.CreateOrClone(options);
                    options.IgnoreState = true;
                    var voiceRegions = await ApiClient.GetVoiceRegionsAsync(options).ConfigureAwait(false);
                    _voiceRegions = voiceRegions.Select(x => RestVoiceRegion.Create(this, x)).ToImmutableDictionary(x => x.Id);
                }
                return _voiceRegions.ToReadOnlyCollection();
            }
            return await _parentClient.GetVoiceRegionsAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async ValueTask<RestVoiceRegion> GetVoiceRegionAsync(string id, RequestOptions options = null)
        {
            if (_parentClient == null)
            {
                if (_voiceRegions == null)
                    await GetVoiceRegionsAsync().ConfigureAwait(false);
                if (_voiceRegions.TryGetValue(id, out RestVoiceRegion region))
                    return region;
                return null;
            }
            return await _parentClient.GetVoiceRegionAsync(id, options).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task DownloadUsersAsync(IEnumerable<IGuild> guilds)
        {
            if (ConnectionState == ConnectionState.Connected)
            {
                //Race condition leads to guilds being requested twice, probably okay
                await ProcessUserDownloadsAsync(guilds.Select(x => GetGuild(x.Id)).Where(x => x != null)).ConfigureAwait(false);
            }
        }
        private async Task ProcessUserDownloadsAsync(IEnumerable<SocketGuild> guilds)
        {
            var cachedGuilds = guilds.ToImmutableArray();

            const short batchSize = 1;
            ulong[] batchIds = new ulong[Math.Min(batchSize, cachedGuilds.Length)];
            Task[] batchTasks = new Task[batchIds.Length];
            int batchCount = (cachedGuilds.Length + (batchSize - 1)) / batchSize;

            for (int i = 0, k = 0; i < batchCount; i++)
            {
                bool isLast = i == batchCount - 1;
                int count = isLast ? (cachedGuilds.Length - (batchCount - 1) * batchSize) : batchSize;

                for (int j = 0; j < count; j++, k++)
                {
                    var guild = cachedGuilds[k];
                    batchIds[j] = guild.Id;
                    batchTasks[j] = guild.DownloaderPromise;
                }

                await ApiClient.SendRequestMembersAsync(batchIds).ConfigureAwait(false);

                if (isLast && batchCount > 1)
                    await Task.WhenAll(batchTasks.Take(count)).ConfigureAwait(false);
                else
                    await Task.WhenAll(batchTasks).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        /// <example>
        ///     The following example sets the status of the current user to Do Not Disturb.
        ///     <code language="cs">
        ///     await client.SetStatusAsync(UserStatus.DoNotDisturb);
        ///     </code>
        /// </example>
        public override async Task SetStatusAsync(UserStatus status)
        {
            Status = status;
            if (status == UserStatus.AFK)
                _statusSince = DateTimeOffset.UtcNow;
            else
                _statusSince = null;
            await SendStatusAsync().ConfigureAwait(false);
        }
        /// <inheritdoc />
        /// <example>
        /// <para>
        ///     The following example sets the activity of the current user to the specified game name.
        ///     <code language="cs">
        ///     await client.SetGameAsync("A Strange Game");
        ///     </code>
        /// </para>
        /// <para>
        ///     The following example sets the activity of the current user to a streaming status.
        ///     <code language="cs">
        ///     await client.SetGameAsync("Great Stream 10/10", "https://twitch.tv/MyAmazingStream1337", ActivityType.Streaming);
        ///     </code>
        /// </para>
        /// </example>
        public override async Task SetGameAsync(string name, string streamUrl = null, ActivityType type = ActivityType.Playing)
        {
            if (!string.IsNullOrEmpty(streamUrl))
                Activity = new StreamingGame(name, streamUrl);
            else if (!string.IsNullOrEmpty(name))
                Activity = new Game(name, type);
            else
                Activity = null;
            await SendStatusAsync().ConfigureAwait(false);
        }
        /// <inheritdoc />
        public override async Task SetActivityAsync(IActivity activity)
        {
            Activity = activity;
            await SendStatusAsync().ConfigureAwait(false);
        }

        private async Task SendStatusAsync()
        {
            if (CurrentUser == null)
                return;
            var activities = _activity.IsSpecified ? ImmutableList.Create(_activity.Value) : null;
            CurrentUser.Presence = new SocketPresence(Status, null, activities);

            var presence = BuildCurrentStatus() ?? (UserStatus.Online, false, null, null);

            await ApiClient.SendPresenceUpdateAsync(
                status: presence.Item1,
                isAFK: presence.Item2,
                since: presence.Item3,
                game: presence.Item4).ConfigureAwait(false);
        }

        private (UserStatus, bool, long?, GameModel)? BuildCurrentStatus()
        {
            var status = _status;
            var statusSince = _statusSince;
            var activity = _activity;

            if (status == null && !activity.IsSpecified)
                return null;

            GameModel game = null;
            //Discord only accepts rich presence over RPC, don't even bother building a payload

            if (activity.GetValueOrDefault() != null)
            {
                var gameModel = new GameModel();
                if (activity.Value is RichGame)
                    throw new NotSupportedException("Outgoing Rich Presences are not supported via WebSocket.");
                gameModel.Name = Activity.Name;
                gameModel.Type = Activity.Type;
                if (Activity is StreamingGame streamGame)
                    gameModel.StreamUrl = streamGame.Url;
                game = gameModel;
            }
            else if (activity.IsSpecified)
                game = null;

            return (status ?? UserStatus.Online,
                    status == UserStatus.AFK,
                    statusSince != null ? _statusSince.Value.ToUnixTimeMilliseconds() : (long?)null,
                    game);
        }

        #region ProcessMessageAsync
        private async Task ProcessMessageAsync(GatewayOpCode opCode, int? seq, string type, object payload)
        {
            if (seq != null)
                _lastSeq = seq.Value;
            _lastMessageTime = Environment.TickCount;

            try
            {
                switch (opCode)
                {
                    case GatewayOpCode.Hello:
                        {
                            await _gatewayLogger.DebugAsync("Received Hello").ConfigureAwait(false);
                            var data = (payload as JToken).ToObject<HelloEvent>(_serializer);

                            _heartbeatTask = RunHeartbeatAsync(data.HeartbeatInterval, _connection.CancelToken);
                        }
                        break;
                    case GatewayOpCode.Heartbeat:
                        {
                            await _gatewayLogger.DebugAsync("Received Heartbeat").ConfigureAwait(false);

                            await ApiClient.SendHeartbeatAsync(_lastSeq).ConfigureAwait(false);
                        }
                        break;
                    case GatewayOpCode.HeartbeatAck:
                        {
                            await _gatewayLogger.DebugAsync("Received HeartbeatAck").ConfigureAwait(false);

                            if (_heartbeatTimes.TryDequeue(out long time))
                            {
                                int latency = (int)(Environment.TickCount - time);
                                int before = Latency;
                                Latency = latency;

                                await TimedInvokeAsync(_latencyUpdatedEvent, nameof(LatencyUpdated), before, latency).ConfigureAwait(false);
                            }
                        }
                        break;
                    case GatewayOpCode.InvalidSession:
                        {
                            await _gatewayLogger.DebugAsync("Received InvalidSession").ConfigureAwait(false);
                            await _gatewayLogger.WarningAsync("Failed to resume previous session").ConfigureAwait(false);

                            _sessionId = null;
                            _lastSeq = 0;

                            if (_shardedClient != null)
                            {
                                await _shardedClient.AcquireIdentifyLockAsync(ShardId, _connection.CancelToken).ConfigureAwait(false);
                                try
                                {
                                    await ApiClient.SendIdentifyAsync(shardID: ShardId, totalShards: TotalShards, gatewayIntents: _gatewayIntents, presence: BuildCurrentStatus()).ConfigureAwait(false);
                                }
                                finally
                                {
                                    _shardedClient.ReleaseIdentifyLock();
                                }
                            }
                            else
                                await ApiClient.SendIdentifyAsync(shardID: ShardId, totalShards: TotalShards, gatewayIntents: _gatewayIntents, presence: BuildCurrentStatus()).ConfigureAwait(false);
                        }
                        break;
                    case GatewayOpCode.Reconnect:
                        {
                            await _gatewayLogger.DebugAsync("Received Reconnect").ConfigureAwait(false);
                            _connection.Error(new GatewayReconnectException("Server requested a reconnect"));
                        }
                        break;
                    case GatewayOpCode.Dispatch:
                        switch (type)
                        {
                            #region Connection
                            case "READY":
                                {
                                    try
                                    {
                                        await _gatewayLogger.DebugAsync("Received Dispatch (READY)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<ReadyEvent>(_serializer);
                                        var state = new ClientState(data.Guilds.Length, data.PrivateChannels.Length);

                                        var currentUser = SocketSelfUser.Create(this, state, data.User);
                                        Rest.CreateRestSelfUser(data.User);
                                        var activities = _activity.IsSpecified ? ImmutableList.Create(_activity.Value) : null;
                                        currentUser.Presence = new SocketPresence(Status, null, activities);
                                        ApiClient.CurrentUserId = currentUser.Id;
                                        Rest.CurrentUser = RestSelfUser.Create(this, data.User);
                                        int unavailableGuilds = 0;
                                        for (int i = 0; i < data.Guilds.Length; i++)
                                        {
                                            var model = data.Guilds[i];
                                            var guild = AddGuild(model, state);
                                            if (!guild.IsAvailable)
                                                unavailableGuilds++;
                                            else
                                                await GuildAvailableAsync(guild).ConfigureAwait(false);
                                        }
                                        for (int i = 0; i < data.PrivateChannels.Length; i++)
                                            AddPrivateChannel(data.PrivateChannels[i], state);

                                        _sessionId = data.SessionId;
                                        _unavailableGuildCount = unavailableGuilds;
                                        CurrentUser = currentUser;
                                        State = state;
                                    }
                                    catch (Exception ex)
                                    {
                                        _connection.CriticalError(new Exception("Processing READY failed", ex));
                                        return;
                                    }

                                    _lastGuildAvailableTime = Environment.TickCount;
                                    _guildDownloadTask = WaitForGuildsAsync(_connection.CancelToken, _gatewayLogger)
                                        .ContinueWith(async x =>
                                        {
                                            if (x.IsFaulted)
                                            {
                                                _connection.Error(x.Exception);
                                                return;
                                            }
                                            else if (_connection.CancelToken.IsCancellationRequested)
                                                return;

                                            if (BaseConfig.AlwaysDownloadUsers)
                                                _ = DownloadUsersAsync(Guilds.Where(x => x.IsAvailable && !x.HasAllMembers));

                                            await TimedInvokeAsync(_readyEvent, nameof(Ready)).ConfigureAwait(false);
                                            await _gatewayLogger.InfoAsync("Ready").ConfigureAwait(false);
                                        });
                                    _ = _connection.CompleteAsync();
                                }
                                break;
                            case "RESUMED":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (RESUMED)").ConfigureAwait(false);

                                    _ = _connection.CompleteAsync();

                                    //Notify the client that these guilds are available again
                                    foreach (var guild in State.Guilds)
                                    {
                                        if (guild.IsAvailable)
                                            await GuildAvailableAsync(guild).ConfigureAwait(false);
                                    }

                                    await _gatewayLogger.InfoAsync("Resumed previous session").ConfigureAwait(false);
                                }
                                break;
                            #endregion

                            #region Guilds
                            case "GUILD_CREATE":
                                {
                                    var data = (payload as JToken).ToObject<ExtendedGuild>(_serializer);

                                    if (data.Unavailable == false)
                                    {
                                        type = "GUILD_AVAILABLE";
                                        _lastGuildAvailableTime = Environment.TickCount;
                                        await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_AVAILABLE)").ConfigureAwait(false);

                                        var guild = State.GetGuild(data.Id);
                                        if (guild != null)
                                        {
                                            guild.Update(State, data);

                                            if (_unavailableGuildCount != 0)
                                                _unavailableGuildCount--;
                                            await GuildAvailableAsync(guild).ConfigureAwait(false);

                                            if (guild.DownloadedMemberCount >= guild.MemberCount && !guild.DownloaderPromise.IsCompleted)
                                            {
                                                guild.CompleteDownloadUsers();
                                                await TimedInvokeAsync(_guildMembersDownloadedEvent, nameof(GuildMembersDownloaded), guild).ConfigureAwait(false);
                                            }
                                        }
                                        else
                                        {
                                            await UnknownGuildAsync(type, data.Id).ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_CREATE)").ConfigureAwait(false);

                                        var guild = AddGuild(data, State);
                                        if (guild != null)
                                        {
                                            await TimedInvokeAsync(_joinedGuildEvent, nameof(JoinedGuild), guild).ConfigureAwait(false);
                                            await GuildAvailableAsync(guild).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            await UnknownGuildAsync(type, data.Id).ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                }
                                break;
                            case "GUILD_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Guild>(_serializer);
                                    var guild = State.GetGuild(data.Id);
                                    if (guild != null)
                                    {
                                        var before = guild.Clone();
                                        guild.Update(State, data);
                                        await TimedInvokeAsync(_guildUpdatedEvent, nameof(GuildUpdated), before, guild).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(type, data.Id).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_EMOJIS_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_EMOJIS_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Gateway.GuildEmojiUpdateEvent>(_serializer);
                                    var guild = State.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var before = guild.Clone();
                                        guild.Update(State, data);
                                        await TimedInvokeAsync(_guildUpdatedEvent, nameof(GuildUpdated), before, guild).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_SYNC":
                                {
                                    await _gatewayLogger.DebugAsync("Ignored Dispatch (GUILD_SYNC)").ConfigureAwait(false);
                                    /*await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_SYNC)").ConfigureAwait(false); //TODO remove? userbot related
                                    var data = (payload as JToken).ToObject<GuildSyncEvent>(_serializer);
                                    var guild = State.GetGuild(data.Id);
                                    if (guild != null)
                                    {
                                        var before = guild.Clone();
                                        guild.Update(State, data);
                                        //This is treated as an extension of GUILD_AVAILABLE
                                        _unavailableGuildCount--;
                                        _lastGuildAvailableTime = Environment.TickCount;
                                        await GuildAvailableAsync(guild).ConfigureAwait(false);
                                        await TimedInvokeAsync(_guildUpdatedEvent, nameof(GuildUpdated), before, guild).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(type, data.Id).ConfigureAwait(false);
                                        return;
                                    }*/
                                }
                                break;
                            case "GUILD_DELETE":
                                {
                                    var data = (payload as JToken).ToObject<ExtendedGuild>(_serializer);
                                    if (data.Unavailable == true)
                                    {
                                        type = "GUILD_UNAVAILABLE";
                                        await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_UNAVAILABLE)").ConfigureAwait(false);

                                        var guild = State.GetGuild(data.Id);
                                        if (guild != null)
                                        {
                                            await GuildUnavailableAsync(guild).ConfigureAwait(false);
                                            _unavailableGuildCount++;
                                        }
                                        else
                                        {
                                            await UnknownGuildAsync(type, data.Id).ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_DELETE)").ConfigureAwait(false);

                                        var guild = RemoveGuild(data.Id);
                                        if (guild != null)
                                        {
                                            await GuildUnavailableAsync(guild).ConfigureAwait(false);
                                            await TimedInvokeAsync(_leftGuildEvent, nameof(LeftGuild), guild).ConfigureAwait(false);
                                            (guild as IDisposable).Dispose();
                                        }
                                        else
                                        {
                                            await UnknownGuildAsync(type, data.Id).ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                }
                                break;
                            #endregion

                            #region Channels
                            case "CHANNEL_CREATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_CREATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                                    SocketChannel channel = null;
                                    if (data.GuildId.IsSpecified)
                                    {
                                        var guild = State.GetGuild(data.GuildId.Value);
                                        if (guild != null)
                                        {
                                            channel = guild.AddChannel(State, data);

                                            if (!guild.IsSynced)
                                            {
                                                await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        channel = State.GetChannel(data.Id);
                                        if (channel != null)
                                            return; //Discord may send duplicate CHANNEL_CREATEs for DMs
                                        channel = AddPrivateChannel(data, State) as SocketChannel;
                                    }

                                    if (channel != null)
                                        await TimedInvokeAsync(_channelCreatedEvent, nameof(ChannelCreated), channel).ConfigureAwait(false);
                                }
                                break;
                            case "CHANNEL_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                                    var channel = State.GetChannel(data.Id);
                                    if (channel != null)
                                    {
                                        var before = channel.Clone();
                                        channel.Update(State, data);

                                        var guild = (channel as SocketGuildChannel)?.Guild;
                                        if (!(guild?.IsSynced ?? true))
                                        {
                                            await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                            return;
                                        }

                                        await TimedInvokeAsync(_channelUpdatedEvent, nameof(ChannelUpdated), before, channel).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownChannelAsync(type, data.Id).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "CHANNEL_DELETE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_DELETE)").ConfigureAwait(false);

                                    SocketChannel channel = null;
                                    var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                                    if (data.GuildId.IsSpecified)
                                    {
                                        var guild = State.GetGuild(data.GuildId.Value);
                                        if (guild != null)
                                        {
                                            channel = guild.RemoveChannel(State, data.Id);

                                            if (!guild.IsSynced)
                                            {
                                                await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                        channel = RemovePrivateChannel(data.Id) as SocketChannel;

                                    if (channel != null)
                                        await TimedInvokeAsync(_channelDestroyedEvent, nameof(ChannelDestroyed), channel).ConfigureAwait(false);
                                    else
                                    {
                                        await UnknownChannelAsync(type, data.Id, data.GuildId.GetValueOrDefault(0)).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            #endregion

                            #region Members
                            case "GUILD_MEMBER_ADD":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_MEMBER_ADD)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildMemberAddEvent>(_serializer);
                                    var guild = State.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var user = guild.AddOrUpdateUser(data);
                                        guild.MemberCount++;

                                        if (!guild.IsSynced)
                                        {
                                            await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                            return;
                                        }

                                        await TimedInvokeAsync(_userJoinedEvent, nameof(UserJoined), user).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_MEMBER_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_MEMBER_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildMemberUpdateEvent>(_serializer);
                                    var guild = State.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var user = guild.GetUser(data.User.Id);

                                        if (!guild.IsSynced)
                                        {
                                            await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                            return;
                                        }

                                        if (user != null)
                                        {
                                            var globalBefore = user.GlobalUser.Clone();
                                            if (user.GlobalUser.Update(State, data.User))
                                            {
                                                //Global data was updated, trigger UserUpdated
                                                await TimedInvokeAsync(_userUpdatedEvent, nameof(UserUpdated), globalBefore, user).ConfigureAwait(false);
                                            }

                                            var before = user.Clone();
                                            user.Update(State, data);

                                            var cacheableBefore = new Cacheable<SocketGuildUser, ulong>(before, user.Id, true, () => Task.FromResult((SocketGuildUser)null));
                                            await TimedInvokeAsync(_guildMemberUpdatedEvent, nameof(GuildMemberUpdated), cacheableBefore, user).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            user = guild.AddOrUpdateUser(data);
                                            var cacheableBefore = new Cacheable<SocketGuildUser, ulong>(null, user.Id, false, () => Task.FromResult((SocketGuildUser)null));
                                            await TimedInvokeAsync(_guildMemberUpdatedEvent, nameof(GuildMemberUpdated), cacheableBefore, user).ConfigureAwait(false);
                                        }
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_MEMBER_REMOVE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_MEMBER_REMOVE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildMemberRemoveEvent>(_serializer);
                                    var guild = State.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var user = guild.RemoveUser(data.User.Id);
                                        guild.MemberCount--;

                                        if (!guild.IsSynced)
                                        {
                                            await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                            return;
                                        }

                                        if (user != null)
                                            await TimedInvokeAsync(_userLeftEvent, nameof(UserLeft), user).ConfigureAwait(false);
                                        else
                                        {
                                            if (!guild.HasAllMembers)
                                                await IncompleteGuildUserAsync(type, data.User.Id, data.GuildId).ConfigureAwait(false);
                                            else
                                                await UnknownGuildUserAsync(type, data.User.Id, data.GuildId).ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_MEMBERS_CHUNK":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_MEMBERS_CHUNK)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildMembersChunkEvent>(_serializer);
                                    var guild = State.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        foreach (var memberModel in data.Members)
                                            guild.AddOrUpdateUser(memberModel);

                                        if (guild.DownloadedMemberCount >= guild.MemberCount && !guild.DownloaderPromise.IsCompleted)
                                        {
                                            guild.CompleteDownloadUsers();
                                            await TimedInvokeAsync(_guildMembersDownloadedEvent, nameof(GuildMembersDownloaded), guild).ConfigureAwait(false);
                                        }
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "CHANNEL_RECIPIENT_ADD":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_RECIPIENT_ADD)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<RecipientEvent>(_serializer);
                                    if (State.GetChannel(data.ChannelId) is SocketGroupChannel channel)
                                    {
                                        var user = channel.GetOrAddUser(data.User);
                                        await TimedInvokeAsync(_recipientAddedEvent, nameof(RecipientAdded), user).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownChannelAsync(type, data.ChannelId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "CHANNEL_RECIPIENT_REMOVE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_RECIPIENT_REMOVE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<RecipientEvent>(_serializer);
                                    if (State.GetChannel(data.ChannelId) is SocketGroupChannel channel)
                                    {
                                        var user = channel.RemoveUser(data.User.Id);
                                        if (user != null)
                                            await TimedInvokeAsync(_recipientRemovedEvent, nameof(RecipientRemoved), user).ConfigureAwait(false);
                                        else
                                        {
                                            await UnknownChannelUserAsync(type, data.User.Id, data.ChannelId).ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await UnknownChannelAsync(type, data.ChannelId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            #endregion

                            #region Roles
                            case "GUILD_ROLE_CREATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_ROLE_CREATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildRoleCreateEvent>(_serializer);
                                    var guild = State.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var role = guild.AddRole(data.Role);

                                        if (!guild.IsSynced)
                                        {
                                            await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                            return;
                                        }
                                        await TimedInvokeAsync(_roleCreatedEvent, nameof(RoleCreated), role).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_ROLE_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_ROLE_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildRoleUpdateEvent>(_serializer);
                                    var guild = State.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var role = guild.GetRole(data.Role.Id);
                                        if (role != null)
                                        {
                                            var before = role.Clone();
                                            role.Update(State, data.Role);

                                            if (!guild.IsSynced)
                                            {
                                                await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                                return;
                                            }

                                            await TimedInvokeAsync(_roleUpdatedEvent, nameof(RoleUpdated), before, role).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            await UnknownRoleAsync(type, data.Role.Id, guild.Id).ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_ROLE_DELETE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_ROLE_DELETE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildRoleDeleteEvent>(_serializer);
                                    var guild = State.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var role = guild.RemoveRole(data.RoleId);
                                        if (role != null)
                                        {
                                            if (!guild.IsSynced)
                                            {
                                                await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                                return;
                                            }

                                            await TimedInvokeAsync(_roleDeletedEvent, nameof(RoleDeleted), role).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            await UnknownRoleAsync(type, data.RoleId, guild.Id).ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            #endregion

                            #region Bans
                            case "GUILD_BAN_ADD":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_BAN_ADD)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildBanEvent>(_serializer);
                                    var guild = State.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        if (!guild.IsSynced)
                                        {
                                            await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                            return;
                                        }

                                        SocketUser user = guild.GetUser(data.User.Id);
                                        if (user == null)
                                            user = SocketUnknownUser.Create(this, State, data.User);
                                        await TimedInvokeAsync(_userBannedEvent, nameof(UserBanned), user, guild).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_BAN_REMOVE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_BAN_REMOVE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildBanEvent>(_serializer);
                                    var guild = State.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        if (!guild.IsSynced)
                                        {
                                            await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                            return;
                                        }

                                        SocketUser user = State.GetUser(data.User.Id);
                                        if (user == null)
                                            user = SocketUnknownUser.Create(this, State, data.User);
                                        await TimedInvokeAsync(_userUnbannedEvent, nameof(UserUnbanned), user, guild).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            #endregion

                            #region Messages
                            case "MESSAGE_CREATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_CREATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Message>(_serializer);
                                    var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                                    var guild = (channel as SocketGuildChannel)?.Guild;
                                    if (guild != null && !guild.IsSynced)
                                    {
                                        await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                        return;
                                    }

                                    if (channel == null)
                                    {
                                        if (!data.GuildId.IsSpecified)  // assume it is a DM
                                        {
                                            channel = CreateDMChannel(data.ChannelId, data.Author.Value, State);
                                        }
                                        else
                                        {
                                            await UnknownChannelAsync(type, data.ChannelId).ConfigureAwait(false);
                                            return;
                                        }
                                    }

                                    SocketUser author;
                                    if (guild != null)
                                    {
                                        if (data.WebhookId.IsSpecified)
                                            author = SocketWebhookUser.Create(guild, State, data.Author.Value, data.WebhookId.Value);
                                        else
                                            author = guild.GetUser(data.Author.Value.Id);
                                    }
                                    else
                                        author = (channel as SocketChannel).GetUser(data.Author.Value.Id);

                                    if (author == null)
                                    {
                                        if (guild != null)
                                        {
                                            if (data.Member.IsSpecified) // member isn't always included, but use it when we can
                                            {
                                                data.Member.Value.User = data.Author.Value;
                                                author = guild.AddOrUpdateUser(data.Member.Value);
                                            }
                                            else
                                                author = guild.AddOrUpdateUser(data.Author.Value); // user has no guild-specific data
                                        }
                                        else if (channel is SocketGroupChannel groupChannel)
                                            author = groupChannel.GetOrAddUser(data.Author.Value);
                                        else
                                        {
                                            await UnknownChannelUserAsync(type, data.Author.Value.Id, channel.Id).ConfigureAwait(false);
                                            return;
                                        }
                                    }

                                    var msg = SocketMessage.Create(this, State, author, channel, data);
                                    SocketChannelHelper.AddMessage(channel, this, msg);
                                    await TimedInvokeAsync(_messageReceivedEvent, nameof(MessageReceived), msg).ConfigureAwait(false);
                                }
                                break;
                            case "MESSAGE_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Message>(_serializer);
                                    var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                                    var guild = (channel as SocketGuildChannel)?.Guild;
                                    if (guild != null && !guild.IsSynced)
                                    {
                                        await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                        return;
                                    }

                                    SocketMessage before = null, after = null;
                                    SocketMessage cachedMsg = channel?.GetCachedMessage(data.Id);
                                    bool isCached = cachedMsg != null;
                                    if (isCached)
                                    {
                                        before = cachedMsg.Clone();
                                        cachedMsg.Update(State, data);
                                        after = cachedMsg;
                                    }
                                    else
                                    {
                                        //Edited message isnt in cache, create a detached one
                                        SocketUser author;
                                        if (data.Author.IsSpecified)
                                        {
                                            if (guild != null)
                                            {
                                                if (data.WebhookId.IsSpecified)
                                                    author = SocketWebhookUser.Create(guild, State, data.Author.Value, data.WebhookId.Value);
                                                else
                                                    author = guild.GetUser(data.Author.Value.Id);
                                            }
                                            else
                                                author = (channel as SocketChannel)?.GetUser(data.Author.Value.Id);

                                            if (author == null)
                                            {
                                                if (guild != null)
                                                {
                                                    if (data.Member.IsSpecified) // member isn't always included, but use it when we can
                                                    {
                                                        data.Member.Value.User = data.Author.Value;
                                                        author = guild.AddOrUpdateUser(data.Member.Value);
                                                    }
                                                    else
                                                        author = guild.AddOrUpdateUser(data.Author.Value); // user has no guild-specific data
                                                }
                                                else if (channel is SocketGroupChannel groupChannel)
                                                    author = groupChannel.GetOrAddUser(data.Author.Value);
                                            }
                                        }
                                        else
                                            // Message author wasn't specified in the payload, so create a completely anonymous unknown user
                                            author = new SocketUnknownUser(this, id: 0);

                                        if (channel == null)
                                        {
                                            if (!data.GuildId.IsSpecified)  // assume it is a DM
                                            {
                                                if (data.Author.IsSpecified)
                                                {
                                                    var dmChannel = CreateDMChannel(data.ChannelId, data.Author.Value, State);
                                                    channel = dmChannel;
                                                    author = dmChannel.Recipient;
                                                }
                                                else
                                                    channel = CreateDMChannel(data.ChannelId, author, State);
                                            }
                                            else
                                            {
                                                await UnknownChannelAsync(type, data.ChannelId).ConfigureAwait(false);
                                                return;
                                            }
                                        }

                                        after = SocketMessage.Create(this, State, author, channel, data);
                                    }
                                    var cacheableBefore = new Cacheable<IMessage, ulong>(before, data.Id, isCached, async () => await channel.GetMessageAsync(data.Id).ConfigureAwait(false));

                                    await TimedInvokeAsync(_messageUpdatedEvent, nameof(MessageUpdated), cacheableBefore, after, channel).ConfigureAwait(false);
                                }
                                break;
                            case "MESSAGE_DELETE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_DELETE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Message>(_serializer);
                                    var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                                    var guild = (channel as SocketGuildChannel)?.Guild;
                                    if (!(guild?.IsSynced ?? true))
                                    {
                                        await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                        return;
                                    }

                                    SocketMessage msg = null;
                                    if (channel != null)
                                        msg = SocketChannelHelper.RemoveMessage(channel, this, data.Id);
                                    var cacheableMsg = new Cacheable<IMessage, ulong>(msg, data.Id, msg != null, () => Task.FromResult((IMessage)null));
                                    var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);

                                    await TimedInvokeAsync(_messageDeletedEvent, nameof(MessageDeleted), cacheableMsg, cacheableChannel).ConfigureAwait(false);
                                }
                                break;
                            case "MESSAGE_REACTION_ADD":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_REACTION_ADD)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Gateway.Reaction>(_serializer);
                                    var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                                    var cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                    bool isMsgCached = cachedMsg != null;
                                    IUser user = null;
                                    if (channel != null)
                                        user = await channel.GetUserAsync(data.UserId, CacheMode.CacheOnly).ConfigureAwait(false);

                                    var optionalMsg = !isMsgCached
                                        ? Optional.Create<SocketUserMessage>()
                                        : Optional.Create(cachedMsg);

                                    if (data.Member.IsSpecified)
                                    {
                                        var guild = (channel as SocketGuildChannel)?.Guild;

                                        if (guild != null)
                                            user = guild.AddOrUpdateUser(data.Member.Value);
                                    }
                                    else
                                        user = GetUser(data.UserId);

                                    var optionalUser = user is null
                                        ? Optional.Create<IUser>()
                                        : Optional.Create(user);

                                    var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);
                                    var cacheableMsg = new Cacheable<IUserMessage, ulong>(cachedMsg, data.MessageId, isMsgCached, async () =>
                                    {
                                        var channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                        return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false) as IUserMessage;
                                    });
                                    var reaction = SocketReaction.Create(data, channel, optionalMsg, optionalUser);

                                    cachedMsg?.AddReaction(reaction);

                                    await TimedInvokeAsync(_reactionAddedEvent, nameof(ReactionAdded), cacheableMsg, cacheableChannel, reaction).ConfigureAwait(false);
                                }
                                break;
                            case "MESSAGE_REACTION_REMOVE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_REACTION_REMOVE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Gateway.Reaction>(_serializer);
                                    var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                                    var cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                    bool isMsgCached = cachedMsg != null;
                                    IUser user = null;
                                    if (channel != null)
                                        user = await channel.GetUserAsync(data.UserId, CacheMode.CacheOnly).ConfigureAwait(false);
                                    else if (!data.GuildId.IsSpecified)
                                        user = GetUser(data.UserId);

                                    var optionalMsg = !isMsgCached
                                        ? Optional.Create<SocketUserMessage>()
                                        : Optional.Create(cachedMsg);

                                    var optionalUser = user is null
                                        ? Optional.Create<IUser>()
                                        : Optional.Create(user);

                                    var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);
                                    var cacheableMsg = new Cacheable<IUserMessage, ulong>(cachedMsg, data.MessageId, isMsgCached, async () =>
                                    {
                                        var channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                        return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false) as IUserMessage;
                                    });
                                    var reaction = SocketReaction.Create(data, channel, optionalMsg, optionalUser);

                                    cachedMsg?.RemoveReaction(reaction);

                                    await TimedInvokeAsync(_reactionRemovedEvent, nameof(ReactionRemoved), cacheableMsg, cacheableChannel, reaction).ConfigureAwait(false);
                                }
                                break;
                            case "MESSAGE_REACTION_REMOVE_ALL":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_REACTION_REMOVE_ALL)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<RemoveAllReactionsEvent>(_serializer);
                                    var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                                    var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);
                                    var cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                    bool isMsgCached = cachedMsg != null;
                                    var cacheableMsg = new Cacheable<IUserMessage, ulong>(cachedMsg, data.MessageId, isMsgCached, async () =>
                                    {
                                        var channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                        return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false) as IUserMessage;
                                    });

                                    cachedMsg?.ClearReactions();

                                    await TimedInvokeAsync(_reactionsClearedEvent, nameof(ReactionsCleared), cacheableMsg, cacheableChannel).ConfigureAwait(false);
                                }
                                break;
                            case "MESSAGE_REACTION_REMOVE_EMOJI":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_REACTION_REMOVE_EMOJI)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Gateway.RemoveAllReactionsForEmoteEvent>(_serializer);
                                    var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                                    var cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                    bool isMsgCached = cachedMsg != null;

                                    var optionalMsg = !isMsgCached
                                        ? Optional.Create<SocketUserMessage>()
                                        : Optional.Create(cachedMsg);

                                    var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);
                                    var cacheableMsg = new Cacheable<IUserMessage, ulong>(cachedMsg, data.MessageId, isMsgCached, async () =>
                                    {
                                        var channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                        return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false) as IUserMessage;
                                    });
                                    var emote = data.Emoji.ToIEmote();

                                    cachedMsg?.RemoveReactionsForEmote(emote);

                                    await TimedInvokeAsync(_reactionsRemovedForEmoteEvent, nameof(ReactionsRemovedForEmote), cacheableMsg, cacheableChannel, emote).ConfigureAwait(false);
                                }
                                break;
                            case "MESSAGE_DELETE_BULK":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_DELETE_BULK)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<MessageDeleteBulkEvent>(_serializer);
                                    var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                                    var guild = (channel as SocketGuildChannel)?.Guild;
                                    if (!(guild?.IsSynced ?? true))
                                    {
                                        await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                        return;
                                    }

                                    var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);
                                    var cacheableList = new List<Cacheable<IMessage, ulong>>(data.Ids.Length);
                                    foreach (ulong id in data.Ids)
                                    {
                                        SocketMessage msg = null;
                                        if (channel != null)
                                            msg = SocketChannelHelper.RemoveMessage(channel, this, id);
                                        bool isMsgCached = msg != null;
                                        var cacheableMsg = new Cacheable<IMessage, ulong>(msg, id, isMsgCached, () => Task.FromResult((IMessage)null));
                                        cacheableList.Add(cacheableMsg);
                                    }

                                    await TimedInvokeAsync(_messagesBulkDeletedEvent, nameof(MessagesBulkDeleted), cacheableList, cacheableChannel).ConfigureAwait(false);
                                }
                                break;
                            #endregion

                            #region Statuses
                            case "PRESENCE_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (PRESENCE_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Presence>(_serializer);

                                    if (data.GuildId.IsSpecified)
                                    {
                                        var guild = State.GetGuild(data.GuildId.Value);
                                        if (guild == null)
                                        {
                                            await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                            return;
                                        }
                                        if (!guild.IsSynced)
                                        {
                                            await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                            return;
                                        }

                                        var user = guild.GetUser(data.User.Id);
                                        if (user == null)
                                        {
                                            if (data.Status == UserStatus.Offline)
                                            {
                                                return;
                                            }
                                            user = guild.AddOrUpdateUser(data);
                                        }
                                        else
                                        {
                                            var globalBefore = user.GlobalUser.Clone();
                                            if (user.GlobalUser.Update(State, data.User))
                                            {
                                                //Global data was updated, trigger UserUpdated
                                                await TimedInvokeAsync(_userUpdatedEvent, nameof(UserUpdated), globalBefore, user).ConfigureAwait(false);
                                            }
                                        }

                                        var before = user.Clone();
                                        user.Update(State, data, true);
                                        var cacheableBefore = new Cacheable<SocketGuildUser, ulong>(before, user.Id, true, () => Task.FromResult(user));
                                        await TimedInvokeAsync(_guildMemberUpdatedEvent, nameof(GuildMemberUpdated), cacheableBefore, user).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        var globalUser = State.GetUser(data.User.Id);
                                        if (globalUser == null)
                                        {
                                            await UnknownGlobalUserAsync(type, data.User.Id).ConfigureAwait(false);
                                            return;
                                        }

                                        var before = globalUser.Clone();
                                        globalUser.Update(State, data.User);
                                        globalUser.Update(State, data);
                                        await TimedInvokeAsync(_userUpdatedEvent, nameof(UserUpdated), before, globalUser).ConfigureAwait(false);
                                    }
                                }
                                break;
                            case "TYPING_START":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (TYPING_START)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<TypingStartEvent>(_serializer);
                                    var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                                    var guild = (channel as SocketGuildChannel)?.Guild;
                                    if (!(guild?.IsSynced ?? true))
                                    {
                                        await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                        return;
                                    }

                                    var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);

                                    var user = (channel as SocketChannel)?.GetUser(data.UserId);
                                    if (user == null)
                                    {
                                        if (guild != null)
                                            user = guild.AddOrUpdateUser(data.Member);
                                    }
                                    var cacheableUser = new Cacheable<IUser, ulong>(user, data.UserId, user != null, async () => await GetUserAsync(data.UserId).ConfigureAwait(false));

                                    await TimedInvokeAsync(_userIsTypingEvent, nameof(UserIsTyping), cacheableUser, cacheableChannel).ConfigureAwait(false);
                                }
                                break;
                            #endregion

                            #region Users
                            case "USER_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (USER_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.User>(_serializer);
                                    if (data.Id == CurrentUser.Id)
                                    {
                                        var before = CurrentUser.Clone();
                                        CurrentUser.Update(State, data);
                                        await TimedInvokeAsync(_selfUpdatedEvent, nameof(CurrentUserUpdated), before, CurrentUser).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("Received USER_UPDATE for wrong user.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            #endregion

                            #region Voice
                            case "VOICE_STATE_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (VOICE_STATE_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.VoiceState>(_serializer);
                                    SocketUser user;
                                    SocketVoiceState before, after;
                                    if (data.GuildId != null)
                                    {
                                        var guild = State.GetGuild(data.GuildId.Value);
                                        if (guild == null)
                                        {
                                            await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                            return;
                                        }
                                        else if (!guild.IsSynced)
                                        {
                                            await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                            return;
                                        }

                                        if (data.ChannelId != null)
                                        {
                                            before = guild.GetVoiceState(data.UserId)?.Clone() ?? SocketVoiceState.Default;
                                            after = await guild.AddOrUpdateVoiceStateAsync(State, data).ConfigureAwait(false);
                                            /*if (data.UserId == CurrentUser.Id)
                                            {
                                                var _ = guild.FinishJoinAudioChannel().ConfigureAwait(false);
                                            }*/
                                        }
                                        else
                                        {
                                            before = await guild.RemoveVoiceStateAsync(data.UserId).ConfigureAwait(false) ?? SocketVoiceState.Default;
                                            after = SocketVoiceState.Create(null, data);
                                        }

                                        //Per g250k, this should always be sent, but apparently not always
                                        user = guild.GetUser(data.UserId)
                                            ?? (data.Member.IsSpecified ? guild.AddOrUpdateUser(data.Member.Value) : null);
                                        if (user == null)
                                        {
                                            await UnknownGuildUserAsync(type, data.UserId, guild.Id).ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        var groupChannel = GetChannel(data.ChannelId.Value) as SocketGroupChannel;
                                        if (groupChannel == null)
                                        {
                                            await UnknownChannelAsync(type, data.ChannelId.Value).ConfigureAwait(false);
                                            return;
                                        }
                                        if (data.ChannelId != null)
                                        {
                                            before = groupChannel.GetVoiceState(data.UserId)?.Clone() ?? SocketVoiceState.Default;
                                            after = groupChannel.AddOrUpdateVoiceState(State, data);
                                        }
                                        else
                                        {
                                            before = groupChannel.RemoveVoiceState(data.UserId) ?? SocketVoiceState.Default;
                                            after = SocketVoiceState.Create(null, data);
                                        }
                                        user = groupChannel.GetUser(data.UserId);
                                        if (user == null)
                                        {
                                            await UnknownChannelUserAsync(type, data.UserId, groupChannel.Id).ConfigureAwait(false);
                                            return;
                                        }
                                    }

                                    if (user is SocketGuildUser guildUser && data.ChannelId.HasValue)
                                    {
                                        SocketStageChannel stage = guildUser.Guild.GetStageChannel(data.ChannelId.Value);

                                        if (stage != null && before.VoiceChannel != null && after.VoiceChannel != null)
                                        {
                                            if (!before.RequestToSpeakTimestamp.HasValue && after.RequestToSpeakTimestamp.HasValue)
                                            {
                                                await TimedInvokeAsync(_requestToSpeak, nameof(RequestToSpeak), stage, guildUser);
                                                return;
                                            }
                                            if(before.IsSuppressed && !after.IsSuppressed)
                                            {
                                                await TimedInvokeAsync(_speakerAdded, nameof(SpeakerAdded), stage, guildUser);
                                                return;
                                            }
                                            if(!before.IsSuppressed && after.IsSuppressed)
                                            {
                                                await TimedInvokeAsync(_speakerRemoved, nameof(SpeakerRemoved), stage, guildUser);
                                            }
                                        }    
                                    }

                                    await TimedInvokeAsync(_userVoiceStateUpdatedEvent, nameof(UserVoiceStateUpdated), user, before, after).ConfigureAwait(false);
                                }
                                break;
                            case "VOICE_SERVER_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (VOICE_SERVER_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<VoiceServerUpdateEvent>(_serializer);
                                    var guild = State.GetGuild(data.GuildId);
                                    var isCached = guild != null;
                                    var cachedGuild = new Cacheable<IGuild, ulong>(guild, data.GuildId, isCached,
                                        () => Task.FromResult(State.GetGuild(data.GuildId) as IGuild));

                                    var voiceServer = new SocketVoiceServer(cachedGuild, data.Endpoint, data.Token);
                                    await TimedInvokeAsync(_voiceServerUpdatedEvent, nameof(UserVoiceStateUpdated), voiceServer).ConfigureAwait(false);

                                    if (isCached)
                                    {
                                        var endpoint = data.Endpoint;

                                        //Only strip out the port if the endpoint contains it
                                        var portBegin = endpoint.LastIndexOf(':');
                                        if (portBegin > 0)
                                            endpoint = endpoint.Substring(0, portBegin);

                                        var _ = guild.FinishConnectAudio(endpoint, data.Token).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                    }

                                }
                                break;
                            #endregion

                            #region Invites
                            case "INVITE_CREATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (INVITE_CREATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Gateway.InviteCreateEvent>(_serializer);
                                    if (State.GetChannel(data.ChannelId) is SocketGuildChannel channel)
                                    {
                                        var guild = channel.Guild;
                                        if (!guild.IsSynced)
                                        {
                                            await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                            return;
                                        }

                                        SocketGuildUser inviter = data.Inviter.IsSpecified
                                            ? (guild.GetUser(data.Inviter.Value.Id) ?? guild.AddOrUpdateUser(data.Inviter.Value))
                                            : null;

                                        SocketUser target = data.TargetUser.IsSpecified
                                            ? (guild.GetUser(data.TargetUser.Value.Id) ?? (SocketUser)SocketUnknownUser.Create(this, State, data.TargetUser.Value))
                                            : null;

                                        var invite = SocketInvite.Create(this, guild, channel, inviter, target, data);

                                        await TimedInvokeAsync(_inviteCreatedEvent, nameof(InviteCreated), invite).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownChannelAsync(type, data.ChannelId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "INVITE_DELETE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (INVITE_DELETE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Gateway.InviteDeleteEvent>(_serializer);
                                    if (State.GetChannel(data.ChannelId) is SocketGuildChannel channel)
                                    {
                                        var guild = channel.Guild;
                                        if (!guild.IsSynced)
                                        {
                                            await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                            return;
                                        }

                                        await TimedInvokeAsync(_inviteDeletedEvent, nameof(InviteDeleted), channel, data.Code).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownChannelAsync(type, data.ChannelId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            #endregion

                            #region Interactions
                            case "INTERACTION_CREATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (INTERACTION_CREATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Interaction>(_serializer);

                                    SocketChannel channel = null;
                                    if(data.ChannelId.IsSpecified)
                                    {
                                        channel = State.GetChannel(data.ChannelId.Value);
                                    }
                                    else if (data.User.IsSpecified)
                                    {
                                        channel = State.GetDMChannel(data.User.Value.Id);
                                    }

                                    if (channel == null)
                                    {
                                        var channelModel = await this.Rest.ApiClient.GetChannelAsync(data.ChannelId.Value);

                                        if (data.GuildId.IsSpecified)
                                            channel = SocketTextChannel.Create(State.GetGuild(data.GuildId.Value), this.State, channelModel);
                                        else
                                            channel = (SocketChannel)SocketChannel.CreatePrivate(this, State, channelModel);

                                        this.State.AddChannel(channel);
                                    }

                                    if (channel is ISocketMessageChannel textChannel)
                                    {
                                        var guild = (channel as SocketGuildChannel)?.Guild;
                                        if (guild != null && !guild.IsSynced)
                                        {
                                            await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                            return;
                                        }

                                        var interaction = SocketInteraction.Create(this, data, channel as ISocketMessageChannel);

                                        await TimedInvokeAsync(_interactionCreatedEvent, nameof(InteractionCreated), interaction).ConfigureAwait(false);

                                        switch (interaction)
                                        {
                                            case SocketSlashCommand slashCommand:
                                                await TimedInvokeAsync(_slashCommandExecuted, nameof(SlashCommandExecuted), slashCommand).ConfigureAwait(false);
                                                break;
                                            case SocketMessageComponent messageComponent:
                                                if(messageComponent.Data.Type == ComponentType.SelectMenu)
                                                    await TimedInvokeAsync(_selectMenuExecuted, nameof(SelectMenuExecuted), messageComponent).ConfigureAwait(false);
                                                if(messageComponent.Data.Type == ComponentType.Button)
                                                    await TimedInvokeAsync(_buttonExecuted, nameof(ButtonExecuted), messageComponent).ConfigureAwait(false);
                                                break;
                                            case SocketUserCommand userCommand:
                                                await TimedInvokeAsync(_userCommandExecuted, nameof(UserCommandExecuted), userCommand).ConfigureAwait(false);
                                                break;
                                            case SocketMessageCommand messageCommand:
                                                await TimedInvokeAsync(_messageCommandExecuted, nameof(MessageCommandExecuted), messageCommand).ConfigureAwait(false);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        await UnknownChannelAsync(type, data.ChannelId.Value).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "APPLICATION_COMMAND_CREATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (APPLICATION_COMMAND_CREATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Gateway.ApplicationCommandCreatedUpdatedEvent>(_serializer);

                                    if (data.GuildId.IsSpecified)
                                    {
                                        var guild = State.GetGuild(data.GuildId.Value);
                                        if (guild == null)
                                        {
                                            await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                            return;
                                        }
                                    }

                                    var applicationCommand = SocketApplicationCommand.Create(this, data);

                                    State.AddCommand(applicationCommand);

                                    await TimedInvokeAsync(_applicationCommandCreated, nameof(ApplicationCommandCreated), applicationCommand).ConfigureAwait(false);
                                }
                                break;
                            case "APPLICATION_COMMAND_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (APPLICATION_COMMAND_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Gateway.ApplicationCommandCreatedUpdatedEvent>(_serializer);

                                    if (data.GuildId.IsSpecified)
                                    {
                                        var guild = State.GetGuild(data.GuildId.Value);
                                        if (guild == null)
                                        {
                                            await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                            return;
                                        }
                                    }

                                    var applicationCommand = SocketApplicationCommand.Create(this, data);

                                    State.AddCommand(applicationCommand);

                                    await TimedInvokeAsync(_applicationCommandUpdated, nameof(ApplicationCommandUpdated), applicationCommand).ConfigureAwait(false);
                                }
                                break;
                            case "APPLICATION_COMMAND_DELETE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (APPLICATION_COMMAND_DELETE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Gateway.ApplicationCommandCreatedUpdatedEvent>(_serializer);

                                    if (data.GuildId.IsSpecified)
                                    {
                                        var guild = State.GetGuild(data.GuildId.Value);
                                        if (guild == null)
                                        {
                                            await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                            return;
                                        }
                                    }

                                    var applicationCommand = SocketApplicationCommand.Create(this, data);

                                    State.RemoveCommand(applicationCommand.Id);

                                    await TimedInvokeAsync(_applicationCommandDeleted, nameof(ApplicationCommandDeleted), applicationCommand).ConfigureAwait(false);
                                }
                                break;
                            #endregion

                            #region Threads
                            case "THREAD_CREATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (THREAD_CREATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<Channel>(_serializer);

                                    var guild = State.GetGuild(data.GuildId.Value);

                                    if (guild == null)
                                    {
                                        await UnknownGuildAsync(type, data.GuildId.Value);
                                        return;
                                    }

                                    SocketThreadChannel threadChannel = null;

                                    if ((threadChannel = guild.ThreadChannels.FirstOrDefault(x => x.Id == data.Id)) != null)
                                    {
                                        threadChannel.Update(this.State, data);

                                        if(data.ThreadMember.IsSpecified)
                                            threadChannel.AddOrUpdateThreadMember(data.ThreadMember.Value, guild.CurrentUser);
                                    }
                                    else
                                    {
                                        threadChannel = (SocketThreadChannel)guild.AddChannel(this.State, data);
                                        if (data.ThreadMember.IsSpecified)
                                            threadChannel.AddOrUpdateThreadMember(data.ThreadMember.Value, guild.CurrentUser);
                                    }

                                    await TimedInvokeAsync(_threadCreated, nameof(ThreadCreated), threadChannel).ConfigureAwait(false);
                                }

                                break;
                            case "THREAD_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (THREAD_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                                    var guild = State.GetGuild(data.GuildId.Value);
                                    if (guild == null)
                                    {
                                        await UnknownGuildAsync(type, data.GuildId.Value);
                                        return;
                                    }

                                    var threadChannel = guild.ThreadChannels.FirstOrDefault(x => x.Id == data.Id);
                                    var before = threadChannel != null
                                        ? new Cacheable<SocketThreadChannel, ulong>(threadChannel.Clone(), data.Id, true, () => Task.FromResult((SocketThreadChannel)null))
                                        : new Cacheable<SocketThreadChannel, ulong>(null, data.Id, false, () => Task.FromResult((SocketThreadChannel)null));

                                    if (threadChannel != null)
                                    {
                                        threadChannel.Update(State, data);

                                        if (data.ThreadMember.IsSpecified)
                                            threadChannel.AddOrUpdateThreadMember(data.ThreadMember.Value, guild.CurrentUser);
                                    }
                                    else
                                    {
                                        //Thread is updated but was not cached, likely meaning the thread was unarchived.
                                        threadChannel = (SocketThreadChannel)guild.AddChannel(State, data);
                                        if (data.ThreadMember.IsSpecified)
                                            threadChannel.AddOrUpdateThreadMember(data.ThreadMember.Value, guild.CurrentUser);
                                    }

                                    if (!(guild?.IsSynced ?? true))
                                    {
                                        await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                        return;
                                    }

                                    await TimedInvokeAsync(_threadUpdated, nameof(ThreadUpdated), before, threadChannel).ConfigureAwait(false);
                                }
                                break;
                            case "THREAD_DELETE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (THREAD_DELETE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Channel>(_serializer);

                                    var guild = State.GetGuild(data.GuildId.Value);

                                    if(guild == null)
                                    {
                                        await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                        return;
                                    }

                                    var thread = (SocketThreadChannel)guild.GetChannel(data.Id);

                                    var cacheable = new Cacheable<SocketThreadChannel, ulong>(thread, data.Id, thread != null, null);

                                    await TimedInvokeAsync(_threadDeleted, nameof(ThreadDeleted), cacheable).ConfigureAwait(false);
                                }
                                break;
                            case "THREAD_LIST_SYNC":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (THREAD_LIST_SYNC)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Gateway.ThreadListSyncEvent>(_serializer);

                                    var guild = State.GetGuild(data.GuildId);

                                    if(guild == null)
                                    {
                                        await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                        return;
                                    }

                                    foreach(var thread in data.Threads)
                                    {
                                        var entity = guild.ThreadChannels.FirstOrDefault(x => x.Id == thread.Id);

                                        if(entity == null)
                                        {
                                            entity = (SocketThreadChannel)guild.AddChannel(this.State, thread);
                                        }
                                        else
                                        {
                                            entity.Update(this.State, thread);
                                        }

                                        foreach(var member in data.Members.Where(x => x.Id.Value == entity.Id))
                                        {
                                            var guildMember = guild.GetUser(member.Id.Value);

                                            entity.AddOrUpdateThreadMember(member, guildMember);
                                        }
                                    }
                                }
                                break;
                            case "THREAD_MEMBER_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (THREAD_MEMBER_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<ThreadMember>(_serializer);

                                    var thread = (SocketThreadChannel)State.GetChannel(data.Id.Value);

                                    if (thread == null)
                                    {
                                        await UnknownChannelAsync(type, data.Id.Value);
                                        return;
                                    }

                                    thread.AddOrUpdateThreadMember(data, thread.Guild.CurrentUser);
                                }

                                break;
                            case "THREAD_MEMBERS_UPDATE": 
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (THREAD_MEMBERS_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<ThreadMembersUpdated>(_serializer);

                                    var guild = State.GetGuild(data.GuildId);

                                    if (guild == null)
                                    {
                                        await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                        return;
                                    }

                                    var thread = (SocketThreadChannel)guild.GetChannel(data.Id);

                                    if(thread == null)
                                    {
                                        await UnknownChannelAsync(type, data.Id);
                                        return;
                                    }

                                    IReadOnlyCollection<SocketThreadUser> leftUsers = null;
                                    IReadOnlyCollection<SocketThreadUser> joinUsers = null;


                                    if (data.RemovedMemberIds.IsSpecified)
                                    {
                                        leftUsers = thread.RemoveUsers(data.RemovedMemberIds.Value);
                                    }

                                    if (data.AddedMembers.IsSpecified)
                                    {
                                        List<SocketThreadUser> newThreadMembers = new List<SocketThreadUser>();
                                        foreach(var threadMember in data.AddedMembers.Value)
                                        {
                                            SocketGuildUser guildMember;

                                            if (threadMember.Member.IsSpecified)
                                            {
                                                guildMember = guild.AddOrUpdateUser(threadMember.Member.Value);
                                            }
                                            else
                                            {
                                                guildMember = guild.GetUser(threadMember.UserId.Value);
                                            }

                                            newThreadMembers.Add(thread.AddOrUpdateThreadMember(threadMember, guildMember));
                                        }

                                        if (newThreadMembers.Any())
                                            joinUsers = newThreadMembers.ToImmutableArray();
                                    }

                                    if (leftUsers != null)
                                    {
                                        foreach(var threadUser in leftUsers)
                                        {
                                            await TimedInvokeAsync(_threadMemberLeft, nameof(ThreadMemberLeft), threadUser).ConfigureAwait(false);
                                        }
                                    }

                                    if(joinUsers != null)
                                    {
                                        foreach(var threadUser in joinUsers)
                                        {
                                            await TimedInvokeAsync(_threadMemberJoined, nameof(ThreadMemberJoined), threadUser).ConfigureAwait(false);
                                        }
                                    }
                                }

                                break;

                            case "STAGE_INSTANCE_CREATE" or "STAGE_INSTANCE_UPDATE" or "STAGE_INSTANCE_DELETE":
                                {
                                    await _gatewayLogger.DebugAsync($"Received Dispatch ({type})").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<StageInstance>(_serializer);

                                    var guild = State.GetGuild(data.GuildId);

                                    if(guild == null)
                                    {
                                        await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                        return;
                                    }

                                    var stageChannel = guild.GetStageChannel(data.ChannelId);

                                    if(stageChannel == null)
                                    {
                                        await UnknownChannelAsync(type, data.ChannelId).ConfigureAwait(false);
                                        return;
                                    }

                                    SocketStageChannel before = type == "STAGE_INSTANCE_UPDATE" ? stageChannel.Clone() : null;

                                    stageChannel.Update(data, type == "STAGE_INSTANCE_CREATE" ? true : type == "STAGE_INSTANCE_DELETE" ? false : false);

                                    switch (type)
                                    {
                                        case "STAGE_INSTANCE_CREATE":
                                            await TimedInvokeAsync(_stageStarted, nameof(StageStarted), stageChannel);
                                            return;
                                        case "STAGE_INSTANCE_DELETE":
                                            await TimedInvokeAsync(_stageEnded, nameof(StageEnded), stageChannel);
                                            return;
                                        case "STAGE_INSTANCE_UPDATE":
                                            await TimedInvokeAsync(_stageUpdated, nameof(StageUpdated), before, stageChannel);
                                            return;
                                    }
                                }
                                break;
                            case "GUILD_STICKERS_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync($"Received Dispatch (GUILD_STICKERS_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildStickerUpdateEvent>(_serializer);

                                    var guild = State.GetGuild(data.GuildId);

                                    if (guild == null)
                                    {
                                        await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                        return;
                                    }

                                    var newStickers = data.Stickers.Where(x => !guild.Stickers.Any(y => y.Id == x.Id));
                                    var deletedStickers = guild.Stickers.Where(x => !data.Stickers.Any(y => y.Id == x.Id));
                                    var updatedStickers = data.Stickers.Select(x =>
                                    {
                                        var s = guild.Stickers.FirstOrDefault(y => y.Id == x.Id);
                                        if (s == null)
                                            return null;

                                        var e = s.Equals(x);
                                        if (!e)
                                        {
                                            return (s, x) as (SocketCustomSticker Entity, API.Sticker Model)?;
                                        }
                                        else
                                        {
                                            return null;
                                        }
                                    }).Where(x => x.HasValue).Select(x => x.Value).ToArray();

                                    foreach(var model in newStickers)
                                    {
                                        var entity = guild.AddSticker(model);
                                        await TimedInvokeAsync(_guildStickerCreated, nameof(GuildStickerCreated), entity);
                                    }
                                    foreach(var sticker in deletedStickers)
                                    {
                                        var entity = guild.RemoveSticker(sticker.Id);
                                        await TimedInvokeAsync(_guildStickerDeleted, nameof(GuildStickerDeleted), entity);
                                    }
                                    foreach(var entityModelPair in updatedStickers)
                                    {
                                        var before = entityModelPair.Entity.Clone();

                                        entityModelPair.Entity.Update(entityModelPair.Model);

                                        await TimedInvokeAsync(_guildStickerUpdated, nameof(GuildStickerUpdated), before, entityModelPair.Entity);
                                    }
                                }
                                break;
                            #endregion

                            #region Ignored (User only)
                            case "CHANNEL_PINS_ACK":
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (CHANNEL_PINS_ACK)").ConfigureAwait(false);
                                break;
                            case "CHANNEL_PINS_UPDATE":
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (CHANNEL_PINS_UPDATE)").ConfigureAwait(false);
                                break;
                            case "GUILD_INTEGRATIONS_UPDATE":
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (GUILD_INTEGRATIONS_UPDATE)").ConfigureAwait(false);
                                break;
                            case "MESSAGE_ACK":
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (MESSAGE_ACK)").ConfigureAwait(false);
                                break;
                            case "PRESENCES_REPLACE":
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (PRESENCES_REPLACE)").ConfigureAwait(false);
                                break;
                            case "USER_SETTINGS_UPDATE":
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (USER_SETTINGS_UPDATE)").ConfigureAwait(false);
                                break;
                            case "WEBHOOKS_UPDATE":
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (WEBHOOKS_UPDATE)").ConfigureAwait(false);
                                break;
                            #endregion

                            #region Others
                            default:
                                await _gatewayLogger.WarningAsync($"Unknown Dispatch ({type})").ConfigureAwait(false);
                                break;
                                #endregion
                        }
                        break;
                    default:
                        await _gatewayLogger.WarningAsync($"Unknown OpCode ({opCode})").ConfigureAwait(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                await _gatewayLogger.ErrorAsync($"Error handling {opCode}{(type != null ? $" ({type})" : "")}", ex).ConfigureAwait(false);
                Console.WriteLine(ex);
            }
        }
        #endregion

        private async Task RunHeartbeatAsync(int intervalMillis, CancellationToken cancelToken)
        {
            try
            {
                await _gatewayLogger.DebugAsync("Heartbeat Started").ConfigureAwait(false);
                while (!cancelToken.IsCancellationRequested)
                {
                    int now = Environment.TickCount;

                    //Did server respond to our last heartbeat, or are we still receiving messages (long load?)
                    if (_heartbeatTimes.Count != 0 && (now - _lastMessageTime) > intervalMillis)
                    {
                        if (ConnectionState == ConnectionState.Connected && (_guildDownloadTask?.IsCompleted ?? true))
                        {
                            _connection.Error(new GatewayReconnectException("Server missed last heartbeat"));
                            return;
                        }
                    }

                    _heartbeatTimes.Enqueue(now);
                    try
                    {
                        await ApiClient.SendHeartbeatAsync(_lastSeq).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await _gatewayLogger.WarningAsync("Heartbeat Errored", ex).ConfigureAwait(false);
                    }

                    await Task.Delay(intervalMillis, cancelToken).ConfigureAwait(false);
                }
                await _gatewayLogger.DebugAsync("Heartbeat Stopped").ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await _gatewayLogger.DebugAsync("Heartbeat Stopped").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _gatewayLogger.ErrorAsync("Heartbeat Errored", ex).ConfigureAwait(false);
            }
        }
        /*public async Task WaitForGuildsAsync()
        {
            var downloadTask = _guildDownloadTask;
            if (downloadTask != null)
                await _guildDownloadTask.ConfigureAwait(false);
        }*/
        private async Task WaitForGuildsAsync(CancellationToken cancelToken, Logger logger)
        {
            //Wait for GUILD_AVAILABLEs
            try
            {
                await logger.DebugAsync("GuildDownloader Started").ConfigureAwait(false);
                while ((_unavailableGuildCount != 0) && (Environment.TickCount - _lastGuildAvailableTime < BaseConfig.MaxWaitBetweenGuildAvailablesBeforeReady))
                    await Task.Delay(500, cancelToken).ConfigureAwait(false);
                await logger.DebugAsync("GuildDownloader Stopped").ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await logger.DebugAsync("GuildDownloader Stopped").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync("GuildDownloader Errored", ex).ConfigureAwait(false);
            }
        }
        private async Task SyncGuildsAsync()
        {
            var guildIds = Guilds.Where(x => !x.IsSynced).Select(x => x.Id).ToImmutableArray();
            if (guildIds.Length > 0)
                await ApiClient.SendGuildSyncAsync(guildIds).ConfigureAwait(false);
        }

        internal SocketGuild AddGuild(ExtendedGuild model, ClientState state)
        {
            var guild = SocketGuild.Create(this, state, model);
            state.AddGuild(guild);
            if (model.Large)
                _largeGuilds.Enqueue(model.Id);
            return guild;
        }
        internal SocketGuild RemoveGuild(ulong id)
            => State.RemoveGuild(id);

        /// <exception cref="InvalidOperationException">Unexpected channel type is created.</exception>
        internal ISocketPrivateChannel AddPrivateChannel(API.Channel model, ClientState state)
        {
            var channel = SocketChannel.CreatePrivate(this, state, model);
            state.AddChannel(channel as SocketChannel);
            return channel;
        }
        internal SocketDMChannel CreateDMChannel(ulong channelId, API.User model, ClientState state)
        {
            return SocketDMChannel.Create(this, state, channelId, model);
        }
        internal SocketDMChannel CreateDMChannel(ulong channelId, SocketUser user, ClientState state)
        {
            return new SocketDMChannel(this, channelId, user);
        }
        internal ISocketPrivateChannel RemovePrivateChannel(ulong id)
        {
            var channel = State.RemoveChannel(id) as ISocketPrivateChannel;
            if (channel != null)
            {
                foreach (var recipient in channel.Recipients)
                    recipient.GlobalUser.RemoveRef(this);
            }
            return channel;
        }
        internal void RemoveDMChannels()
        {
            var channels = State.DMChannels;
            State.PurgeDMChannels();
            foreach (var channel in channels)
                channel.Recipient.GlobalUser.RemoveRef(this);
        }

        private async Task GuildAvailableAsync(SocketGuild guild)
        {
            if (!guild.IsConnected)
            {
                guild.IsConnected = true;
                await TimedInvokeAsync(_guildAvailableEvent, nameof(GuildAvailable), guild).ConfigureAwait(false);
            }
        }
        private async Task GuildUnavailableAsync(SocketGuild guild)
        {
            if (guild.IsConnected)
            {
                guild.IsConnected = false;
                await TimedInvokeAsync(_guildUnavailableEvent, nameof(GuildUnavailable), guild).ConfigureAwait(false);
            }
        }

        private async Task TimedInvokeAsync(AsyncEvent<Func<Task>> eventHandler, string name)
        {
            if (eventHandler.HasSubscribers)
            {
                if (HandlerTimeout.HasValue)
                    await TimeoutWrap(name, eventHandler.InvokeAsync).ConfigureAwait(false);
                else
                    await eventHandler.InvokeAsync().ConfigureAwait(false);
            }
        }
        private async Task TimedInvokeAsync<T>(AsyncEvent<Func<T, Task>> eventHandler, string name, T arg)
        {
            if (eventHandler.HasSubscribers)
            {
                if (HandlerTimeout.HasValue)
                    await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg)).ConfigureAwait(false);
                else
                    await eventHandler.InvokeAsync(arg).ConfigureAwait(false);
            }
        }
        private async Task TimedInvokeAsync<T1, T2>(AsyncEvent<Func<T1, T2, Task>> eventHandler, string name, T1 arg1, T2 arg2)
        {
            if (eventHandler.HasSubscribers)
            {
                if (HandlerTimeout.HasValue)
                    await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2)).ConfigureAwait(false);
                else
                    await eventHandler.InvokeAsync(arg1, arg2).ConfigureAwait(false);
            }
        }
        private async Task TimedInvokeAsync<T1, T2, T3>(AsyncEvent<Func<T1, T2, T3, Task>> eventHandler, string name, T1 arg1, T2 arg2, T3 arg3)
        {
            if (eventHandler.HasSubscribers)
            {
                if (HandlerTimeout.HasValue)
                    await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2, arg3)).ConfigureAwait(false);
                else
                    await eventHandler.InvokeAsync(arg1, arg2, arg3).ConfigureAwait(false);
            }
        }
        private async Task TimedInvokeAsync<T1, T2, T3, T4>(AsyncEvent<Func<T1, T2, T3, T4, Task>> eventHandler, string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (eventHandler.HasSubscribers)
            {
                if (HandlerTimeout.HasValue)
                    await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2, arg3, arg4)).ConfigureAwait(false);
                else
                    await eventHandler.InvokeAsync(arg1, arg2, arg3, arg4).ConfigureAwait(false);
            }
        }
        private async Task TimedInvokeAsync<T1, T2, T3, T4, T5>(AsyncEvent<Func<T1, T2, T3, T4, T5, Task>> eventHandler, string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (eventHandler.HasSubscribers)
            {
                if (HandlerTimeout.HasValue)
                    await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2, arg3, arg4, arg5)).ConfigureAwait(false);
                else
                    await eventHandler.InvokeAsync(arg1, arg2, arg3, arg4, arg5).ConfigureAwait(false);
            }
        }
        private async Task TimeoutWrap(string name, Func<Task> action)
        {
            try
            {
                var timeoutTask = Task.Delay(HandlerTimeout.Value);
                var handlersTask = action();
                if (await Task.WhenAny(timeoutTask, handlersTask).ConfigureAwait(false) == timeoutTask)
                {
                    await _gatewayLogger.WarningAsync($"A {name} handler is blocking the gateway task.").ConfigureAwait(false);
                }
                await handlersTask.ConfigureAwait(false); //Ensure the handler completes
            }
            catch (Exception ex)
            {
                await _gatewayLogger.WarningAsync($"A {name} handler has thrown an unhandled exception.", ex).ConfigureAwait(false);
            }
        }

        private async Task UnknownGlobalUserAsync(string evnt, ulong userId)
        {
            string details = $"{evnt} User={userId}";
            await _gatewayLogger.WarningAsync($"Unknown User ({details}).").ConfigureAwait(false);
        }
        private async Task UnknownChannelUserAsync(string evnt, ulong userId, ulong channelId)
        {
            string details = $"{evnt} User={userId} Channel={channelId}";
            await _gatewayLogger.WarningAsync($"Unknown User ({details}).").ConfigureAwait(false);
        }
        private async Task UnknownGuildUserAsync(string evnt, ulong userId, ulong guildId)
        {
            string details = $"{evnt} User={userId} Guild={guildId}";
            await _gatewayLogger.WarningAsync($"Unknown User ({details}).").ConfigureAwait(false);
        }
        private async Task IncompleteGuildUserAsync(string evnt, ulong userId, ulong guildId)
        {
            string details = $"{evnt} User={userId} Guild={guildId}";
            await _gatewayLogger.DebugAsync($"User has not been downloaded ({details}).").ConfigureAwait(false);
        }
        private async Task UnknownChannelAsync(string evnt, ulong channelId)
        {
            string details = $"{evnt} Channel={channelId}";
            await _gatewayLogger.WarningAsync($"Unknown Channel ({details}).").ConfigureAwait(false);
        }
        private async Task UnknownChannelAsync(string evnt, ulong channelId, ulong guildId)
        {
            if (guildId == 0)
            {
                await UnknownChannelAsync(evnt, channelId).ConfigureAwait(false);
                return;
            }
            string details = $"{evnt} Channel={channelId} Guild={guildId}";
            await _gatewayLogger.WarningAsync($"Unknown Channel ({details}).").ConfigureAwait(false);
        }
        private async Task UnknownRoleAsync(string evnt, ulong roleId, ulong guildId)
        {
            string details = $"{evnt} Role={roleId} Guild={guildId}";
            await _gatewayLogger.WarningAsync($"Unknown Role ({details}).").ConfigureAwait(false);
        }
        private async Task UnknownGuildAsync(string evnt, ulong guildId)
        {
            string details = $"{evnt} Guild={guildId}";
            await _gatewayLogger.WarningAsync($"Unknown Guild ({details}).").ConfigureAwait(false);
        }
        private async Task UnsyncedGuildAsync(string evnt, ulong guildId)
        {
            string details = $"{evnt} Guild={guildId}";
            await _gatewayLogger.DebugAsync($"Unsynced Guild ({details}).").ConfigureAwait(false);
        }

        internal int GetAudioId() => _nextAudioId++;

        #region IDiscordClient
        /// <inheritdoc />
        async Task<IApplication> IDiscordClient.GetApplicationInfoAsync(RequestOptions options)
            => await GetApplicationInfoAsync().ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IChannel> IDiscordClient.GetChannelAsync(ulong id, CacheMode mode, RequestOptions options)
            => mode == CacheMode.AllowDownload ? await GetChannelAsync(id, options).ConfigureAwait(false) : GetChannel(id);
        /// <inheritdoc />
        Task<IReadOnlyCollection<IPrivateChannel>> IDiscordClient.GetPrivateChannelsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IPrivateChannel>>(PrivateChannels);
        /// <inheritdoc />
        Task<IReadOnlyCollection<IDMChannel>> IDiscordClient.GetDMChannelsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IDMChannel>>(DMChannels);
        /// <inheritdoc />
        Task<IReadOnlyCollection<IGroupChannel>> IDiscordClient.GetGroupChannelsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IGroupChannel>>(GroupChannels);

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IConnection>> IDiscordClient.GetConnectionsAsync(RequestOptions options)
            => await GetConnectionsAsync().ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IInvite> IDiscordClient.GetInviteAsync(string inviteId, RequestOptions options)
            => await GetInviteAsync(inviteId, options).ConfigureAwait(false);

        /// <inheritdoc />
        Task<IGuild> IDiscordClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuild>(GetGuild(id));
        /// <inheritdoc />
        Task<IReadOnlyCollection<IGuild>> IDiscordClient.GetGuildsAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IReadOnlyCollection<IGuild>>(Guilds);
        /// <inheritdoc />
        async Task<IGuild> IDiscordClient.CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon, RequestOptions options)
            => await CreateGuildAsync(name, region, jpegIcon).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IUser> IDiscordClient.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => mode == CacheMode.AllowDownload ? await GetUserAsync(id, options).ConfigureAwait(false) : GetUser(id);
        /// <inheritdoc />
        Task<IUser> IDiscordClient.GetUserAsync(string username, string discriminator, RequestOptions options)
            => Task.FromResult<IUser>(GetUser(username, discriminator));

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IVoiceRegion>> IDiscordClient.GetVoiceRegionsAsync(RequestOptions options)
            => await GetVoiceRegionsAsync(options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IVoiceRegion> IDiscordClient.GetVoiceRegionAsync(string id, RequestOptions options)
            => await GetVoiceRegionAsync(id, options).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IApplicationCommand> IDiscordClient.GetGlobalApplicationCommandAsync(ulong id, RequestOptions options)
            => await GetGlobalApplicationCommandAsync(id, options);
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IApplicationCommand>> IDiscordClient.GetGlobalApplicationCommandsAsync(RequestOptions options)
            => await GetGlobalApplicationCommandsAsync(options);

        /// <inheritdoc />
        async Task IDiscordClient.StartAsync()
            => await StartAsync().ConfigureAwait(false);
        /// <inheritdoc />
        async Task IDiscordClient.StopAsync()
            => await StopAsync().ConfigureAwait(false);
        #endregion
    }
}
