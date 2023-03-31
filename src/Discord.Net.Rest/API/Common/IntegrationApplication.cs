using Newtonsoft.Json;

namespace Discord.API
{
    internal class IntegrationApplication
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("icon")]
        public Optional<string> Icon { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("summary")]
        public string Summary { get; set; }
        [JsonProperty("bot")]
        public Optional<User> Bot { get; set; }
    }
}
