#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyGuildChannelsParams
    {
        [JsonProperty("id")]
        public ulong Id { internal get; set; }

        [JsonProperty("position")]
        public int Position { internal get; set; }
    }
}
