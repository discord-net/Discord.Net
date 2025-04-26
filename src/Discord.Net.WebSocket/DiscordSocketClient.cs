using Discord.API;
using Discord.API.Gateway;
using Discord.Logging;
using Discord.Net.Converters;
using Discord.Net.Udp;
using Discord.Net.WebSockets;
using Discord.Rest;
using Discord.Utils;

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
        private SocketSelfUser _previousSessionUser;

        /// <summary>
        ///     Provides access to a REST-only client with a shared state from this client.
        /// </summary>
        public override DiscordSocketRestClient Rest { get; }
        /// <summary> Gets the shard of this client. </summary>
        public int ShardId { get; }
        /// <inheritdoc />
        public override ConnectionState ConnectionState => _connection.State;
        /// <inheritdoc />
        public override int Latency { get; protected set; }
        /// <inheritdoc />
        public override UserStatus Status { get => _status ?? UserStatus.Online; protected set => _status = value; }
        private UserStatus? _status;
        /// <inheritdoc />
        public override IActivity Activity { get => _activity.GetValueOrDefault(); protected set => _activity = Optional.Create(value); }
        private Optional<IActivity> _activity;
        #endregion

        // From DiscordSocketConfig
        internal int TotalShards { get; private set; }
        internal int MessageCacheSize { get; private set; }
        internal int LargeThreshold { get; private set; }
        internal ClientState State { get; private set; }
        internal UdpSocketProvider UdpSocketProvider { get; private set; }
        internal WebSocketProvider WebSocketProvider { get; private set; }
        internal bool AlwaysDownloadUsers { get; private set; }
        internal int? HandlerTimeout { get; private set; }
        internal bool AlwaysDownloadDefaultStickers { get; private set; }
        internal bool AlwaysResolveStickers { get; private set; }
        internal bool LogGatewayIntentWarnings { get; private set; }
        internal bool SuppressUnknownDispatchWarnings { get; private set; }
        internal bool IncludeRawPayloadOnGatewayErrors { get; private set; }
        internal int AuditLogCacheSize { get; private set; }

        internal new DiscordSocketApiClient ApiClient => base.ApiClient;
        /// <inheritdoc />
        public override IReadOnlyCollection<SocketGuild> Guilds => State.Guilds;
        /// <inheritdoc/>
        public override IReadOnlyCollection<StickerPack<SocketSticker>> DefaultStickerPacks
        {
            get
            {
                if (_shardedClient != null)
                    return _shardedClient.DefaultStickerPacks;
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
            AlwaysDownloadDefaultStickers = config.AlwaysDownloadDefaultStickers;
            AlwaysResolveStickers = config.AlwaysResolveStickers;
            LogGatewayIntentWarnings = config.LogGatewayIntentWarnings;
            SuppressUnknownDispatchWarnings = config.SuppressUnknownDispatchWarnings;
            IncludeRawPayloadOnGatewayErrors = config.IncludeRawPayloadOnGatewayErrors;
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
                return Task.CompletedTask;
            };

            _largeGuilds = new ConcurrentQueue<ulong>();
            AuditLogCacheSize = config.AuditLogCacheSize;
        }
        private static API.DiscordSocketApiClient CreateApiClient(DiscordSocketConfig config)
            => new DiscordSocketApiClient(config.RestClientProvider, config.WebSocketProvider, DiscordRestConfig.UserAgent, config.GatewayHost,
                useSystemClock: config.UseSystemClock, defaultRatelimitCallback: config.DefaultRatelimitCallback);
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


        internal override async ValueTask DisposeAsync(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    await StopAsync().ConfigureAwait(false);

                    if (!(ApiClient is null))
                        await ApiClient.DisposeAsync().ConfigureAwait(false);

                    _stateLock?.Dispose();
                }
                _isDisposed = true;
            }

            await base.DisposeAsync(disposing).ConfigureAwait(false);
        }

        /// <inheritdoc />
        internal override async Task OnLoginAsync(TokenType tokenType, string token)
        {
            if (_shardedClient == null && _defaultStickers.Length == 0 && AlwaysDownloadDefaultStickers)
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
        public override Task StartAsync()
            => _connection.StartAsync();

        /// <inheritdoc />
        public override Task StopAsync()
            => _connection.StopAsync();

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

            // Log warnings on ready event
            if (LogGatewayIntentWarnings)
                await LogGatewayIntentsWarning().ConfigureAwait(false);
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

            while (_heartbeatTimes.TryDequeue(out _))
            { }
            _lastMessageTime = 0;

            await _gatewayLogger.DebugAsync("Waiting for guild downloader").ConfigureAwait(false);
            var guildDownloadTask = _guildDownloadTask;
            if (guildDownloadTask != null)
                await guildDownloadTask.ConfigureAwait(false);
            _guildDownloadTask = null;

            //Clear large guild queue
            await _gatewayLogger.DebugAsync("Clearing large guild queue").ConfigureAwait(false);
            while (_largeGuilds.TryDequeue(out _))
            { }

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
            => _applicationInfo ??= await ClientHelper.GetApplicationInfoAsync(this, options ?? RequestOptions.Default).ConfigureAwait(false);

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
        ///     with the snowflake identifier; <see langword="null" /> when the channel cannot be found.
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
        ///     the snowflake identifier; <see langword="null" /> if the user is not found.
        /// </returns>
        public async ValueTask<IUser> GetUserAsync(ulong id, RequestOptions options = null)
            => await ((IDiscordClient)this).GetUserAsync(id, CacheMode.AllowDownload, options).ConfigureAwait(false);
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
        public override SocketUser GetUser(string username, string discriminator = null)
            => State.Users.FirstOrDefault(x => (discriminator is null || x.Discriminator == discriminator) && x.Username == username);

        /// <inheritdoc cref="IDiscordClient.CreateTestEntitlementAsync"/>
        public Task<RestEntitlement> CreateTestEntitlementAsync(ulong skuId, ulong ownerId, SubscriptionOwnerType ownerType, RequestOptions options = null)
            => ClientHelper.CreateTestEntitlementAsync(this, skuId, ownerId, ownerType, options);

        /// <inheritdoc />
        public Task DeleteTestEntitlementAsync(ulong entitlementId, RequestOptions options = null)
            => ApiClient.DeleteEntitlementAsync(entitlementId, options);

        /// <inheritdoc cref="IDiscordClient.GetEntitlementsAsync"/>
        public IAsyncEnumerable<IReadOnlyCollection<IEntitlement>> GetEntitlementsAsync(int? limit = 100,
            ulong? afterId = null, ulong? beforeId = null, bool excludeEnded = false, ulong? guildId = null, ulong? userId = null,
            ulong[] skuIds = null, RequestOptions options = null, bool? excludeDeleted = null)
            => ClientHelper.ListEntitlementsAsync(this, limit, afterId, beforeId, excludeEnded, guildId, userId, skuIds, excludeDeleted, options);

        /// <inheritdoc />
        public Task<IReadOnlyCollection<SKU>> GetSKUsAsync(RequestOptions options = null)
            => ClientHelper.ListSKUsAsync(this, options);

        /// <inheritdoc />
        public Task ConsumeEntitlementAsync(ulong entitlementId, RequestOptions options = null)
            => ClientHelper.ConsumeEntitlementAsync(this, entitlementId, options);

        /// <inheritdoc cref="IDiscordClient.GetSKUSubscriptionAsync" />
        public Task<RestSubscription> GetSKUSubscriptionAsync(ulong skuId, ulong subscriptionId, RequestOptions options = null)
            => ClientHelper.GetSKUSubscriptionAsync(this, skuId, subscriptionId, options);

        /// <inheritdoc cref="IDiscordClient.GetSKUSubscriptionsAsync" />
        public IAsyncEnumerable<IReadOnlyCollection<RestSubscription>> GetSKUSubscriptionsAsync(ulong skuId, int limit = 100, ulong? afterId = null,
            ulong? beforeId = null, ulong? userId = null, RequestOptions options = null)
            => ClientHelper.ListSubscriptionsAsync(this, skuId, limit, afterId, beforeId, userId, options);

        /// <inheritdoc />
        public Task<Emote> GetApplicationEmoteAsync(ulong emoteId, RequestOptions options = null)
            => ClientHelper.GetApplicationEmojiAsync(this, emoteId, options);

        /// <inheritdoc />
        public Task<IReadOnlyCollection<Emote>> GetApplicationEmotesAsync(RequestOptions options = null)
            => ClientHelper.GetApplicationEmojisAsync(this, options);

        /// <inheritdoc />
        public Task<Emote> ModifyApplicationEmoteAsync(ulong emoteId, Action<ApplicationEmoteProperties> args, RequestOptions options = null)
            => ClientHelper.ModifyApplicationEmojiAsync(this, emoteId, args, options);

        /// <inheritdoc />
        public Task<Emote> CreateApplicationEmoteAsync(string name, Image image, RequestOptions options = null)
            => ClientHelper.CreateApplicationEmojiAsync(this, name, image, options);

        /// <inheritdoc />
        public Task DeleteApplicationEmoteAsync(ulong emoteId, RequestOptions options = null)
            => ClientHelper.DeleteApplicationEmojiAsync(this, emoteId, options);

        /// <summary>
        ///     Gets entitlements from cache.
        /// </summary>
        public IReadOnlyCollection<SocketEntitlement> Entitlements => State.Entitlements;

        /// <summary>
        ///     Gets subscriptions from cache.
        /// </summary>
        public IReadOnlyCollection<SocketSubscription> Subscription => State.Subscriptions;

        /// <summary>
        ///     Gets an entitlement from cache. <see langword="null"/> if not found.
        /// </summary>
        public SocketEntitlement GetEntitlement(ulong id)
            => State.GetEntitlement(id);

        /// <summary>
        ///     Gets a subscription from cache. <see langword="null"/> if not found.
        /// </summary>
        public SocketSubscription GetSubscription(ulong id)
            => State.GetSubscription(id);

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
        /// <param name="withLocalizations">Whether to include full localization dictionaries in the returned objects, instead of the name localized and description localized fields.</param>
        /// <param name="locale">The target locale of the localized name and description fields. Sets <c>X-Discord-Locale</c> header, which takes precedence over <c>Accept-Language</c>.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of global
        ///     application commands.
        /// </returns>
        public async Task<IReadOnlyCollection<SocketApplicationCommand>> GetGlobalApplicationCommandsAsync(bool withLocalizations = false, string locale = null, RequestOptions options = null)
        {
            var commands = (await ApiClient.GetGlobalApplicationCommandsAsync(withLocalizations, locale, options)).Select(x => SocketApplicationCommand.Create(this, x));

            foreach (var command in commands)
            {
                State.AddCommand(command);
            }

            return commands.ToImmutableArray();
        }

        public async Task<SocketApplicationCommand> CreateGlobalApplicationCommandAsync(ApplicationCommandProperties properties, RequestOptions options = null)
        {
            var model = await InteractionHelper.CreateGlobalCommandAsync(this, properties, options).ConfigureAwait(false);

            var entity = State.GetOrAddCommand(model.Id, (id) => SocketApplicationCommand.Create(this, model));

            //Update it in case it was cached
            entity.Update(model);

            return entity;
        }
        public async Task<IReadOnlyCollection<SocketApplicationCommand>> BulkOverwriteGlobalApplicationCommandsAsync(
            ApplicationCommandProperties[] properties, RequestOptions options = null)
        {
            var models = await InteractionHelper.BulkOverwriteGlobalCommandsAsync(this, properties, options);

            var entities = models.Select(x => SocketApplicationCommand.Create(this, x));

            //Purge our previous commands
            State.PurgeCommands(x => x.IsGlobalCommand);

            foreach (var entity in entities)
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

            foreach (var guild in Guilds)
            {
                sticker = await guild.GetStickerAsync(id, CacheMode.CacheOnly).ConfigureAwait(false);

                if (sticker != null)
                    return sticker;
            }

            if (mode == CacheMode.CacheOnly)
                return null;

            var model = await ApiClient.GetStickerAsync(id, options).ConfigureAwait(false);

            if (model == null)
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
        public override Task DownloadUsersAsync(IEnumerable<IGuild> guilds)
        {
            if (ConnectionState == ConnectionState.Connected)
            {
                EnsureGatewayIntent(GatewayIntents.GuildMembers);
                //Race condition leads to guilds being requested twice, probably okay
                return ProcessUserDownloadsAsync(guilds.Select(x => GetGuild(x.Id)).Where(x => x != null));
            }
            return Task.CompletedTask;
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
        public override Task SetStatusAsync(UserStatus status)
        {
            Status = status;
            if (status == UserStatus.AFK)
                _statusSince = DateTimeOffset.UtcNow;
            else
                _statusSince = null;
            return SendStatusAsync();
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
        public override Task SetGameAsync(string name, string streamUrl = null, ActivityType type = ActivityType.Playing)
        {
            if (!string.IsNullOrEmpty(streamUrl))
                Activity = new StreamingGame(name, streamUrl);
            else if (!string.IsNullOrEmpty(name))
            {
                if (type is ActivityType.CustomStatus)
                    Activity = new CustomStatusGame(name);
                else
                    Activity = new Game(name, type);
            }
            else
                Activity = null;
            return SendStatusAsync();
        }

        /// <inheritdoc />
        public override Task SetActivityAsync(IActivity activity)
        {
            Activity = activity;
            return SendStatusAsync();
        }

        /// <inheritdoc />
        public override Task SetCustomStatusAsync(string status)
        {
            var statusGame = new CustomStatusGame(status);
            return SetActivityAsync(statusGame);
        }

        private Task SendStatusAsync()
        {
            if (CurrentUser == null)
                return Task.CompletedTask;
            var activities = _activity.IsSpecified
                ? ImmutableList.Create(_activity.Value)
                : null;
            CurrentUser.Presence = new SocketPresence(Status, null, activities);

            var presence = BuildCurrentStatus() ?? (UserStatus.Online, false, null, null);

            return ApiClient.SendPresenceUpdateAsync(
                status: presence.Item1,
                isAFK: presence.Item2,
                since: presence.Item3,
                game: presence.Item4);
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
                if (Activity is CustomStatusGame customStatus)
                    gameModel.State = customStatus.State;
                game = gameModel;
            }
            else if (activity.IsSpecified)
                game = null;

            return (status ?? UserStatus.Online,
                    status == UserStatus.AFK,
                    statusSince != null ? _statusSince.Value.ToUnixTimeMilliseconds() : (long?)null,
                    game);
        }

        private async Task LogGatewayIntentsWarning()
        {
            if (_gatewayIntents.HasFlag(GatewayIntents.GuildPresences) &&
                (_shardedClient is null && !_presenceUpdated.HasSubscribers ||
               (_shardedClient is not null && !_shardedClient._presenceUpdated.HasSubscribers)))
            {
                await _gatewayLogger.WarningAsync("You're using the GuildPresences intent without listening to the PresenceUpdate event, consider removing the intent from your config.").ConfigureAwait(false);
            }

            if (!_gatewayIntents.HasFlag(GatewayIntents.GuildPresences) &&
               ((_shardedClient is null && _presenceUpdated.HasSubscribers) ||
                (_shardedClient is not null && _shardedClient._presenceUpdated.HasSubscribers)))
            {
                await _gatewayLogger.WarningAsync("You're using the PresenceUpdate event without specifying the GuildPresences intent. Discord wont send this event to your client without the intent set in your config.").ConfigureAwait(false);
            }

            bool hasGuildScheduledEventsSubscribers =
                _guildScheduledEventCancelled.HasSubscribers ||
                 _guildScheduledEventUserRemove.HasSubscribers ||
                 _guildScheduledEventCompleted.HasSubscribers ||
                 _guildScheduledEventCreated.HasSubscribers ||
                 _guildScheduledEventStarted.HasSubscribers ||
                 _guildScheduledEventUpdated.HasSubscribers ||
                 _guildScheduledEventUserAdd.HasSubscribers;

            bool shardedClientHasGuildScheduledEventsSubscribers =
                 _shardedClient is not null &&
                 (_shardedClient._guildScheduledEventCancelled.HasSubscribers ||
                 _shardedClient._guildScheduledEventUserRemove.HasSubscribers ||
                 _shardedClient._guildScheduledEventCompleted.HasSubscribers ||
                 _shardedClient._guildScheduledEventCreated.HasSubscribers ||
                 _shardedClient._guildScheduledEventStarted.HasSubscribers ||
                 _shardedClient._guildScheduledEventUpdated.HasSubscribers ||
                 _shardedClient._guildScheduledEventUserAdd.HasSubscribers);

            if (_gatewayIntents.HasFlag(GatewayIntents.GuildScheduledEvents) &&
                ((_shardedClient is null && !hasGuildScheduledEventsSubscribers) ||
                 (_shardedClient is not null && !shardedClientHasGuildScheduledEventsSubscribers)))
            {
                await _gatewayLogger.WarningAsync("You're using the GuildScheduledEvents gateway intent without listening to any events related to that intent, consider removing the intent from your config.").ConfigureAwait(false);
            }

            if (!_gatewayIntents.HasFlag(GatewayIntents.GuildScheduledEvents) &&
               ((_shardedClient is null && hasGuildScheduledEventsSubscribers) ||
                (_shardedClient is not null && shardedClientHasGuildScheduledEventsSubscribers)))
            {
                await _gatewayLogger.WarningAsync("You're using events related to the GuildScheduledEvents gateway intent without specifying the intent. Discord wont send this event to your client without the intent set in your config.").ConfigureAwait(false);
            }

            bool hasInviteEventSubscribers =
                _inviteCreatedEvent.HasSubscribers ||
                _inviteDeletedEvent.HasSubscribers;

            bool shardedClientHasInviteEventSubscribers =
                _shardedClient is not null &&
                (_shardedClient._inviteCreatedEvent.HasSubscribers ||
                 _shardedClient._inviteDeletedEvent.HasSubscribers);

            if (_gatewayIntents.HasFlag(GatewayIntents.GuildInvites) &&
                ((_shardedClient is null && !hasInviteEventSubscribers) ||
                 (_shardedClient is not null && !shardedClientHasInviteEventSubscribers)))
            {
                await _gatewayLogger.WarningAsync("You're using the GuildInvites gateway intent without listening to any events related to that intent, consider removing the intent from your config.").ConfigureAwait(false);
            }

            if (!_gatewayIntents.HasFlag(GatewayIntents.GuildInvites) &&
                ((_shardedClient is null && hasInviteEventSubscribers) ||
                (_shardedClient is not null && shardedClientHasInviteEventSubscribers)))
            {
                await _gatewayLogger.WarningAsync("You're using events related to the GuildInvites gateway intent without specifying the intent. Discord wont send this event to your client without the intent set in your config.").ConfigureAwait(false);
            }
        }

        private async Task RunHeartbeatAsync(int intervalMillis, CancellationToken cancelToken)
        {
            int delayInterval = (int)(intervalMillis * DiscordConfig.HeartbeatIntervalFactor);

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

                    int delay = Math.Max(0, delayInterval - Latency);
                    await Task.Delay(delay, cancelToken).ConfigureAwait(false);
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

        private Task SyncGuildsAsync()
        {
            var guildIds = Guilds.Where(x => !x.IsSynced).Select(x => x.Id).ToImmutableArray();
            if (guildIds.Length > 0)
                return ApiClient.SendGuildSyncAsync(guildIds);

            return Task.CompletedTask;
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

        internal void EnsureGatewayIntent(GatewayIntents intents)
        {
            if (!_gatewayIntents.HasFlag(intents))
            {
                var vals = Enum.GetValues(typeof(GatewayIntents)).Cast<GatewayIntents>();

                var missingValues = vals.Where(x => intents.HasFlag(x) && !_gatewayIntents.HasFlag(x));

                throw new InvalidOperationException($"Missing required gateway intent{(missingValues.Count() > 1 ? "s" : "")} {string.Join(", ", missingValues.Select(x => x.ToString()))} in order to execute this operation.");
            }
        }

        internal bool HasGatewayIntent(GatewayIntents intents)
            => _gatewayIntents.HasFlag(intents);

        private Task GuildAvailableAsync(SocketGuild guild)
        {
            if (!guild.IsConnected)
            {
                guild.IsConnected = true;
                return TimedInvokeAsync(_guildAvailableEvent, nameof(GuildAvailable), guild);
            }

            return Task.CompletedTask;
        }

        private Task GuildUnavailableAsync(SocketGuild guild)
        {
            if (guild.IsConnected)
            {
                guild.IsConnected = false;
                return TimedInvokeAsync(_guildUnavailableEvent, nameof(GuildUnavailable), guild);
            }

            return Task.CompletedTask;
        }

        private Task TimedInvokeAsync(AsyncEvent<Func<Task>> eventHandler, string name)
        {
            if (eventHandler.HasSubscribers)
            {
                if (HandlerTimeout.HasValue)
                    return TimeoutWrap(name, eventHandler.InvokeAsync);
                else
                    return eventHandler.InvokeAsync();
            }

            return Task.CompletedTask;
        }

        private Task TimedInvokeAsync<T>(AsyncEvent<Func<T, Task>> eventHandler, string name, T arg)
        {
            if (eventHandler.HasSubscribers)
            {
                if (HandlerTimeout.HasValue)
                    return TimeoutWrap(name, () => eventHandler.InvokeAsync(arg));
                else
                    return eventHandler.InvokeAsync(arg);
            }

            return Task.CompletedTask;
        }

        private Task TimedInvokeAsync<T1, T2>(AsyncEvent<Func<T1, T2, Task>> eventHandler, string name, T1 arg1, T2 arg2)
        {
            if (eventHandler.HasSubscribers)
            {
                if (HandlerTimeout.HasValue)
                    return TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2));
                else
                    return eventHandler.InvokeAsync(arg1, arg2);
            }

            return Task.CompletedTask;
        }

        private Task TimedInvokeAsync<T1, T2, T3>(AsyncEvent<Func<T1, T2, T3, Task>> eventHandler, string name, T1 arg1, T2 arg2, T3 arg3)
        {
            if (eventHandler.HasSubscribers)
            {
                if (HandlerTimeout.HasValue)
                    return TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2, arg3));
                else
                    return eventHandler.InvokeAsync(arg1, arg2, arg3);
            }

            return Task.CompletedTask;
        }

        private Task TimedInvokeAsync<T1, T2, T3, T4>(AsyncEvent<Func<T1, T2, T3, T4, Task>> eventHandler, string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (eventHandler.HasSubscribers)
            {
                if (HandlerTimeout.HasValue)
                    return TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2, arg3, arg4));
                else
                    return eventHandler.InvokeAsync(arg1, arg2, arg3, arg4);
            }

            return Task.CompletedTask;
        }

        private Task TimedInvokeAsync<T1, T2, T3, T4, T5>(AsyncEvent<Func<T1, T2, T3, T4, T5, Task>> eventHandler, string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (eventHandler.HasSubscribers)
            {
                if (HandlerTimeout.HasValue)
                    return TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2, arg3, arg4, arg5));
                else
                    return eventHandler.InvokeAsync(arg1, arg2, arg3, arg4, arg5);
            }

            return Task.CompletedTask;
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

        private Task UnknownGlobalUserAsync(string evnt, ulong userId)
        {
            string details = $"{evnt} User={userId}";
            return _gatewayLogger.WarningAsync($"Unknown User ({details}).");
        }

        private Task UnknownChannelUserAsync(string evnt, ulong userId, ulong channelId)
        {
            string details = $"{evnt} User={userId} Channel={channelId}";
            return _gatewayLogger.WarningAsync($"Unknown User ({details}).");
        }

        private Task UnknownGuildUserAsync(string evnt, ulong userId, ulong guildId)
        {
            string details = $"{evnt} User={userId} Guild={guildId}";
            return _gatewayLogger.WarningAsync($"Unknown User ({details}).");
        }

        private Task IncompleteGuildUserAsync(string evnt, ulong userId, ulong guildId)
        {
            string details = $"{evnt} User={userId} Guild={guildId}";
            return _gatewayLogger.DebugAsync($"User has not been downloaded ({details}).");
        }

        private Task UnknownChannelAsync(string evnt, ulong channelId)
        {
            string details = $"{evnt} Channel={channelId}";
            return _gatewayLogger.WarningAsync($"Unknown Channel ({details}).");
        }

        private Task UnknownChannelAsync(string evnt, ulong channelId, ulong guildId)
        {
            if (guildId == 0)
            {
                return UnknownChannelAsync(evnt, channelId);
            }
            string details = $"{evnt} Channel={channelId} Guild={guildId}";
            return _gatewayLogger.WarningAsync($"Unknown Channel ({details}).");
        }

        private Task UnknownRoleAsync(string evnt, ulong roleId, ulong guildId)
        {
            string details = $"{evnt} Role={roleId} Guild={guildId}";
            return _gatewayLogger.WarningAsync($"Unknown Role ({details}).");
        }

        private Task UnknownGuildAsync(string evnt, ulong guildId)
        {
            string details = $"{evnt} Guild={guildId}";
            return _gatewayLogger.WarningAsync($"Unknown Guild ({details}).");
        }

        private Task UnknownGuildEventAsync(string evnt, ulong eventId, ulong guildId)
        {
            string details = $"{evnt} Event={eventId} Guild={guildId}";
            return _gatewayLogger.WarningAsync($"Unknown Guild Event ({details}).");
        }

        private Task UnsyncedGuildAsync(string evnt, ulong guildId)
        {
            string details = $"{evnt} Guild={guildId}";
            return _gatewayLogger.DebugAsync($"Unsynced Guild ({details}).");
        }

        internal int GetAudioId() => _nextAudioId++;

        #region IDiscordClient

        /// <inheritdoc />
        async Task<ISubscription> IDiscordClient.GetSKUSubscriptionAsync(ulong skuId, ulong subscriptionId, RequestOptions options)
            => await GetSKUSubscriptionAsync(skuId, subscriptionId, options);

        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<ISubscription>> IDiscordClient.GetSKUSubscriptionsAsync(ulong skuId, int limit, ulong? afterId,
            ulong? beforeId, ulong? userId, RequestOptions options) => GetSKUSubscriptionsAsync(skuId, limit, afterId, beforeId, userId, options);

        async Task<IEntitlement> IDiscordClient.CreateTestEntitlementAsync(ulong skuId, ulong ownerId, SubscriptionOwnerType ownerType, RequestOptions options)
            => await CreateTestEntitlementAsync(skuId, ownerId, ownerType, options).ConfigureAwait(false);
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
        {
            var user = GetUser(id);
            if (user is not null || mode == CacheMode.CacheOnly)
                return user;

            return await Rest.GetUserAsync(id, options).ConfigureAwait(false);
        }

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
        async Task<IReadOnlyCollection<IApplicationCommand>> IDiscordClient.GetGlobalApplicationCommandsAsync(bool withLocalizations, string locale, RequestOptions options)
            => await GetGlobalApplicationCommandsAsync(withLocalizations, locale, options);
        /// <inheritdoc />
        async Task<IApplicationCommand> IDiscordClient.CreateGlobalApplicationCommand(ApplicationCommandProperties properties, RequestOptions options)
            => await CreateGlobalApplicationCommandAsync(properties, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IApplicationCommand>> IDiscordClient.BulkOverwriteGlobalApplicationCommand(ApplicationCommandProperties[] properties, RequestOptions options)
            => await BulkOverwriteGlobalApplicationCommandsAsync(properties, options);

        /// <inheritdoc />
        Task IDiscordClient.StartAsync()
            => StartAsync();

        /// <inheritdoc />
        Task IDiscordClient.StopAsync()
            => StopAsync();
        #endregion
    }
}
