using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateMessageParams
    {
        [JsonPropertyName("content")]
        public string Content { get; }

        [JsonPropertyName("nonce")]
        public Optional<string> Nonce { get; set; }

        [JsonPropertyName("tts")]
        public Optional<bool> IsTTS { get; set; }

        [JsonPropertyName("embeds")]
        public Optional<Embed[]> Embeds { get; set; }

        [JsonPropertyName("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }

        [JsonPropertyName("message_reference")]
        public Optional<MessageReference> MessageReference { get; set; }

        [JsonPropertyName("components")]
        public Optional<API.ActionRowComponent[]> Components { get; set; }

        [JsonPropertyName("sticker_ids")]
        public Optional<ulong[]> Stickers { get; set; }

        [JsonPropertyName("flags")]
        public Optional<MessageFlags> Flags { get; set; }

        public CreateMessageParams(string content)
        {
            Content = content;
        }
    }
}
