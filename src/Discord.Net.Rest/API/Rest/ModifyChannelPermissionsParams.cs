using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyChannelPermissionsParams
    {
        [JsonPropertyName("type")]
        public int Type { get; }
        [JsonPropertyName("allow")]
        public string Allow { get; }
        [JsonPropertyName("deny")]
        public string Deny { get; }

        public ModifyChannelPermissionsParams(int type, string allow, string deny)
        {
            Type = type;
            Allow = allow;
            Deny = deny;
        }
    }
}
