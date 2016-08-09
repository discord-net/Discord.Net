#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildPruneParams
    {
        [JsonProperty("days")]
        public int Days { internal get; set; }
    }
}
