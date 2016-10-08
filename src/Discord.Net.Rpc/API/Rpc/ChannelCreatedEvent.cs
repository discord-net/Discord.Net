using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class ChannelCreatedEvent
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public ChannelType Type { get; set; }
    }
}
