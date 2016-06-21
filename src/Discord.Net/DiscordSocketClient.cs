using Discord.API.Gateway;
using Discord.Data;
using Discord.Extensions;
using Discord.Logging;
using Discord.Net.Converters;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    //TODO: Add event docstrings
    //TODO: Add reconnect logic (+ensure the heartbeat task to shut down)
    //TODO: Add resume logic
    public class DiscordSocketClient : DiscordClient, IDiscordClient
    {
        public event Func<Task> Connected, Disconnected;
        public event Func<Task> Ready;
        //public event Func<Channel> VoiceConnected, VoiceDisconnected;
        public event Func<IChannel, Task> ChannelCreated, ChannelDestroyed;
        public event Func<IChannel, IChannel, Task> ChannelUpdated;
        public event Func<IMessage, Task> MessageReceived, MessageDeleted;
        public event Func<IMessage, IMessage, Task> MessageUpdated;
        public event Func<IRole, Task> RoleCreated, RoleDeleted;
        public event Func<IRole, IRole, Task> RoleUpdated;
        public event Func<IGuild, Task> JoinedGuild, LeftGuild, GuildAvailable, GuildUnavailable, GuildDownloadedMembers;
        public event Func<IGuild, IGuild, Task> GuildUpdated;
        public event Func<IUser, Task> UserJoined, UserLeft, UserBanned, UserUnbanned;
        public event Func<IUser, IUser, Task> UserUpdated;
        public event Func<ISelfUser, ISelfUser, Task> CurrentUserUpdated;
        public event Func<IChannel, IUser, Task> UserIsTyping;
        public event Func<int, Task> LatencyUpdated;
        //TODO: Add PresenceUpdated? VoiceStateUpdated?

        private readonly ConcurrentQueue<ulong> _largeGuilds;
        private readonly Logger _gatewayLogger;
#if BENCHMARK
        private readonly Logger _benchmarkLogger;
#endif
        private readonly DataStoreProvider _dataStoreProvider;
        private readonly JsonSerializer _serializer;
        private readonly int _connectionTimeout, _reconnectDelay, _failedReconnectDelay;
        private readonly bool _enablePreUpdateEvents;
        private readonly int _largeThreshold;
        private readonly int _totalShards;

        private string _sessionId;
        private int _lastSeq;
        private ImmutableDictionary<string, VoiceRegion> _voiceRegions;
        private TaskCompletionSource<bool> _connectTask;
        private CancellationTokenSource _cancelToken;
        private Task _heartbeatTask, _guildDownloadTask, _reconnectTask;
        private long _heartbeatTime;
        private bool _isReconnecting;
        private int _unavailableGuilds;
        private long _lastGuildAvailableTime;

        /// <summary> Gets the shard if of this client. </summary>
        public int ShardId { get; }
        /// <summary> Gets the current connection state of this client. </summary>
        public ConnectionState ConnectionState { get; private set; }
        /// <summary> Gets the estimated round-trip latency, in milliseconds, to the gateway server. </summary>
        public int Latency { get; private set; }

        internal IWebSocketClient GatewaySocket { get; private set; }
        internal int MessageCacheSize { get; private set; }
        internal DataStore DataStore { get; private set; }

        internal CachedSelfUser CurrentUser => _currentUser as CachedSelfUser;
        internal IReadOnlyCollection<CachedGuild> Guilds => DataStore.Guilds;
        internal IReadOnlyCollection<CachedDMChannel> DMChannels => DataStore.DMChannels;
        internal IReadOnlyCollection<VoiceRegion> VoiceRegions => _voiceRegions.ToReadOnlyCollection();

        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordSocketClient()
            : this(new DiscordSocketConfig()) { }
        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordSocketClient(DiscordSocketConfig config)
            : base(config)
        {
            ShardId = config.ShardId;
            _totalShards = config.TotalShards;

            _connectionTimeout = config.ConnectionTimeout;
            _reconnectDelay = config.ReconnectDelay;
            _failedReconnectDelay = config.FailedReconnectDelay;
            _dataStoreProvider = config.DataStoreProvider;

            MessageCacheSize = config.MessageCacheSize;
            _enablePreUpdateEvents = config.EnablePreUpdateEvents;
            _largeThreshold = config.LargeThreshold;
            
            _gatewayLogger = _log.CreateLogger("Gateway");
#if BENCHMARK
            _benchmarkLogger = _log.CreateLogger("Benchmark");
#endif

            _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };
            _serializer.Error += (s, e) =>
            {
                _gatewayLogger.WarningAsync(e.ErrorContext.Error).GetAwaiter().GetResult();
                e.ErrorContext.Handled = true;
            };
            
            ApiClient.SentGatewayMessage += async opCode => await _gatewayLogger.DebugAsync($"Sent {(GatewayOpCode)opCode}").ConfigureAwait(false);
            ApiClient.ReceivedGatewayEvent += ProcessMessageAsync;
            ApiClient.Disconnected += async ex =>
            {
                if (ex != null)
                {
                    await _gatewayLogger.WarningAsync($"Connection Closed: {ex.Message}").ConfigureAwait(false);
                    await StartReconnectAsync().ConfigureAwait(false);
                }
                else
                    await _gatewayLogger.WarningAsync($"Connection Closed").ConfigureAwait(false);
            };
            GatewaySocket = config.WebSocketProvider();

            _voiceRegions = ImmutableDictionary.Create<string, VoiceRegion>();
            _largeGuilds = new ConcurrentQueue<ulong>();
        }

        protected override async Task OnLoginAsync()
        {
            var voiceRegions = await ApiClient.GetVoiceRegionsAsync().ConfigureAwait(false);
            _voiceRegions = voiceRegions.Select(x => new VoiceRegion(x)).ToImmutableDictionary(x => x.Id);
        }
        protected override async Task OnLogoutAsync()
        {
            if (ConnectionState != ConnectionState.Disconnected)
                await DisconnectInternalAsync().ConfigureAwait(false);

            _voiceRegions = ImmutableDictionary.Create<string, VoiceRegion>();
        }

        /// <inheritdoc />
        public async Task ConnectAsync()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                _isReconnecting = false;
                await ConnectInternalAsync().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ConnectInternalAsync()
        {
            if (LoginState != LoginState.LoggedIn)
                throw new InvalidOperationException("You must log in before connecting.");

            ConnectionState = ConnectionState.Connecting;
            await _gatewayLogger.InfoAsync("Connecting");
            try
            {
                _connectTask = new TaskCompletionSource<bool>();
                _cancelToken = new CancellationTokenSource();
                await ApiClient.ConnectAsync().ConfigureAwait(false);

                await _connectTask.Task.ConfigureAwait(false);
                
                ConnectionState = ConnectionState.Connected;
                await _gatewayLogger.InfoAsync("Connected");
            }
            catch (Exception)
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
                throw;
            }

            await Connected.RaiseAsync().ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task DisconnectAsync()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                _isReconnecting = false;
                await DisconnectInternalAsync().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task DisconnectInternalAsync()
        {
            ulong guildId;

            if (ConnectionState == ConnectionState.Disconnected) return;
            ConnectionState = ConnectionState.Disconnecting;
            await _gatewayLogger.InfoAsync("Disconnecting");

            //Signal tasks to complete
            try { _cancelToken.Cancel(); } catch { }

            //Disconnect from server
            await ApiClient.DisconnectAsync().ConfigureAwait(false);

            //Wait for tasks to complete
            var heartbeatTask = _heartbeatTask;
            if (heartbeatTask != null)
                await heartbeatTask.ConfigureAwait(false);
            _heartbeatTask = null;

            var guildDownloadTask = _guildDownloadTask;
            if (guildDownloadTask != null)
                await guildDownloadTask.ConfigureAwait(false);
            _guildDownloadTask = null;

            //Clear large guild queue
            while (_largeGuilds.TryDequeue(out guildId)) { }

            ConnectionState = ConnectionState.Disconnected;
            await _gatewayLogger.InfoAsync("Disconnected").ConfigureAwait(false);

            await Disconnected.RaiseAsync().ConfigureAwait(false);
        }
        private async Task StartReconnectAsync()
        {
            //TODO: Is this thread-safe?
            await _log.InfoAsync("Debug", "Trying to reconnect...").ConfigureAwait(false);
            if (_reconnectTask != null) return;

            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_reconnectTask != null) return;
                _isReconnecting = true;
                _reconnectTask = ReconnectInternalAsync();
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ReconnectInternalAsync()
        {
            await _log.InfoAsync("Debug", "Reconnecting...").ConfigureAwait(false);
            try
            {
                int nextReconnectDelay = 1000;
                while (_isReconnecting)
                {
                    try
                    {
                        await Task.Delay(nextReconnectDelay).ConfigureAwait(false);
                        nextReconnectDelay *= 2;
                        if (nextReconnectDelay > 30000)
                            nextReconnectDelay = 30000;

                        await _connectionLock.WaitAsync().ConfigureAwait(false);
                        try
                        {
                            await ConnectInternalAsync().ConfigureAwait(false);
                        }
                        finally { _connectionLock.Release(); }
                        return;
                    }
                    catch (Exception ex)
                    {
                        await _gatewayLogger.WarningAsync("Reconnect failed", ex).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                await _connectionLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    _isReconnecting = false;
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
        public override Task<IReadOnlyCollection<IUserGuild>> GetGuildsAsync()
        {
            return Task.FromResult<IReadOnlyCollection<IUserGuild>>(Guilds);
        }
        internal CachedGuild AddGuild(API.Gateway.ExtendedGuild model, DataStore dataStore)
        {
            var guild = new CachedGuild(this, model, dataStore);
            dataStore.AddGuild(guild);
            if (model.Large)
                _largeGuilds.Enqueue(model.Id);
            return guild;
        }
        internal CachedGuild RemoveGuild(ulong id)
        {
            var guild = DataStore.RemoveGuild(id);
            foreach (var channel in guild.Channels)
                guild.RemoveChannel(channel.Id);
            foreach (var user in guild.Members)
                guild.RemoveUser(user.Id);
            return guild;
        }
        
        /// <inheritdoc />
        public override Task<IChannel> GetChannelAsync(ulong id)
        {
            return Task.FromResult<IChannel>(DataStore.GetChannel(id));
        }
        public override Task<IReadOnlyCollection<IDMChannel>> GetDMChannelsAsync()
        {
            return Task.FromResult<IReadOnlyCollection<IDMChannel>>(DMChannels);
        }
        internal CachedDMChannel AddDMChannel(API.Channel model, DataStore dataStore)
        {
            var recipient = GetOrAddUser(model.Recipient.Value, dataStore);
            var channel = new CachedDMChannel(this, new CachedDMUser(recipient), model);
            recipient.AddRef();
            dataStore.AddDMChannel(channel);
            return channel;
        }
        internal CachedDMChannel RemoveDMChannel(ulong id)
        {            
            var dmChannel = DataStore.RemoveDMChannel(id);
            if (dmChannel != null)
            {
                var recipient = dmChannel.Recipient;
                recipient.User.RemoveRef(this);
            }
            return dmChannel;
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
        internal CachedGlobalUser GetOrAddUser(API.User model, DataStore dataStore)
        {
            var user = dataStore.GetOrAddUser(model.Id, _ => new CachedGlobalUser(model));
            user.AddRef();
            return user;
        }
        internal CachedGlobalUser RemoveUser(ulong id)
        {
            return DataStore.RemoveUser(id);
        }

        /// <summary> Downloads the members list for all large guilds. </summary>
        public Task DownloadAllMembersAsync()
            => DownloadMembersAsync(DataStore.Guilds.Where(x => !x.HasAllMembers));
        /// <summary> Downloads the members list for the provided guilds, if they don't have a complete list. </summary>
        public async Task DownloadMembersAsync(IEnumerable<IGuild> guilds)
        {
            const short batchSize = 50;
            var cachedGuilds = guilds.Select(x => x as CachedGuild).ToArray();
            if (cachedGuilds.Length == 0)
                return;
            else if (cachedGuilds.Length == 1)
            {
                await cachedGuilds[0].DownloadMembersAsync().ConfigureAwait(false);
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

                            if (_sessionId != null)
                                await ApiClient.SendResumeAsync(_sessionId, _lastSeq).ConfigureAwait(false);
                            else
                                await ApiClient.SendIdentifyAsync().ConfigureAwait(false);
                            _heartbeatTime = 0;
                            _heartbeatTask = RunHeartbeatAsync(data.HeartbeatInterval, _cancelToken.Token);
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
                                var latency = (int)(Environment.TickCount - _heartbeatTime);
                                _heartbeatTime = 0;
                                await _gatewayLogger.VerboseAsync($"Latency = {latency} ms").ConfigureAwait(false);
                                Latency = latency;

                                await LatencyUpdated.RaiseAsync(latency).ConfigureAwait(false);
                            }
                        }
                        break;
                    case GatewayOpCode.InvalidSession:
                        {
                            await _gatewayLogger.DebugAsync("Received InvalidSession").ConfigureAwait(false);
                            await _gatewayLogger.WarningAsync("Failed to resume previous session");

                            _sessionId = null;
                            _lastSeq = 0;
                            await ApiClient.SendIdentifyAsync().ConfigureAwait(false);
                        }
                        break;
                    case GatewayOpCode.Reconnect:
                        {
                            await _gatewayLogger.DebugAsync("Received Reconnect").ConfigureAwait(false);
                            await _gatewayLogger.WarningAsync("Server requested a reconnect");

                            await StartReconnectAsync().ConfigureAwait(false);
                        }
                        break;
                    case GatewayOpCode.Dispatch:
                        switch (type)
                        {
                            //Global
                            case "READY":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (READY)").ConfigureAwait(false);
                                    
                                    var data = (payload as JToken).ToObject<ReadyEvent>(_serializer);
                                    var dataStore = _dataStoreProvider(ShardId, _totalShards, data.Guilds.Length, data.PrivateChannels.Length);

                                    var currentUser = new CachedSelfUser(this, data.User);
                                    int unavailableGuilds = 0;
                                    //dataStore.GetOrAddUser(data.User.Id, _ => currentUser);

                                    for (int i = 0; i < data.Guilds.Length; i++)
                                    {
                                        var model = data.Guilds[i];
                                        AddGuild(model, dataStore);
                                        if (model.Unavailable == true)
                                            unavailableGuilds++;
                                    }
                                    for (int i = 0; i < data.PrivateChannels.Length; i++)
                                        AddDMChannel(data.PrivateChannels[i], dataStore);

                                    _sessionId = data.SessionId;
                                    _currentUser = currentUser;
                                    _unavailableGuilds = unavailableGuilds;
                                    _lastGuildAvailableTime = Environment.TickCount;
                                    DataStore = dataStore;

                                    _guildDownloadTask = WaitForGuildsAsync(_cancelToken.Token);

                                    await Ready.RaiseAsync().ConfigureAwait(false);

                                    _connectTask.TrySetResult(true); //Signal the .Connect() call to complete
                                    await _gatewayLogger.InfoAsync("Ready");
                                }
                                break;

                            //Guilds
                            case "GUILD_CREATE":
                                {
                                    var data = (payload as JToken).ToObject<ExtendedGuild>(_serializer);

                                    if (data.Unavailable == false)
                                    {
                                        type = "GUILD_AVAILABLE";
                                        _lastGuildAvailableTime = Environment.TickCount;
                                    }
                                    await _gatewayLogger.DebugAsync($"Received Dispatch ({type})").ConfigureAwait(false);

                                    CachedGuild guild;
                                    if (data.Unavailable != false)
                                    {
                                        guild = AddGuild(data, DataStore);
                                        await JoinedGuild.RaiseAsync(guild).ConfigureAwait(false);
                                        await _gatewayLogger.InfoAsync($"Joined {data.Name}").ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        guild = DataStore.GetGuild(data.Id);
                                        if (guild != null)
                                            guild.Update(data, UpdateSource.WebSocket, DataStore);
                                        else
                                        {
                                            await _gatewayLogger.WarningAsync($"{type} referenced an unknown guild.").ConfigureAwait(false);
                                            return;
                                        }

                                        var unavailableGuilds = _unavailableGuilds;
                                        if (unavailableGuilds != 0)
                                            _unavailableGuilds = unavailableGuilds - 1;
                                    }

                                    if (data.Unavailable != true)
                                    {
                                        await _gatewayLogger.VerboseAsync($"Connected to {data.Name}").ConfigureAwait(false);
                                        await GuildAvailable.RaiseAsync(guild).ConfigureAwait(false);
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
                                        var before = _enablePreUpdateEvents ? guild.Clone() : null;
                                        guild.Update(data, UpdateSource.WebSocket);
                                        await GuildUpdated.RaiseAsync(before, guild).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("GUILD_UPDATE referenced an unknown guild.");
                                        return;
                                    }
                                }
                                break;
                            case "GUILD_DELETE":
                                {
                                    var data = (payload as JToken).ToObject<ExtendedGuild>(_serializer);
                                    if (data.Unavailable == true)
                                        type = "GUILD_UNAVAILABLE";
                                    await _gatewayLogger.DebugAsync($"Received Dispatch ({type})").ConfigureAwait(false);

                                    var guild = RemoveGuild(data.Id);
                                    if (guild != null)
                                    {
                                        foreach (var member in guild.Members)
                                            member.User.RemoveRef(this);

                                        await GuildUnavailable.RaiseAsync(guild).ConfigureAwait(false);
                                        await _gatewayLogger.VerboseAsync($"Disconnected from {data.Name}").ConfigureAwait(false);
                                        if (data.Unavailable != true)
                                        {
                                            await LeftGuild.RaiseAsync(guild).ConfigureAwait(false);
                                            await _gatewayLogger.InfoAsync($"Left {data.Name}").ConfigureAwait(false);
                                        }
                                        else
                                            _unavailableGuilds++;

                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync($"{type} referenced an unknown guild.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;

                            //Channels
                            case "CHANNEL_CREATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_CREATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                                    ICachedChannel channel = null;
                                    if (!data.IsPrivate)
                                    {
                                        var guild = DataStore.GetGuild(data.GuildId.Value);
                                        if (guild != null)
                                            guild.AddChannel(data, DataStore);
                                        else
                                        {
                                            await _gatewayLogger.WarningAsync("CHANNEL_CREATE referenced an unknown guild.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                        channel = AddDMChannel(data, DataStore);
                                    if (channel != null)
                                        await ChannelCreated.RaiseAsync(channel).ConfigureAwait(false);
                                }
                                break;
                            case "CHANNEL_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                                    var channel = DataStore.GetChannel(data.Id);
                                    if (channel != null)
                                    {
                                        var before = _enablePreUpdateEvents ? channel.Clone() : null;
                                        channel.Update(data, UpdateSource.WebSocket);
                                        await ChannelUpdated.RaiseAsync(before, channel).ConfigureAwait(false);
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

                                    ICachedChannel channel = null;
                                    var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                                    if (!data.IsPrivate)
                                    {
                                        var guild = DataStore.GetGuild(data.GuildId.Value);
                                        if (guild != null)
                                            channel = guild.RemoveChannel(data.Id);
                                        else
                                        {
                                            await _gatewayLogger.WarningAsync("CHANNEL_DELETE referenced an unknown guild.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                        channel = RemoveDMChannel(data.Recipient.Value.Id);
                                    if (channel != null)
                                        await ChannelDestroyed.RaiseAsync(channel).ConfigureAwait(false);
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
                                        var user = guild.AddUser(data, DataStore);
                                        await UserJoined.RaiseAsync(user).ConfigureAwait(false);
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
                                        if (user != null)
                                        {
                                            var before = _enablePreUpdateEvents ? user.Clone() : null;
                                            user.Update(data, UpdateSource.WebSocket);
                                            await UserUpdated.RaiseAsync(before, user).ConfigureAwait(false);
                                        }
                                        else
                                        {
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
                                        if (user != null)
                                        {
                                            user.User.RemoveRef(this);
                                            await UserLeft.RaiseAsync(user).ConfigureAwait(false);
                                        }
                                        else
                                        {
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
                                            guild.AddUser(memberModel, DataStore);

                                        if (guild.DownloadedMemberCount >= guild.MemberCount) //Finished downloading for there
                                        {
                                            guild.CompleteDownloadMembers();
                                            await GuildDownloadedMembers.RaiseAsync(guild).ConfigureAwait(false);
                                        }
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("GUILD_MEMBERS_CHUNK referenced an unknown guild.").ConfigureAwait(false);
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
                                        await RoleCreated.RaiseAsync(role).ConfigureAwait(false);
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
                                            var before = _enablePreUpdateEvents ? role.Clone() : null;
                                            role.Update(data.Role, UpdateSource.WebSocket);
                                            await RoleUpdated.RaiseAsync(before, role).ConfigureAwait(false);
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
                                            await RoleDeleted.RaiseAsync(role).ConfigureAwait(false);
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
                                        await UserBanned.RaiseAsync(new User(data.User)).ConfigureAwait(false);
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
                                        await UserUnbanned.RaiseAsync(new User(data.User)).ConfigureAwait(false);
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
                                    var channel = DataStore.GetChannel(data.ChannelId) as ICachedMessageChannel;
                                    if (channel != null)
                                    {
                                        var author = channel.GetUser(data.Author.Value.Id, true);

                                        if (author != null)
                                        {
                                            var msg = channel.AddMessage(author, data);
                                            await MessageReceived.RaiseAsync(msg).ConfigureAwait(false);
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
                                    var channel = DataStore.GetChannel(data.ChannelId) as ICachedMessageChannel;
                                    if (channel != null)
                                    {
                                        IMessage before = null, after = null;
                                        CachedMessage cachedMsg = channel.GetMessage(data.Id);
                                        if (cachedMsg != null)
                                        {
                                            before = _enablePreUpdateEvents ? cachedMsg.Clone() : null;
                                            cachedMsg.Update(data, UpdateSource.WebSocket);
                                            after = cachedMsg;
                                        }
                                        else if (data.Author.IsSpecified)
                                        {
                                            //Edited message isnt in cache, create a detached one
                                            var author = channel.GetUser(data.Author.Value.Id, true);
                                            if (author != null)
                                                after = new Message(channel, author, data);
                                        }
                                        if (after != null)
                                            await MessageUpdated.RaiseAsync(before, after).ConfigureAwait(false);
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
                                    var channel = DataStore.GetChannel(data.ChannelId) as ICachedMessageChannel;
                                    if (channel != null)
                                    {
                                        var msg = channel.RemoveMessage(data.Id);
                                        await MessageDeleted.RaiseAsync(msg).ConfigureAwait(false);
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
                                    var channel = DataStore.GetChannel(data.ChannelId) as ICachedMessageChannel;
                                    if (channel != null)
                                    {
                                        foreach (var id in data.Ids)
                                        {
                                            var msg = channel.RemoveMessage(id);
                                            await MessageDeleted.RaiseAsync(msg).ConfigureAwait(false);
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
                                        guild.UpdatePresence(data, DataStore);
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
                                    var channel = DataStore.GetChannel(data.ChannelId) as ICachedMessageChannel;
                                    if (channel != null)
                                    {
                                        var user = channel.GetUser(data.UserId, true);
                                        if (user != null)
                                            await UserIsTyping.RaiseAsync(channel, user).ConfigureAwait(false);
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
                                        var guild = DataStore.GetGuild(data.GuildId.Value);
                                        if (guild != null)
                                        {
                                            if (data.ChannelId == null)
                                                guild.RemoveVoiceState(data.UserId);
                                            else
                                                guild.AddOrUpdateVoiceState(data, DataStore);

                                            var user = guild.GetUser(data.UserId);
                                            if (user != null)
                                                user.Update(data, UpdateSource.WebSocket);
                                        }
                                        else
                                        {
                                            await _gatewayLogger.WarningAsync("VOICE_STATE_UPDATE referenced an unknown guild.").ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                }
                                break;

                            //Settings
                            case "USER_UPDATE":
                                {
                                    await _gatewayLogger.DebugAsync("Received Dispatch (USER_UPDATE)").ConfigureAwait(false);

                                    var data = (payload as JToken).ToObject<API.User>(_serializer);
                                    if (data.Id == CurrentUser.Id)
                                    {
                                        var before = _enablePreUpdateEvents ? CurrentUser.Clone() : null;
                                        CurrentUser.Update(data, UpdateSource.WebSocket);
                                        await CurrentUserUpdated.RaiseAsync(before, CurrentUser).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await _gatewayLogger.WarningAsync("Received USER_UPDATE for wrong user.").ConfigureAwait(false);
                                        return;
                                    }
                                }
                                break;

                            //Ignored
                            case "USER_SETTINGS_UPDATE":
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (USER_SETTINGS_UPDATE)").ConfigureAwait(false);
                                return;
                            case "MESSAGE_ACK": //TODO: Add (User only)
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (MESSAGE_ACK)").ConfigureAwait(false);
                                return;
                            case "GUILD_EMOJIS_UPDATE": //TODO: Add
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (GUILD_EMOJIS_UPDATE)").ConfigureAwait(false);
                                return;
                            case "GUILD_INTEGRATIONS_UPDATE": //TODO: Add
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (GUILD_INTEGRATIONS_UPDATE)").ConfigureAwait(false);
                                return;
                            case "VOICE_SERVER_UPDATE": //TODO: Add
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (VOICE_SERVER_UPDATE)").ConfigureAwait(false);
                                return;
                            case "RESUMED": //TODO: Add
                                await _gatewayLogger.DebugAsync("Ignored Dispatch (RESUMED)").ConfigureAwait(false);
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
        private async Task RunHeartbeatAsync(int intervalMillis, CancellationToken cancelToken)
        {
            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    await Task.Delay(intervalMillis, cancelToken).ConfigureAwait(false);

                    if (_heartbeatTime != 0) //Server never responded to our last heartbeat
                    {
                        if (ConnectionState == ConnectionState.Connected && (_guildDownloadTask?.IsCompleted ?? false))
                        {
                            await _gatewayLogger.WarningAsync("Server missed last heartbeat").ConfigureAwait(false);
                            await StartReconnectAsync().ConfigureAwait(false);
                            return;
                        }
                    }
                    else
                        _heartbeatTime = Environment.TickCount;
                    await ApiClient.SendHeartbeatAsync(_lastSeq).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) { }
        }

        private async Task WaitForGuildsAsync(CancellationToken cancelToken)
        {
            while ((_unavailableGuilds != 0) && (Environment.TickCount - _lastGuildAvailableTime < 2000))
                await Task.Delay(500, cancelToken).ConfigureAwait(false);
        }
        public async Task WaitForGuildsAsync()
        {
            var downloadTask = _guildDownloadTask;
            if (downloadTask != null)
                await _guildDownloadTask.ConfigureAwait(false);
        }
    }
}
