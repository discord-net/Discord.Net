using Discord.Net.Rest;
using Discord.Net.WebSockets;
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
        public event EventHandler<LogMessageEventArgs> Log = delegate { };

        public event EventHandler LoggedIn = delegate { };
        public event EventHandler LoggedOut = delegate { };
        public event EventHandler Connected = delegate { };
        public event EventHandler Disconnected = delegate { };
        public event EventHandler<ServerEventArgs> VoiceConnected = delegate { };
        public event EventHandler<ServerEventArgs> VoiceDisconnected = delegate { };

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

        public MessageQueue MessageQueue { get; }
        public IRestClient RestClient { get; }
        public GatewaySocket GatewaySocket { get; }
        public Profile CurrentUser { get; }        

        public DiscordClient() { }
        public DiscordClient(DiscordConfig config) { }
        
        public Task Login(string token) => null;
        public Task Logout() => null;

        public Task Connect() => null;
        public Task Connect(int connectionId, int totalConnections) => null;        
        public Task Disconnect() => null;

        public Task<IEnumerable<PrivateChannel>> GetPrivateChannels() => null;
        public Task<PrivateChannel> GetPrivateChannel(ulong userId) => null;
        public Task<Invite> GetInvite(string inviteIdOrXkcd) => null;
        public Task<IReadOnlyList<Region>> GetRegions() => null;
        public Task<Region> GetRegion(string id) => null;
        public Task<IEnumerable<Server>> GetServers() => null;
        public Task<Server> GetServer(ulong id) => null;

        public Task<PrivateChannel> CreatePrivateChannel(ulong userId) => null;
        public Task<Server> CreateServer(string name, Region region, ImageType iconType = ImageType.None, Stream icon = null) => null;

        public void Dispose() { }
    }
}