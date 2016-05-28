using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class ResumedEvent 
    { 
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
