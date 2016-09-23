using Discord.API.Gateway;
using Discord.Audio;
using Discord.Logging;
using Discord.Net.Converters;
using Discord.Net.Queue;
using Discord.Net.WebSockets;
using Discord.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public partial class DiscordSocketClient : DiscordRestClient, IDiscordClient
    {
        private readonly ConcurrentQueue<ulong> _largeGuilds;
        private readonly ILogger _gatewayLogger;
#if BENCHMARK
        private readonly ILogger _benchmarkLogger;
#endif
        private readonly JsonSerializer _serializer;

        private string _sessionId;
        private int _lastSeq;
        private ImmutableDictionary<string, VoiceRegion> _voiceRegions;
        private TaskCompletionSource<bool> _connectTask;
        private CancellationTokenSource _cancelToken, _reconnectCancelToken;
        private Task _heartbeatTask, _guildDownloadTask, _reconnectTask;
        private long _heartbeatTime;
        private int _unavailableGuilds;
        private long _lastGuildAvailableTime;
        private int _nextAudioId;
        private bool _canReconnect;

        /// <summary> Gets the shard if of this client. </summary>
        public int ShardId { get; }
        /// <summary> Gets the current connection state of this client. </summary>
        public ConnectionState ConnectionState { get; private set; }
        /// <summary> Gets the estimated round-trip latency, in milliseconds, to the gateway server. </summary>
        public int Latency { get; private set; }

        //From DiscordSocketConfig
        internal int TotalShards { get; private set; }
        internal int MessageCacheSize { get; private set; }
        internal int LargeThreshold { get; private set; }
        internal AudioMode AudioMode { get; private set; }
        internal DataStore DataStore { get; private set; }
        internal int ConnectionTimeout { get; private set; }
        internal WebSocketProvider WebSocketProvider { get; private set; }

        public new API.DiscordSocketApiClient ApiClient => base.ApiClient as API.DiscordSocketApiClient;
        internal SocketSelfUser CurrentUser => _currentUser as SocketSelfUser;
        internal IReadOnlyCollection<SocketGuild> Guilds => DataStore.Guilds;
        internal IReadOnlyCollection<VoiceRegion> VoiceRegions => _voiceRegions.ToReadOnlyCollection();

        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordSocketClient() : this(new DiscordSocketConfig()) { }
        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordSocketClient(DiscordSocketConfig config)
            : base(config, CreateApiClient(config))
        {
            ShardId = config.ShardId;
            TotalShards = config.TotalShards;
            MessageCacheSize = config.MessageCacheSize;
            LargeThreshold = config.LargeThreshold;
            AudioMode = config.AudioMode;
            WebSocketProvider = config.WebSocketProvider;
            ConnectionTimeout = config.ConnectionTimeout;

            DataStore = new DataStore(0, 0);
            _nextAudioId = 1;

            _gatewayLogger = LogManager.CreateLogger("Gateway");
#if BENCHMARK
            _benchmarkLogger = _log.CreateLogger("Benchmark");
#endif

            _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };
            _serializer.Error += (s, e) =>
            {
                _gatewayLogger.WarningAsync(e.ErrorContext.Error).GetAwaiter().GetResult();
                e.ErrorContext.Handled = true;
            };
            
            ApiClient.SentGatewayMessage += async opCode => await _gatewayLogger.DebugAsync($"Sent {opCode}").ConfigureAwait(false);
            ApiClient.ReceivedGatewayEvent += ProcessMessageAsync;
            ApiClient.Disconnected += async ex =>
            {
                if (ex != null)
                {
                    await _gatewayLogger.WarningAsync($"Connection Closed", ex).ConfigureAwait(false);
                    await StartReconnectAsync(ex).ConfigureAwait(false);
                }
                else
                    await _gatewayLogger.WarningAsync($"Connection Closed").ConfigureAwait(false);
            };

            LeftGuild += async g => await _gatewayLogger.InfoAsync($"Left {g.Name}").ConfigureAwait(false);
            JoinedGuild += async g => await _gatewayLogger.InfoAsync($"Joined {g.Name}").ConfigureAwait(false);
            GuildAvailable += async g => await _gatewayLogger.VerboseAsync($"Connected to {g.Name}").ConfigureAwait(false);
            GuildUnavailable += async g => await _gatewayLogger.VerboseAsync($"Disconnected from {g.Name}").ConfigureAwait(false);
            LatencyUpdated += async (old, val) => await _gatewayLogger.VerboseAsync($"Latency = {val} ms").ConfigureAwait(false);

            _voiceRegions = ImmutableDictionary.Create<string, VoiceRegion>();
            _largeGuilds = new ConcurrentQueue<ulong>();
        }
        private static API.DiscordSocketApiClient CreateApiClient(DiscordSocketConfig config)
            => new API.DiscordSocketApiClient(config.RestClientProvider, config.WebSocketProvider, requestQueue: new RequestQueue());

        protected override async Task OnLoginAsync(TokenType tokenType, string token)
        {
            var voiceRegions = await ApiClient.GetVoiceRegionsAsync().ConfigureAwait(false);
            _voiceRegions = voiceRegions.Select(x => new VoiceRegion(x)).ToImmutableDictionary(x => x.Id);
        }
        protected override async Task OnLogoutAsync()
        {
            if (ConnectionState != ConnectionState.Disconnected)
                await DisconnectInternalAsync(null, false).ConfigureAwait(false);

            _voiceRegions = ImmutableDictionary.Create<string, VoiceRegion>();
        }

        /// <inheritdoc />
        public async Task ConnectAsync(bool waitForGuilds = true)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await ConnectInternalAsync(false).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }

            if (waitForGuilds)
            {
                var downloadTask = _guildDownloadTask;
                if (downloadTask != null)
                    await _guildDownloadTask.ConfigureAwait(false);
            }
        }
        private async Task ConnectInternalAsync(bool isReconnecting)
        {
            if (LoginState != LoginState.LoggedIn)
                throw new InvalidOperationException("Client is not logged in.");

            if (!isReconnecting && _reconnectCancelToken != null && !_reconnectCancelToken.IsCancellationRequested)
                _reconnectCancelToken.Cancel();

            var state = ConnectionState;
            if (state == ConnectionState.Connecting || state == ConnectionState.Connected)
                await DisconnectInternalAsync(null, isReconnecting).ConfigureAwait(false);

            ConnectionState = ConnectionState.Connecting;
            await _gatewayLogger.InfoAsync("Connecting").ConfigureAwait(false);
            
            try
            {
                var connectTask = new TaskCompletionSource<bool>();
                _connectTask = connectTask;
                _cancelToken = new CancellationTokenSource();

                //Abort connection on timeout
                var _ = Task.Run(async () =>
                {
                    await Task.Delay(ConnectionTimeout);
                    connectTask.TrySetException(new TimeoutException());
                });

                await ApiClient.ConnectAsync().ConfigureAwait(false);
                await _connectedEvent.InvokeAsync().ConfigureAwait(false);

                if (_sessionId != null)
                    await ApiClient.SendResumeAsync(_sessionId, _lastSeq).ConfigureAwait(false);
                else
                    await ApiClient.SendIdentifyAsync(shardID: ShardId, totalShards:TotalShards).ConfigureAwait(false);

                await _connectTask.Task.ConfigureAwait(false);
                if (!isReconnecting)
                    _canReconnect = true;
                ConnectionState = ConnectionState.Connected;
                await _gatewayLogger.InfoAsync("Connected").ConfigureAwait(false);
            }
            catch (Exception)
            {
                await DisconnectInternalAsync(null, isReconnecting).ConfigureAwait(false);
                throw;
            }
        }
        /// <inheritdoc />
        public async Task DisconnectAsync()
        {
            if (_connectTask?.TrySetCanceled() ?? false) return;
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync(null, false).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task DisconnectInternalAsync(Exception ex, bool isReconnecting)
        {
            if (!isReconnecting)
            {
                _canReconnect = false;
                _sessionId = null;
                _lastSeq = 0;

                if (_reconnectCancelToken != null && !_reconnectCancelToken.IsCancellationRequested)
                    _reconnectCancelToken.Cancel();
            }

            ulong guildId;

            if (ConnectionState == ConnectionState.Disconnected) return;
            ConnectionState = ConnectionState.Disconnecting;
            await _gatewayLogger.InfoAsync("Disconnecting").ConfigureAwait(false);

            await _gatewayLogger.DebugAsync("Disconnecting - CancelToken").ConfigureAwait(false);
            //Signal tasks to complete
            try { _cancelToken.Cancel(); } catch { }

            await _gatewayLogger.DebugAsync("Disconnecting - ApiClient").ConfigureAwait(false);
            //Disconnect from server
            await ApiClient.DisconnectAsync().ConfigureAwait(false);

            //Wait for tasks to complete
            await _gatewayLogger.DebugAsync("Disconnecting - Heartbeat").ConfigureAwait(false);
            var heartbeatTask = _heartbeatTask;
            if (heartbeatTask != null)
                await heartbeatTask.ConfigureAwait(false);
            _heartbeatTask = null;

            await _gatewayLogger.DebugAsync("Disconnecting - Guild Downloader").ConfigureAwait(false);
            var guildDownloadTask = _guildDownloadTask;
            if (guildDownloadTask != null)
                await guildDownloadTask.ConfigureAwait(false);
            _guildDownloadTask = null;

            //Clear large guild queue
            await _gatewayLogger.DebugAsync("Disconnecting - Clean Large Guilds").ConfigureAwait(false);
            while (_largeGuilds.TryDequeue(out guildId)) { }

            //Raise virtual GUILD_UNAVAILABLEs
            foreach (var guild in DataStore.Guilds)
            {
                if (guild._available)
                    await _guildUnavailableEvent.InvokeAsync(guild).ConfigureAwait(false);
            }

            ConnectionState = ConnectionState.Disconnected;
            await _gatewayLogger.InfoAsync("Disconnected").ConfigureAwait(false);

            await _disconnectedEvent.InvokeAsync(ex).ConfigureAwait(false);
        }

        private async Task StartReconnectAsync(Exception ex)
        {
            if (ex == null)
            {
                if (_connectTask?.TrySetCanceled() ?? false) return;
            }
            else
            {
                if (_connectTask?.TrySetException(ex) ?? false) return;
            }

            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!_canReconnect || _reconnectTask != null) return;
                _reconnectCancelToken = new CancellationTokenSource();
                _reconnectTask = ReconnectInternalAsync(ex, _reconnectCancelToken.Token);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ReconnectInternalAsync(Exception ex, CancellationToken cancelToken)
        {
            try
            {
                Random jitter = new Random();
                int nextReconnectDelay = 1000;
                while (true)
                {
                    await Task.Delay(nextReconnectDelay, cancelToken).ConfigureAwait(false);
                    nextReconnectDelay = nextReconnectDelay * 2 + jitter.Next(-250, 250);
                    if (nextReconnectDelay > 60000)
                        nextReconnectDelay = 60000;

                    await _connectionLock.WaitAsync().ConfigureAwait(false);
                    try
                    {
                        if (cancelToken.IsCancellationRequested) return;
                        await ConnectInternalAsync(true).ConfigureAwait(false);
                        _reconnectTask = null;
                        return;
                    }
                    catch (Exception ex2)
                    {
                        await _gatewayLogger.WarningAsync("Reconnect failed", ex2).ConfigureAwait(false);
                    }
                    finally {  _connectionLock.Release(); }
                }
            }
            catch (OperationCanceledException)
            {
                await _connectionLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    await _gatewayLogger.DebugAsync("Reconnect cancelled").ConfigureAwait(false);
                    _reconnectTask = null;
                }
                finally { _connectionLock.Release(); }
            }
        }

        /// <inheritdoc />
        public override Task<IVoiceRegion> GetVoiceRegionAsync(string id)
        {
            VoiceRegion region;
            if (_voiceRegions.TryGetValue(id, out region))
                return Task.FromResult<IVoiceRegion>(region);
            return Task.FromResult<IVoiceRegion>(null);
        }

        /// <inheritdoc />
        public override Task<IGuild> GetGuildAsync(ulong id)
        {
            return Task.FromResult<IGuild>(DataStore.GetGuild(id));
        }
        public override Task<GuildEmbed?> GetGuildEmbedAsync(ulong id)
        {
            var guild = DataStore.GetGuild(id);
            if (guild != null)
                return Task.FromResult<GuildEmbed?>(new GuildEmbed(guild.IsEmbeddable, guild.EmbedChannelId));
            else
                return Task.FromResult<GuildEmbed?>(null);
        }
        public override Task<IReadOnlyCollection<IUserGuild>> GetGuildSummariesAsync()
        {
            return Task.FromResult<IReadOnlyCollection<IUserGuild>>(Guilds);
        }
        public override Task<IReadOnlyCollection<IGuild>> GetGuildsAsync()
        {
            return Task.FromResult<IReadOnlyCollection<IGuild>>(Guilds);
        }
        internal SocketGuild AddGuild(ExtendedGuild model, DataStore dataStore)
        {
            var guild = new SocketGuild(this, model, dataStore);
            dataStore.AddGuild(guild);
            if (model.Large)
                _largeGuilds.Enqueue(model.Id);
            return guild;
        }
        internal SocketGuild RemoveGuild(ulong id)
        {
            var guild = DataStore.RemoveGuild(id);
            if (guild != null)
            {
                foreach (var channel in guild.Channels)
                    DataStore.RemoveChannel(id);
                foreach (var user in guild.Members)
                    user.User.RemoveRef(this);
            }
            return guild;
        }
        
        /// <inheritdoc />
        public override Task<IChannel> GetChannelAsync(ulong id)
        {
            return Task.FromResult<IChannel>(DataStore.GetChannel(id));
        }
        public override Task<IReadOnlyCollection<IPrivateChannel>> GetPrivateChannelsAsync()
        {
            return Task.FromResult<IReadOnlyCollection<IPrivateChannel>>(DataStore.PrivateChannels);
        }
        internal ISocketChannel AddPrivateChannel(API.Channel model, DataStore dataStore)
        {
            switch (model.Type)
            {
                case ChannelType.DM:
                    {
                        var recipients = model.Recipients.Value;
                        var user = GetOrAddUser(recipients[0], dataStore);
                        var channel = new SocketDMChannel(this, new SocketDMUser(user), model);
                        dataStore.AddChannel(channel);
                        return channel;
                    }
                case ChannelType.Group:
                    {
                        var channel = new SocketGroupChannel(this, model);
                        channel.UpdateUsers(model.Recipients.Value, dataStore);
                        dataStore.AddChannel(channel);
                        return channel;
                    }
                default:
                    throw new InvalidOperationException($"Unexpected channel type: {model.Type}");
            }
        }
        internal ISocketChannel RemovePrivateChannel(ulong id)
        {
            var channel = DataStore.RemoveChannel(id) as ISocketPrivateChannel;
            if (channel != null)
            {
                foreach (var recipient in channel.Recipients)
                    recipient.User.RemoveRef(this);
            }
            return channel;
        }

        /// <inheritdoc />
        public override Task<IUser> GetUserAsync(ulong id)
        {
            return Task.FromResult<IUser>(DataStore.GetUser(id));
        }
        /// <inheritdoc />
        public override Task<IUser> GetUserAsync(string username, string discriminator)
        {
            return Task.FromResult<IUser>(DataStore.Users.Where(x => x.Discriminator == discriminator && x.Username == username).FirstOrDefault());
        }
        /// <inheritdoc />
        public override Task<ISelfUser> GetCurrentUserAsync()
        {
            return Task.FromResult<ISelfUser>(_currentUser);
        }
        internal SocketGlobalUser GetOrAddUser(API.User model, DataStore dataStore)
        {
            var user = dataStore.GetOrAddUser(model.Id, _ => new SocketGlobalUser(model));
            user.AddRef();
            return user;
        }
        internal SocketGlobalUser RemoveUser(ulong id)
        {
            return DataStore.RemoveUser(id);
        }

        /// <summary> Downloads the users list for all large guilds. </summary>
        public Task DownloadAllUsersAsync() 
            => DownloadUsersAsync(DataStore.Guilds.Where(x => !x.HasAllMembers));
        /// <summary> Downloads the users list for the provided guilds, if they don't have a complete list. </summary>
        public Task DownloadUsersAsync(IEnumerable<IGuild> guilds)
            => DownloadUsersAsync(guilds.Select(x => x as SocketGuild).Where(x => x != null));
        public Task DownloadUsersAsync(params IGuild[] guilds)
            => DownloadUsersAsync(guilds.Select(x => x as SocketGuild).Where(x => x != null));
        private async Task DownloadUsersAsync(IEnumerable<SocketGuild> guilds)
        {
            var cachedGuilds = guilds.ToArray();
            if (cachedGuilds.Length == 0) return;

            //Wait for unsynced guilds to sync first.
            var unsyncedGuilds = guilds.Select(x => x.SyncPromise).Where(x => !x.IsCompleted).ToArray();
            if (unsyncedGuilds.Length > 0)
                await Task.WhenAll(unsyncedGuilds);

            //Download offline members
            const short batchSize = 50;

            if (cachedGuilds.Length == 1)
            {
                if (!cachedGuilds[0].HasAllMembers)
                    await ApiClient.SendRequestMembersAsync(new ulong[] { cachedGuilds[0].Id }).ConfigureAwait(false);
                await cachedGuilds[0].DownloaderPromise.ConfigureAwait(false);
                return;
            }

            ulong[] batchIds = new ulong[Math.Min(batchSize, cachedGuilds.Length)];
            Task[] batchTasks = new Task[batchIds.Length];
            int batchCount = (cachedGuilds.Length + (batchSize - 1)) / batchSize;

            for (int i = 0, k = 0; i < batchCount; i++)
            {
                bool isLast = i == batchCount - 1;
                int count = isLast ? (batchIds.Length - (batchCount - 1) * batchSize) : batchSize;

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

        public override Task<IReadOnlyCollection<IVoiceRegion>> GetVoiceRegionsAsync()
        {
            return Task.FromResult<IReadOnlyCollection<IVoiceRegion>>(_voiceRegions.ToReadOnlyCollection());
        }
        
        private async Task ProcessMessageAsync(GatewayOpCode opCode, int? seq, string type, object payload)
        {
#if BENCHMARK
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
#endif
            if (seq != null)
                _lastSeq = seq.Value;
            try
            {
                switch (opCode)
                {
                    case GatewayOpCode.Hello:
                        {
                            await _gatewayLogger.DebugAsync("Received Hello").ConfigureAwait(false);
                            var data = (payload as JToken).ToObject<HelloEvent>(_serializer);

                            _heartbeatTime = 0;
                            _heartbeatTask = RunHeartbeatAsync(data.HeartbeatInterval, _cancelToken.Token, _clientLogger);
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

                            var heartbeatTime = _heartbeatTime;
                            if (heartbeatTime != 0)
                            {
                                int latency = (int)(Environment.TickCount - _heartbeatTime);
                                _heartbeatTime = 0;

                                int before = Latency;
                                Latency = latency;

                                await _latencyUpdatedEvent.InvokeAsync(before, latency).ConfigureAwait(false);
                            }
                        }
                        break;
                    case GatewayOpCode.InvalidSession:
                        {
                            await _gatewayLogger.DebugAsync("Received InvalidSession").ConfigureAwait(false);
                            await _gatewayLogger.WarningAsync("Failed to resume previous session").ConfigureAwait(false);

                            _sessionId = null;
                            _lastSeq = 0;
                            await ApiClient.SendIdentifyAsync(shardID: ShardId, totalShards: TotalShards).ConfigureAwait(false);
                        }
                        break;
                    case GatewayOpCode.Reconnect:
                        {
                            await _gatewayLogger.DebugAsync("Received Reconnect").ConfigureAwait(false);
                            await _gatewayLogger.WarningAsync("Server requested a reconnect").ConfigureAwait(false);

                            await StartReconnectAsync(new Exception("Server requested a reconnect")).ConfigureAwait(false);
                        }
                        break;
                    case GatewayOpCode.Dispatch:
                        switch (type)
                        {
                            //Connection
                            case "READY":
                                {
                                    try
                                    {
                                        await _gatewayLogger.DebugAsync("Received Dispatch (READY)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<ReadyEvent>(_serializer);
                                        var dataStore = new DataStore(data.Guilds.Length, data.PrivateChannels.Length);

                                        var currentUser = new SocketSelfUser(this, data.User);
                                        int unavailableGuilds = 0;
                                        for (int i = 0; i < data.Guilds.Length; i++)
                                        {
                                            var model = data.Guilds[i];
                                            var guild = AddGuild(model, dataStore);
                                            if (!guild._available || ApiClient.AuthTokenType == TokenType.User)
                                                unavailableGuilds++;
                                            else
                                                await _guildAvailableEvent.InvokeAsync(guild).ConfigureAwait(false);
                                        }
                                        for (int i = 0; i < data.PrivateChannels.Length; i++)
                                            AddPrivateChannel(data.PrivateChannels[i], dataStore);

                                        _sessionId = data.SessionId;
                                        _currentUser = currentUser;
                                        _unavailableGuilds = unavailableGuilds;
                                        DataStore = dataStore;
                                    }
                                    catch (Exception ex)
                                    {
                                        _connectTask.TrySetException(new Exception("Processing READY failed", ex));
                                        return;
                                    }

                                    if (ApiClient.AuthTokenType == TokenType.User)
                                        await SyncGuildsAsync().ConfigureAwait(false);

                                    _lastGuildAvailableTime = Environment.TickCount;
                                    _guildDownloadTask = WaitForGuildsAsync(_cancelToken.Token, _clientLogger);

                                    await _readyEvent.InvokeAsync().ConfigureAwait(false);

                                    var _ = _connectTask.TrySetResultAsync(true); //Signal the .Connect() call to complete
                                    await _gatewayLogger.InfoAsync("Ready").ConfigureAwait(false);
                                }
                                break;
                            case "RESUMED":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (RESUMED)").ConfigureAwait(false);

                                    var _ = _connectTask.TrySetResultAsync(true); //Signal the .Connect() call to complete

                                    //Notify the client that these guilds are available again
                                    foreach (var guild in DataStore.Guilds)
                                    {
                                        if (guild._available)
                                            await _guildAvailableEvent.InvokeAsync(guild).ConfigureAwait(false);
                                    }

                                    await _gatewayLogger.InfoAsync("Resumed previous session").ConfigureAwait(false);
                                }
                                return;

                            //Guilds
                            case "GUILD_CREATE":
                                {
                                    var data = (payload as JToken).ToObject<ExtendedGuild>(_serializer);

                                    if (data.Unavailable == false)
                                    {
                                        type = "GUILD_AVAILABLE";
                                        _lastGuildAvailableTime = Environment.TickCount;
                                        await _gatewayLogger.DebugAsync($"Received Dispatch (GUILD_AVAILABLE)").ConfigureAwait(false);

                                        var guild = DataStore.GetGuild(data.Id);
                                        if (guild != null)
                                        {
                                            guild.Update(data, UpdateSource.WebSocket, DataStore);

                                            var unavailableGuilds = _unavailableGuilds;
                                            if (unavailableGuilds != 0)
                                                _unavailableGuilds = unavailableGuilds - 1;
                                            await _guildAvailableEvent.InvokeAsync(guild).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            await _gatewayLogger.WarningAsync($"GUILD_AVAILABLE referenced an unknown guild.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await _gatewayLogger.DebugAsync($"Received Dispatch (GUILD_CREATE)").ConfigureAwait(false);

                                        var guild = AddGuild(data, DataStore);
                                        if (guild != null)
                                        {
                                            if (ApiClient.AuthTokenType == TokenType.User)
                                                await SyncGuildsAsync().ConfigureAwait(false);
                                            await _joinedGuildEvent.InvokeAsync(guild).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            await _gatewayLogger.WarningAsync($"GUILD_CREATE referenced an unknown guild.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                }
                                break;
                            case "GUILD_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Guild>(_serializer);
                                    var guild = DataStore.GetGuild(data.Id);
                                    if (guild != null)
                                    {
                                        var before = guild.Clone();
                                        guild.Update(data, UpdateSource.WebSocket);
                                        await _guildUpdatedEvent.InvokeAsync(before, guild).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("GUILD_UPDATE referenced an unknown guild.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_EMOJIS_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_EMOJIS_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Gateway.GuildEmojiUpdateEvent>(_serializer);
                                    var guild = DataStore.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var before = guild.Clone();
                                        guild.Update(data, UpdateSource.WebSocket);
                                        await _guildUpdatedEvent.InvokeAsync(before, guild).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("GUILD_EMOJIS_UPDATE referenced an unknown guild.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                return;
                            case "GUILD_INTEGRATIONS_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Ignored Dispatch (GUILD_INTEGRATIONS_UPDATE)").ConfigureAwait(false);
                                }
                                return;
                            case "GUILD_SYNC":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_SYNC)").ConfigureAwait(false);
                                    var data = (payload as JToken).ToObject<GuildSyncEvent>(_serializer);
                                    var guild = DataStore.GetGuild(data.Id);
                                    if (guild != null)
                                    {
                                        var before = guild.Clone();
                                        guild.Update(data, UpdateSource.WebSocket, DataStore);
                                        //This is treated as an extension of GUILD_AVAILABLE
                                        _unavailableGuilds--;
                                        _lastGuildAvailableTime = Environment.TickCount;
                                        await _guildAvailableEvent.InvokeAsync(guild).ConfigureAwait(false);
                                        await _guildUpdatedEvent.InvokeAsync(before, guild).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("GUILD_SYNC referenced an unknown guild.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                return;
                            case "GUILD_DELETE":
                                {
                                    var data = (payload as JToken).ToObject<ExtendedGuild>(_serializer);
                                    if (data.Unavailable == true)
                                    {
                                        type = "GUILD_UNAVAILABLE";
                                        await _gatewayLogger.DebugAsync($"Received Dispatch (GUILD_UNAVAILABLE)").ConfigureAwait(false);

                                        var guild = DataStore.GetGuild(data.Id);
                                        if (guild != null)
                                        {
                                            foreach (var member in guild.Members)
                                                member.User.RemoveRef(this);
                                            await _guildUnavailableEvent.InvokeAsync(guild).ConfigureAwait(false);
                                            _unavailableGuilds++;
                                        }
                                        else
                                        {
                                            await _gatewayLogger.WarningAsync($"GUILD_UNAVAILABLE referenced an unknown guild.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await _gatewayLogger.DebugAsync($"Received Dispatch (GUILD_DELETE)").ConfigureAwait(false);

                                        var guild = RemoveGuild(data.Id);
                                        if (guild != null)
                                        {
                                            await _guildUnavailableEvent.InvokeAsync(guild).ConfigureAwait(false);
                                            await _leftGuildEvent.InvokeAsync(guild).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            await _gatewayLogger.WarningAsync($"GUILD_DELETE referenced an unknown guild.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                }
                                break;

                            //Channels
                            case "CHANNEL_CREATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_CREATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                                    ISocketChannel channel = null;
                                    if (data.GuildId.IsSpecified)
                                    {
                                        var guild = DataStore.GetGuild(data.GuildId.Value);
                                        if (guild != null)
                                        {
                                            guild.AddChannel(data, DataStore);

                                            if (!guild.IsSynced)
                                            {
                                                await _gatewayLogger.DebugAsync("Ignored CHANNEL_CREATE, guild is not synced yet.").ConfigureAwait(false);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            await _gatewayLogger.WarningAsync("CHANNEL_CREATE referenced an unknown guild.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                        channel = AddPrivateChannel(data, DataStore);

                                    if (channel != null)
                                        await _channelCreatedEvent.InvokeAsync(channel).ConfigureAwait(false);
                                }
                                break;
                            case "CHANNEL_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                                    var channel = DataStore.GetChannel(data.Id);
                                    if (channel != null)
                                    {
                                        var before = channel.Clone();
                                        channel.Update(data, UpdateSource.WebSocket);

                                        if (!((channel as ISocketGuildChannel)?.Guild.IsSynced ?? true))
                                        {
                                            await _gatewayLogger.DebugAsync("Ignored CHANNEL_UPDATE, guild is not synced yet.").ConfigureAwait(false);
                                            return;
                                        }

                                        await _channelUpdatedEvent.InvokeAsync(before, channel).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("CHANNEL_UPDATE referenced an unknown channel.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "CHANNEL_DELETE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_DELETE)").ConfigureAwait(false);

                                    ISocketChannel channel = null;
                                    var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                                    if (data.GuildId.IsSpecified)
                                    {
                                        var guild = DataStore.GetGuild(data.GuildId.Value);
                                        if (guild != null)
                                        {
                                            channel = guild.RemoveChannel(data.Id);

                                            if (!guild.IsSynced)
                                            {
                                                await _gatewayLogger.DebugAsync("Ignored CHANNEL_DELETE, guild is not synced yet.").ConfigureAwait(false);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            await _gatewayLogger.WarningAsync("CHANNEL_DELETE referenced an unknown guild.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                        channel = RemovePrivateChannel(data.Id);

                                    if (channel != null)
                                        await _channelDestroyedEvent.InvokeAsync(channel).ConfigureAwait(false);
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("CHANNEL_DELETE referenced an unknown channel.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;

                            //Members
                            case "GUILD_MEMBER_ADD":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_MEMBER_ADD)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildMemberAddEvent>(_serializer);
                                    var guild = DataStore.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var user = guild.AddOrUpdateUser(data, DataStore);
                                        guild.MemberCount++;

                                        if (!guild.IsSynced)
                                        {
                                            await _gatewayLogger.DebugAsync("Ignored GUILD_MEMBER_ADD, guild is not synced yet.").ConfigureAwait(false);
                                            return;
                                        }

                                        await _userJoinedEvent.InvokeAsync(user).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("GUILD_MEMBER_ADD referenced an unknown guild.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_MEMBER_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_MEMBER_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildMemberUpdateEvent>(_serializer);
                                    var guild = DataStore.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var user = guild.GetUser(data.User.Id);

                                        if (!guild.IsSynced)
                                        {
                                            await _gatewayLogger.DebugAsync("Ignored GUILD_MEMBER_UPDATE, guild is not synced yet.").ConfigureAwait(false);
                                            return;
                                        }

                                        if (user != null)
                                        {
                                            var before = user.Clone();
                                            user.Update(data, UpdateSource.WebSocket);
                                            await _userUpdatedEvent.InvokeAsync(before, user).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            if (!guild.HasAllMembers)
                                            {
                                                await _gatewayLogger.DebugAsync("Ignored GUILD_MEMBER_UPDATE, this user has not been downloaded yet.").ConfigureAwait(false);
                                                return;
                                            }

                                            await _gatewayLogger.WarningAsync("GUILD_MEMBER_UPDATE referenced an unknown user.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("GUILD_MEMBER_UPDATE referenced an unknown guild.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_MEMBER_REMOVE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_MEMBER_REMOVE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildMemberRemoveEvent>(_serializer);
                                    var guild = DataStore.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var user = guild.RemoveUser(data.User.Id);
                                        guild.MemberCount--;

                                        if (!guild.IsSynced)
                                        {
                                            await _gatewayLogger.DebugAsync("Ignored GUILD_MEMBER_REMOVE, guild is not synced yet.").ConfigureAwait(false);
                                            return;
                                        }

                                        if (user != null)
                                        {
                                            user.User.RemoveRef(this);
                                            await _userLeftEvent.InvokeAsync(user).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            if (!guild.HasAllMembers)
                                            {
                                                await _gatewayLogger.DebugAsync("Ignored GUILD_MEMBER_REMOVE, this user has not been downloaded yet.").ConfigureAwait(false);
                                                return;
                                            }

                                            await _gatewayLogger.WarningAsync("GUILD_MEMBER_REMOVE referenced an unknown user.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("GUILD_MEMBER_REMOVE referenced an unknown guild.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_MEMBERS_CHUNK":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_MEMBERS_CHUNK)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildMembersChunkEvent>(_serializer);
                                    var guild = DataStore.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        foreach (var memberModel in data.Members)
                                            guild.AddOrUpdateUser(memberModel, DataStore);

                                        if (guild.DownloadedMemberCount >= guild.MemberCount) //Finished downloading for there
                                        {
                                            guild.CompleteDownloadMembers();
                                            await _guildMembersDownloadedEvent.InvokeAsync(guild).ConfigureAwait(false);
                                        }
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("GUILD_MEMBERS_CHUNK referenced an unknown guild.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "CHANNEL_RECIPIENT_ADD":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_RECIPIENT_ADD)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<RecipientEvent>(_serializer);
                                    var channel = DataStore.GetChannel(data.ChannelId) as SocketGroupChannel;
                                    if (channel != null)
                                    {
                                        var user = channel.AddUser(data.User, DataStore);
                                        await _recipientAddedEvent.InvokeAsync(user).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("CHANNEL_RECIPIENT_ADD referenced an unknown channel.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "CHANNEL_RECIPIENT_REMOVE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_RECIPIENT_REMOVE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<RecipientEvent>(_serializer);
                                    var channel = DataStore.GetChannel(data.ChannelId) as SocketGroupChannel;
                                    if (channel != null)
                                    {
                                        var user = channel.RemoveUser(data.User.Id);
                                        if (user != null)
                                        {
                                            user.User.RemoveRef(this);
                                            await _recipientRemovedEvent.InvokeAsync(user).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            await _gatewayLogger.WarningAsync("CHANNEL_RECIPIENT_REMOVE referenced an unknown user.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("CHANNEL_RECIPIENT_ADD referenced an unknown channel.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;

                            //Roles
                            case "GUILD_ROLE_CREATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_ROLE_CREATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildRoleCreateEvent>(_serializer);
                                    var guild = DataStore.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var role = guild.AddRole(data.Role);

                                        if (!guild.IsSynced)
                                        {
                                            await _gatewayLogger.DebugAsync("Ignored GUILD_ROLE_CREATE, guild is not synced yet.").ConfigureAwait(false);
                                            return;
                                        }
                                        await _roleCreatedEvent.InvokeAsync(role).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("GUILD_ROLE_CREATE referenced an unknown guild.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_ROLE_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_ROLE_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildRoleUpdateEvent>(_serializer);
                                    var guild = DataStore.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var role = guild.GetRole(data.Role.Id);
                                        if (role != null)
                                        {
                                            var before = role.Clone();
                                            role.Update(data.Role, UpdateSource.WebSocket);

                                            if (!guild.IsSynced)
                                            {
                                                await _gatewayLogger.DebugAsync("Ignored GUILD_ROLE_UPDATE, guild is not synced yet.").ConfigureAwait(false);
                                                return;
                                            }

                                            await _roleUpdatedEvent.InvokeAsync(before, role).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            await _gatewayLogger.WarningAsync("GUILD_ROLE_UPDATE referenced an unknown role.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("GUILD_ROLE_UPDATE referenced an unknown guild.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_ROLE_DELETE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_ROLE_DELETE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildRoleDeleteEvent>(_serializer);
                                    var guild = DataStore.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var role = guild.RemoveRole(data.RoleId);
                                        if (role != null)
                                        {
                                            if (!guild.IsSynced)
                                            {
                                                await _gatewayLogger.DebugAsync("Ignored GUILD_ROLE_DELETE, guild is not synced yet.").ConfigureAwait(false);
                                                return;
                                            }

                                            await _roleDeletedEvent.InvokeAsync(role).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            await _gatewayLogger.WarningAsync("GUILD_ROLE_DELETE referenced an unknown role.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("GUILD_ROLE_DELETE referenced an unknown guild.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;

                            //Bans
                            case "GUILD_BAN_ADD":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_BAN_ADD)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildBanEvent>(_serializer);
                                    var guild = DataStore.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        if (!guild.IsSynced)
                                        {
                                            await _gatewayLogger.DebugAsync("Ignored GUILD_BAN_ADD, guild is not synced yet.").ConfigureAwait(false);
                                            return;
                                        }

                                        await _userBannedEvent.InvokeAsync(new User(data.User), guild).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("GUILD_BAN_ADD referenced an unknown guild.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_BAN_REMOVE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_BAN_REMOVE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<GuildBanEvent>(_serializer);
                                    var guild = DataStore.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        if (!guild.IsSynced)
                                        {
                                            await _gatewayLogger.DebugAsync("Ignored GUILD_BAN_REMOVE, guild is not synced yet.").ConfigureAwait(false);
                                            return;
                                        }

                                        await _userUnbannedEvent.InvokeAsync(new User(data.User), guild).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("GUILD_BAN_REMOVE referenced an unknown guild.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;

                            //Messages
                            case "MESSAGE_CREATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_CREATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Message>(_serializer);
                                    var channel = DataStore.GetChannel(data.ChannelId) as ISocketMessageChannel;
                                    if (channel != null)
                                    {
                                        if (!((channel as ISocketGuildChannel)?.Guild.IsSynced ?? true))
                                        { 
                                            await _gatewayLogger.DebugAsync("Ignored MESSAGE_CREATE, guild is not synced yet.").ConfigureAwait(false);
                                            return;
                                        }

                                        var author = channel.GetUser(data.Author.Value.Id, true);

                                        if (author != null)
                                        {
                                            var msg = channel.AddMessage(author, data);
                                            await _messageReceivedEvent.InvokeAsync(msg).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            await _gatewayLogger.WarningAsync("MESSAGE_CREATE referenced an unknown user.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("MESSAGE_CREATE referenced an unknown channel.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "MESSAGE_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Message>(_serializer);
                                    var channel = DataStore.GetChannel(data.ChannelId) as ISocketMessageChannel;
                                    if (channel != null)
                                    {
                                        if (!((channel as ISocketGuildChannel)?.Guild.IsSynced ?? true))
                                        { 
                                            await _gatewayLogger.DebugAsync("Ignored MESSAGE_UPDATE, guild is not synced yet.").ConfigureAwait(false);
                                            return;
                                        }

                                        IMessage before = null, after = null;
                                        ISocketMessage cachedMsg = channel.GetMessage(data.Id);
                                        if (cachedMsg != null)
                                        {
                                            before = cachedMsg.Clone();
                                            cachedMsg.Update(data, UpdateSource.WebSocket);
                                            after = cachedMsg;
                                        }
                                        else if (data.Author.IsSpecified)
                                        {
                                            //Edited message isnt in cache, create a detached one
                                            var author = channel.GetUser(data.Author.Value.Id, true);
                                            if (author != null)
                                                after = channel.CreateMessage(author, data);
                                        }
                                        if (after != null)
                                            await _messageUpdatedEvent.InvokeAsync(Optional.Create(before), after).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("MESSAGE_UPDATE referenced an unknown channel.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "MESSAGE_DELETE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_DELETE)").ConfigureAwait(false);
                                    
                                    var data = (payload as JToken).ToObject<API.Message>(_serializer);
                                    var channel = DataStore.GetChannel(data.ChannelId) as ISocketMessageChannel;
                                    if (channel != null)
                                    {
                                        if (!((channel as ISocketGuildChannel)?.Guild.IsSynced ?? true))
                                        { 
                                            await _gatewayLogger.DebugAsync("Ignored MESSAGE_DELETE, guild is not synced yet.").ConfigureAwait(false);
                                            return;
                                        }

                                        var msg = channel.RemoveMessage(data.Id);
                                        if (msg != null)
                                            await _messageDeletedEvent.InvokeAsync(data.Id, Optional.Create<IMessage>(msg)).ConfigureAwait(false);
                                        else
                                            await _messageDeletedEvent.InvokeAsync(data.Id, Optional.Create<IMessage>()).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("MESSAGE_DELETE referenced an unknown channel.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;
                            case "MESSAGE_DELETE_BULK":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_DELETE_BULK)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<MessageDeleteBulkEvent>(_serializer);
                                    var channel = DataStore.GetChannel(data.ChannelId) as ISocketMessageChannel;
                                    if (channel != null)
                                    {
                                        if (!((channel as ISocketGuildChannel)?.Guild.IsSynced ?? true))
                                        {
                                            await _gatewayLogger.DebugAsync("Ignored MESSAGE_DELETE_BULK, guild is not synced yet.").ConfigureAwait(false);
                                            return;
                                        }

                                        foreach (var id in data.Ids)
                                        {
                                            var msg = channel.RemoveMessage(id);
                                            if (msg != null)
                                                await _messageDeletedEvent.InvokeAsync(id, Optional.Create<IMessage>(msg)).ConfigureAwait(false);
                                            else
                                                await _messageDeletedEvent.InvokeAsync(id, Optional.Create<IMessage>()).ConfigureAwait(false);
                                        }
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("MESSAGE_DELETE_BULK referenced an unknown channel.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;

                            //Statuses
                            case "PRESENCE_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (PRESENCE_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Presence>(_serializer);
                                    if (data.GuildId.IsSpecified)
                                    {
                                        var guild = DataStore.GetGuild(data.GuildId.Value);
                                        if (guild == null)
                                        {
                                            await _gatewayLogger.WarningAsync("PRESENCE_UPDATE referenced an unknown guild.").ConfigureAwait(false);
                                            break;
                                        }

                                        if (!guild.IsSynced)
                                        {
                                            await _gatewayLogger.DebugAsync("Ignored PRESENCE_UPDATE, guild is not synced yet.").ConfigureAwait(false);
                                            return;
                                        }

                                        IPresence before;
                                        var user = guild.GetUser(data.User.Id);
                                        if (user != null)
                                        {
                                            before = user.Presence.Clone();
                                            user.Update(data, UpdateSource.WebSocket);
                                        }
                                        else
                                        {
                                            before = new Presence(null, UserStatus.Offline);
                                            user = guild.AddOrUpdateUser(data, DataStore);
                                        }

                                        await _userPresenceUpdatedEvent.InvokeAsync(user, before, user).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        var channel = DataStore.GetDMChannel(data.User.Id);
                                        if (channel != null)
                                            channel.Recipient.Update(data, UpdateSource.WebSocket);
                                    }
                                }
                                break;
                            case "TYPING_START":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (TYPING_START)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<TypingStartEvent>(_serializer);
                                    var channel = DataStore.GetChannel(data.ChannelId) as ISocketMessageChannel;
                                    if (channel != null)
                                    {
                                        if (!((channel as ISocketGuildChannel)?.Guild.IsSynced ?? true))
                                        {
                                            await _gatewayLogger.DebugAsync("Ignored TYPING_START, guild is not synced yet.").ConfigureAwait(false);
                                            return;
                                        }

                                        var user = channel.GetUser(data.UserId, true);
                                        if (user != null)
                                            await _userIsTypingEvent.InvokeAsync(user, channel).ConfigureAwait(false);
                                    }
                                }
                                break;

                            //Users
                            case "USER_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (USER_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.User>(_serializer);
                                    if (data.Id == CurrentUser.Id)
                                    {
                                        var before = CurrentUser.Clone();
                                        CurrentUser.Update(data, UpdateSource.WebSocket);
                                        await _selfUpdatedEvent.InvokeAsync(before, CurrentUser).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("Received USER_UPDATE for wrong user.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;

                            //Voice
                            case "VOICE_STATE_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (VOICE_STATE_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.VoiceState>(_serializer);
                                    if (data.GuildId.HasValue)
                                    {
                                        ISocketUser user;
                                        VoiceState before, after;
                                        if (data.GuildId != null)
                                        {
                                            var guild = DataStore.GetGuild(data.GuildId.Value);
                                            if (guild != null)
                                            {
                                                if (!guild.IsSynced)
                                                {
                                                    await _gatewayLogger.DebugAsync("Ignored VOICE_STATE_UPDATE, guild is not synced yet.").ConfigureAwait(false);
                                                    return;
                                                }

                                                if (data.ChannelId != null)
                                                {
                                                    before = guild.GetVoiceState(data.UserId)?.Clone() ?? new VoiceState(null, null, false, false, false);
                                                    after = guild.AddOrUpdateVoiceState(data, DataStore);
                                                    if (data.UserId == _currentUser.Id)
                                                    {
                                                        var _ = guild.FinishJoinAudioChannel().ConfigureAwait(false);
                                                    }
                                                }
                                                else
                                                {
                                                    before = guild.RemoveVoiceState(data.UserId) ?? new VoiceState(null, null, false, false, false);
                                                    after = new VoiceState(null, data);
                                                }

                                                user = guild.GetUser(data.UserId);
                                            }
                                            else
                                            {
                                                await _gatewayLogger.WarningAsync("VOICE_STATE_UPDATE referenced an unknown guild.").ConfigureAwait(false);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            var groupChannel = DataStore.GetChannel(data.ChannelId.Value) as SocketGroupChannel;
                                            if (groupChannel != null)
                                            {
                                                if (data.ChannelId != null)
                                                {
                                                    before = groupChannel.GetVoiceState(data.UserId)?.Clone() ?? new VoiceState(null, null, false, false, false);
                                                    after = groupChannel.AddOrUpdateVoiceState(data, DataStore);
                                                }
                                                else
                                                {
                                                    before = groupChannel.RemoveVoiceState(data.UserId) ?? new VoiceState(null, null, false, false, false);
                                                    after = new VoiceState(null, data);
                                                }
                                                user = groupChannel.GetUser(data.UserId);
                                            }
                                            else
                                            {
                                                await _gatewayLogger.WarningAsync("VOICE_STATE_UPDATE referenced an unknown channel.").ConfigureAwait(false);
                                                return;
                                            }
                                        }

                                        if (user != null)
                                            await _userVoiceStateUpdatedEvent.InvokeAsync(user, before, after).ConfigureAwait(false);
                                        else
                                        {
                                            await _gatewayLogger.WarningAsync("VOICE_STATE_UPDATE referenced an unknown user.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                }
                                break;
                            case "VOICE_SERVER_UPDATE":
                                await _gatewayLogger.DebugAsync("Received Dispatch (VOICE_SERVER_UPDATE)").ConfigureAwait(false);

                                if (AudioMode != AudioMode.Disabled)
                                {
                                    var data = (payload as JToken).ToObject<VoiceServerUpdateEvent>(_serializer);
                                    var guild = DataStore.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        string endpoint = data.Endpoint.Substring(0, data.Endpoint.LastIndexOf(':'));
                                        var _ = guild.FinishConnectAudio(_nextAudioId++, endpoint, data.Token).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("VOICE_SERVER_UPDATE referenced an unknown guild.").ConfigureAwait(false);
                                        return;
                                    }
                                }

                                return;

                            //Ignored (User only)
                            case "CHANNEL_PINS_ACK":
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (CHANNEL_PINS_ACK)");
                                break;
                            case "CHANNEL_PINS_UPDATE":
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (CHANNEL_PINS_UPDATE)");
                                break;
                            case "USER_SETTINGS_UPDATE":
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (USER_SETTINGS_UPDATE)").ConfigureAwait(false);
                                return;
                            case "MESSAGE_ACK":
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (MESSAGE_ACK)").ConfigureAwait(false);
                                return;

                            //Others
                            default:
                                await _gatewayLogger.WarningAsync($"Unknown Dispatch ({type})").ConfigureAwait(false);
                                return;
                        }
                        break;
                    default:
                        await _gatewayLogger.WarningAsync($"Unknown OpCode ({opCode})").ConfigureAwait(false);
                        return;
                }
            }
            catch (Exception ex)
            {
                await _gatewayLogger.ErrorAsync($"Error handling {opCode}{(type != null ? $" ({type})" : "")}", ex).ConfigureAwait(false);
                return;
            }
#if BENCHMARK
            }
            finally
            {
                stopwatch.Stop();
                double millis = Math.Round(stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0, 2);
                await _benchmarkLogger.DebugAsync($"{millis} ms").ConfigureAwait(false);
            }
#endif
        }

        private async Task RunHeartbeatAsync(int intervalMillis, CancellationToken cancelToken, ILogger logger)
        {
            try
            {
                await logger.DebugAsync("Heartbeat Started").ConfigureAwait(false);
                while (!cancelToken.IsCancellationRequested)
                {
                    if (_heartbeatTime != 0) //Server never responded to our last heartbeat
                    {
                        if (ConnectionState == ConnectionState.Connected && (_guildDownloadTask?.IsCompleted ?? false))
                        {
                            await _gatewayLogger.WarningAsync("Server missed last heartbeat").ConfigureAwait(false);
                            await StartReconnectAsync(new Exception("Server missed last heartbeat")).ConfigureAwait(false);
                            return;
                        }
                    }
                    _heartbeatTime = Environment.TickCount;

                    try
                    {
                        await ApiClient.SendHeartbeatAsync(_lastSeq).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await logger.WarningAsync("Heartbeat Errored", ex).ConfigureAwait(false);
                    }

                    await Task.Delay(intervalMillis, cancelToken).ConfigureAwait(false);
                }
                await logger.DebugAsync("Heartbeat Stopped").ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await logger.DebugAsync("Heartbeat Stopped").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync("Heartbeat Errored", ex).ConfigureAwait(false);
            }
        }
        private async Task WaitForGuildsAsync(CancellationToken cancelToken, ILogger logger)
        {
            //Wait for GUILD_AVAILABLEs
            try
            {
                await logger.DebugAsync("GuildDownloader Started").ConfigureAwait(false);
                while ((_unavailableGuilds != 0) && (Environment.TickCount - _lastGuildAvailableTime < 2000))
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
            var guildIds = Guilds.Where(x => !x.IsSynced).Select(x => x.Id).ToArray();
            if (guildIds.Length > 0)
                await ApiClient.SendGuildSyncAsync(guildIds).ConfigureAwait(false);
        }
    }
}
