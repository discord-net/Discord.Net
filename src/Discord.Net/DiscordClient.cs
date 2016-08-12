using Discord.API.Client.GatewaySocket;
using Discord.API.Client.Rest;
using Discord.Logging;
using Discord.Net;
using Discord.Net.Rest;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary> Provides a connection to the DiscordApp service. </summary>
    public partial class DiscordClient : IDisposable
    {
        private readonly AsyncLock _connectionLock;
        private readonly ManualResetEvent _disconnectedEvent;
        private readonly ManualResetEventSlim _connectedEvent;
        private readonly TaskManager _taskManager;
        private readonly ServiceCollection _services;
        private ConcurrentDictionary<ulong, Server> _servers;
        private ConcurrentDictionary<ulong, Channel> _channels;
        private ConcurrentDictionary<ulong, Channel> _privateChannels; //Key = RecipientId
        private Dictionary<string, Region> _regions;
        private Stopwatch _connectionStopwatch;
        private ConcurrentQueue<ulong> _largeServers;

        internal Logger Logger { get; }

        /// <summary> Gets the configuration object used to make this client. </summary>
        public DiscordConfig Config { get; }
        /// <summary> Gets the log manager. </summary>
        public LogManager Log { get; }
        /// <summary> Gets the internal RestClient for the Client API endpoint. </summary>
        public RestClient ClientAPI { get; }
        /// <summary> Gets the internal RestClient for the Status API endpoint. </summary>
        public RestClient StatusAPI { get; }
        /// <summary> Gets the internal WebSocket for the Gateway event stream. </summary>
        public GatewaySocket GatewaySocket { get; }
        /// <summary> Gets the queue used for outgoing messages, if enabled. </summary>
        public MessageQueue MessageQueue { get; }
        /// <summary> Gets the JSON serializer used by this client. </summary>
        public JsonSerializer Serializer { get; }

        /// <summary> Gets the current connection state of this client. </summary>
        public ConnectionState State { get; private set; }
        /// <summary> Gets a cancellation token that triggers when the client is manually disconnected. </summary>
        public CancellationToken CancelToken { get; private set; }
        /// <summary> Gets the current logged-in user used in private channels. </summary>
        internal User PrivateUser { get; private set; }
        /// <summary> Gets information about the current logged-in account. </summary>
        public Profile CurrentUser { get; private set; }
        /// <summary> Gets the session id for the current connection. </summary>
        public string SessionId { get; private set; }
        /// <summary> Gets the status of the current user. </summary>
        public UserStatus Status { get; private set; }
        /// <summary> Gets the game the current user is displayed as playing. </summary>
        public Game CurrentGame { get; private set; }

        /// <summary> Gets a collection of all extensions added to this DiscordClient. </summary>
        public IEnumerable<IService> Services => _services;
        /// <summary> Gets a collection of all servers this client is a member of. </summary>
        public IEnumerable<Server> Servers => _servers.Select(x => x.Value);
        /// <summary> Gets a collection of all private channels this client is a member of. </summary>
        public IEnumerable<Channel> PrivateChannels => _privateChannels.Select(x => x.Value);
        /// <summary> Gets a collection of all voice regions currently offered by Discord. </summary>
        public IEnumerable<Region> Regions => _regions.Select(x => x.Value);

        /// <summary> Initializes a new instance of the DiscordClient class. </summary>
        public DiscordClient(Action<DiscordConfigBuilder> configFunc)
            : this(ProcessConfig(configFunc))
        {
        }
        private static DiscordConfigBuilder ProcessConfig(Action<DiscordConfigBuilder> func)
        {
            var config = new DiscordConfigBuilder();
            func(config);
            return config;
        }

        /// <summary> Initializes a new instance of the DiscordClient class. </summary>
        public DiscordClient()
            : this(new DiscordConfigBuilder())
        {
        }
        /// <summary> Initializes a new instance of the DiscordClient class. </summary>
        public DiscordClient(DiscordConfigBuilder builder)
            : this(builder.Build())
        {
            if (builder.LogHandler != null)
                Log.Message += builder.LogHandler;
        }
        /// <summary> Initializes a new instance of the DiscordClient class. </summary>
        public DiscordClient(DiscordConfig config)
        {
            Config = config;

            State = (int)ConnectionState.Disconnected;
            Status = UserStatus.Online;

            //Logging
            Log = new LogManager(this);
            Logger = Log.CreateLogger("Discord");
            if (config.LogLevel >= LogSeverity.Verbose)
                _connectionStopwatch = new Stopwatch();

            //Async
            _taskManager = new TaskManager(Cleanup);
            _connectionLock = new AsyncLock();
            _disconnectedEvent = new ManualResetEvent(true);
            _connectedEvent = new ManualResetEventSlim(false);
            CancelToken = new CancellationToken(true);

            //Cache
            //ConcurrentLevel = 2 (only REST and WebSocket can add/remove)
            _servers = new ConcurrentDictionary<ulong, Server>(2, 0);
            _channels = new ConcurrentDictionary<ulong, Channel>(2, 0);
            _privateChannels = new ConcurrentDictionary<ulong, Channel>(2, 0);
            _largeServers = new ConcurrentQueue<ulong>();

            //Serialization
            Serializer = new JsonSerializer();
            Serializer.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
#if TEST_RESPONSES
            Serializer.CheckAdditionalContent = true;
            Serializer.MissingMemberHandling = MissingMemberHandling.Error;
#else
            Serializer.CheckAdditionalContent = false;
            Serializer.MissingMemberHandling = MissingMemberHandling.Ignore;
#endif
            Serializer.Error += (s, e) =>
            {
                e.ErrorContext.Handled = true;
                Logger.Error("Serialization Failed", e.ErrorContext.Error);
            };

            //Networking
            ClientAPI = new JsonRestClient(Config, DiscordConfig.ClientAPIUrl, Log.CreateLogger("ClientAPI"));
            StatusAPI = new JsonRestClient(Config, DiscordConfig.StatusAPIUrl, Log.CreateLogger("StatusAPI"));
            GatewaySocket = new GatewaySocket(Config, Serializer, Log.CreateLogger("Gateway"));

            //GatewaySocket.Disconnected += (s, e) => OnDisconnected(e.WasUnexpected, e.Exception);
            GatewaySocket.ReceivedDispatch += (s, e) => OnReceivedEvent(e);

            MessageQueue = new MessageQueue(ClientAPI, Log.CreateLogger("MessageQueue"));

            //Extensibility
            _services = new ServiceCollection(this);
        }

        /// <summary> Connects to the Discord server with the provided email and password. </summary>
        /// <returns> Returns a token that can be optionally stored to speed up future connections. </returns>
        public async Task<string> Connect(string email, string password, string token = null)
        {
            if (email == null) throw new ArgumentNullException(email);
            if (password == null) throw new ArgumentNullException(password);

            await BeginConnect(email, password, null).ConfigureAwait(false);
            return ClientAPI.Token;
        }
        /// <summary> Connects to the Discord server with the provided token. </summary>
        public async Task Connect(string token, TokenType tokenType)
        {
            if (token == null) throw new ArgumentNullException(token);

            await BeginConnect(null, null, token, tokenType).ConfigureAwait(false);
        }

        private async Task BeginConnect(string email, string password, string token = null, TokenType tokenType = TokenType.User)
        {
            try
            {
                using (await _connectionLock.LockAsync().ConfigureAwait(false))
                {
                    await Disconnect().ConfigureAwait(false);
                    _taskManager.ClearException();

                    Stopwatch stopwatch = null;
                    if (Config.LogLevel >= LogSeverity.Verbose)
                    {
                        _connectionStopwatch.Restart();
                        stopwatch = Stopwatch.StartNew();
                    }
                    State = ConnectionState.Connecting;
                    _disconnectedEvent.Reset();

                    var cancelSource = new CancellationTokenSource();
                    CancelToken = cancelSource.Token;
                    ClientAPI.CancelToken = CancelToken;
                    StatusAPI.CancelToken = CancelToken;

                    switch (tokenType)
                    {
                        case TokenType.Bot:
                            token = $"Bot {token}";
                            break;
                        case TokenType.User:
                            break;
                        default:
                            throw new ArgumentException("Unknown oauth token type", nameof(tokenType));
                    }

                    await Login(email, password, token).ConfigureAwait(false);
                    await GatewaySocket.Connect(ClientAPI, CancelToken).ConfigureAwait(false);

                    var tasks = new[] { CancelToken.Wait(), LargeServerDownloader(CancelToken) }
                        .Concat(MessageQueue.Run(CancelToken));

                    await _taskManager.Start(tasks, cancelSource).ConfigureAwait(false);
                    GatewaySocket.WaitForConnection(CancelToken);

                    if (Config.LogLevel >= LogSeverity.Verbose)
                    {
                        stopwatch.Stop();
                        double seconds = Math.Round(stopwatch.ElapsedTicks / (double)TimeSpan.TicksPerSecond, 2);
                        Logger.Verbose($"Handshake + Ready took {seconds} sec");
                    }
                }
            }
            catch (Exception ex)
            {
                await _taskManager.SignalError(ex).ConfigureAwait(false);
                throw;
            }
        }
        private async Task Login(string email = null, string password = null, string token = null)
        {
            string tokenPath = null, oldToken = null;
            byte[] cacheKey = null;

            //Get Token
            if (email != null && Config.CacheDir != null)
            {
                tokenPath = GetTokenCachePath(email);
                if (token == null && password != null)
                {
                    Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password,
                        new byte[] { 0x5A, 0x2A, 0xF8, 0xCF, 0x78, 0xD3, 0x7D, 0x0D });
                    cacheKey = deriveBytes.GetBytes(16);

                    oldToken = LoadToken(tokenPath, cacheKey);
                    token = oldToken;
                }
            }

            ClientAPI.Token = token;
            if (email != null && password != null)
            {
                var request = new LoginRequest() { Email = email, Password = password };
                var response = await ClientAPI.Send(request).ConfigureAwait(false);
                token = response.Token;
                if (Config.CacheDir != null && token != oldToken && tokenPath != null)
                    SaveToken(tokenPath, cacheKey, token);
                ClientAPI.Token = token;
            }

            //Cache other stuff
            var regionsResponse = (await ClientAPI.Send(new GetVoiceRegionsRequest()).ConfigureAwait(false));
            _regions = regionsResponse.Select(x => new Region(x.Id, x.Name, x.Hostname, x.Port, x.Vip))
                .ToDictionary(x => x.Id);
        }
        private void EndConnect()
        {
            if (State == ConnectionState.Connecting)
            {
                State = ConnectionState.Connected;
                _connectedEvent.Set();

                if (Config.LogLevel >= LogSeverity.Verbose)
                {
                    _connectionStopwatch.Stop();
                    double seconds = Math.Round(_connectionStopwatch.ElapsedTicks / (double)TimeSpan.TicksPerSecond, 2);
                    Logger.Verbose($"Connection took {seconds} sec");
                }

                SendStatus();
                OnReady();
            }
        }

        /// <summary> Disconnects from the Discord server, canceling any pending requests. </summary>
        public Task Disconnect() => _taskManager.Stop(true);
        private async Task Cleanup()
        {
            var oldState = State;
            State = ConnectionState.Disconnecting;

            if (oldState == ConnectionState.Connected)
            {
                try { await ClientAPI.Send(new LogoutRequest()).ConfigureAwait(false); }
                catch (OperationCanceledException) { }
            }

            ulong serverId;
            while (_largeServers.TryDequeue(out serverId)) { }

            MessageQueue.Clear();

            await GatewaySocket.Disconnect().ConfigureAwait(false);
            ClientAPI.Token = null;

            _servers.Clear();
            _channels.Clear();
            _privateChannels.Clear();

            PrivateUser = null;
            CurrentUser = null;

            State = (int)ConnectionState.Disconnected;
            _connectedEvent.Reset();
            _disconnectedEvent.Set();
        }

        public void SetStatus(UserStatus status)
        {
            if (status == null) throw new ArgumentNullException(nameof(status));
            if (status != UserStatus.Online && status != UserStatus.Idle)
                throw new ArgumentException($"Invalid status, must be {UserStatus.Online} or {UserStatus.Idle}", nameof(status));

            Status = status;
            SendStatus();
        }
        public void SetGame(Game game)
        {
            CurrentGame = game;
            SendStatus();
        }
        public void SetGame(string game)
        {
            CurrentGame = new Game(game);
            SendStatus();
        }
        public void SetGame(string game, GameType type, string url)
        {
            CurrentGame = new Game(game, type, url);
            SendStatus();
        }
        private void SendStatus()
        {
            PrivateUser.Status = Status;
            PrivateUser.CurrentGame = CurrentGame;
            foreach (var server in Servers)
            {
                var current = server.CurrentUser;
                if (current != null)
                {
                    current.Status = Status;
                    current.CurrentGame = CurrentGame;
                }
            }
            var socket = GatewaySocket;
            if (socket != null)
                socket.SendUpdateStatus(Status == UserStatus.Idle ? EpochTime.GetMilliseconds() - (10 * 60 * 1000) : (long?)null, CurrentGame);
        }

        #region Channels
        internal void AddChannel(Channel channel)
        {
            _channels.GetOrAdd(channel.Id, channel);
        }
        private Channel RemoveChannel(ulong id)
        {
            Channel channel;
            if (_channels.TryRemove(id, out channel))
            {
                if (channel.IsPrivate)
                    _privateChannels.TryRemove(channel.Recipient.Id, out channel);
                else
                    channel.Server.RemoveChannel(id);
            }
            return channel;
        }
        public Channel GetChannel(ulong id)
        {
            Channel channel;
            _channels.TryGetValue(id, out channel);
            return channel;
        }

        private Channel AddPrivateChannel(ulong id, ulong recipientId)
        {
            Channel channel;
            if (_channels.TryGetOrAdd(id, x => new Channel(this, x, new User(this, recipientId, null)), out channel))
                _privateChannels[recipientId] = channel;
            return channel;
        }
        internal Channel GetPrivateChannel(ulong recipientId)
        {
            Channel channel;
            _privateChannels.TryGetValue(recipientId, out channel);
            return channel;
        }
        internal Task<Channel> CreatePMChannel(User user)
            => CreatePrivateChannel(user.Id);
        public async Task<Channel> CreatePrivateChannel(ulong userId)
        {
            var channel = GetPrivateChannel(userId);
            if (channel != null) return channel;

            var request = new CreatePrivateChannelRequest() { RecipientId = userId };
            var response = await ClientAPI.Send(request).ConfigureAwait(false);

            channel = AddPrivateChannel(response.Id, userId);
            channel.Update(response);
            return channel;
        }
        #endregion

        #region Invites
        /// <summary> Gets more info about the provided invite code. </summary>
        /// <remarks> Supported formats: inviteCode, xkcdCode, https://discord.gg/inviteCode, https://discord.gg/xkcdCode </remarks>
        /// <returns> The invite object if found, null if not. </returns>
        public async Task<Invite> GetInvite(string inviteIdOrXkcd)
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
                var response = await ClientAPI.Send(new GetInviteRequest(inviteIdOrXkcd)).ConfigureAwait(false);
                var invite = new Invite(this, response.Code, response.XkcdPass);
                invite.Update(response);
                return invite;
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }
        #endregion

        #region Regions
        public Region GetRegion(string id)
        {
            Region region;
            if (_regions.TryGetValue(id, out region))
                return region;
            else
                return new Region(id, id, "", 0, false);
        }
        #endregion

        #region Servers
        private Server AddServer(ulong id)
            => _servers.GetOrAdd(id, x => new Server(this, x));
        private Server RemoveServer(ulong id)
        {
            Server server;
            if (_servers.TryRemove(id, out server))
            {
                foreach (var channel in server.AllChannels)
                    RemoveChannel(channel.Id);
            }
            return server;
        }

        public Server GetServer(ulong id)
        {
            Server server;
            _servers.TryGetValue(id, out server);
            return server;
        }
        public IEnumerable<Server> FindServers(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return _servers.Select(x => x.Value).Find(name);
        }

        /// <summary> Creates a new server with the provided name and region. </summary>
        public async Task<Server> CreateServer(string name, Region region, ImageType iconType = ImageType.None, Stream icon = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (region == null) throw new ArgumentNullException(nameof(region));

            var request = new CreateGuildRequest()
            {
                Name = name,
                Region = region.Id,
                IconBase64 = icon.Base64(iconType, null)
            };
            var response = await ClientAPI.Send(request).ConfigureAwait(false);

            var server = AddServer(response.Id);
            server.Update(response);
            return server;
        }
        #endregion

        #region Gateway Events
        private void OnReceivedEvent(WebSocketEventEventArgs e)
        {
            try
            {
                switch (e.Type)
                {
                    //Global
                    case "READY":
                        {
                            //TODO: None of this is really threadsafe - should only replace the cache collections when they have been fully populated

                            var data = e.Payload.ToObject<ReadyEvent>(Serializer);

                            //ConcurrencyLevel = 2 (only REST and WebSocket can add/remove)
                            _servers = new ConcurrentDictionary<ulong, Server>(2, (int)(data.Guilds.Length * 1.05));
                            _channels = new ConcurrentDictionary<ulong, Channel>(2, (int)(data.Guilds.Length * 2 * 1.05));
                            _privateChannels = new ConcurrentDictionary<ulong, Channel>(2, (int)(data.PrivateChannels.Length * 1.05));

                            SessionId = data.SessionId;
                            PrivateUser = new User(this, data.User.Id, null);
                            PrivateUser.Update(data.User);
                            CurrentUser = new Profile(this, data.User.Id);
                            CurrentUser.Update(data.User);

                            for (int i = 0; i < data.Guilds.Length; i++)
                            {
                                var model = data.Guilds[i];
                                if (model.Unavailable != true)
                                {
                                    var server = AddServer(model.Id);
                                    server.Update(model);
                                }
                                if (model.IsLarge)
                                    _largeServers.Enqueue(model.Id);
                            }
                            for (int i = 0; i < data.PrivateChannels.Length; i++)
                            {
                                var model = data.PrivateChannels[i];
                                var channel = AddPrivateChannel(model.Id, model.Recipient.Id);
                                channel.Update(model);
                            }

                            EndConnect();
                        }
                        break;

                    //Servers
                    case "GUILD_CREATE":
                        {
                            var data = e.Payload.ToObject<GuildCreateEvent>(Serializer);
                            if (data.Unavailable != true)
                            {
                                var server = AddServer(data.Id);
                                server.Update(data);

                                if (data.Unavailable != false)
                                {
                                    Logger.Info($"GUILD_CREATE: {server.Path}");
                                    OnJoinedServer(server);
                                }
                                else
                                    Logger.Info($"GUILD_AVAILABLE: {server.Path}");

                                if (!data.IsLarge)
                                    OnServerAvailable(server);
                                else
                                    _largeServers.Enqueue(data.Id);
                            }
                        }
                        break;
                    case "GUILD_UPDATE":
                        {
                            var data = e.Payload.ToObject<GuildUpdateEvent>(Serializer);
                            var server = GetServer(data.Id);
                            if (server != null)
                            {
                                var before = Config.EnablePreUpdateEvents ? server.Clone() : null;
                                server.Update(data);
                                Logger.Info($"GUILD_UPDATE: {server.Path}");
                                OnServerUpdated(before, server);
                            }
                            else
                                Logger.Warning("GUILD_UPDATE referenced an unknown guild.");
                        }
                        break;
                    case "GUILD_DELETE":
                        {
                            var data = e.Payload.ToObject<GuildDeleteEvent>(Serializer);
                            Server server = RemoveServer(data.Id);
                            if (server != null)
                            {
                                if (data.Unavailable != true)
                                    Logger.Info($"GUILD_DELETE: {server.Path}");
                                else
                                    Logger.Info($"GUILD_UNAVAILABLE: {server.Path}");

                                OnServerUnavailable(server);
                                if (data.Unavailable != true)
                                    OnLeftServer(server);
                            }
                            else
                                Logger.Warning("GUILD_DELETE referenced an unknown guild.");
                        }
                        break;

                    //Channels
                    case "CHANNEL_CREATE":
                        {
                            var data = e.Payload.ToObject<ChannelCreateEvent>(Serializer);

                            Channel channel = null;
                            if (data.GuildId != null)
                            {
                                var server = GetServer(data.GuildId.Value);
                                if (server != null)
                                    channel = server.AddChannel(data.Id, true);
                                else
                                    Logger.Warning("CHANNEL_CREATE referenced an unknown guild.");
                            }
                            else
                                channel = AddPrivateChannel(data.Id, data.Recipient.Id);
                            if (channel != null)
                            {
                                channel.Update(data);
                                Logger.Info($"CHANNEL_CREATE: {channel.Path}");
                                OnChannelCreated(channel);
                            }
                        }
                        break;
                    case "CHANNEL_UPDATE":
                        {
                            var data = e.Payload.ToObject<ChannelUpdateEvent>(Serializer);
                            var channel = GetChannel(data.Id);
                            if (channel != null)
                            {
                                var before = Config.EnablePreUpdateEvents ? channel.Clone() : null;
                                channel.Update(data);
                                Logger.Info($"CHANNEL_UPDATE: {channel.Path}");
                                OnChannelUpdated(before, channel);
                            }
                            else
                                Logger.Warning("CHANNEL_UPDATE referenced an unknown channel.");
                        }
                        break;
                    case "CHANNEL_DELETE":
                        {
                            var data = e.Payload.ToObject<ChannelDeleteEvent>(Serializer);
                            var channel = RemoveChannel(data.Id);
                            if (channel != null)
                            {
                                Logger.Info($"CHANNEL_DELETE: {channel.Path}");
                                OnChannelDestroyed(channel);
                            }
                            else
                                Logger.Warning("CHANNEL_DELETE referenced an unknown channel.");
                        }
                        break;

                    //Members
                    case "GUILD_MEMBER_ADD":
                        {
                            var data = e.Payload.ToObject<GuildMemberAddEvent>(Serializer);
                            var server = GetServer(data.GuildId.Value);
                            if (server != null)
                            {
                                var user = server.AddUser(data.User.Id, true, true);
                                user.Update(data);
                                user.UpdateActivity();
                                Logger.Info($"GUILD_MEMBER_ADD: {user.Path}");
                                OnUserJoined(user);
                            }
                            else
                                Logger.Warning("GUILD_MEMBER_ADD referenced an unknown guild.");
                        }
                        break;
                    case "GUILD_MEMBER_UPDATE":
                        {
                            var data = e.Payload.ToObject<GuildMemberUpdateEvent>(Serializer);
                            var server = GetServer(data.GuildId.Value);
                            if (server != null)
                            {
                                var user = server.GetUser(data.User.Id);
                                if (user != null)
                                {
                                    var before = Config.EnablePreUpdateEvents ? user.Clone() : null;
                                    user.Update(data);
                                    Logger.Info($"GUILD_MEMBER_UPDATE: {user.Path}");
                                    OnUserUpdated(before, user);
                                }
                                else
                                    Logger.Warning("GUILD_MEMBER_UPDATE referenced an unknown user.");
                            }
                            else
                                Logger.Warning("GUILD_MEMBER_UPDATE referenced an unknown guild.");
                        }
                        break;
                    case "GUILD_MEMBER_REMOVE":
                        {
                            var data = e.Payload.ToObject<GuildMemberRemoveEvent>(Serializer);
                            var server = GetServer(data.GuildId.Value);
                            if (server != null)
                            {
                                var user = server.RemoveUser(data.User.Id);
                                if (user != null)
                                {
                                    Logger.Info($"GUILD_MEMBER_REMOVE: {user.Path}");
                                    OnUserLeft(user);
                                }
                                else
                                    Logger.Warning("GUILD_MEMBER_REMOVE referenced an unknown user.");
                            }
                            else
                                Logger.Warning("GUILD_MEMBER_REMOVE referenced an unknown guild.");
                        }
                        break;
                    case "GUILD_MEMBERS_CHUNK":
                        {
                            var data = e.Payload.ToObject<GuildMembersChunkEvent>(Serializer);
                            var server = GetServer(data.GuildId);
                            if (server != null)
                            {
                                foreach (var memberData in data.Members)
                                {
                                    var user = server.AddUser(memberData.User.Id, true, false);
                                    user.Update(memberData);
                                }
                                Logger.Verbose($"GUILD_MEMBERS_CHUNK: {data.Members.Length} users");

                                if (server.CurrentUserCount >= server.UserCount) //Finished downloading for there
                                    OnServerAvailable(server);
                            }
                            else
                                Logger.Warning("GUILD_MEMBERS_CHUNK referenced an unknown guild.");
                        }
                        break;

                    //Roles
                    case "GUILD_ROLE_CREATE":
                        {
                            var data = e.Payload.ToObject<GuildRoleCreateEvent>(Serializer);
                            var server = GetServer(data.GuildId);
                            if (server != null)
                            {
                                var role = server.AddRole(data.Data.Id);
                                role.Update(data.Data, false);
                                Logger.Info($"GUILD_ROLE_CREATE: {role.Path}");
                                OnRoleCreated(role);
                            }
                            else
                                Logger.Warning("GUILD_ROLE_CREATE referenced an unknown guild.");
                        }
                        break;
                    case "GUILD_ROLE_UPDATE":
                        {
                            var data = e.Payload.ToObject<GuildRoleUpdateEvent>(Serializer);
                            var server = GetServer(data.GuildId);
                            if (server != null)
                            {
                                var role = server.GetRole(data.Data.Id);
                                if (role != null)
                                {
                                    var before = Config.EnablePreUpdateEvents ? role.Clone() : null;
                                    role.Update(data.Data, true);
                                    Logger.Info($"GUILD_ROLE_UPDATE: {role.Path}");
                                    OnRoleUpdated(before, role);
                                }
                                else
                                    Logger.Warning("GUILD_ROLE_UPDATE referenced an unknown role.");
                            }
                            else
                                Logger.Warning("GUILD_ROLE_UPDATE referenced an unknown guild.");
                        }
                        break;
                    case "GUILD_ROLE_DELETE":
                        {
                            var data = e.Payload.ToObject<GuildRoleDeleteEvent>(Serializer);
                            var server = GetServer(data.GuildId);
                            if (server != null)
                            {
                                var role = server.RemoveRole(data.RoleId);
                                if (role != null)
                                {
                                    Logger.Info($"GUILD_ROLE_DELETE: {role.Path}");
                                    OnRoleDeleted(role);
                                }
                                else
                                    Logger.Warning("GUILD_ROLE_DELETE referenced an unknown role.");
                            }
                            else
                                Logger.Warning("GUILD_ROLE_DELETE referenced an unknown guild.");
                        }
                        break;

                    //Bans
                    case "GUILD_BAN_ADD":
                        {
                            var data = e.Payload.ToObject<GuildBanAddEvent>(Serializer);
                            var server = GetServer(data.GuildId.Value);
                            if (server != null)
                            {
                                var user = server.GetUser(data.User.Id);
                                if (user != null)
                                {
                                    Logger.Info($"GUILD_BAN_ADD: {user.Path}");
                                    OnUserBanned(user);
                                }
                                else
                                    Logger.Warning("GUILD_BAN_ADD referenced an unknown user.");
                            }
                            else
                                Logger.Warning("GUILD_BAN_ADD referenced an unknown guild.");
                        }
                        break;
                    case "GUILD_BAN_REMOVE":
                        {
                            var data = e.Payload.ToObject<GuildBanRemoveEvent>(Serializer);
                            var server = GetServer(data.GuildId.Value);
                            if (server != null)
                            {
                                var user = new User(this, data.User.Id, server);
                                user.Update(data.User);
                                Logger.Info($"GUILD_BAN_REMOVE: {user.Path}");
                                OnUserUnbanned(user);
                            }
                            else
                                Logger.Warning("GUILD_BAN_REMOVE referenced an unknown guild.");
                        }
                        break;

                    //Messages
                    case "MESSAGE_CREATE":
                        {
                            var data = e.Payload.ToObject<MessageCreateEvent>(Serializer);

                            Channel channel = GetChannel(data.ChannelId);
                            if (channel != null)
                            {
                                var user = channel.GetUserFast(data.Author.Id);

                                if (user != null)
                                {
                                    Message msg = null;
                                    bool isAuthor = data.Author.Id == CurrentUser.Id;
                                    //ulong nonce = 0;

                                    /*if (data.Author.Id == _privateUser.Id && Config.UseMessageQueue)
                                    {
                                        if (data.Nonce != null && ulong.TryParse(data.Nonce, out nonce))
                                            msg = _messages[nonce];
                                    }*/
                                    if (msg == null)
                                    {
                                        msg = channel.AddMessage(data.Id, user, data.Timestamp.Value);
                                        //nonce = 0;
                                    }

                                    //Remapped queued message
                                    /*if (nonce != 0)
                                    {
                                        msg = _messages.Remap(nonce, data.Id);
                                        msg.Id = data.Id;
                                        RaiseMessageSent(msg);
                                    }*/

                                    msg.Update(data);
                                    user.UpdateActivity();

                                    Logger.Verbose($"MESSAGE_CREATE: {channel.Path} ({user.Name ?? "Unknown"})");
                                    OnMessageReceived(msg);
                                }
                                else
                                    Logger.Warning("MESSAGE_CREATE referenced an unknown user.");
                            }
                            else
                                Logger.Warning("MESSAGE_CREATE referenced an unknown channel.");
                        }
                        break;
                    case "MESSAGE_UPDATE":
                        {
                            var data = e.Payload.ToObject<MessageUpdateEvent>(Serializer);
                            var channel = GetChannel(data.ChannelId);
                            if (channel != null)
                            {
                                var msg = channel.GetMessage(data.Id, data.Author?.Id);
                                var before = Config.EnablePreUpdateEvents ? msg.Clone() : null;
                                msg.Update(data);
                                Logger.Verbose($"MESSAGE_UPDATE: {channel.Path} ({data.Author?.Username ?? "Unknown"})");
                                OnMessageUpdated(before, msg);
                            }
                            else
                                Logger.Warning("MESSAGE_UPDATE referenced an unknown channel.");
                        }
                        break;
                    case "MESSAGE_DELETE":
                        {
                            var data = e.Payload.ToObject<MessageDeleteEvent>(Serializer);
                            var channel = GetChannel(data.ChannelId);
                            if (channel != null)
                            {
                                var msg = channel.RemoveMessage(data.Id);
                                Logger.Verbose($"MESSAGE_DELETE: {channel.Path} ({msg.User?.Name ?? "Unknown"})");
                                OnMessageDeleted(msg);
                            }
                            else
                                Logger.Warning("MESSAGE_DELETE referenced an unknown channel.");
                        }
                        break;

                    //Statuses
                    case "PRESENCE_UPDATE":
                        {
                            var data = e.Payload.ToObject<PresenceUpdateEvent>(Serializer);
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
                                    Logger.Warning("PRESENCE_UPDATE referenced an unknown server.");
                                    break;
                                }
                                else
                                    user = server.GetUser(data.User.Id);
                            }

                            if (user != null)
                            {
                                if (Config.LogLevel == LogSeverity.Debug)
                                    Logger.Debug($"PRESENCE_UPDATE: {user.Path}");
                                var before = Config.EnablePreUpdateEvents ? user.Clone() : null;
                                user.Update(data);
                                OnUserUpdated(before, user);
                            }
                            /*else //Occurs when a user leaves a server
                                Logger.Warning("PRESENCE_UPDATE referenced an unknown user.");*/
                        }
                        break;
                    case "TYPING_START":
                        {
                            var data = e.Payload.ToObject<TypingStartEvent>(Serializer);
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
                                        Logger.Debug($"TYPING_START: {channel.Path} ({user.Name})");
                                    OnUserIsTypingUpdated(channel, user);
                                    user.UpdateActivity();
                                }
                            }
                            else
                                Logger.Warning("TYPING_START referenced an unknown channel.");
                        }
                        break;

                    //Voice
                    case "VOICE_STATE_UPDATE":
                        {
                            var data = e.Payload.ToObject<VoiceStateUpdateEvent>(Serializer);
                            var server = GetServer(data.GuildId);
                            if (server != null)
                            {
                                var user = server.GetUser(data.UserId);
                                if (user != null)
                                {
                                    if (Config.LogLevel == LogSeverity.Debug)
                                        Logger.Debug($"VOICE_STATE_UPDATE: {user.Path}");
                                    var before = Config.EnablePreUpdateEvents ? user.Clone() : null;
                                    user.Update(data);
                                    //Logger.Verbose($"Voice Updated: {server.Name}/{user.Name}");
                                    OnUserUpdated(before, user);
                                }
                                /*else //Occurs when a user leaves a server
                                    Logger.Warning("VOICE_STATE_UPDATE referenced an unknown user.");*/
                            }
                            else
                                Logger.Warning("VOICE_STATE_UPDATE referenced an unknown server.");
                        }
                        break;

                    //Settings
                    case "USER_UPDATE":
                        {
                            var data = e.Payload.ToObject<UserUpdateEvent>(Serializer);
                            if (data.Id == CurrentUser.Id)
                            {
                                var before = Config.EnablePreUpdateEvents ? CurrentUser.Clone() : null;
                                CurrentUser.Update(data);
                                PrivateUser.Update(data);
                                foreach (var server in _servers)
                                    server.Value.CurrentUser.Update(data);
                                Logger.Info($"USER_UPDATE");
                                OnProfileUpdated(before, CurrentUser);
                            }
                        }
                        break;

                    //Handled in GatewaySocket
                    case "RESUMED":
                        break;

                    //Ignored
                    case "USER_SETTINGS_UPDATE":
                    case "GUILD_INTEGRATIONS_UPDATE":
                    case "VOICE_SERVER_UPDATE":
                    case "GUILD_EMOJIS_UPDATE":
                    case "MESSAGE_ACK":
                        Logger.Debug($"{e.Type} [Ignored]");
                        break;

                    //Others
                    default:
                        Logger.Warning($"Unknown message type: {e.Type}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error handling {e.Type} event", ex);
            }
        }
        #endregion

        #region Services
        public T AddService<T>(T instance)
            where T : class, IService
            => _services.Add(instance);
        public T AddService<T>()
            where T : class, IService, new()
            => _services.Add(new T());
        public T GetService<T>(bool isRequired = true)
            where T : class, IService
            => _services.Get<T>(isRequired);
        #endregion

        #region Async Wrapper
        /// <summary> Blocking call that will execute the provided async method and wait until the client has been manually stopped. This is mainly intended for use in console applications. </summary>
        public void ExecuteAndWait(Func<Task> asyncAction)
        {
            asyncAction().GetAwaiter().GetResult();
            _disconnectedEvent.WaitOne();
        }
        #endregion

        #region IDisposable
        private bool _isDisposed = false;

        protected virtual void Dispose(bool isDisposing)
        {
            if (!_isDisposed)
            {
                if (isDisposing)
                {
                    _disconnectedEvent.Dispose();
                    _connectedEvent.Dispose();
                }
                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        private Task LargeServerDownloader(CancellationToken cancelToken)
        {
            //Temporary hotfix to download all large guilds before raising READY
            return Task.Run(async () =>
            {
                try
                {
                    const short batchSize = 50;
                    ulong[] serverIds = new ulong[batchSize];

                    while (!cancelToken.IsCancellationRequested && State == ConnectionState.Connecting)
                        await Task.Delay(1000, cancelToken).ConfigureAwait(false);

                    while (!cancelToken.IsCancellationRequested && State == ConnectionState.Connected)
                    {
                        if (_largeServers.Count > 0)
                        {
                            int count = 0;
                            while (count < batchSize && _largeServers.TryDequeue(out serverIds[count]))
                                count++;

                            if (count > 0)
                                GatewaySocket.SendRequestMembers(serverIds.Take(count), "", 0);
                        }
                        await Task.Delay(1250, cancelToken).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) { }
            });
        }

        //Helpers
        private string GetTokenCachePath(string email)
        {
            using (var md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(email.ToLowerInvariant()));
                StringBuilder filenameBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                    filenameBuilder.Append(data[i].ToString("x2"));
                return Path.Combine(Config.CacheDir, filenameBuilder.ToString());
            }
        }
        private string LoadToken(string path, byte[] key)
        {
            if (File.Exists(path))
            {
                try
                {
                    using (var fileStream = File.Open(path, FileMode.Open))
                    using (var aes = Aes.Create())
                    {
                        byte[] iv = new byte[aes.BlockSize / 8];
                        fileStream.Read(iv, 0, iv.Length);
                        aes.IV = iv;
                        aes.Key = key;
                        using (var cryptoStream = new CryptoStream(fileStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            byte[] tokenBuffer = new byte[64];
                            int length = cryptoStream.Read(tokenBuffer, 0, tokenBuffer.Length);
                            return Encoding.UTF8.GetString(tokenBuffer, 0, length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warning("Failed to load cached token. Wrong/changed password?", ex);
                }
            }
            return null;
        }
        private void SaveToken(string path, byte[] key, string token)
        {
            byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
            try
            {
                string parentDir = Path.GetDirectoryName(path);
                if (!Directory.Exists(parentDir))
                    Directory.CreateDirectory(parentDir);

                using (var fileStream = File.Open(path, FileMode.Create))
                using (var aes = Aes.Create())
                {
                    aes.GenerateIV();
                    aes.Key = key;
                    using (var cryptoStream = new CryptoStream(fileStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        fileStream.Write(aes.IV, 0, aes.IV.Length);
                        cryptoStream.Write(tokenBytes, 0, tokenBytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warning("Failed to cache token", ex);
            }
        }
    }
}