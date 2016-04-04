using Discord.Logging;
using Discord.Net.WebSockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
    public class DiscordSocketClient : DiscordClient
    {
        public event EventHandler Connected, Disconnected;
        public event EventHandler<VoiceChannelEventArgs> VoiceConnected, VoiceDisconnected;

        public event EventHandler<ChannelEventArgs> ChannelCreated, ChannelDestroyed;
        public event EventHandler<ChannelUpdatedEventArgs> ChannelUpdated;
        public event EventHandler<MessageEventArgs> MessageReceived, MessageDeleted;
        public event EventHandler<MessageUpdatedEventArgs> MessageUpdated;
        public event EventHandler<RoleEventArgs> RoleCreated, RoleDeleted;
        public event EventHandler<RoleUpdatedEventArgs> RoleUpdated;
        public event EventHandler<GuildEventArgs> JoinedGuild, LeftGuild;
        public event EventHandler<GuildEventArgs> GuildAvailable, GuildUnavailable;
        public event EventHandler<GuildUpdatedEventArgs> GuildUpdated;
        public event EventHandler<CurrentUserUpdatedEventArgs> CurrentUserUpdated;
        public event EventHandler<UserEventArgs> UserJoined, UserLeft;
        public event EventHandler<UserEventArgs> UserBanned, UserUnbanned;
        public event EventHandler<UserUpdatedEventArgs> UserUpdated;
        public event EventHandler<TypingEventArgs> UserIsTyping;

        private readonly Logger _discordLogger, _gatewayLogger;
        private readonly int _connectionTimeout, _reconnectDelay, _failedReconnectDelay;
        private readonly bool _enablePreUpdateEvents, _usePermissionCache;
        private readonly int _largeThreshold, _messageCacheSize;
        private ConcurrentDictionary<ulong, Guild> _guilds;
        private ConcurrentDictionary<ulong, IChannel> _channels;
        private ConcurrentDictionary<ulong, DMChannel> _privateChannels; //Key = RecipientId

        public int ConnectionId { get; }
        public int TotalConnections { get; }
        /// <summary> Gets the internal WebSocket for the Gateway event stream. </summary>
        public GatewaySocket GatewaySocket { get; private set; }
        /*/// <summary> Gets the current logged-in account. </summary>
        public CurrentUser CurrentUser { get; private set; }*/

        public bool IsConnected => GatewaySocket.State == ConnectionState.Connected;

        public DiscordSocketClient(string token, int connectionId = 0, int totalConnections = 1, DiscordConfig config = null)
            : base(token, config)
        {
            if (totalConnections < 1) throw new ArgumentOutOfRangeException(nameof(totalConnections));
            if (connectionId < 0) throw new ArgumentOutOfRangeException(nameof(connectionId));
            if (connectionId >= totalConnections) throw new ArgumentException($"{nameof(connectionId)} must be less than {nameof(totalConnections)}.", nameof(connectionId));

            ConnectionId = connectionId;
            TotalConnections = totalConnections;

            _connectionTimeout = config.ConnectionTimeout;
            _reconnectDelay = config.ReconnectDelay;
            _failedReconnectDelay = config.FailedReconnectDelay;

            _messageCacheSize = config.MessageCacheSize;
            _usePermissionCache = config.UsePermissionsCache;
            _enablePreUpdateEvents = config.EnablePreUpdateEvents;
            _largeThreshold = config.LargeThreshold;

            _discordLogger = _logManager.CreateLogger("Discord");
            _gatewayLogger = _logManager.CreateLogger("Gateway");

            _guilds = new ConcurrentDictionary<ulong, Guild>(2, 0);
            _channels = new ConcurrentDictionary<ulong, IChannel>(2, 0);
            _privateChannels = new ConcurrentDictionary<ulong, DMChannel>(2, 0);
        }

        protected override async Task LogoutInternal()
        {
            await DisconnectInternal().ConfigureAwait(false);

            _guilds.Clear();
            _channels.Clear();
            _privateChannels.Clear();

            CurrentUser = null;

            await base.LogoutInternal();
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
        protected virtual Task ConnectInternal()
        {
            throw new NotImplementedException();
            //GatewaySocket = new GatewaySocket(_webSocketProvider(_cancelToken.Token));
            //GatewaySocket.SetHeader("user-agent", _userAgent);
            //await GatewaySocket.Connect(_cancelToken.Token).ConfigureAwait(false);
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
        protected virtual async Task DisconnectInternal()
        {
            if (GatewaySocket != null)
            {
                await GatewaySocket.Disconnect().ConfigureAwait(false);
                GatewaySocket = null;
            }
        }

        public override async Task<DMChannel> GetOrCreateDMChannel(ulong userId)
        {
            DMChannel channel;
            if (_privateChannels.TryGetValue(userId, out channel))
                return channel;

            return await base.GetOrCreateDMChannel(userId).ConfigureAwait(false);
        }
        public override Task<IEnumerable<DMChannel>> GetDMChannels()
        {
            return Task.FromResult(_privateChannels.Select(x => x.Value));
        }
        public override Task<IEnumerable<Guild>> GetGuilds()
        {
            return Task.FromResult(_guilds.Select(x => x.Value));
        }
        public override Task<Guild> GetGuild(ulong id)
        {
            Guild guild;
            _guilds.TryGetValue(id, out guild);
            return Task.FromResult(guild);
        }

        internal override DMChannel CreateDMChannel(API.Channel model)
        {
            var channel = new DMChannel(model.Id, this, _messageCacheSize);
            channel.Update(model);
            return channel;
        }
        internal override TextChannel CreateTextChannel(Guild guild, API.Channel model)
        {
            var channel = new TextChannel(model.Id, guild, _messageCacheSize, _usePermissionCache);
            channel.Update(model);
            return channel;
        }
        internal override VoiceChannel CreateVoiceChannel(Guild guild, API.Channel model)
        {
            var channel = new VoiceChannel(model.Id, guild, _usePermissionCache);
            channel.Update(model);
            return channel;
        }

        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    GatewaySocket.Dispose();
                }
            }
        }
    }
}
