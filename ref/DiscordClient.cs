using Discord.Net;
using Discord.Net.Rest;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary> Provides a connection to the DiscordApp service. </summary>
    public class DiscordClient : IDisposable
    {
        public event EventHandler Ready = delegate { };
        public event EventHandler<ChannelEventArgs> ChannelCreated = delegate { };
        public event EventHandler<ChannelUpdatedEventArgs> ChannelUpdated = delegate { };
        public event EventHandler<ChannelEventArgs> ChannelDestroyed = delegate { };
        public event EventHandler<MessageEventArgs> MessageAcknowledged = delegate { };
        public event EventHandler<MessageEventArgs> MessageDeleted = delegate { };
        public event EventHandler<MessageEventArgs> MessageReceived = delegate { };
        public event EventHandler<MessageEventArgs> MessageSent = delegate { };
        public event EventHandler<MessageUpdatedEventArgs> MessageUpdated = delegate { };
        public event EventHandler<ProfileUpdatedEventArgs> ProfileUpdated = delegate { };
        public event EventHandler<RoleEventArgs> RoleCreated = delegate { };
        public event EventHandler<RoleUpdatedEventArgs> RoleUpdated = delegate { };
        public event EventHandler<RoleEventArgs> RoleDeleted = delegate { };
        public event EventHandler<ServerEventArgs> JoinedServer = delegate { };
        public event EventHandler<ServerEventArgs> LeftServer = delegate { };
        public event EventHandler<ServerEventArgs> ServerAvailable = delegate { };
        public event EventHandler<ServerUpdatedEventArgs> ServerUpdated = delegate { };
        public event EventHandler<ServerEventArgs> ServerUnavailable = delegate { };
        public event EventHandler<UserEventArgs> UserBanned = delegate { };
        public event EventHandler<TypingEventArgs> UserIsTyping = delegate { };
        public event EventHandler<UserEventArgs> UserJoined = delegate { };
        public event EventHandler<UserEventArgs> UserLeft = delegate { };
        public event EventHandler<UserUpdatedEventArgs> UserUpdated = delegate { };
        public event EventHandler<UserEventArgs> UserUnbanned = delegate { };

        public DiscordConfig Config { get; }
        public RestClient ClientAPI { get; }
        public RestClient StatusAPI { get; }
        public GatewaySocket GatewaySocket { get; }
        public MessageQueue MessageQueue { get; }
        public JsonSerializer Serializer { get; }
        
        public ConnectionState State { get; }
        public CancellationToken CancelToken { get; }
        public Profile CurrentUser { get; }
        public string SessionId { get; }
        public UserStatus Status { get; }
        public string CurrentGame { get; }
        
        public IEnumerable<Server> Servers { get; }
        public IEnumerable<PrivateChannel> PrivateChannels { get; }
        public IEnumerable<Region> Regions { get; }

        public DiscordClient() { }
        public DiscordClient(DiscordConfig config) { }
        public DiscordClient(Action<DiscordConfig> configFunc) { }

        public Task<string> Connect(string email, string password, string token = null) => null;
        public Task Connect(string token) => null;        
        public Task Disconnect() => null;

        public void SetStatus(UserStatus status) { }
        public void SetGame(string game) { }

        public PrivateChannel GetPrivateChannel(ulong id) => null;
        public Task<PrivateChannel> CreatePrivateChannel(ulong userId) => null;

        public Task<Invite> GetInvite(string inviteIdOrXkcd) => null;

        public Region GetRegion(string id) => null;

        public Server GetServer(ulong id) => null;
        public IEnumerable<Server> FindServers(string name) => null;        
        public Task<Server> CreateServer(string name, Region region, ImageType iconType = ImageType.None, Stream icon = null) => null;

        public void Dispose() { }
    }
}