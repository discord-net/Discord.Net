using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    public sealed class ResumedEvent 
    { 
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
