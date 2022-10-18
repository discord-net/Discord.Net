using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class EmbedProvider
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
