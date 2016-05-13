using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyChannelPermissionsParams
    {
        [JsonProperty("allow")]
        public Optional<uint> Allow { get; set; }
        [JsonProperty("deny")]
        public Optional<uint> Deny { get; set; }
    }
}
