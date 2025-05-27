using Newtonsoft.Json;

namespace Discord.API
{
    internal class MessageInteraction
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("type")]
        public InteractionType Type { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
    }
}
