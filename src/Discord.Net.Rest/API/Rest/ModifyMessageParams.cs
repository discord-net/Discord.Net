using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyMessageParams
    {
        [JsonPropertyName("content")]
        public Optional<string> Content { get; set; }
        [JsonPropertyName("embeds")]
        public Optional<API.Embed[]> Embeds { get; set; }
        [JsonPropertyName("components")]
        public Optional<API.ActionRowComponent[]> Components { get; set; }
        [JsonPropertyName("flags")]
        public Optional<MessageFlags?> Flags { get; set; }
        [JsonPropertyName("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }
    }
}
