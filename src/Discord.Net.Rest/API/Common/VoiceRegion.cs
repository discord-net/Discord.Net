using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class VoiceRegion
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("vip")]
        public bool IsVip { get; set; }
        [JsonPropertyName("optimal")]
        public bool IsOptimal { get; set; }
        [JsonPropertyName("deprecated")]
        public bool IsDeprecated { get; set; }
        [JsonPropertyName("custom")]
        public bool IsCustom { get; set; }
    }
}
