using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class Game
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("type")]
        public GameType Type { get; set; }
    }
}
