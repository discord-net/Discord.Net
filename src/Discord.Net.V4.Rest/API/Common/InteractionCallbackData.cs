using Newtonsoft.Json;

namespace Discord.API
{
    internal class InteractionCallbackData
    {
        [JsonPropertyName("tts")]
        public Optional<bool> TTS { get; set; }

        [JsonPropertyName("content")]
        public Optional<string> Content { get; set; }

        [JsonPropertyName("embeds")]
        public Optional<Embed[]> Embeds { get; set; }

        [JsonPropertyName("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }

        [JsonPropertyName("flags")]
        public Optional<MessageFlags> Flags { get; set; }

        [JsonPropertyName("components")]
        public Optional<ActionRowComponent[]> Components { get; set; }

        [JsonPropertyName("choices")]
        public Optional<ApplicationCommandOptionChoice[]> Choices { get; set; }

        [JsonPropertyName("title")]
        public Optional<string> Title { get; set; }

        [JsonPropertyName("custom_id")]
        public Optional<string> CustomId { get; set; }
    }
}
