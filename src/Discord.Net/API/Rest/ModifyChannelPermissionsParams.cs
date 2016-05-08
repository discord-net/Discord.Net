using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyChannelPermissionsParams
    {
        [JsonProperty("allow")]
        public uint Allow { get; set; }
        [JsonProperty("deny")]
        public uint Deny { get; set; }
    }
}
