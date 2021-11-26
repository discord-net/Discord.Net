using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Gateway
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class IdentifyParams
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("properties")]
        public IDictionary<string, string> Properties { get; set; }
        [JsonProperty("large_threshold")]
        public int LargeThreshold { get; set; }
        [JsonProperty("shard")]
        public Optional<int[]> ShardingParams { get; set; }
        [JsonProperty("presence")]
        public Optional<PresenceUpdateParams> Presence { get; set; }
        [JsonProperty("intents")]
        public Optional<int> Intents { get; set; }
    }
}
