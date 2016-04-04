using System;

namespace Discord.API.VoiceSocket
{
    public class HeartbeatCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCode.Heartbeat;
        object IWebSocketMessage.Payload => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
