using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class ResumedEvent
    {
        [JsonPropertyName("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
