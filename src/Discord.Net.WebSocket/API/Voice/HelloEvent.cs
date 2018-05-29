using Newtonsoft.Json;

namespace Discord.API.Voice
{
    internal class HelloEvent
    {
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
