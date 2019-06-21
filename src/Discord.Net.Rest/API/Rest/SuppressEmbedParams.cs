using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class SuppressEmbedParams
    {
        [JsonProperty("suppress")]
        public bool Suppressed { get; set; }
    }
}
