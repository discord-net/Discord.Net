using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyChannelPermissionsParams
    {
        [JsonProperty("allow")]
        public Optional<ulong> Allow { get; set; }
        [JsonProperty("deny")]
        public Optional<ulong> Deny { get; set; }
    }
}
