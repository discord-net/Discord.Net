#pragma warning disable CS8618 // Uninitialized NRT expected in models
using System.Text.Json.Serialization;

namespace Discord.Models
{
    public class GatewayInfo
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("shards")]
        public int? Shards { get; set; }
        [JsonPropertyName("session_start_limit")]
        public GatewaySessionStartInfo? SessionStartInfo { get; set; }
    }

    public class GatewaySessionStartInfo
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("remaining")]
        public int Remaining { get; set; }
        [JsonPropertyName("reset_after")]
        public int ResetAfter { get; set; }
    }
}
