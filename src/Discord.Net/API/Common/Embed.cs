#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    public class Embed
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("thumbnail")]
        public Optional<EmbedThumbnail> Thumbnail { get; set; }
        [JsonProperty("provider")]
        public Optional<EmbedProvider> Provider { get; set; }
    }
}
