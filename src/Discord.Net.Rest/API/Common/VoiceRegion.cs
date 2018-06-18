#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class VoiceRegion
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("vip")]
        public bool IsVip { get; set; }
        [JsonProperty("optimal")]
        public bool IsOptimal { get; set; }
        [JsonProperty("deprecated")]
        public bool IsDeprecated { get; set; }
        [JsonProperty("custom")]
        public bool IsCustom { get; set; }
    }
}
