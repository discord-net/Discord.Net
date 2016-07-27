using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyChannelPermissionsParams
    {
        [JsonProperty("allow")]
        public ulong Allow { internal get; set; }

        [JsonProperty("deny")]
        public ulong Deny { internal get; set; }
    }
}
