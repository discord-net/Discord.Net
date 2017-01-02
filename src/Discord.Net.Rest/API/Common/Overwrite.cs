#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class Overwrite
    {
        [JsonProperty("id")]
        public ulong TargetId { get; set; }
        [JsonProperty("type")]
        public PermissionTarget TargetType { get; set; }
        [JsonProperty("deny"), Int53]
        public ulong Deny { get; set; }
        [JsonProperty("allow"), Int53]
        public ulong Allow { get; set; }
    }
}
