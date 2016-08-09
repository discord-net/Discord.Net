#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CreateGuildIntegrationParams
    {
        [JsonProperty("id")]
        public ulong Id { internal get; set; }

        [JsonProperty("type")]
        public string Type { internal get; set; }
    }
}
