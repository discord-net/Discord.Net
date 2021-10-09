using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class ModifyStageInstanceParams
    {
        [JsonProperty("topic")]
        public Optional<string> Topic { get; set; }

        [JsonProperty("privacy_level")]
        public Optional<StagePrivacyLevel> PrivacyLevel { get; set; }
    }
}
