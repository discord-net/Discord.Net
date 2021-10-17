using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class ModifyStickerParams
    {
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
        [JsonProperty("description")]
        public Optional<string> Description { get; set; }
        [JsonProperty("tags")]
        public Optional<string> Tags { get; set; }
    }
}
