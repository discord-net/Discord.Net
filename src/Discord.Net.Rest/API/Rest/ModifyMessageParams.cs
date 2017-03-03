#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyMessageParams
    {
        [JsonProperty("content")]
        public Optional<string> Content { get; set; }
        [JsonProperty("embed")]
        public Optional<Embed> Embed { get; set; }
    }
}
