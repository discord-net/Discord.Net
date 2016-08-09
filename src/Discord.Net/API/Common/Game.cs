#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    public class Game
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public Optional<string> StreamUrl { get; set; }
        [JsonProperty("type")]
        public Optional<StreamType?> StreamType { get; set; }
    }
}
