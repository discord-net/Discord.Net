using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class UpdateStatusCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCodes.StatusUpdate;
        object IWebSocketMessage.Payload => this;
        bool IWebSocketMessage.IsPrivate => false;

        public sealed class GameInfo
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
