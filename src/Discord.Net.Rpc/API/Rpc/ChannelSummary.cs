using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class ChannelSummary
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public ChannelType Type { get; set; }
    }
}
