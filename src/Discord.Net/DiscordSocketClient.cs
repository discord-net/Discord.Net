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
using System.Diagnostics;
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
        private CancellationTokenSource _heartbeatCancelToken;
        private Task _heartbeatTask;
        private long _heartbeatTime;

        /// <summary> Gets the shard if of this client. </summary>
        public int ShardId { get; }
        /// <summary> Gets the current connection state of this client. </summary>
        public ConnectionState ConnectionState { get; private set; }
        /// <summary> Gets the estimated round-trip latency, in milliseconds, to the gateway server. </summary>
        public int Latency { get; private set; }
        internal IWebSocketClient GatewaySocket { get; private set; }
        internal int MessageCacheSize { get; private set; }
        //internal bool UsePermissionCache { get; private set; }
        internal DataStore DataStore { get; private set; }

        internal CachedSelfUser CurrentUser => _currentUser as CachedSelfUser;
        internal IReadOnlyCollection<CachedGuild> Guilds
        {
            get
            {
                var guilds = DataStore.Guilds;
                return guilds.ToReadOnlyCollection(guilds);
            }
        }
        internal IReadOnlyCollection<CachedDMChannel> DMChannels
        {
            get
            {
                var users = DataStore.Users;
                return users.Select(x => x.DMChannel).Where(x => x != null).ToReadOnlyCollection(users);
            }
        }
        internal IReadOnlyCollection<VoiceRegion> VoiceRegions => _voiceRegions.ToReadOnlyCollection();

        /// <summary> Creates a new discord client using the REST and WebSocket APIs. </summary>
        public DiscordSocketClient()
            : this(new DiscordSocketConfig()) { }
        /// <summary> Creates a new discord client using the REST and WebSocket APIs. </summary>
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
            //UsePermissionCache = config.UsePermissionsCache;
            _enablePreUpdateEvents = config.EnablePreUpdateEvents;
            _largeThreshold = config.LargeThreshold;
            
            _gatewayLogger = _log.CreateLogger("Gateway");
#if BENCHMARK
            _benchmarkLogger = _log.CreateLogger("Benchmark");
#endif

            _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };
            
            ApiClient.SentGatewayMessage += async opCode => await _gatewayLogger.Debug($"Sent {(GatewayOpCode)opCode}");
            ApiClient.ReceivedGatewayEvent += ProcessMessage;
            GatewaySocket = config.WebSocketProvider();

            _voiceRegions = ImmutableDictionary.Create<string, VoiceRegion>();
            _largeGuilds = new ConcurrentQueue<ulong>();
        }

        protected override async Task OnLogin()
        {
            var voiceRegions = await ApiClient.GetVoiceRegions().ConfigureAwait(false);
            _voiceRegions = voiceRegions.Select(x => new VoiceRegion(x)).ToImmutableDictionary(x => x.Id);
        }
        protected override async Task OnLogout()
        {
            if (ConnectionState != ConnectionState.Disconnected)
                await DisconnectInternal().ConfigureAwait(false);

            _voiceRegions = ImmutableDictionary.Create<string, VoiceRegion>();
        }

        /// <inheritdoc />
        public async Task Connect()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await ConnectInternal().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ConnectInternal()
        {
            if (LoginState != LoginState.LoggedIn)
                throw new InvalidOperationException("You must log in before connecting.");

            ConnectionState = ConnectionState.Connecting;
            try
            {
                _connectTask = new TaskCompletionSource<bool>();
                _heartbeatCancelToken = new CancellationTokenSource();
                await ApiClient.Connect().ConfigureAwait(false);

                await _connectTask.Task.ConfigureAwait(false);
                ConnectionState = ConnectionState.Connected;
            }
            catch (Exception)
            {
                await DisconnectInternal().ConfigureAwait(false);
                throw;
            }

            await Connected.Raise().ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task Disconnect()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternal().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task DisconnectInternal()
        {
            ulong guildId;

            if (ConnectionState == ConnectionState.Disconnected) return;
            ConnectionState = ConnectionState.Disconnecting;

            await ApiClient.Disconnect().ConfigureAwait(false);
            await _heartbeatTask.ConfigureAwait(false);
            while (_largeGuilds.TryDequeue(out guildId)) { }

            ConnectionState = ConnectionState.Disconnected;

            await Disconnected.Raise().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override Task<IVoiceRegion> GetVoiceRegion(string id)
        {
            VoiceRegion region;
            if (_voiceRegions.TryGetValue(id, out region))
                return Task.FromResult<IVoiceRegion>(region);
            return Task.FromResult<IVoiceRegion>(null);
        }

        /// <inheritdoc />
        public override Task<IGuild> GetGuild(ulong id)
        {
            return Task.FromResult<IGuild>(DataStore.GetGuild(id));
        }
        internal CachedGuild AddCachedGuild(API.Gateway.ExtendedGuild model, DataStore dataStore = null)
        {
            dataStore = dataStore ?? DataStore;

            var guild = new CachedGuild(this, model, dataStore);
            if (model.Unavailable != true)
            {
                for (int i = 0; i < model.Channels.Length; i++)
                    AddCachedChannel(guild, model.Channels[i], dataStore);
            }
            dataStore.AddGuild(guild);
            if (model.Large)
                _largeGuilds.Enqueue(model.Id);
            return guild;
        }
        internal CachedGuild RemoveCachedGuild(ulong id, DataStore dataStore = null)
        {
            dataStore = dataStore ?? DataStore;

            var guild = dataStore.RemoveGuild(id);
            foreach (var channel in guild.Channels)
                guild.RemoveCachedChannel(channel.Id);
            foreach (var user in guild.Members)
                guild.RemoveCachedUser(user.Id);
            return guild;
        }

        /// <inheritdoc />
        public override Task<IChannel> GetChannel(ulong id)
        {
            return Task.FromResult<IChannel>(DataStore.GetChannel(id));
        }
        internal ICachedGuildChannel AddCachedChannel(CachedGuild guild, API.Channel model, DataStore dataStore = null)
        {
            dataStore = dataStore ?? DataStore;

            var channel = guild.AddCachedChannel(model);
            dataStore.AddChannel(channel);
            return channel;
        }
        internal CachedDMChannel AddCachedDMChannel(API.Channel model, DataStore dataStore = null)
        {
            dataStore = dataStore ?? DataStore;

            var recipient = GetOrAddCachedUser(model.Recipient, dataStore);
            var channel = recipient.AddDMChannel(model);
            dataStore.AddChannel(channel);
            return channel;
        }
        internal ICachedChannel RemoveCachedChannel(ulong id, DataStore dataStore = null)
        {
            dataStore = dataStore ?? DataStore;

            //TODO: C#7 Typeswitch Candidate
            var channel = DataStore.RemoveChannel(id);

            var guildChannel = channel as ICachedGuildChannel;
            if (guildChannel != null)
            {
                guildChannel.Guild.RemoveCachedChannel(guildChannel.Id);
                return channel;
            }

            var dmChannel = channel as CachedDMChannel;
            if (dmChannel != null)
            {
                var recipient = dmChannel.Recipient;
                recipient.RemoveDMChannel(id);
                return channel;
            }

            return null;
        }

        /// <inheritdoc />
        public override Task<IUser> GetUser(ulong id)
        {
            return Task.FromResult<IUser>(DataStore.GetUser(id));
        }
        /// <inheritdoc />
        public override Task<IUser> GetUser(string username, string discriminator)
        {
            return Task.FromResult<IUser>(DataStore.Users.Where(x => x.Discriminator == discriminator && x.Username == username).FirstOrDefault());
        }
        internal CachedPublicUser GetOrAddCachedUser(API.User model, DataStore dataStore = null)
        {
            dataStore = dataStore ?? DataStore;

            var user = dataStore.GetOrAddUser(model.Id, _ => new CachedPublicUser(this, model));
            user.AddRef();
            return user;
        }
        internal CachedPublicUser RemoveCachedUser(ulong id, DataStore dataStore = null)
        {
            dataStore = dataStore ?? DataStore;

            var user = dataStore.RemoveUser(id);
            return user;
        }

        /// <summary> Downloads the members list for all large guilds. </summary>
        public Task DownloadAllMembers()
            => DownloadMembers(DataStore.Guilds.Where(x => !x.HasAllMembers));
        /// <summary> Downloads the members list for the provided guilds, if they don't have a complete list. </summary>
        public async Task DownloadMembers(IEnumerable<IGuild> guilds)
        {
            const short batchSize = 50;
            var cachedGuilds = guilds.Select(x => x as CachedGuild).ToArray();
            if (cachedGuilds.Length == 0)
                return;
            else if (cachedGuilds.Length == 1)
            {
                await cachedGuilds[0].DownloadMembers().ConfigureAwait(false);
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

                await ApiClient.SendRequestMembers(batchIds).ConfigureAwait(false);

                if (isLast && batchCount > 1)
                    await Task.WhenAll(batchTasks.Take(count)).ConfigureAwait(false);
                else
                    await Task.WhenAll(batchTasks).ConfigureAwait(false);
            }
        }

        private async Task ProcessMessage(GatewayOpCode opCode, int? seq, string type, object payload)
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
                                await _gatewayLogger.Debug($"Received Hello").ConfigureAwait(false);
                                var data = (payload as JToken).ToObject<HelloEvent>(_serializer);

                                await ApiClient.SendIdentify().ConfigureAwait(false);
                                _heartbeatTask = RunHeartbeat(data.HeartbeatInterval, _heartbeatCancelToken.Token);
                            }
                            break;
                        case GatewayOpCode.HeartbeatAck:
                            {
                                await _gatewayLogger.Debug($"Received HeartbeatAck").ConfigureAwait(false);

                                var latency = (int)(Environment.TickCount - _heartbeatTime);
                                await _gatewayLogger.Debug($"Latency = {latency} ms").ConfigureAwait(false);
                                Latency = latency;

                                await LatencyUpdated.Raise(latency).ConfigureAwait(false);
                            }
                            break;
                        case GatewayOpCode.Dispatch:
                            switch (type)
                            {
                                //Global
                                case "READY":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (READY)").ConfigureAwait(false);

                                        //TODO: Make downloading large guilds optional
                                        var data = (payload as JToken).ToObject<ReadyEvent>(_serializer);
                                        var dataStore = _dataStoreProvider(ShardId, _totalShards, data.Guilds.Length, data.PrivateChannels.Length);

                                        _currentUser = new CachedSelfUser(this, data.User);

                                        for (int i = 0; i < data.Guilds.Length; i++)
                                            AddCachedGuild(data.Guilds[i], dataStore);
                                        for (int i = 0; i < data.PrivateChannels.Length; i++)
                                            AddCachedDMChannel(data.PrivateChannels[i], dataStore);

                                        _sessionId = data.SessionId;
                                        DataStore = dataStore;

                                        await Ready.Raise().ConfigureAwait(false);

                                        _connectTask.TrySetResult(true); //Signal the .Connect() call to complete
                                    }
                                    break;

                                //Guilds
                                case "GUILD_CREATE":
                                    {
                                        var data = (payload as JToken).ToObject<ExtendedGuild>(_serializer);
                                        var guild = new CachedGuild(this, data, DataStore);
                                        DataStore.AddGuild(guild);

                                        if (data.Unavailable == false)
                                            type = "GUILD_AVAILABLE";
                                        await _gatewayLogger.Debug($"Received Dispatch ({type})").ConfigureAwait(false);

                                        if (data.Unavailable != false)
                                            await JoinedGuild.Raise(guild).ConfigureAwait(false);

                                        await GuildAvailable.Raise(guild).ConfigureAwait(false);
                                    }
                                    break;
                                case "GUILD_UPDATE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (GUILD_UPDATE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<API.Guild>(_serializer);
                                        var guild = DataStore.GetGuild(data.Id);
                                        if (guild != null)
                                        {
                                            var before = _enablePreUpdateEvents ? guild.Clone() : null;
                                            guild.Update(data, UpdateSource.WebSocket);
                                            await GuildUpdated.Raise(before, guild).ConfigureAwait(false);
                                        }
                                        else
                                            await _gatewayLogger.Warning("GUILD_UPDATE referenced an unknown guild.");
                                    }
                                    break;
                                case "GUILD_DELETE":
                                    {
                                        var data = (payload as JToken).ToObject<ExtendedGuild>(_serializer);
                                        if (data.Unavailable == true)
                                            type = "GUILD_UNAVAILABLE";
                                        await _gatewayLogger.Debug($"Received Dispatch ({type})").ConfigureAwait(false);

                                        var guild = DataStore.RemoveGuild(data.Id);
                                        if (guild != null)
                                        {
                                            await GuildUnavailable.Raise(guild).ConfigureAwait(false);
                                            if (data.Unavailable != true)
                                                await LeftGuild.Raise(guild).ConfigureAwait(false);
                                            foreach (var member in guild.Members)
                                                member.User.RemoveRef();
                                        }
                                        else
                                            await _gatewayLogger.Warning($"{type} referenced an unknown guild.").ConfigureAwait(false);
                                    }
                                    break;

                                //Channels
                                case "CHANNEL_CREATE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (CHANNEL_CREATE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                                        ICachedChannel channel = null;
                                        if (data.GuildId != null)
                                        {
                                            var guild = DataStore.GetGuild(data.GuildId.Value);
                                            if (guild != null)
                                            {
                                                channel = guild.AddCachedChannel(data);
                                                DataStore.AddChannel(channel);
                                            }
                                            else
                                                await _gatewayLogger.Warning("CHANNEL_CREATE referenced an unknown guild.");
                                        }
                                        else
                                            channel = AddCachedDMChannel(data);
                                        if (channel != null)
                                            await ChannelCreated.Raise(channel);
                                    }
                                    break;
                                case "CHANNEL_UPDATE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (CHANNEL_UPDATE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                                        var channel = DataStore.GetChannel(data.Id);
                                        if (channel != null)
                                        {
                                            var before = _enablePreUpdateEvents ? channel.Clone() : null;
                                            channel.Update(data, UpdateSource.WebSocket);
                                            await ChannelUpdated.Raise(before, channel);
                                        }
                                        else
                                            await _gatewayLogger.Warning("CHANNEL_UPDATE referenced an unknown channel.");
                                    }
                                    break;
                                case "CHANNEL_DELETE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (CHANNEL_DELETE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                                        var channel = RemoveCachedChannel(data.Id);
                                        if (channel != null)
                                            await ChannelDestroyed.Raise(channel);
                                        else
                                            await _gatewayLogger.Warning("CHANNEL_DELETE referenced an unknown channel.");
                                    }
                                    break;

                                //Members
                                case "GUILD_MEMBER_ADD":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (GUILD_MEMBER_ADD)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<GuildMemberAddEvent>(_serializer);
                                        var guild = DataStore.GetGuild(data.GuildId);
                                        if (guild != null)
                                        {
                                            var user = guild.AddCachedUser(data);
                                            await UserJoined.Raise(user).ConfigureAwait(false);
                                        }
                                        else
                                            await _gatewayLogger.Warning("GUILD_MEMBER_ADD referenced an unknown guild.");
                                    }
                                    break;
                                case "GUILD_MEMBER_UPDATE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (GUILD_MEMBER_UPDATE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<GuildMemberUpdateEvent>(_serializer);
                                        var guild = DataStore.GetGuild(data.GuildId);
                                        if (guild != null)
                                        {
                                            var user = guild.GetCachedUser(data.User.Id);
                                            if (user != null)
                                            {
                                                var before = _enablePreUpdateEvents ? user.Clone() : null;
                                                user.Update(data, UpdateSource.WebSocket);
                                                await UserUpdated.Raise(before, user);
                                            }
                                            else
                                                await _gatewayLogger.Warning("GUILD_MEMBER_UPDATE referenced an unknown user.");
                                        }
                                        else
                                            await _gatewayLogger.Warning("GUILD_MEMBER_UPDATE referenced an unknown guild.");
                                    }
                                    break;
                                case "GUILD_MEMBER_REMOVE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (GUILD_MEMBER_REMOVE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<GuildMemberRemoveEvent>(_serializer);
                                        var guild = DataStore.GetGuild(data.GuildId);
                                        if (guild != null)
                                        {
                                            var user = guild.RemoveCachedUser(data.User.Id);
                                            if (user != null)
                                            {
                                                user.User.RemoveRef();
                                                await UserLeft.Raise(user);
                                            }
                                            else
                                                await _gatewayLogger.Warning("GUILD_MEMBER_REMOVE referenced an unknown user.");
                                        }
                                        else
                                            await _gatewayLogger.Warning("GUILD_MEMBER_REMOVE referenced an unknown guild.");
                                    }
                                    break;
                                case "GUILD_MEMBERS_CHUNK":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (GUILD_MEMBERS_CHUNK)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<GuildMembersChunkEvent>(_serializer);
                                        var guild = DataStore.GetGuild(data.GuildId);
                                        if (guild != null)
                                        {
                                            foreach (var memberModel in data.Members)
                                                guild.AddCachedUser(memberModel);

                                            if (guild.DownloadedMemberCount >= guild.MemberCount) //Finished downloading for there
                                            {
                                                guild.CompleteDownloadMembers();
                                                await GuildDownloadedMembers.Raise(guild).ConfigureAwait(false);
                                            }
                                        }
                                        else
                                            await _gatewayLogger.Warning("GUILD_MEMBERS_CHUNK referenced an unknown guild.");
                                    }
                                    break;

                                //Roles
                                case "GUILD_ROLE_CREATE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (GUILD_ROLE_CREATE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<GuildRoleCreateEvent>(_serializer);
                                        var guild = DataStore.GetGuild(data.GuildId);
                                        if (guild != null)
                                        {
                                            var role = guild.AddCachedRole(data.Role);
                                            await RoleCreated.Raise(role).ConfigureAwait(false);
                                        }
                                        else
                                            await _gatewayLogger.Warning("GUILD_ROLE_CREATE referenced an unknown guild.");
                                    }
                                    break;
                                case "GUILD_ROLE_UPDATE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (GUILD_ROLE_UPDATE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<GuildRoleUpdateEvent>(_serializer);
                                        var guild = DataStore.GetGuild(data.GuildId);
                                        if (guild != null)
                                        {
                                            var role = guild.GetRole(data.Role.Id);
                                            if (role != null)
                                            {
                                                var before = _enablePreUpdateEvents ? role.Clone() : null;
                                                role.Update(data.Role, UpdateSource.WebSocket);
                                                await RoleUpdated.Raise(before, role).ConfigureAwait(false);
                                            }
                                            else
                                                await _gatewayLogger.Warning("GUILD_ROLE_UPDATE referenced an unknown role.");
                                        }
                                        else
                                            await _gatewayLogger.Warning("GUILD_ROLE_UPDATE referenced an unknown guild.");
                                    }
                                    break;
                                case "GUILD_ROLE_DELETE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (GUILD_ROLE_DELETE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<GuildRoleDeleteEvent>(_serializer);
                                        var guild = DataStore.GetGuild(data.GuildId);
                                        if (guild != null)
                                        {
                                            var role = guild.RemoveCachedRole(data.RoleId);
                                            if (role != null)
                                                await RoleDeleted.Raise(role).ConfigureAwait(false);
                                            else
                                                await _gatewayLogger.Warning("GUILD_ROLE_DELETE referenced an unknown role.");
                                        }
                                        else
                                            await _gatewayLogger.Warning("GUILD_ROLE_DELETE referenced an unknown guild.");
                                    }
                                    break;

                                //Bans
                                case "GUILD_BAN_ADD":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (GUILD_BAN_ADD)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<GuildBanEvent>(_serializer);
                                        var guild = DataStore.GetGuild(data.GuildId);
                                        if (guild != null)
                                            await UserBanned.Raise(new User(this, data));
                                        else
                                            await _gatewayLogger.Warning("GUILD_BAN_ADD referenced an unknown guild.");
                                    }
                                    break;
                                case "GUILD_BAN_REMOVE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (GUILD_BAN_REMOVE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<GuildBanEvent>(_serializer);
                                        var guild = DataStore.GetGuild(data.GuildId);
                                        if (guild != null)
                                            await UserUnbanned.Raise(new User(this, data));
                                        else
                                            await _gatewayLogger.Warning("GUILD_BAN_REMOVE referenced an unknown guild.");
                                    }
                                    break;

                                //Messages
                                case "MESSAGE_CREATE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (MESSAGE_CREATE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<API.Message>(_serializer);
                                        var channel = DataStore.GetChannel(data.ChannelId) as ICachedMessageChannel;
                                        if (channel != null)
                                        {
                                            var author = channel.GetCachedUser(data.Author.Id);

                                            if (author != null)
                                            {
                                                var msg = channel.AddCachedMessage(author, data);
                                                await MessageReceived.Raise(msg).ConfigureAwait(false);
                                            }
                                            else
                                                await _gatewayLogger.Warning("MESSAGE_CREATE referenced an unknown user.");
                                        }
                                        else
                                            await _gatewayLogger.Warning("MESSAGE_CREATE referenced an unknown channel.");
                                    }
                                    break;
                                case "MESSAGE_UPDATE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (MESSAGE_UPDATE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<API.Message>(_serializer);
                                        var channel = DataStore.GetChannel(data.ChannelId) as ICachedMessageChannel;
                                        if (channel != null)
                                        {
                                            var msg = channel.GetCachedMessage(data.Id);
                                            var before = _enablePreUpdateEvents ? msg.Clone() : null;
                                            msg.Update(data, UpdateSource.WebSocket);
                                            await MessageUpdated.Raise(before, msg).ConfigureAwait(false);
                                        }
                                        else
                                            await _gatewayLogger.Warning("MESSAGE_UPDATE referenced an unknown channel.");
                                    }
                                    break;
                                case "MESSAGE_DELETE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (MESSAGE_DELETE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<API.Message>(_serializer);
                                        var channel = DataStore.GetChannel(data.ChannelId) as ICachedMessageChannel;
                                        if (channel != null)
                                        {
                                            var msg = channel.RemoveCachedMessage(data.Id);
                                            await MessageDeleted.Raise(msg).ConfigureAwait(false);
                                        }
                                        else
                                            await _gatewayLogger.Warning("MESSAGE_DELETE referenced an unknown channel.");
                                    }
                                    break;

                                //Statuses
                                case "PRESENCE_UPDATE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (PRESENCE_UPDATE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<API.Presence>(_serializer);
                                        if (data.GuildId == null)
                                        {
                                            var user = DataStore.GetUser(data.User.Id);
                                            if (user == null)
                                                user.Update(data, UpdateSource.WebSocket);
                                        }
                                        else
                                        {
                                            var guild = DataStore.GetGuild(data.GuildId.Value);
                                            if (guild == null)
                                            {
                                                await _gatewayLogger.Warning("PRESENCE_UPDATE referenced an unknown guild.");
                                                break;
                                            }
                                            if (data.Status == UserStatus.Offline)
                                                guild.RemoveCachedPresence(data.User.Id);
                                            else
                                                guild.AddOrUpdateCachedPresence(data);
                                        }
                                    }
                                    break;
                                case "TYPING_START":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (TYPING_START)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<TypingStartEvent>(_serializer);
                                        var channel = DataStore.GetChannel(data.ChannelId) as ICachedMessageChannel;
                                        if (channel != null)
                                        {
                                            var user = channel.GetCachedUser(data.UserId);
                                            if (user != null)
                                                await UserIsTyping.Raise(channel, user).ConfigureAwait(false);
                                        }
                                        else
                                            await _gatewayLogger.Warning("TYPING_START referenced an unknown channel.").ConfigureAwait(false);
                                    }
                                    break;

                                //Voice
                                case "VOICE_STATE_UPDATE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (VOICE_STATE_UPDATE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<API.VoiceState>(_serializer);
                                        if (data.GuildId.HasValue)
                                        {
                                            var guild = DataStore.GetGuild(data.GuildId.Value);
                                            if (guild != null)
                                            {
                                                if (data.ChannelId == null)
                                                    guild.RemoveCachedVoiceState(data.UserId);
                                                else
                                                    guild.AddOrUpdateCachedVoiceState(data);

                                                var user = guild.GetCachedUser(data.UserId);
                                                user.Update(data, UpdateSource.WebSocket);
                                            }
                                            else
                                                await _gatewayLogger.Warning("VOICE_STATE_UPDATE referenced an unknown guild.").ConfigureAwait(false);
                                        }
                                    }
                                    break;

                                //Settings
                                case "USER_UPDATE":
                                    {
                                        await _gatewayLogger.Debug($"Received Dispatch (USER_UPDATE)").ConfigureAwait(false);

                                        var data = (payload as JToken).ToObject<API.User>(_serializer);
                                        if (data.Id == CurrentUser.Id)
                                        {
                                            var before = _enablePreUpdateEvents ? CurrentUser.Clone() : null;
                                            CurrentUser.Update(data, UpdateSource.WebSocket);
                                            await CurrentUserUpdated.Raise(before, CurrentUser).ConfigureAwait(false);
                                        }
                                    }
                                    break;

                                //Ignored
                                case "USER_SETTINGS_UPDATE":
                                case "MESSAGE_ACK": //TODO: Add (User only)
                                case "GUILD_EMOJIS_UPDATE": //TODO: Add
                                case "GUILD_INTEGRATIONS_UPDATE": //TODO: Add
                                case "VOICE_SERVER_UPDATE": //TODO: Add
                                case "RESUMED": //TODO: Add
                                    await _gatewayLogger.Debug($"Ignored Dispatch ({type})").ConfigureAwait(false);
                                    return;

                                //Others
                                default:
                                    await _gatewayLogger.Warning($"Unknown Dispatch ({type})").ConfigureAwait(false);
                                    return;
                            }
                            break;
                        default:
                            await _gatewayLogger.Warning($"Unknown OpCode ({opCode})").ConfigureAwait(false);
                            return;
                    }
                }
                catch (Exception ex)
                {
                    await _gatewayLogger.Error($"Error handling {opCode}{(type != null ? $" ({type})" : "")}", ex).ConfigureAwait(false);
                    return;
                }
#if BENCHMARK
            }
            finally
            {
                stopwatch.Stop();
                double millis = Math.Round(stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0, 2);
                await _benchmarkLogger.Debug($"{millis} ms").ConfigureAwait(false);
            }
#endif
        }
        private async Task RunHeartbeat(int intervalMillis, CancellationToken cancelToken)
        {
            try
            {
                var state = ConnectionState;
                while (state == ConnectionState.Connecting || state == ConnectionState.Connected)
                {
                    //if (_heartbeatTime != 0) //TODO: Connection lost, reconnect

                    _heartbeatTime = Environment.TickCount;
                    await ApiClient.SendHeartbeat(_lastSeq).ConfigureAwait(false);
                    await Task.Delay(intervalMillis, cancelToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}
