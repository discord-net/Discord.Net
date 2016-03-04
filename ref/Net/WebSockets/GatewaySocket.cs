using Discord.Net.Rest;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    public class GatewaySocket
    {
        public string SessionId { get; }

        public event EventHandler<WebSocketEventEventArgs> ReceivedDispatch = delegate { };

        public Task Connect(IRestClient rest, CancellationToken parentCancelToken) => null;
        public Task Disconnect() => null;

        public void SendIdentify(string token) { }

        public void SendResume() { }
        public void SendHeartbeat() { }
        public void SendUpdateStatus(long? idleSince, string gameName) { }
        public void SendUpdateVoice(ulong? serverId, ulong? channelId, bool isSelfMuted, bool isSelfDeafened) { }
        public void SendRequestMembers(IEnumerable<ulong> serverId, string query, int limit) { }
        
        public void WaitForConnection(CancellationToken cancelToken) { }
    }
}
