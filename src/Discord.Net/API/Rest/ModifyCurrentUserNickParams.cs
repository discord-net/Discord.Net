#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyCurrentUserNickParams
    {
        [JsonProperty("nick")]
        public string Nickname { internal get; set; }
    }
}
