using Discord.Logging;
using Discord.Net.Rest;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    public class GatewaySocket : WebSocket
    {
        public string SessionId { get; private set; }

        public event EventHandler<WebSocketEventEventArgs> ReceivedDispatch = delegate { };

        public GatewaySocket(DiscordConfig config, ILogger logger) : base(config, logger) { }

        public Task Connect(RestClient rest, CancellationToken parentCancelToken) => null;
        public Task Disconnect() => null;

        public void SendIdentify(string token) { }

        public void SendResume() { }
        public override void SendHeartbeat() { }
        public void SendUpdateStatus(long? idleSince, string gameName) { }
        public void SendUpdateVoice(ulong? serverId, ulong? channelId, bool isSelfMuted, bool isSelfDeafened) { }
        public void SendRequestMembers(IEnumerable<ulong> serverId, string query, int limit) { }
        
        public override void WaitForConnection(CancellationToken cancelToken) { }
    }
}
