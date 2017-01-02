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
    }
}
