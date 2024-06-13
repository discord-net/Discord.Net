using Discord.API.Rest;
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
        public Optional<Embed[]> Embeds { get; set; }

        [JsonProperty("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }

        [JsonProperty("flags")]
        public Optional<MessageFlags> Flags { get; set; }

        [JsonProperty("components")]
        public Optional<ActionRowComponent[]> Components { get; set; }

        [JsonProperty("choices")]
        public Optional<ApplicationCommandOptionChoice[]> Choices { get; set; }

        [JsonProperty("title")]
        public Optional<string> Title { get; set; }

        [JsonProperty("custom_id")]
        public Optional<string> CustomId { get; set; }

        [JsonProperty("poll")]
        public Optional<CreatePollParams> Poll { get; set; }
    }
}
