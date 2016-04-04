using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.WebSockets
{
    public class GatewaySocket : WebSocket
    {
        public event EventHandler<WebSocketEventEventArgs> ReceivedDispatch = delegate { };

        public ConnectionState State { get; }
        public string Host { get; }
        public string SessionId { get; }

        internal GatewaySocket(IWebSocketEngine engine)
            : base(engine)
        {
        }

        public void SetHeader(string key, string value) => _engine.SetHeader(key, value);

        internal Task Connect(CancellationToken cancelToken)
        {
            return Task.Delay(0);
        }
        internal Task Disconnect()
        {
            return Task.Delay(0);
        }

        public void SendIdentify(string token) { }

        public void SendResume() { }
        public void SendHeartbeat() { }
        public void SendUpdateStatus(long? idleSince, string gameName) { }
        public void SendUpdateVoice(ulong? guildId, ulong? channelId, bool isSelfMuted, bool isSelfDeafened) { }
        public void SendRequestMembers(IEnumerable<ulong> guildId, string query, int limit) { }
        
        public void WaitForConnection(CancellationToken cancelToken) { }
    }
}
