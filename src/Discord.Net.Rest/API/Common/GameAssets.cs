using Newtonsoft.Json;

namespace Discord.API
{
    internal class GameAssets
    {
        [JsonProperty("small_text")]
        public Optional<string> SmallText { get; set; }
        [JsonProperty("small_image")]
        public Optional<string> SmallImage { get; set; }
        [JsonProperty("large_text")]
        public Optional<string> LargeText { get; set; }
        [JsonProperty("large_image")]
        public Optional<string> LargeImage { get; set; }
    }
}
