using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateStatusCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCodes.StatusUpdate;
        object IWebSocketMessage.Payload => this;
        bool IWebSocketMessage.IsPrivate => false;

        [JsonProperty("afk")]
        public bool? Afk { get; set; }
        [JsonProperty("since")]
        public long? IdleSince { get; set; }
        [JsonProperty("game")]
        public Game Game { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
