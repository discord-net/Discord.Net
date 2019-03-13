#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyNestedChannelParams : ModifyGuildChannelParams
    {
        [JsonProperty("parent_id")]
        public Optional<ulong?> CategoryId { get; set; }
    }
}
