using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HeartbeatCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCodes.Heartbeat;
        object IWebSocketMessage.Payload => EpochTime.GetMilliseconds();
        bool IWebSocketMessage.IsPrivate => false;
    }
}
