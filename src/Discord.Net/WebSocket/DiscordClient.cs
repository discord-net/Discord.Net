using Discord.API;
using Discord.API.Gateway;
using Discord.API.Rest;
using Discord.Logging;
using Discord.Net;
using Discord.Net.Converters;
using Discord.Net.Queue;
using Discord.Net.WebSockets;
using Discord.WebSocket.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    //TODO: Docstrings
    //TODO: Log Logins/Logouts
    //TODO: Do a final namespace and file structure review
    public sealed class DiscordClient : IDiscordClient, IDisposable
    {
        public event Func<LogMessageEventArgs, Task> Log;
        public event Func<Task> LoggedIn, LoggedOut;
        public event Func<Task> Connected, Disconnected;
        //public event Func<Channel> VoiceConnected, VoiceDisconnected;
        public event Func<Channel, Task> ChannelCreated, ChannelDestroyed;
        public event Func<Channel, Channel, Task> ChannelUpdated;
        public event Func<Message, Task> MessageReceived, MessageDeleted;
        public event Func<Message, Message, Task> MessageUpdated;
        public event Func<Role, Task> RoleCreated, RoleDeleted;
        public event Func<Role, Role, Task> RoleUpdated;
        public event Func<Guild, Task> JoinedGuild, LeftGuild, GuildAvailable, GuildUnavailable;
        public event Func<Guild, Guild, Task> GuildUpdated;
        public event Func<User, Task> UserJoined, UserLeft, UserBanned, UserUnbanned;
        public event Func<User, User, Task> UserUpdated;
        public event Func<Channel, User, Task> UserIsTyping;

        private readonly ConcurrentQueue<ulong> _largeGuilds;
        private readonly Logger _discordLogger, _gatewayLogger;
        private readonly SemaphoreSlim _connectionLock;
        private readonly DataStoreProvider _dataStoreProvider;
        private readonly LogManager _log;
        private readonly RequestQueue _requestQueue;
        private readonly JsonSerializer _serializer;
        private readonly int _connectionTimeout, _reconnectDelay, _failedReconnectDelay;
        private readonly bool _enablePreUpdateEvents;
        private readonly int _largeThreshold;
        private readonly int _totalShards;
        private ImmutableDictionary<string, VoiceRegion> _voiceRegions;
        private string _sessionId;
        private bool _isDisposed;

        public int ShardId { get; }
        public LoginState LoginState { get; private set; }
        public ConnectionState ConnectionState { get; private set; }
        public API.DiscordApiClient ApiClient { get; private set; }
        public IWebSocketClient GatewaySocket { get; private set; }
        public IDataStore DataStore { get; private set; }
        public SelfUser CurrentUser { get; private set; }
        internal int MessageCacheSize { get; private set; }
        internal bool UsePermissionCache { get; private set; }
        
        public IRequestQueue RequestQueue => _requestQueue;
        public IEnumerable<Guild> Guilds => DataStore.Guilds;
        public IEnumerable<DMChannel> DMChannels => DataStore.Users.Select(x => x.DMChannel).Where(x => x != null);
        public IEnumerable<VoiceRegion> VoiceRegions => _voiceRegions.Select(x => x.Value);

        public DiscordClient(DiscordSocketConfig config = null)
        {
            if (config == null)
                config = new DiscordSocketConfig();
            
            ShardId = config.ShardId;
            _totalShards = config.TotalShards;

            _connectionTimeout = config.ConnectionTimeout;
            _reconnectDelay = config.ReconnectDelay;
            _failedReconnectDelay = config.FailedReconnectDelay;
            _dataStoreProvider = config.DataStoreProvider;

            MessageCacheSize = config.MessageCacheSize;
            UsePermissionCache = config.UsePermissionsCache;
            _enablePreUpdateEvents = config.EnablePreUpdateEvents;
            _largeThreshold = config.LargeThreshold;

            _log = new LogManager(config.LogLevel);
            _log.Message += async e => await Log.Raise(e).ConfigureAwait(false);
            _discordLogger = _log.CreateLogger("Discord");
            _gatewayLogger = _log.CreateLogger("Gateway");

            _connectionLock = new SemaphoreSlim(1, 1);
            _requestQueue = new RequestQueue();
            _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };

            ApiClient = new API.DiscordApiClient(config.RestClientProvider, config.WebSocketProvider, _serializer, _requestQueue);
            ApiClient.SentRequest += async e => await _log.Verbose("Rest", $"{e.Method} {e.Endpoint}: {e.Milliseconds} ms");
            GatewaySocket = config.WebSocketProvider();
            GatewaySocket.BinaryMessage += async e =>
            {
                using (var compressed = new MemoryStream(e.Data, 2, e.Data.Length - 2))
                using (var decompressed = new MemoryStream())
                {
                    using (var zlib = new DeflateStream(compressed, CompressionMode.Decompress))
                        zlib.CopyTo(decompressed);
                    decompressed.Position = 0;
                    using (var reader = new StreamReader(decompressed))
                        await ProcessMessage(reader.ReadToEnd()).ConfigureAwait(false);
                }
            };
            GatewaySocket.TextMessage += async  e => await ProcessMessage(e.Message).ConfigureAwait(false);

            _voiceRegions = ImmutableDictionary.Create<string, VoiceRegion>();
            _largeGuilds = new ConcurrentQueue<ulong>();
        }
        
        void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                    ApiClient?.Dispose();
                _isDisposed = true;
            }
        }
        public void Dispose() => Dispose(true);

        public async Task Login(string email, string password)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LoginInternal(TokenType.User, null, email, password, true, false).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        public async Task Login(TokenType tokenType, string token, bool validateToken = true)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LoginInternal(tokenType, token, null, null, false, validateToken).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task LoginInternal(TokenType tokenType, string token, string email, string password, bool useEmail, bool validateToken)
        {
            if (LoginState != LoginState.LoggedOut)
                await LogoutInternal().ConfigureAwait(false);
            LoginState = LoginState.LoggingIn;

            try
            {
                if (useEmail)
                {
                    var args = new LoginParams { Email = email, Password = password };
                    await ApiClient.Login(args).ConfigureAwait(false);
                }
                else
                    await ApiClient.Login(tokenType, token).ConfigureAwait(false);

                if (validateToken)
                {
                    try
                    {
                        await ApiClient.ValidateToken().ConfigureAwait(false);
                        var gateway = await ApiClient.GetGateway();
                        var voiceRegions = await ApiClient.GetVoiceRegions().ConfigureAwait(false);
                        _voiceRegions = voiceRegions.Select(x => new VoiceRegion(x)).ToImmutableDictionary(x => x.Id);

                        await GatewaySocket.Connect(gateway.Url).ConfigureAwait(false);
                    }
                    catch (HttpException ex)
                    {
                        throw new ArgumentException("Token validation failed", nameof(token), ex);
                    }
                }

                LoginState = LoginState.LoggedIn;
            }
            catch (Exception)
            {
                await LogoutInternal().ConfigureAwait(false);
                throw;
            }

            await LoggedIn.Raise().ConfigureAwait(false);
        }

        public async Task Logout()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LogoutInternal().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task LogoutInternal()
        {
            if (LoginState == LoginState.LoggedOut) return;            
            LoginState = LoginState.LoggingOut;

            if (ConnectionState != ConnectionState.Disconnected)
                await DisconnectInternal().ConfigureAwait(false);

            await ApiClient.Logout().ConfigureAwait(false);
            
            _voiceRegions = ImmutableDictionary.Create<string, VoiceRegion>();
            CurrentUser = null;

            LoginState = LoginState.LoggedOut;

            await LoggedOut.Raise().ConfigureAwait(false);
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
                await ApiClient.Connect().ConfigureAwait(false);

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

        public async Task<IEnumerable<Connection>> GetConnections()
        {
            var models = await ApiClient.GetCurrentUserConnections().ConfigureAwait(false);
            return models.Select(x => new Connection(x));
        }

        public Channel GetChannel(ulong id)
        {
            return DataStore.GetChannel(id);
        }

        public async Task<Invite> GetInvite(string inviteIdOrXkcd)
        {
            var model = await ApiClient.GetInvite(inviteIdOrXkcd).ConfigureAwait(false);
            if (model != null)
                return new Invite(this, model);
            return null;
        }

        public Guild GetGuild(ulong id)
        {
            return DataStore.GetGuild(id);
        }
        public async Task<Guild> CreateGuild(string name, IVoiceRegion region, Stream jpegIcon = null)
        {
            var args = new CreateGuildParams();
            var model = await ApiClient.CreateGuild(args).ConfigureAwait(false);
            return new Guild(this, model);
        }

        public User GetUser(ulong id)
        {
            return DataStore.GetUser(id);
        }
        public User GetUser(string username, ushort discriminator)
        {
            return DataStore.Users.Where(x => x.Discriminator == discriminator && x.Username == username).FirstOrDefault();
        }
        public async Task<IEnumerable<User>> QueryUsers(string query, int limit)
        {
            var models = await ApiClient.QueryUsers(query, limit).ConfigureAwait(false);
            return models.Select(x => new User(this, x));
        }

        public VoiceRegion GetVoiceRegion(string id)
        {
            VoiceRegion region;
            if (_voiceRegions.TryGetValue(id, out region))
                return region;
            return null;
        }

        private async Task ProcessMessage(string json)
        {
            var msg = JsonConvert.DeserializeObject<WebSocketMessage>(json);
            try
            {
                switch (msg.Type)
                {
                    //Global
                    case "READY":
                        {
                            //TODO: Store guilds even if they're unavailable
                            //TODO: Make downloading large guilds optional

                            var data = msg.Payload.ToObject<ReadyEvent>(_serializer);
                            var store = _dataStoreProvider(ShardId, _totalShards, data.Guilds.Length, data.PrivateChannels.Length);

                            _sessionId = data.SessionId;
                            var currentUser = new SelfUser(this, data.User);
                            store.AddUser(currentUser);

                            for (int i = 0; i < data.Guilds.Length; i++)
                            {
                                var model = data.Guilds[i];
                                var guild = new Guild(this, model);
                                store.AddGuild(guild);

                                foreach (var channel in guild.Channels)
                                    store.AddChannel(channel);

                                /*if (model.IsLarge)
                                    _largeGuilds.Enqueue(model.Id);*/
                            }

                            for (int i = 0; i < data.PrivateChannels.Length; i++)
                            {
                                var model = data.PrivateChannels[i];
                                var recipient = new User(this, model.Recipient);
                                var channel = new DMChannel(this, recipient, model);

                                recipient.DMChannel = channel;
                                store.AddChannel(channel);
                            }

                            CurrentUser = currentUser;
                            DataStore = store;
                        }
                        break;

                    //Servers
                    case "GUILD_CREATE":
                        {
                            /*var data = msg.Payload.ToObject<ExtendedGuild>(Serializer);
                            if (data.Unavailable != true)
                            {
                                var server = AddServer(data.Id);
                                server.Update(data);

                                if (data.Unavailable != false)
                                {
                                    _gatewayLogger.Info($"GUILD_CREATE: {server.Path}");
                                    JoinedServer.Raise(server);
                                }
                                else
                                    _gatewayLogger.Info($"GUILD_AVAILABLE: {server.Path}");

                                if (!data.IsLarge)
                                    await GuildAvailable.Raise(server);
                                else
                                    _largeServers.Enqueue(data.Id);
                            }*/
                        }
                        break;
                    case "GUILD_UPDATE":
                        {
                            /*var data = msg.Payload.ToObject<Guild>(Serializer);
                            var server = GetServer(data.Id);
                            if (server != null)
                            {
                                var before = Config.EnablePreUpdateEvents ? server.Clone() : null;
                                server.Update(data);
                                _gatewayLogger.Info($"GUILD_UPDATE: {server.Path}");
                                await GuildUpdated.Raise(before, server);
                            }
                            else
                                _gatewayLogger.Warning("GUILD_UPDATE referenced an unknown guild.");*/
                        }
                        break;
                    case "GUILD_DELETE":
                        {
                            /*var data = msg.Payload.ToObject<ExtendedGuild>(Serializer);
                            Server server = RemoveServer(data.Id);
                            if (server != null)
                            {
                                if (data.Unavailable != true)
                                    _gatewayLogger.Info($"GUILD_DELETE: {server.Path}");
                                else
                                    _gatewayLogger.Info($"GUILD_UNAVAILABLE: {server.Path}");

                                OnServerUnavailable(server);
                                if (data.Unavailable != true)
                                    OnLeftServer(server);
                            }
                            else
                                _gatewayLogger.Warning("GUILD_DELETE referenced an unknown guild.");*/
                        }
                        break;

                    //Channels
                    case "CHANNEL_CREATE":
                        {
                            /*var data = msg.Payload.ToObject<ChannelCreateEvent>(Serializer);

                            Channel channel = null;
                            if (data.GuildId != null)
                            {
                                var server = GetServer(data.GuildId.Value);
                                if (server != null)
                                    channel = server.AddChannel(data.Id, true);
                                else
                                    _gatewayLogger.Warning("CHANNEL_CREATE referenced an unknown guild.");
                            }
                            else
                                channel = AddPrivateChannel(data.Id, data.Recipient.Id);
                            if (channel != null)
                            {
                                channel.Update(data);
                                _gatewayLogger.Info($"CHANNEL_CREATE: {channel.Path}");
                                ChannelCreated.Raise(new ChannelEventArgs(channel));
                            }*/
                        }
                        break;
                    case "CHANNEL_UPDATE":
                        {
                            /*var data = msg.Payload.ToObject<ChannelUpdateEvent>(Serializer);
                            var channel = GetChannel(data.Id);
                            if (channel != null)
                            {
                                var before = Config.EnablePreUpdateEvents ? channel.Clone() : null;
                                channel.Update(data);
                                _gateway_gatewayLogger.Info($"CHANNEL_UPDATE: {channel.Path}");
                                OnChannelUpdated(before, channel);
                            }
                            else
                                _gateway_gatewayLogger.Warning("CHANNEL_UPDATE referenced an unknown channel.");*/
                        }
                        break;
                    case "CHANNEL_DELETE":
                        {
                            /*var data = msg.Payload.ToObject<ChannelDeleteEvent>(Serializer);
                            var channel = RemoveChannel(data.Id);
                            if (channel != null)
                            {
                                _gateway_gatewayLogger.Info($"CHANNEL_DELETE: {channel.Path}");
                                OnChannelDestroyed(channel);
                            }
                            else
                                _gateway_gatewayLogger.Warning("CHANNEL_DELETE referenced an unknown channel.");*/
                        }
                        break;

                    //Members
                    case "GUILD_MEMBER_ADD":
                        {
                            /*var data = msg.Payload.ToObject<GuildMemberAddEvent>(Serializer);
                            var server = GetServer(data.GuildId.Value);
                            if (server != null)
                            {
                                var user = server.AddUser(data.User.Id, true, true);
                                user.Update(data);
                                user.UpdateActivity();
                                _gatewayLogger.Info($"GUILD_MEMBER_ADD: {user.Path}");
                                OnUserJoined(user);
                            }
                            else
                                _gatewayLogger.Warning("GUILD_MEMBER_ADD referenced an unknown guild.");*/
                        }
                        break;
                    case "GUILD_MEMBER_UPDATE":
                        {
                            /*var data = msg.Payload.ToObject<GuildMemberUpdateEvent>(Serializer);
                            var server = GetServer(data.GuildId.Value);
                            if (server != null)
                            {
                                var user = server.GetUser(data.User.Id);
                                if (user != null)
                                {
                                    var before = Config.EnablePreUpdateEvents ? user.Clone() : null;
                                    user.Update(data);
                                    _gatewayLogger.Info($"GUILD_MEMBER_UPDATE: {user.Path}");
                                    OnUserUpdated(before, user);
                                }
                                else
                                    _gatewayLogger.Warning("GUILD_MEMBER_UPDATE referenced an unknown user.");
                            }
                            else
                                _gatewayLogger.Warning("GUILD_MEMBER_UPDATE referenced an unknown guild.");*/
                        }
                        break;
                    case "GUILD_MEMBER_REMOVE":
                        {
                            /*var data = msg.Payload.ToObject<GuildMemberRemoveEvent>(Serializer);
                            var server = GetServer(data.GuildId.Value);
                            if (server != null)
                            {
                                var user = server.RemoveUser(data.User.Id);
                                if (user != null)
                                {
                                    _gatewayLogger.Info($"GUILD_MEMBER_REMOVE: {user.Path}");
                                    OnUserLeft(user);
                                }
                                else
                                    _gatewayLogger.Warning("GUILD_MEMBER_REMOVE referenced an unknown user.");
                            }
                            else
                                _gatewayLogger.Warning("GUILD_MEMBER_REMOVE referenced an unknown guild.");*/
                        }
                        break;
                    case "GUILD_MEMBERS_CHUNK":
                        {
                            /*var data = msg.Payload.ToObject<GuildMembersChunkEvent>(Serializer);
                            var server = GetServer(data.GuildId);
                            if (server != null)
                            {
                                foreach (var memberData in data.Members)
                                {
                                    var user = server.AddUser(memberData.User.Id, true, false);
                                    user.Update(memberData);
                                }
                                _gateway_gatewayLogger.Verbose($"GUILD_MEMBERS_CHUNK: {data.Members.Length} users");

                                if (server.CurrentUserCount >= server.UserCount) //Finished downloading for there
                                    OnServerAvailable(server);
                            }
                            else
                                _gateway_gatewayLogger.Warning("GUILD_MEMBERS_CHUNK referenced an unknown guild.");*/
                        }
                        break;

                    //Roles
                    case "GUILD_ROLE_CREATE":
                        {
                            /*var data = msg.Payload.ToObject<GuildRoleCreateEvent>(Serializer);
                            var server = GetServer(data.GuildId);
                            if (server != null)
                            {
                                var role = server.AddRole(data.Data.Id);
                                role.Update(data.Data, false);
                                _gateway_gatewayLogger.Info($"GUILD_ROLE_CREATE: {role.Path}");
                                OnRoleCreated(role);
                            }
                            else
                                _gateway_gatewayLogger.Warning("GUILD_ROLE_CREATE referenced an unknown guild.");*/
                        }
                        break;
                    case "GUILD_ROLE_UPDATE":
                        {
                            /*var data = msg.Payload.ToObject<GuildRoleUpdateEvent>(Serializer);
                            var server = GetServer(data.GuildId);
                            if (server != null)
                            {
                                var role = server.GetRole(data.Data.Id);
                                if (role != null)
                                {
                                    var before = Config.EnablePreUpdateEvents ? role.Clone() : null;
                                    role.Update(data.Data, true);
                                    _gateway_gatewayLogger.Info($"GUILD_ROLE_UPDATE: {role.Path}");
                                    OnRoleUpdated(before, role);
                                }
                                else
                                    _gateway_gatewayLogger.Warning("GUILD_ROLE_UPDATE referenced an unknown role.");
                            }
                            else
                                _gateway_gatewayLogger.Warning("GUILD_ROLE_UPDATE referenced an unknown guild.");*/
                        }
                        break;
                    case "GUILD_ROLE_DELETE":
                        {
                            /*var data = msg.Payload.ToObject<GuildRoleDeleteEvent>(Serializer);
                            var server = GetServer(data.GuildId);
                            if (server != null)
                            {
                                var role = server.RemoveRole(data.RoleId);
                                if (role != null)
                                {
                                    _gateway_gatewayLogger.Info($"GUILD_ROLE_DELETE: {role.Path}");
                                    OnRoleDeleted(role);
                                }
                                else
                                    _gateway_gatewayLogger.Warning("GUILD_ROLE_DELETE referenced an unknown role.");
                            }
                            else
                                _gateway_gatewayLogger.Warning("GUILD_ROLE_DELETE referenced an unknown guild.");*/
                        }
                        break;

                    //Bans
                    case "GUILD_BAN_ADD":
                        {
                            /*var data = msg.Payload.ToObject<GuildBanAddEvent>(Serializer);
                            var server = GetServer(data.GuildId.Value);
                            if (server != null)
                            {
                                var user = server.GetUser(data.User.Id);
                                if (user != null)
                                {
                                    _gateway_gatewayLogger.Info($"GUILD_BAN_ADD: {user.Path}");
                                    OnUserBanned(user);
                                }
                                else
                                    _gateway_gatewayLogger.Warning("GUILD_BAN_ADD referenced an unknown user.");
                            }
                            else
                                _gateway_gatewayLogger.Warning("GUILD_BAN_ADD referenced an unknown guild.");*/
                        }
                        break;
                    case "GUILD_BAN_REMOVE":
                        {
                            /*var data = msg.Payload.ToObject<GuildBanRemoveEvent>(Serializer);
                            var server = GetServer(data.GuildId.Value);
                            if (server != null)
                            {
                                var user = new User(this, data.User.Id, server);
                                user.Update(data.User);
                                _gateway_gatewayLogger.Info($"GUILD_BAN_REMOVE: {user.Path}");
                                OnUserUnbanned(user);
                            }
                            else
                                _gateway_gatewayLogger.Warning("GUILD_BAN_REMOVE referenced an unknown guild.");*/
                        }
                        break;

                    //Messages
                    case "MESSAGE_CREATE":
                        {
                            /*var data = msg.Payload.ToObject<MessageCreateEvent>(Serializer);

                            Channel channel = GetChannel(data.ChannelId);
                            if (channel != null)
                            {
                                var user = channel.GetUserFast(data.Author.Id);

                                if (user != null)
                                {
                                    Message msg = null;
                                    bool isAuthor = data.Author.Id == CurrentUser.Id;
                                    //ulong nonce = 0;

                                    //if (data.Author.Id == _privateUser.Id && Config.UseMessageQueue)
                                    //{
                                    //    if (data.Nonce != null && ulong.TryParse(data.Nonce, out nonce))
                                    //        msg = _messages[nonce];
                                    //}
                                    if (msg == null)
                                    {
                                        msg = channel.AddMessage(data.Id, user, data.Timestamp.Value);
                                        //nonce = 0;
                                    }

                                    //Remapped queued message
                                    //if (nonce != 0)
                                    //{
                                    //    msg = _messages.Remap(nonce, data.Id);
                                    //    msg.Id = data.Id;
                                    //    RaiseMessageSent(msg);
                                    //}

                                    msg.Update(data);
                                    user.UpdateActivity();

                                    _gateway_gatewayLogger.Verbose($"MESSAGE_CREATE: {channel.Path} ({user.Name ?? "Unknown"})");
                                    OnMessageReceived(msg);
                                }
                                else
                                    _gateway_gatewayLogger.Warning("MESSAGE_CREATE referenced an unknown user.");
                            }
                            else
                                _gateway_gatewayLogger.Warning("MESSAGE_CREATE referenced an unknown channel.");*/
                        }
                        break;
                    case "MESSAGE_UPDATE":
                        {
                            /*var data = msg.Payload.ToObject<MessageUpdateEvent>(Serializer);
                            var channel = GetChannel(data.ChannelId);
                            if (channel != null)
                            {
                                var msg = channel.GetMessage(data.Id, data.Author?.Id);
                                var before = Config.EnablePreUpdateEvents ? msg.Clone() : null;
                                msg.Update(data);
                                _gatewayLogger.Verbose($"MESSAGE_UPDATE: {channel.Path} ({data.Author?.Username ?? "Unknown"})");
                                OnMessageUpdated(before, msg);
                            }
                            else
                                _gatewayLogger.Warning("MESSAGE_UPDATE referenced an unknown channel.");*/
                        }
                        break;
                    case "MESSAGE_DELETE":
                        {
                            /*var data = msg.Payload.ToObject<MessageDeleteEvent>(Serializer);
                            var channel = GetChannel(data.ChannelId);
                            if (channel != null)
                            {
                                var msg = channel.RemoveMessage(data.Id);
                                _gatewayLogger.Verbose($"MESSAGE_DELETE: {channel.Path} ({msg.User?.Name ?? "Unknown"})");
                                OnMessageDeleted(msg);
                            }
                            else
                                _gatewayLogger.Warning("MESSAGE_DELETE referenced an unknown channel.");*/
                        }
                        break;

                    //Statuses
                    case "PRESENCE_UPDATE":
                        {
                            /*var data = msg.Payload.ToObject<PresenceUpdateEvent>(Serializer);
                            User user;
                            Server server;
                            if (data.GuildId == null)
                            {
                                server = null;
                                user = GetPrivateChannel(data.User.Id)?.Recipient;
                            }
                            else
                            {
                                server = GetServer(data.GuildId.Value);
                                if (server == null)
                                {
                                    _gatewayLogger.Warning("PRESENCE_UPDATE referenced an unknown server.");
                                    break;
                                }
                                else
                                    user = server.GetUser(data.User.Id);
                            }

                            if (user != null)
                            {
                                if (Config.LogLevel == LogSeverity.Debug)
                                    _gatewayLogger.Debug($"PRESENCE_UPDATE: {user.Path}");
                                var before = Config.EnablePreUpdateEvents ? user.Clone() : null;
                                user.Update(data);
                                OnUserUpdated(before, user);
                            }
                            //else //Occurs when a user leaves a server
                            //    _gatewayLogger.Warning("PRESENCE_UPDATE referenced an unknown user.");*/
                        }
                        break;
                    case "TYPING_START":
                        {
                            /*var data = msg.Payload.ToObject<TypingStartEvent>(Serializer);
                            var channel = GetChannel(data.ChannelId);
                            if (channel != null)
                            {
                                User user;
                                if (channel.IsPrivate)
                                {
                                    if (channel.Recipient.Id == data.UserId)
                                        user = channel.Recipient;
                                    else
                                        break;
                                }
                                else
                                    user = channel.Server.GetUser(data.UserId);
                                if (user != null)
                                {
                                    if (Config.LogLevel == LogSeverity.Debug)
                                        _gatewayLogger.Debug($"TYPING_START: {channel.Path} ({user.Name})");
                                    OnUserIsTypingUpdated(channel, user);
                                    user.UpdateActivity();
                                }
                            }
                            else
                                _gatewayLogger.Warning("TYPING_START referenced an unknown channel.");*/
                        }
                        break;

                    //Voice
                    case "VOICE_STATE_UPDATE":
                        {
                            /*var data = msg.Payload.ToObject<VoiceStateUpdateEvent>(Serializer);
                            var server = GetServer(data.GuildId);
                            if (server != null)
                            {
                                var user = server.GetUser(data.UserId);
                                if (user != null)
                                {
                                    if (Config.LogLevel == LogSeverity.Debug)
                                        _gatewayLogger.Debug($"VOICE_STATE_UPDATE: {user.Path}");
                                    var before = Config.EnablePreUpdateEvents ? user.Clone() : null;
                                    user.Update(data);
                                    //_gatewayLogger.Verbose($"Voice Updated: {server.Name}/{user.Name}");
                                    OnUserUpdated(before, user);
                                }
                                //else //Occurs when a user leaves a server
                                //    _gatewayLogger.Warning("VOICE_STATE_UPDATE referenced an unknown user.");
                            }
                            else
                                _gatewayLogger.Warning("VOICE_STATE_UPDATE referenced an unknown server.");*/
                        }
                        break;

                    //Settings
                    case "USER_UPDATE":
                        {
                            /*var data = msg.Payload.ToObject<UserUpdateEvent>(Serializer);
                            if (data.Id == CurrentUser.Id)
                            {
                                var before = Config.EnablePreUpdateEvents ? CurrentUser.Clone() : null;
                                CurrentUser.Update(data);
                                foreach (var server in _servers)
                                    server.Value.CurrentUser.Update(data);
                                _gatewayLogger.Info($"USER_UPDATE");
                                OnProfileUpdated(before, CurrentUser);
                            }*/
                        }
                        break;

                    //Handled in GatewaySocket
                    case "RESUMED":
                        break;

                    //Ignored
                    case "USER_SETTINGS_UPDATE":
                    case "MESSAGE_ACK": //TODO: Add (User only)
                    case "GUILD_EMOJIS_UPDATE": //TODO: Add
                    case "GUILD_INTEGRATIONS_UPDATE": //TODO: Add
                    case "VOICE_SERVER_UPDATE": //TODO: Add
                        _gatewayLogger.Debug($"{msg.Type} [Ignored]");
                        break;

                    //Others
                    default:
                        _gatewayLogger.Warning($"Unknown message type: {msg.Type}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _gatewayLogger.Error($"Error handling {msg.Type} event", ex);
            }
        }

        Task<IChannel> IDiscordClient.GetChannel(ulong id)
            => Task.FromResult<IChannel>(GetChannel(id));
        Task<IEnumerable<IDMChannel>> IDiscordClient.GetDMChannels()
            => Task.FromResult<IEnumerable<IDMChannel>>(DMChannels.ToImmutableArray());
        async Task<IEnumerable<IConnection>> IDiscordClient.GetConnections()
            => await GetConnections().ConfigureAwait(false);
        async Task<IInvite> IDiscordClient.GetInvite(string inviteIdOrXkcd)
            => await GetInvite(inviteIdOrXkcd).ConfigureAwait(false);
        Task<IGuild> IDiscordClient.GetGuild(ulong id)
            => Task.FromResult<IGuild>(GetGuild(id));
        Task<IEnumerable<IUserGuild>> IDiscordClient.GetGuilds()
            => Task.FromResult<IEnumerable<IUserGuild>>(Guilds.ToImmutableArray());
        async Task<IGuild> IDiscordClient.CreateGuild(string name, IVoiceRegion region, Stream jpegIcon)
            => await CreateGuild(name, region, jpegIcon).ConfigureAwait(false);
        Task<IUser> IDiscordClient.GetUser(ulong id)
            => Task.FromResult<IUser>(GetUser(id));
        Task<IUser> IDiscordClient.GetUser(string username, ushort discriminator)
            => Task.FromResult<IUser>(GetUser(username, discriminator));
        Task<ISelfUser> IDiscordClient.GetCurrentUser()
            => Task.FromResult<ISelfUser>(CurrentUser);
        async Task<IEnumerable<IUser>> IDiscordClient.QueryUsers(string query, int limit)
            => await QueryUsers(query, limit).ConfigureAwait(false);
        Task<IEnumerable<IVoiceRegion>> IDiscordClient.GetVoiceRegions()
            => Task.FromResult<IEnumerable<IVoiceRegion>>(VoiceRegions.ToImmutableArray());
        Task<IVoiceRegion> IDiscordClient.GetVoiceRegion(string id)
            => Task.FromResult<IVoiceRegion>(GetVoiceRegion(id));
    }
}
