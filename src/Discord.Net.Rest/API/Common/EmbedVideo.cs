using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class EmbedVideo
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("height")]
        public Optional<int> Height { get; set; }
        [JsonPropertyName("width")]
        public Optional<int> Width { get; set; }
    }
}
