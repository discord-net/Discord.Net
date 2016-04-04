using Newtonsoft.Json;

namespace Discord.API.GatewaySocket
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateStatusCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCode.StatusUpdate;
        object IWebSocketMessage.Payload => this;

        public class GameInfo
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }

        [JsonProperty("idle_since")]
        public long? IdleSince { get; set; }
        [JsonProperty("game")]
        public GameInfo Game { get; set; }
    }
}
