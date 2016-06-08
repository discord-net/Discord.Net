using Discord.API;
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
using System.Threading.Tasks;

namespace Discord
{
    //TODO: Remove unnecessary `as` casts
    //TODO: Add docstrings
    public class DiscordSocketClient : DiscordClient, IDiscordClient
    {
        public event Func<Task> Connected, Disconnected;
        public event Func<Task> Ready;
        //public event Func<Channel> VoiceConnected, VoiceDisconnected;
        /*public event Func<IChannel, Task> ChannelCreated, ChannelDestroyed;
        public event Func<IChannel, IChannel, Task> ChannelUpdated;
        public event Func<IMessage, Task> MessageReceived, MessageDeleted;
        public event Func<IMessage, IMessage, Task> MessageUpdated;
        public event Func<IRole, Task> RoleCreated, RoleDeleted;
        public event Func<IRole, IRole, Task> RoleUpdated;
        public event Func<IGuild, Task> JoinedGuild, LeftGuild, GuildAvailable, GuildUnavailable;
        public event Func<IGuild, IGuild, Task> GuildUpdated;
        public event Func<IUser, Task> UserJoined, UserLeft, UserBanned, UserUnbanned;
        public event Func<IUser, IUser, Task> UserUpdated;
        public event Func<ISelfUser, ISelfUser, Task> CurrentUserUpdated;
        public event Func<IChannel, IUser, Task> UserIsTyping;*/

        private readonly ConcurrentQueue<ulong> _largeGuilds;
        private readonly Logger _gatewayLogger;
        private readonly DataStoreProvider _dataStoreProvider;
        private readonly JsonSerializer _serializer;
        private readonly int _connectionTimeout, _reconnectDelay, _failedReconnectDelay;
        private readonly bool _enablePreUpdateEvents;
        private readonly int _largeThreshold;
        private readonly int _totalShards;
        private ImmutableDictionary<string, VoiceRegion> _voiceRegions;
        private string _sessionId;
        private TaskCompletionSource<bool> _connectTask;

        public int ShardId { get; }
        public ConnectionState ConnectionState { get; private set; }
        public IWebSocketClient GatewaySocket { get; private set; }
        internal int MessageCacheSize { get; private set; }
        //internal bool UsePermissionCache { get; private set; }
        internal DataStore DataStore { get; private set; }

        internal CachedSelfUser CurrentUser => _currentUser as CachedSelfUser;
        internal IReadOnlyCollection<CachedGuild> Guilds
        {
            get
            {
                var guilds = DataStore.Guilds;
                return guilds.Select(x => x as CachedGuild).ToReadOnlyCollection(guilds);
            }
        }
        internal IReadOnlyCollection<CachedDMChannel> DMChannels
        {
            get
            {
                var users = DataStore.Users;
                return users.Select(x => (x as CachedPublicUser).DMChannel).Where(x => x != null).ToReadOnlyCollection(users);
            }
        }
        internal IReadOnlyCollection<VoiceRegion> VoiceRegions => _voiceRegions.ToReadOnlyCollection();

        public DiscordSocketClient()
            : this(new DiscordSocketConfig()) { }
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
            
            _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };
            
            ApiClient.SentGatewayMessage += async opCode => await _gatewayLogger.Verbose($"Sent Op {(GatewayOpCode)opCode}");
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
            while (_largeGuilds.TryDequeue(out guildId)) { }

            ConnectionState = ConnectionState.Disconnected;

            await Disconnected.Raise().ConfigureAwait(false);
        }
        
        public override Task<IVoiceRegion> GetVoiceRegion(string id)
        {
            VoiceRegion region;
            if (_voiceRegions.TryGetValue(id, out region))
                return Task.FromResult<IVoiceRegion>(region);
            return Task.FromResult<IVoiceRegion>(null);
        }

        public override Task<IGuild> GetGuild(ulong id)
        {
            return Task.FromResult<IGuild>(DataStore.GetGuild(id));
        }
        internal CachedGuild AddCachedGuild(API.Gateway.ExtendedGuild model, DataStore dataStore = null)
        {
            var guild = new CachedGuild(this, model);
            if (model.Unavailable != true)
            {
                for (int i = 0; i < model.Channels.Length; i++)
                    AddCachedChannel(model.Channels[i], dataStore);
            }
            (dataStore ?? DataStore).AddGuild(guild);
            if (model.Large)
                _largeGuilds.Enqueue(model.Id);
            return guild;
        }
        internal CachedGuild RemoveCachedGuild(ulong id, DataStore dataStore = null)
        {
            var guild = DataStore.RemoveGuild(id) as CachedGuild;
            foreach (var channel in guild.Channels)
                guild.RemoveCachedChannel(channel.Id);
            foreach (var user in guild.Members)
                guild.RemoveCachedUser(user.Id);
            return guild;
        }
        internal CachedGuild GetCachedGuild(ulong id) => DataStore.GetGuild(id) as CachedGuild;

        public override Task<IChannel> GetChannel(ulong id)
        {
            return Task.FromResult<IChannel>(DataStore.GetChannel(id));
        }
        internal ICachedChannel AddCachedChannel(API.Channel model, DataStore dataStore = null)
        {
            if (model.IsPrivate)
            {
                var recipient = AddCachedUser(model.Recipient);
                return recipient.SetDMChannel(model);
            }
            else
            {
                var guild = GetCachedGuild(model.GuildId.Value);
                return guild.AddCachedChannel(model);
            }
        }
        internal ICachedChannel RemoveCachedChannel(ulong id, DataStore dataStore = null)
        {
            var channel = DataStore.RemoveChannel(id) as ICachedChannel;
            var dmChannel = channel as CachedDMChannel;
            if (dmChannel != null)
            {
                var recipient = dmChannel.Recipient;
                recipient.RemoveDMChannel(id);
            }
            return channel;
        }
        internal ICachedChannel GetCachedChannel(ulong id) => DataStore.GetChannel(id) as ICachedChannel;

        public override Task<IUser> GetUser(ulong id)
        {
            return Task.FromResult<IUser>(DataStore.GetUser(id));
        }
        public override Task<IUser> GetUser(string username, ushort discriminator)
        {
            return Task.FromResult<IUser>(DataStore.Users.Where(x => x.Discriminator == discriminator && x.Username == username).FirstOrDefault());
        }
        internal CachedPublicUser AddCachedUser(API.User model, DataStore dataStore = null)
        {
            var user = DataStore.GetOrAddUser(model.Id, _ => new CachedPublicUser(this, model)) as CachedPublicUser;
            user.AddRef();
            return user;
        }
        internal CachedPublicUser RemoveCachedUser(ulong id, DataStore dataStore = null)
        {
            var user = DataStore.GetUser(id) as CachedPublicUser;
            user.RemoveRef();
            return user;
        }

        private async Task ProcessMessage(GatewayOpCode opCode, string type, JToken payload)
        {
            try
            {
                switch (opCode)
                {
                    case GatewayOpCode.Hello:
                        {
                            var data = payload.ToObject<HelloEvent>(_serializer);

                            await ApiClient.SendIdentify().ConfigureAwait(false);
                        }
                        break;
                    case GatewayOpCode.Dispatch:
                        switch (type)
                        {
                            //Global
                            case "READY":
                                {
                                    //TODO: Make downloading large guilds optional
                                    var data = payload.ToObject<ReadyEvent>(_serializer);
                                    var dataStore = _dataStoreProvider(ShardId, _totalShards, data.Guilds.Length, data.PrivateChannels.Length);

                                    _currentUser = new CachedSelfUser(this,data.User);

                                    for (int i = 0; i < data.Guilds.Length; i++)
                                        AddCachedGuild(data.Guilds[i], dataStore);
                                    for (int i = 0; i < data.PrivateChannels.Length; i++)
                                        AddCachedChannel(data.PrivateChannels[i], dataStore);

                                    _sessionId = data.SessionId;
                                    DataStore = dataStore;

                                    await Ready().ConfigureAwait(false);

                                    _connectTask.TrySetResult(true); //Signal the .Connect() call to complete
                                }
                                break;

                            //Guilds
                            /*case "GUILD_CREATE":
                                {
                                    var data = payload.ToObject<ExtendedGuild>(_serializer);
                                    var guild = new CachedGuild(this, data);
                                    DataStore.AddGuild(guild);

                                    if (data.Unavailable == false)
                                        type = "GUILD_AVAILABLE";
                                    else
                                        await JoinedGuild.Raise(guild).ConfigureAwait(false);

                                    if (!data.Large)
                                        await GuildAvailable.Raise(guild);
                                    else
                                        _largeGuilds.Enqueue(data.Id);
                                }
                                break;
                            case "GUILD_UPDATE":
                                {
                                    var data = payload.ToObject<API.Guild>(_serializer);
                                    var guild = DataStore.GetGuild(data.Id);
                                    if (guild != null)
                                    {
                                        var before = _enablePreUpdateEvents ? guild.Clone() : null;
                                        guild.Update(data);
                                        await GuildUpdated.Raise(before, guild);
                                    }
                                    else
                                        await _gatewayLogger.Warning("GUILD_UPDATE referenced an unknown guild.");
                                }
                                break;
                            case "GUILD_DELETE":
                                {
                                    var data = payload.ToObject<ExtendedGuild>(_serializer);
                                    var guild = DataStore.RemoveGuild(data.Id);
                                    if (guild != null)
                                    {
                                        if (data.Unavailable == true)
                                            type = "GUILD_UNAVAILABLE";

                                        await GuildUnavailable.Raise(guild);
                                        if (data.Unavailable != true)
                                            await LeftGuild.Raise(guild);
                                    }
                                    else
                                        await _gatewayLogger.Warning("GUILD_DELETE referenced an unknown guild.");
                                }
                                break;

                            //Channels
                            case "CHANNEL_CREATE":
                                {
                                    var data = payload.ToObject<API.Channel>(_serializer);

                                    IChannel channel = null;
                                    if (data.GuildId != null)
                                    {
                                        var guild = GetCachedGuild(data.GuildId.Value);
                                        if (guild != null)
                                            channel = guild.AddCachedChannel(data.Id, true);
                                        else
                                            await _gatewayLogger.Warning("CHANNEL_CREATE referenced an unknown guild.");
                                    }
                                    else
                                        channel = AddCachedPrivateChannel(data.Id, data.Recipient.Id);
                                    if (channel != null)
                                    {
                                        channel.Update(data);
                                        await ChannelCreated.Raise(channel);
                                    }
                                }
                                break;
                            case "CHANNEL_UPDATE":
                                {
                                    var data = payload.ToObject<API.Channel>(_serializer);
                                    var channel = DataStore.GetChannel(data.Id) as Channel;
                                    if (channel != null)
                                    {
                                        var before = _enablePreUpdateEvents ? channel.Clone() : null;
                                        channel.Update(data);
                                        await ChannelUpdated.Raise(before, channel);
                                    }
                                    else
                                        await _gatewayLogger.Warning("CHANNEL_UPDATE referenced an unknown channel.");
                                }
                                break;
                            case "CHANNEL_DELETE":
                                {
                                    var data = payload.ToObject<API.Channel>(_serializer);
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
                                    var data = payload.ToObject<API.GuildMember>(_serializer);
                                    var guild = GetGuild(data.GuildId.Value);
                                    if (guild != null)
                                    {
                                        var user = guild.AddCachedUser(data.User.Id, true, true);
                                        user.Update(data);
                                        user.UpdateActivity();
                                        UserJoined.Raise(user);
                                    }
                                    else
                                        await _gatewayLogger.Warning("GUILD_MEMBER_ADD referenced an unknown guild.");
                                }
                                break;
                            case "GUILD_MEMBER_UPDATE":
                                {
                                    var data = payload.ToObject<API.GuildMember>(_serializer);
                                    var guild = GetGuild(data.GuildId.Value);
                                    if (guild != null)
                                    {
                                        var user = guild.GetCachedUser(data.User.Id);
                                        if (user != null)
                                        {
                                            var before = _enablePreUpdateEvents ? user.Clone() : null;
                                            user.Update(data);
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
                                    var data = payload.ToObject<API.GuildMember>(_serializer);
                                    var guild = GetGuild(data.GuildId.Value);
                                    if (guild != null)
                                    {
                                        var user = guild.RemoveCachedUser(data.User.Id);
                                        if (user != null)
                                        {
                                            user.GlobalUser.RemoveGuild();
                                            if (user.GuildCount == 0 && user.DMChannel == null)
                                                DataStore.RemoveUser(user.Id);
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
                                    var data = payload.ToObject<GuildMembersChunkEvent>(_serializer);
                                    var guild = GetCachedGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        foreach (var memberData in data.Members)
                                        {
                                            var user = guild.AddCachedUser(memberData.User.Id, true, false);
                                            user.Update(memberData);
                                        }

                                        if (guild.CurrentUserCount >= guild.UserCount) //Finished downloading for there
                                            await GuildAvailable.Raise(guild);
                                    }
                                    else
                                        await _gatewayLogger.Warning("GUILD_MEMBERS_CHUNK referenced an unknown guild.");
                                }
                                break;

                            //Roles
                            case "GUILD_ROLE_CREATE":
                                {
                                    var data = payload.ToObject<GuildRoleCreateEvent>(_serializer);
                                    var guild = GetCachedGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var role = guild.AddCachedRole(data.Data.Id);
                                        role.Update(data.Data, false);
                                        RoleCreated.Raise(role);
                                    }
                                    else
                                        await _gatewayLogger.Warning("GUILD_ROLE_CREATE referenced an unknown guild.");
                                }
                                break;
                            case "GUILD_ROLE_UPDATE":
                                {
                                    var data = payload.ToObject<GuildRoleUpdateEvent>(_serializer);
                                    var guild = GetCachedGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var role = guild.GetRole(data.Data.Id);
                                        if (role != null)
                                        {
                                            var before = _enablePreUpdateEvents ? role.Clone() : null;
                                            role.Update(data.Data, true);
                                            RoleUpdated.Raise(before, role);
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
                                    var data = payload.ToObject<GuildRoleDeleteEvent>(_serializer);
                                    var guild = DataStore.GetGuild(data.GuildId) as CachedGuild;
                                    if (guild != null)
                                    {
                                        var role = guild.RemoveRole(data.RoleId);
                                        if (role != null)
                                            RoleDeleted.Raise(role);
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
                                    var data = payload.ToObject<GuildBanEvent>(_serializer);
                                    var guild = GetCachedGuild(data.GuildId);
                                    if (guild != null)
                                        await UserBanned.Raise(new User(this, data));
                                    else
                                        await _gatewayLogger.Warning("GUILD_BAN_ADD referenced an unknown guild.");
                                }
                                break;
                            case "GUILD_BAN_REMOVE":
                                {
                                    var data = payload.ToObject<GuildBanEvent>(_serializer);
                                    var guild = GetCachedGuild(data.GuildId);
                                    if (guild != null)
                                        await UserUnbanned.Raise(new User(this, data));
                                    else
                                        await _gatewayLogger.Warning("GUILD_BAN_REMOVE referenced an unknown guild.");
                                }
                                break;

                            //Messages
                            case "MESSAGE_CREATE":
                                {
                                    var data = payload.ToObject<API.Message>(_serializer);

                                    var channel = DataStore.GetChannel(data.ChannelId);
                                    if (channel != null)
                                    {
                                        var user = channel.GetUser(data.Author.Id);

                                        if (user != null)
                                        {
                                            bool isAuthor = data.Author.Id == CurrentUser.Id;
                                            var msg = channel.AddMessage(data.Id, user, data.Timestamp.Value);

                                            msg.Update(data);

                                            MessageReceived.Raise(msg);
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
                                    var data = payload.ToObject<API.Message>(_serializer);
                                    var channel = GetCachedChannel(data.ChannelId);
                                    if (channel != null)
                                    {
                                        var msg = channel.GetMessage(data.Id, data.Author?.Id);
                                        var before = _enablePreUpdateEvents ? msg.Clone() : null;
                                        msg.Update(data);
                                        MessageUpdated.Raise(before, msg);
                                    }
                                    else
                                        await _gatewayLogger.Warning("MESSAGE_UPDATE referenced an unknown channel.");
                                }
                                break;
                            case "MESSAGE_DELETE":
                                {
                                    var data = payload.ToObject<API.Message>(_serializer);
                                    var channel = GetCachedChannel(data.ChannelId);
                                    if (channel != null)
                                    {
                                        var msg = channel.RemoveMessage(data.Id);
                                        MessageDeleted.Raise(msg);
                                    }
                                    else
                                        await _gatewayLogger.Warning("MESSAGE_DELETE referenced an unknown channel.");
                                }
                                break;

                            //Statuses
                            case "PRESENCE_UPDATE":
                                {
                                    var data = payload.ToObject<API.Presence>(_serializer);
                                    User user;
                                    Guild guild;
                                    if (data.GuildId == null)
                                    {
                                        guild = null;
                                        user = GetPrivateChannel(data.User.Id)?.Recipient;
                                    }
                                    else
                                    {
                                        guild = GetGuild(data.GuildId.Value);
                                        if (guild == null)
                                        {
                                            await _gatewayLogger.Warning("PRESENCE_UPDATE referenced an unknown guild.");
                                            break;
                                        }
                                        else
                                            user = guild.GetUser(data.User.Id);
                                    }

                                    if (user != null)
                                    {
                                        var before = _enablePreUpdateEvents ? user.Clone() : null;
                                        user.Update(data);
                                        UserUpdated.Raise(before, user);
                                    }
                                    else
                                    {
                                        //Occurs when a user leaves a guild
                                        //await _gatewayLogger.Warning("PRESENCE_UPDATE referenced an unknown user.");
                                    }
                                }
                                break;
                            case "TYPING_START":
                                {
                                    var data = payload.ToObject<TypingStartEvent>(_serializer);
                                    var channel = GetCachedChannel(data.ChannelId);
                                    if (channel != null)
                                    {
                                        var user = channel.GetUser(data.UserId);
                                        if (user != null)
                                        {
                                            await UserIsTyping.Raise(channel, user);
                                            user.UpdateActivity();
                                        }
                                    }
                                    else
                                        await _gatewayLogger.Warning("TYPING_START referenced an unknown channel.");
                                }
                                break;

                            //Voice
                            case "VOICE_STATE_UPDATE":
                                {
                                    var data = payload.ToObject<API.VoiceState>(_serializer);
                                    var guild = GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var user = guild.GetUser(data.UserId);
                                        if (user != null)
                                        {
                                            var before = _enablePreUpdateEvents ? user.Clone() : null;
                                            user.Update(data);
                                            UserUpdated.Raise(before, user);
                                        }
                                        else
                                        {
                                            //Occurs when a user leaves a guild
                                            //await _gatewayLogger.Warning("VOICE_STATE_UPDATE referenced an unknown user.");
                                        }
                                    }
                                    else
                                        await _gatewayLogger.Warning("VOICE_STATE_UPDATE referenced an unknown guild.");
                                }
                                break;

                            //Settings
                            case "USER_UPDATE":
                                {
                                    var data = payload.ToObject<SelfUser>(_serializer);
                                    if (data.Id == CurrentUser.Id)
                                    {
                                        var before = _enablePreUpdateEvents ? CurrentUser.Clone() : null;
                                        CurrentUser.Update(data);
                                        await CurrentUserUpdated.Raise(before, CurrentUser).ConfigureAwait(false);
                                    }
                                }
                                break;*/

                            //Ignored
                            case "USER_SETTINGS_UPDATE":
                            case "MESSAGE_ACK": //TODO: Add (User only)
                            case "GUILD_EMOJIS_UPDATE": //TODO: Add
                            case "GUILD_INTEGRATIONS_UPDATE": //TODO: Add
                            case "VOICE_SERVER_UPDATE": //TODO: Add
                            case "RESUMED": //TODO: Add
                                await _gatewayLogger.Debug($"Ignored message {opCode}{(type != null ? $" ({type})" : "")}").ConfigureAwait(false);
                                return;

                            //Others
                            default:
                                await _gatewayLogger.Warning($"Unknown message {opCode}{(type != null ? $" ({type})" : "")}").ConfigureAwait(false);
                                return;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                await _gatewayLogger.Error($"Error handling msg {opCode}{(type != null ? $" ({type})" : "")}", ex).ConfigureAwait(false);
                return;
            }
            await _gatewayLogger.Debug($"Received {opCode}{(type != null ? $" ({type})" : "")}").ConfigureAwait(false);
        }
    }
}
