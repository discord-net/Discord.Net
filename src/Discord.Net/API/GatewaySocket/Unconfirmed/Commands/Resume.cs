using Newtonsoft.Json;

namespace Discord.API.GatewaySocket
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ResumeCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCode.Resume;
        object IWebSocketMessage.Payload => this;

        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("seq")]
        public uint Sequence { get; set; }
    }
}
