using Newtonsoft.Json;
using System;

namespace Discord.API.GatewaySocket
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HeartbeatCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCode.Heartbeat;
        object IWebSocketMessage.Payload => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
