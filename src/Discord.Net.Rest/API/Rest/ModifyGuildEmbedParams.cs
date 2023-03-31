using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyGuildEmbedParams
    {
        [JsonProperty("enabled")]
        public Optional<bool> Enabled { get; set; }
        [JsonProperty("channel")]
        public Optional<ulong?> ChannelId { get; set; }
    }
}
