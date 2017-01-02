using Newtonsoft.Json;

namespace Discord.API
{
    internal class Reaction
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("me")]
        public bool Me { get; set; }
        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }
    }
}
