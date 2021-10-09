using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class CreateStageInstanceParams
    {
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }

        [JsonProperty("topic")]
        public string Topic { get; set; }

        [JsonProperty("privacy_level")]
        public Optional<StagePrivacyLevel> PrivacyLevel { get; set; }
    }
}
