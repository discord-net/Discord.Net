#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyChannelPermissionsParams
    {
        [JsonProperty("type")]
        public int Type { get; }
        [JsonProperty("allow")]
        public string Allow { get; }
        [JsonProperty("deny")]
        public string Deny { get; }

        public ModifyChannelPermissionsParams(int type, string allow, string deny)
        {
            Type = type;
            Allow = allow;
            Deny = deny;
        }
    }
}
