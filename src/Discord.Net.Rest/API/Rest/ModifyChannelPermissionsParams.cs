#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyChannelPermissionsParams
    {
        [JsonProperty("type")]
        public string Type { get; }
        [JsonProperty("allow")]
        public ulong Allow { get; }
        [JsonProperty("deny")]
        public ulong Deny { get; }

        public ModifyChannelPermissionsParams(string type, ulong allow, ulong deny)
        {
            Type = type;
            Allow = allow;
            Deny = deny;
        }
    }
}
