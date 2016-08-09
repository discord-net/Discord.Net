#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyMessageParams
    {
        [JsonProperty("content")]
        internal Optional<string> _content { get; set; }
        public string Content { set { _content = value; } }
    }
}
