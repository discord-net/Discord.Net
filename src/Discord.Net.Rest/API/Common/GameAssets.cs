using Newtonsoft.Json;

namespace Discord.API
{
    internal class GameAssets
    {
        [JsonProperty("small_text")]
        public Optional<string> SmallText { get; }
        [JsonProperty("small_image")]
        public Optional<string> SmallImage { get; }
        [JsonProperty("large_image")]
        public Optional<string> LargeText { get; }
        [JsonProperty("large_text")]
        public Optional<string> LargeImage { get; }
    }
}