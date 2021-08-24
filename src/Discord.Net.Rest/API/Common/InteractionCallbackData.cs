using Newtonsoft.Json;

namespace Discord.API
{
    internal class InteractionCallbackData
    {
        [JsonProperty("tts")]
        public Optional<bool> TTS { get; set; }

        [JsonProperty("content")]
        public Optional<string> Content { get; set; }

        [JsonProperty("embeds")]
        public Optional<API.Embed[]> Embeds { get; set; }

        [JsonProperty("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }

        [JsonProperty("flags")]
        public Optional<MessageFlags> Flags { get; set; }

        [JsonProperty("components")]
        public Optional<API.ActionRowComponent[]> Components { get; set; }
    }
}
