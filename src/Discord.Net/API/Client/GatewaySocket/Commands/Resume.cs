using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ResumeCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCodes.Resume;
        object IWebSocketMessage.Payload => this;
        bool IWebSocketMessage.IsPrivate => false;

        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("seq")]
        public uint Sequence { get; set; }
    }
}
