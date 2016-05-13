using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class CreateGuildChannelParams
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public ChannelType Type { get; set; }

        [JsonProperty("bitrate")]
        public Optional<int> Bitrate { get; set; }
    }
}
