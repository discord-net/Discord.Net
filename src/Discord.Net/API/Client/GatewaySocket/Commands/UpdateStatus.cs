using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateStatusCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCodes.StatusUpdate;
        object IWebSocketMessage.Payload => this;
        bool IWebSocketMessage.IsPrivate => false;

        [JsonProperty("idle_since")]
        public long? IdleSince { get; set; }
        [JsonProperty("game")]
        public Game Game { get; set; }
    }
}
