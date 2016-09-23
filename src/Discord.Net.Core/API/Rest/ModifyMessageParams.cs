#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyMessageParams
    {
        [JsonProperty("content")]
        public Optional<string> Content { get; set; }
    }
}
