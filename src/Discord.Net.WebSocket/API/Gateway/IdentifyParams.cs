#pragma warning disable CS1591
using Discord.Serialization;
using System.Collections.Generic;

namespace Discord.API.Gateway
{
    internal class IdentifyParams
    {
        [ModelProperty("token")]
        public string Token { get; set; }
        [ModelProperty("properties")]
        public IDictionary<string, string> Properties { get; set; }
        [ModelProperty("large_threshold")]
        public int LargeThreshold { get; set; }
        [ModelProperty("compress")]
        public bool UseCompression { get; set; }
        [ModelProperty("shard")]
        public Optional<int[]> ShardingParams { get; set; }
    }
}
