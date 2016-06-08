using Newtonsoft.Json;

namespace Discord.API
{
    public class Game
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string StreamUrl { get; set; }
        [JsonProperty("type")]
        public StreamType? StreamType { get; set; }
    }
}
