using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Discord.API.Gateway
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class IdentifyParams
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        [JsonPropertyName("properties")]
        public IDictionary<string, string> Properties { get; set; }
        [JsonPropertyName("large_threshold")]
        public int LargeThreshold { get; set; }
        [JsonPropertyName("shard")]
        public Optional<int[]> ShardingParams { get; set; }
        [JsonPropertyName("presence")]
        public Optional<PresenceUpdateParams> Presence { get; set; }
        [JsonPropertyName("intents")]
        public Optional<int> Intents { get; set; }
    }
}
