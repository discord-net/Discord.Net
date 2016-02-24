using Discord.Net;
using Discord.Net.Rest;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary> Provides a connection to the DiscordApp service. </summary>
    public partial class DiscordClient : IDisposable
    {
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
        public IEnumerable<Channel> PrivateChannels { get; }
        public IEnumerable<Region> Regions { get; }

        public DiscordClient() { }
        public DiscordClient(DiscordConfig config) { }
        public DiscordClient(Action<DiscordConfig> configFunc) { }

        public Task<string> Connect(string email, string password, string token = null) => null;
        public Task Connect(string token) => null;        
        public Task Disconnect() => null;

        public void SetStatus(UserStatus status) { }
        public void SetGame(string game) { }

        public Channel GetChannel(ulong id) => null;
        public Task<Channel> CreatePrivateChannel(ulong userId) => null;

        public Task<Invite> GetInvite(string inviteIdOrXkcd) => null;

        public Region GetRegion(string id) => null;

        public Server GetServer(ulong id) => null;
        public IEnumerable<Server> FindServers(string name) => null;        
        public Task<Server> CreateServer(string name, Region region, ImageType iconType = ImageType.None, Stream icon = null) => null;

        public void Dispose() { }
    }
}