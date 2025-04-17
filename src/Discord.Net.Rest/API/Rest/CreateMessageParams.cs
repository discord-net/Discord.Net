using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateMessageParams
    {
        [JsonProperty("content")]
        public Optional<string> Content { get; set; }

        [JsonProperty("nonce")]
        public Optional<string> Nonce { get; set; }

        [JsonProperty("tts")]
        public Optional<bool> IsTTS { get; set; }

        [JsonProperty("embeds")]
        public Optional<Embed[]> Embeds { get; set; }

        [JsonProperty("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }

        [JsonProperty("message_reference")]
        public Optional<MessageReference> MessageReference { get; set; }

        [JsonProperty("components")]
        public Optional<API.ActionRowComponent[]> Components { get; set; }

        [JsonProperty("sticker_ids")]
        public Optional<ulong[]> Stickers { get; set; }

        [JsonProperty("flags")]
        public Optional<MessageFlags> Flags { get; set; }

        [JsonProperty("poll")]
        public Optional<CreatePollParams> Poll { get; set; }
    }
}
