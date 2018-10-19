#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyGuildChannelParams
    {
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
        [JsonProperty("position")]
        public Optional<int> Position { get; set; }
        [JsonProperty("parent_id")]
        public Optional<ulong?> CategoryId { get; set; }
        [JsonProperty("permission_overwrites")]
        public Optional<Overwrite[]> Overwrites { get; set; }
    }
}
